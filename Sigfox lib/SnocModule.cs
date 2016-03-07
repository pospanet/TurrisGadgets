using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace Pospa.NET.Sigfox
{
    public class SnocModule : IDisposable
    {
        private DataWriter _dataWriter;
        private DataReader _dataReader;

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
            foreach (DeviceInformation deviceInformation in deviceInformations.Where(di => di.Id.Contains("FTDIBUS#")))
            {
                //string readString = "";
                //SerialDevice serialPort = await InitializeSerialDeviceAsync(deviceInformation.Id);
                //DataWriter dataWriter = new DataWriter(serialPort.OutputStream);
                //dataWriter.WriteString(ComposeCommand(ProbeCommand));
                //await dataWriter.StoreAsync();
                //DataReader dataReader = new DataReader(serialPort.InputStream);
                //readString = await GetDataFromDataReader(dataReader, _readCancellationTokenSource.Token);


                //if (TurrisDongleResponceRegex.IsMatch(readString))
                //{
                //    _turrisDingleDeviceIDs.Add(deviceInformation.Id);
                //    _turrisDongle = serialPort;
                //    _dataReader = dataReader;
                //    _dataWriter = dataWriter;
                //}
                //else
                //{
                //    dataReader.DetachBuffer();
                //    dataReader.DetachStream();
                //    dataReader.Dispose();
                //    dataWriter.DetachBuffer();
                //    dataWriter.DetachStream();
                //    dataWriter.Dispose();
                //    serialPort.Dispose();
                //}
            }
            //int dongleCount = _turrisDingleDeviceIDs.Count();
            //if (dongleCount.Equals(0))
            //{
            //    throw new NullReferenceException("Turris dongle not found.");
            //}
            //if (dongleCount > 1)
            //{
            //    throw new NotSupportedException("Only one Turris dongle is supported (at the moment).");
            //}
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

        public void Dispose()
        {
            throw new NotImplementedException();
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