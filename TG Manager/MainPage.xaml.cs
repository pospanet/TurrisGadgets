﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
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
        private readonly Task _initTask;

        public MainPage()
        {
            InitializeComponent();
            _dongle = new TurrisDongle();
            _dongle.MessageReceived += Dongle_MessageReceived;
            _dongle.InitializationFinishedNotification += _dongle_InitializationFinishedNotification;
            _initTask = _dongle.Initialize(true);
        }

        private void _dongle_InitializationFinishedNotification(object sender, InitializationFinishedEventArgs e)
        {
            JablotronDevice[] pirs =
                _dongle.GetRegisteredDevices().Where(d => d.GetDeviceType().Equals(JablotronDevicType.JA_83P)).ToArray();
            foreach (JA_83P pir in pirs)
            {
                pir.BeaconNotification += Pir_BeaconNotification;
                pir.SensorNotification += Pir_SensorNotification;
            }
        }

        private void Dongle_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = e.Message;
        }

        private void Pir_SensorNotification(object sender, SensorEventArgs e)
        {
        }

        private void Pir_BeaconNotification(object sender, BeaconEventArgs e)
        {
        }
    }
}