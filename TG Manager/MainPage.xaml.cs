using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.ApplicationInsights;
using Pospa.NET.Sigfox;
using Pospa.NET.TurrisGadgets;
using Pospa.NET.TurrisGadgets.Jablotron;
using Pospa.NET.TurrisGadgets.Jablotron.Devices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pospa.NET.TGManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly TurrisDongle _dongle;
        private readonly SnocModule _snoc;
        private readonly Task _dongleInitTask;
        private readonly Task _snocInitTask;

        private readonly ApplicationDataContainer _settings;
        public static TelemetryClient Telemetry { get; }

        static MainPage()
        {
            Telemetry = new TelemetryClient();
        }
        public MainPage()
        {
            InitializeComponent();

            _settings = ApplicationData.Current.LocalSettings;

            _snoc = new SnocModule();
            _dongle = new TurrisDongle();

            _snocInitTask = _snoc.InitializeAsync(_initializationFailedNotification, null);

            _dongle.MessageReceived += Dongle_MessageReceived;
            _dongleInitTask = _dongle.InitializeAsync(_initializationFailedNotification,
                _dongle_InitializationFinishedNotification, true);
        }

        private static async void _initializationFailedNotification(Exception ex)
        {
            MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Initialization error");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("OK (5s)") { Id = 0 });

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
            {
                // Adding a 3rd command will crash the app when running on Mobile !!!
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Maybe later") { Id = 2 });
            }

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            IUICommand result = await dialog.ShowAsync();

            //Window.Current.Dispatcher.RunAsync()
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
            JablotronDevice[] sensors =
                dongle.GetRegisteredDevices().Where(d => d is JablotronSensorDevice).ToArray();
            foreach (JablotronSensorDevice sensor in sensors)
            {
                sensor.BeaconNotification += Pir_BeaconNotification;
                sensor.SensorNotification += Pir_SensorNotification;
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

        private static void SendMessageToAzure(JablotronSensorDevice sensorDevice)
        {
            DataMessage dataMessage = new DataMessage(sensorDevice.LastRawMessage);
            dataMessage.RawDeviceAddress = sensorDevice.AddressString;
            dataMessage.DeviceAddress = sensorDevice.Address;
            dataMessage.DeviceType = sensorDevice.GetDeviceType();
            dataMessage.LowBattery = sensorDevice.IsBatteryLow;
            dataMessage.Tamper = sensorDevice.IsTampered;
            dataMessage.Sensor = sensorDevice.IsSensorActivated;
            //DeviceMessage deviceMessage = new DeviceMessage(dataMessage);
            //AzureIoTHubHelper.SendMessageDataAsync(DeviceId.ToString(), _iotHub, _sas, deviceMessage);
        }
    }
}