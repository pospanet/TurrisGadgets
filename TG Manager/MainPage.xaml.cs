using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Pospa.NET.Sigfox;
using Pospa.NET.TurrisGadgets;
using Pospa.NET.TurrisGadgets.Jablotron;
using Pospa.NET.TurrisGadgets.Jablotron.Devices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TG_Manager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly TurrisDongle _dongle;
        private readonly SnocModule _snoc;
        private readonly Task _initTask;

        public MainPage()
        {
            InitializeComponent();
            _snoc = new SnocModule();
            _dongle = new TurrisDongle();

            _snoc.InitializeAsync(null, null);

            _dongle.MessageReceived += Dongle_MessageReceived;
            _initTask = _dongle.InitializeAsync(_dongle_InitializationFailedNotification, _dongle_InitializationFinishedNotification, true);

        }

        private static void _dongle_InitializationFailedNotification(Exception ex)
        {
            string message = ex.Message;
        }

        private static void _dongle_InitializationFinishedNotification(TurrisDongle dongle)
        {
            JablotronDevice[] pirs =
                dongle.GetRegisteredDevices().Where(d => d.GetDeviceType().Equals(JablotronDevicType.JA_83P)).ToArray();
            foreach (JA_83P pir in pirs)
            {
                pir.BeaconNotification += Pir_BeaconNotification;
                pir.SensorNotification += Pir_SensorNotification;
            }
            JablotronDevice[] thermostats =
               dongle.GetRegisteredDevices().Where(d => d.GetDeviceType().Equals(JablotronDevicType.TP_82N)).ToArray();
            foreach (TP_82N thermostat in thermostats)
            {
                thermostat.TemperatureMeasuredNotification += Thermostat_TemperatureMeasuredNotification;
                thermostat.TemperatureSetNotification += Thermostat_TemperatureSetNotification;
            }
        }

        private static void Thermostat_TemperatureSetNotification(object sender, TemperatureEventArgs e)
        {
            decimal temperature = e.Temperature;
        }

        private static void Thermostat_TemperatureMeasuredNotification(object sender, TemperatureEventArgs e)
        {
            decimal temperature = e.Temperature;
        }

        private void Dongle_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = e.Message;
        }

        private static void Pir_SensorNotification(object sender, SensorEventArgs e)
        {
        }

        private static void Pir_BeaconNotification(object sender, BeaconEventArgs e)
        {
        }

        private static void SendMessageToAzure()
        {
            //DataMessage dataMessage = new DataMessage(e.Message);
            //dataMessage.RawDeviceAddress = addressString;
            //dataMessage.DeviceAddress = deviceAddress;
            //dataMessage.DeviceType = deviceType;
            //dataMessage.LowBattery = lowBatteryNotification;
            //dataMessage.Tamper = tamperNotification;
            //DeviceMessage deviceMessage = new DeviceMessage(dataMessage);
            //AzureIoTHubHelper.SendMessageDataAsync(DeviceId.ToString(), _iotHub, _sas, deviceMessage);
        }
    }
}