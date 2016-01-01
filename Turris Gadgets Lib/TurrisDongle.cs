using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Pospa.NET.TurrisGadgets.Jablotron;

namespace Pospa.NET.TurrisGadgets
{
    public class TurrisDongle : IDisposable
    {
        private const string ProbeCommand = "WHO AM I?";
        private const string TamperMessagePatern = "TAMPER";
        private const string ActActivePatern = "ACT:1";
        private const string LowBatteryMessagePatern = "LB:1";
        private const string NoDeviceAddressPatern = "[--------]";
        private const string TurrisDongleResponceRegexString = @"TURRIS DONGLE V\d.\d";
        private const string JablotronDeviceSlotAddressRegexString = @"SLOT:\d{2}\s\[(?<address>\d+)\]";
        private const string JablotronDeviceAddressRegexString = @"\[(?<address>\d+)\]";
        private const int BufferLength = 128;
        private static readonly Regex _turrisDongleResponceRegex;
        private static readonly Regex _jablotronDeviceSlotAddressRegex;
        private static readonly Regex _jablotronDeviceAddressRegex;
        private List<string> _turrisDingleDeviceIDs;
        private readonly string[] _jablotronDeviceMap;
        private readonly Dictionary<string, JablotronDevice> _jablotronDevices;

        private Task _messageProcessingTask;

        private const int CancelTimeout = 3;

        private SerialDevice _turrisDongle;
        private DataWriter _dataWriter;
        private DataReader _dataReader;

        private readonly CancellationTokenSource _readCancellationTokenSource;


        public event MessageReceivedEventHandler MessageReceived;
        public event LowBatteryNotificationEventHandler LowBatteryNotificationReceived;
        public event TamperNotificationEventHandler TamperNotificationReceived;

        public bool IsInitialized { get; private set; }

        static TurrisDongle()
        {
            _turrisDongleResponceRegex = new Regex(TurrisDongleResponceRegexString);
            _jablotronDeviceSlotAddressRegex = new Regex(JablotronDeviceSlotAddressRegexString);
            _jablotronDeviceAddressRegex = new Regex(JablotronDeviceAddressRegexString);
        }

        public TurrisDongle()
        {
            _turrisDongle = null;
            _readCancellationTokenSource = new CancellationTokenSource();
            _jablotronDeviceMap = new string[32];
            _jablotronDevices = new Dictionary<string, JablotronDevice>();
            IsInitialized = false;
        }

        public event InitializationFinishedEventHandler InitializationFinishedNotification;
        public async Task Initialize(bool initializeDeviceList = true)
        {
            if (IsInitialized) return;
            await InitializeTurrisDongle();
            if (initializeDeviceList)
            {
                for (int i = 0; i < 32; i++)
                {
                    string command = string.Format("GET SLOT:{0:00}", i);
                    _dataWriter.WriteString(ComposeCommand(command));
                    await _dataWriter.StoreAsync();
                    string message = await GetDataFromDataReader(_dataReader, _readCancellationTokenSource.Token);
                    if (message.Contains(NoDeviceAddressPatern))
                    {
                        _jablotronDeviceMap[i] = null;
                        continue;
                    }
                    Match match = _jablotronDeviceSlotAddressRegex.Match(message);
                    string addressString = match.Groups["address"].Value;
                    int address = Convert.ToInt32(addressString);
                    JablotronDevice jablotronDevice = JablotronDevice.Create(this, (byte) (address/65536),
                        (ushort) (address%65536));
                    _jablotronDevices.Add(addressString, jablotronDevice);
                    _jablotronDeviceMap[i] = addressString;
                }
            }
            IsInitialized = true;
            foreach (KeyValuePair<string, JablotronDevice> device in _jablotronDevices)
            {
                await device.Value.Initialize();
            }
            OnInitializationFinishedNotification(new InitializationFinishedEventArgs());
            _messageProcessingTask = MessageProcessing();
        }

        public void Cancel()
        {
            _readCancellationTokenSource.Cancel();
        }

        public async Task SendCommand(string command)
        {
            _dataWriter.WriteString(ComposeCommand(command));
            await _dataWriter.StoreAsync();
        }

        public IEnumerable<JablotronDevice> GetRegisteredDevices()
        {
            return _jablotronDevices.Select(pair => pair.Value);
        }

        internal string[] GetRegisteredDeviceMap()
        {
            return _jablotronDeviceMap;
        }

        private async Task<SerialDevice> InitializeSerialDevice(string id)
        {
            SerialDevice serialPort = await SerialDevice.FromIdAsync(id);
            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(500);
            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(500);
            serialPort.BaudRate = 57600;
            serialPort.Parity = SerialParity.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = SerialHandshake.None;
            return serialPort;
        }


