using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace Pospa.NET.TurrisGadgets
{
    public class TurrisDongle : IDisposable
    {
        private const string ProbeCommand = "WHO AM I?";
        private const string TurrisDongleResponceRegexString = @"TURRIS DONGLE V\d.\d";
        private const int BufferLength = 128;
        private readonly Regex _turrisDongleResponceRegex;
        private List<string> _turrisDingleDeviceIDs;

        private const int CancelTimeout = 3;

        private SerialDevice _turrisDongle;

        private readonly CancellationTokenSource _readCancellationTokenSource;


        public event MessageReceivedEventHandler MessageReceived;

        public TurrisDongle()
        {
            _turrisDongleResponceRegex = new Regex(TurrisDongleResponceRegexString);
            _turrisDongle = null;
            _readCancellationTokenSource = new CancellationTokenSource();
        }

        public void Initialize()
        {
            InitializeTurrisDongle()
                .ContinueWith(
                    t => InitializeMessageProcessing());
        }

        public void Cancel()
        {
            _readCancellationTokenSource.Cancel();
        }

        public async Task SendCommand(string command)
        {
            using (DataWriter dataWriter = new DataWriter(_turrisDongle.OutputStream))
            {
                dataWriter.WriteString(ComposeCommand(command));
                await dataWriter.StoreAsync();
            }
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


        private async Task InitializeMessageProcessing()
        {
            using (DataReader reader = new DataReader(_turrisDongle.InputStream))
            {
                while (!_readCancellationTokenSource.IsCancellationRequested)
                {
                    string message = await GetDataFromDataReader(reader, _readCancellationTokenSource.Token);
                    Task.Run(() => OnMessageReceived(new MessageReceivedEventArgs(message)));
                }
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
                using (SerialDevice serialPort = await InitializeSerialDevice(deviceInformation.Id))
                {
                    using (DataWriter dataWriter = new DataWriter(serialPort.OutputStream))
                    {
                        dataWriter.WriteString(ComposeCommand(ProbeCommand));
                        await dataWriter.StoreAsync();
                        using (DataReader dataReader = new DataReader(serialPort.InputStream))
                        {
                            readString = await GetDataFromDataReader(dataReader, _readCancellationTokenSource.Token);
                        }
                    }
                }
                if (_turrisDongleResponceRegex.IsMatch(readString))
                {
                    _turrisDingleDeviceIDs.Add(deviceInformation.Id);
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

            _turrisDongle = await InitializeSerialDevice(_turrisDingleDeviceIDs.First());
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
            _turrisDongle?.Dispose();
            _readCancellationTokenSource.Dispose();
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}