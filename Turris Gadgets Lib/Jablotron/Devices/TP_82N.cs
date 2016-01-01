using System;
using System.Text.RegularExpressions;

namespace Pospa.NET.TurrisGadgets.Jablotron.Devices
{
    public class TP_82N : JablotronDevice
    {
        private const string MessagePaternRegexString = @"TP-82N\s(?<operation>SET|INT):(?<temperature>\d{2}\.\d{1})°C";
        private static readonly Regex MessagePaternRegex;

        static TP_82N()
        {
            MessagePaternRegex = new Regex(MessagePaternRegexString);
        }

        internal TP_82N(TurrisDongle dongle, byte type, ushort address) : base(dongle, type, address)
        {
            SetTemperature = 0;
            ActualTemperature = 0;
        }

        protected internal override void ProcessMessage(string message)
        {
            Match match = MessagePaternRegex.Match(message);
            if (match.Success)
            {
                decimal temperature = decimal.Parse(match.Groups["temperature"].Value);
                switch (match.Groups["operation"].Value)
                {
                    case "SET":
                        OnTemperatureSetNotification(new TemperatureEventArgs(temperature));
                        break;
                    case "INT":
                        OnTemperatureMeasuredNotification(new TemperatureEventArgs(temperature));
                        break;
                }
            }
        }

        public double SetTemperature { get; }
        public double ActualTemperature { get; }
        public event TemperatureEventHandler TemperatureSetNotification;

        protected virtual void OnTemperatureSetNotification(TemperatureEventArgs e)
        {
            TemperatureSetNotification?.Invoke(this, e);
        }

        public event TemperatureEventHandler TemperatureMeasuredNotification;

        protected virtual void OnTemperatureMeasuredNotification(TemperatureEventArgs e)
        {
            TemperatureMeasuredNotification?.Invoke(this, e);
        }
    }
    public delegate void TemperatureEventHandler(object sender, TemperatureEventArgs e);

    public class TemperatureEventArgs : EventArgs
    {
        public decimal Temperature { get; }
        public TemperatureEventArgs(decimal temperature)
        {
            Temperature = temperature;
        }
    }
}