        private async Task MessageProcessing()
        {
            while (!_readCancellationTokenSource.IsCancellationRequested)
            {
                string message = await GetDataFromDataReader(_dataReader, _readCancellationTokenSource.Token);
                Task.Run(() => OnMessageReceivedInternal(new MessageReceivedEventArgs(message)));
                Task.Run(() => OnMessageReceived(new MessageReceivedEventArgs(message)));
            }
        }

        public async Task InitializeTurrisDongle()
        {
            _turrisDingleDeviceIDs = new List<string>();
            string deviceSelector = SerialDevice.GetDeviceSelector();
            DeviceInformationCollection deviceInformations = await DeviceInformation.FindAllAsync(deviceSelector);
            foreach (DeviceInformation deviceInformation in deviceInformations.Where(di => di.Id.Contains("FTDIBUS#")))
            {
                string readString = "";
                SerialDevice serialPort = await InitializeSerialDevice(deviceInformation.Id);
                DataWriter dataWriter = new DataWriter(serialPort.OutputStream);
                dataWriter.WriteString(ComposeCommand(ProbeCommand));
                await dataWriter.StoreAsync();
                DataReader dataReader = new DataReader(serialPort.InputStream);
                readString = await GetDataFromDataReader(dataReader, _readCancellationTokenSource.Token);


                if (_turrisDongleResponceRegex.IsMatch(readString))
                {
                    _turrisDingleDeviceIDs.Add(deviceInformation.Id);
                    _turrisDongle = serialPort;
                    _dataReader = dataReader;
                    _dataWriter = dataWriter;
                }
                else
                {
                    dataReader.DetachBuffer();
                    dataReader.DetachStream();
                    dataReader.Dispose();
                    dataWriter.DetachBuffer();
                    dataWriter.DetachStream();
                    dataWriter.Dispose();
                    serialPort.Dispose();
                }
            }
            int dongleCount = _turrisDingleDeviceIDs.Count();
            if (dongleCount.Equals(0))
            {
                throw new NullReferenceException("Turris dongle not found.");
            }
            if (dongleCount > 1)
            {
                throw new NotSupportedException("Only one Turris dongle is supported (at the moment).");
            }
        }

        private static async Task<string> GetDataFromDataReader(DataReader dataReader,
            CancellationToken cancellationToken)
        {
            string readString = string.Empty;
            dataReader.InputStreamOptions = InputStreamOptions.Partial;
            uint bytesRead = await dataReader.LoadAsync(BufferLength).AsTask(cancellationToken);
            if (bytesRead == 0) return readString;
            while (dataReader.UnconsumedBufferLength > 0)
            {
                char c = (char) dataReader.ReadByte();
                readString += c;
            }
            readString = readString.Trim('\n');
            return readString;
        }

        private static string ComposeCommand(string command)
        {
            return string.Concat(Convert.ToChar(27), command, "\n");
        }

        public void Dispose()
        {
            Cancel();
            Task.Delay(TimeSpan.FromSeconds(CancelTimeout));
            _dataReader?.DetachBuffer();
            _dataReader?.DetachStream();
            _dataReader?.Dispose();
            _dataWriter?.DetachBuffer();
            _dataWriter?.DetachStream();
            _dataWriter?.Dispose();
            _turrisDongle?.Dispose();
            _readCancellationTokenSource.Dispose();
        }

        protected virtual void OnMessageReceivedInternal(MessageReceivedEventArgs e)
        {
            Match match = _jablotronDeviceAddressRegex.Match(e.Message);
            string addressString = match.Groups["address"].Value;
            _jablotronDevices[addressString].ProcessMessage(e.Message);

            if (e.Message.Contains(LowBatteryMessagePatern))
            {
                LowBatteryNotificationReceived?.Invoke(this, new LowBatteryNotificationEventArgs());
            }
            if (e.Message.Contains(TamperMessagePatern))
            {
                TamperNotificationReceived?.Invoke(this, new TamperNotificationEventArgs(e.Message.Contains(ActActivePatern)));
            }
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        protected virtual void OnInitializationFinishedNotification(InitializationFinishedEventArgs e)
        {
            InitializationFinishedNotification?.Invoke(this, e);
        }
    }
    public delegate void InitializationFinishedEventHandler(object sender, InitializationFinishedEventArgs e);

    public class InitializationFinishedEventArgs : EventArgs
    {
        public InitializationFinishedEventArgs()
        {
        }
    }
}