using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;

namespace Pospa.NET.Sigfox
{
    public class SnocModule : IDisposable
    {
        private DataWriter _dataWriter;
        private DataReader _dataReader;
        private const string ProbeCommand = "AT\r";
        private const int BufferLength = 128;
        private readonly CancellationTokenSource _readCancellationTokenSource;
        private const int CancelTimeoutSeconds = 3;
        private SerialDevice _snocModule;
        private static readonly EasClientDeviceInformation DeviceInfo;
        private static readonly Regex SnocModuleResponseRegex;
        private const string SnocModuleResponseRegexString = @"OK";

        static SnocModule()
        {
            SnocModuleResponseRegex = new Regex(SnocModuleResponseRegexString);
            DeviceInfo = new EasClientDeviceInformation();
        }

        public SnocModule()
        {
            _readCancellationTokenSource = new CancellationTokenSource();
            _snocModule = null;
        }

        public async Task InitializeAsync(bool initializeDeviceList = true)
        {
            try
            {
                await InitializeSerialPort();
            }
            catch (Exception ex)
            {
                OnInitializationFailedNotification(new InitializationEventArgs(ex));
            }
        }

        public event InitializationEventHandler InitializationFailedNotification;

        protected virtual void OnInitializationFailedNotification(InitializationEventArgs e)
        {
            InitializationFailedNotification?.Invoke(this, e);
        }

        private async Task InitializeSerialPort()
        {
            string deviceSelector = SerialDevice.GetDeviceSelector();
            DeviceInformationCollection deviceInformations = await DeviceInformation.FindAllAsync(deviceSelector);
            SerialDevice serialPort = await InitializeSerialDeviceAsync(deviceInformations.First().Id);

            SetSerialPortParameters(serialPort);

            DataWriter dataWriter = new DataWriter(serialPort.OutputStream);
            dataWriter.WriteString(ProbeCommand);
            await dataWriter.StoreAsync();
            DataReader dataReader = new DataReader(serialPort.InputStream);
            string readString = await GetDataFromDataReader(dataReader, _readCancellationTokenSource.Token);


            if (SnocModuleResponseRegex.IsMatch(readString))
            {
                _snocModule = serialPort;
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
                OnInitializationFailedNotification(new InitializationEventArgs());
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

        private static void SetSerialPortParameters(SerialDevice serialPort)
        {
            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.BaudRate = 9600;
            serialPort.Parity = SerialParity.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.DataBits = 8;
        }

        private async Task<SerialDevice> InitializeSerialDeviceAsync(string id)
        {
            SerialDevice serialPort = await SerialDevice.FromIdAsync(id);
            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.BaudRate = 9600;
            serialPort.Parity = SerialParity.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = SerialHandshake.None;
            return serialPort;
        }

        public void Cancel()
        {
            _readCancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            Cancel();
            Task.Delay(TimeSpan.FromSeconds(CancelTimeoutSeconds));
            _dataReader?.DetachBuffer();
            _dataReader?.DetachStream();
            _dataReader?.Dispose();
            _dataWriter?.DetachBuffer();
            _dataWriter?.DetachStream();
            _dataWriter?.Dispose();
            _snocModule?.Dispose();
            _readCancellationTokenSource.Dispose();
        }
    }

    public delegate void InitializationEventHandler(object sender, InitializationEventArgs e);

    public class InitializationEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public InitializationEventArgs()
        {
            Exception = null;
        }

        public InitializationEventArgs(Exception ex) : this()
        {
            Exception = ex;
        }
    }
}