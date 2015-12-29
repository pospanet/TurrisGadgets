using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace Pospa.NET.TurrisGadgets
{
    public class TurrisDongle
    {
        private const string ProbeCommand = "WHO AM I?";
        private const string TurrisDongleResponceRegexString = @"TURRIS DONGLE V\d.\d";
        private const int BufferLength = 128;
        private readonly Regex _turrisDongleResponceRegex;
        private List<string> _turrisDingleDeviceIDs;

        public TurrisDongle()
        {
            _turrisDongleResponceRegex = new Regex(TurrisDongleResponceRegexString);
            _turrisDingleDeviceIDs = new List<string>();
        }

        public async Task<int> Initialize()
        {
            _turrisDingleDeviceIDs = new List<string>();
            string deviceSelector = SerialDevice.GetDeviceSelector();
            DeviceInformationCollection deviceInformations = await DeviceInformation.FindAllAsync(deviceSelector);
            foreach (DeviceInformation deviceInformation in deviceInformations.Where(di => di.Id.Contains("FTDIBUS#")))
            {
                string readString = "";
                using (SerialDevice serialPort = await SerialDevice.FromIdAsync(deviceInformation.Id))
                {
                    serialPort.WriteTimeout = TimeSpan.FromMilliseconds(500);
                    serialPort.ReadTimeout = TimeSpan.FromMilliseconds(500);
                    serialPort.BaudRate = 57600;
                    serialPort.Parity = SerialParity.None;
                    serialPort.StopBits = SerialStopBitCount.One;
                    serialPort.DataBits = 8;
                    serialPort.Handshake = SerialHandshake.None;
                    using (DataWriter dataWriter = new DataWriter(serialPort.OutputStream))
                    {
                        dataWriter.WriteString(ComposeCommand(ProbeCommand));
                        await dataWriter.StoreAsync();
                        using (DataReader dataReader = new DataReader(serialPort.InputStream))
                        {
                            dataReader.InputStreamOptions = InputStreamOptions.Partial;
                            uint bytesRead = await dataReader.LoadAsync(BufferLength).AsTask();
                            if (bytesRead > 0)
                            {
                                while (dataReader.UnconsumedBufferLength > 0)
                                {
                                    char c = (char) dataReader.ReadByte();
                                    readString += c;
                                }
                                readString = readString.Trim('\n');
                            }
                        }
                    }
                }
                if (_turrisDongleResponceRegex.IsMatch(readString))
                {
                    _turrisDingleDeviceIDs.Add(deviceInformation.Id);
                }
            }
            return _turrisDingleDeviceIDs.Count();
        }

        private static string ComposeCommand(string command)
        {
            return string.Concat(Convert.ToChar(27), command, "\n");
        }
    }
}