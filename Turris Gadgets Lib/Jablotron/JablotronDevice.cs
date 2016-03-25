using System;
using System.Threading.Tasks;
using Pospa.NET.TurrisGadgets.Jablotron.Devices;

namespace Pospa.NET.TurrisGadgets.Jablotron
{
    public abstract class JablotronDevice:IDisposable
    {
        private const string TxMessagePatern = "TX ENROLL:{0} PGX:{1} PGY:{2} ALARM:{3} BEEP:{4}";
        protected readonly byte _type;
        protected readonly ushort _address;

        protected readonly TurrisDongle _turrisDongle;

        internal JablotronDevice(TurrisDongle dongle, byte type, ushort address)
        {
            _turrisDongle = dongle;
            _type = type;
            _address = address;
            AddressString = Address.ToString("00000000");
        }

        public int Address => _type*65536 + _address;

        public string AddressString { get; }

        internal static JablotronDevicType GetDeviceType(byte deviceType)
        {
            switch (deviceType)
            {
                case 0x11:
                    return JablotronDevicType.AC_82;
                    break;
                case 0xCF:
                    return JablotronDevicType.AC_88;
                    break;
                case 0x58:
                case 0x59:
                    return JablotronDevicType.JA_80L;
                    break;
                case 0x18:
                case 0x19:
                case 0x1A:
                case 0x1B:
                    return JablotronDevicType.JA_81M;
                    break;
                case 0x7F:
                    return JablotronDevicType.JA_82SH;
                    break;
                case 0x1C:
                case 0x1D:
                    return JablotronDevicType.JA_83M;
                    break;
                case 0x64:
                case 0x65:
                    return JablotronDevicType.JA_83P;
                    break;
                case 0x76:
                    return JablotronDevicType.JA_85ST;
                    break;
                case 0x80:
                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                case 0x86:
                case 0x87:
                    return JablotronDevicType.RC_86K;
                    break;
                case 0x90:
                case 0x91:
                case 0x92:
                case 0x93:
                case 0x94:
                case 0x95:
                case 0x96:
                case 0x97:
                    return JablotronDevicType.RC_86K_2nd;
                    break;
                case 0x24:
                case 0x25:
                    return JablotronDevicType.TP_82N;
                    break;
                default:
                    return JablotronDevicType.Unknown;
                    break;
            }
        }

        public JablotronDevicType GetDeviceType()
        {
            return GetDeviceType(_type);
        }

        public static JablotronDevice Create(TurrisDongle dongle, byte type, ushort address)
        {
            switch (GetDeviceType(type))
            {
                case JablotronDevicType.AC_82:
                    return new AC_82(dongle, type, address);
                case JablotronDevicType.AC_88:
                    return new AC_88(dongle, type, address);
                case JablotronDevicType.JA_80L:
                    return new JA_80L(dongle, type, address);
                case JablotronDevicType.JA_81M:
                    return new JA_81M(dongle, type, address);
                case JablotronDevicType.JA_82SH:
                    return new JA_82SH(dongle, type, address);
                case JablotronDevicType.JA_83M:
                    return new JA_83M(dongle, type, address);
                case JablotronDevicType.JA_83P:
                    return new JA_83P(dongle, type, address);
                case JablotronDevicType.JA_85ST:
                    return new JA_85ST(dongle, type, address);
                case JablotronDevicType.RC_86K:
                    return new RC_86K(dongle, type, address);
                case JablotronDevicType.RC_86K_2nd:
                    return new RC_86K_2nd(dongle, type, address);
                case JablotronDevicType.TP_82N:
                    return new TP_82N(dongle, type, address);
                default:
                    return new UnknownJablotronDevice(dongle, type, address);

            }
        }

        public event LowBatteryNotificationEventHandler LowBatteryNotification;

        protected virtual void OnLowBatteryNotification(LowBatteryNotificationEventArgs e)
        {
            LowBatteryNotification?.Invoke(this, e);
        }

        public event TamperNotificationEventHandler TamperNotification;

        protected virtual void OnTamperNotification(TamperNotificationEventArgs e)
        {
            TamperNotification?.Invoke(this, e);
        }

        protected async Task SendMessageAsync(bool pgx, bool pgy)
        {
            string message = string.Format(TxMessagePatern, 0, pgx ? 1 : 0, pgy ? 1 : 0, 0, "NONE");
            await _turrisDongle.SendCommandAsync(message);
        }

        internal virtual async Task OnMessageReceiverAsync(string message)
        {
            if (message.Contains(TurrisDongle.LowBatteryMessagePatern))
            {
                LowBatteryNotification?.Invoke(this, new LowBatteryNotificationEventArgs());
            }
            if (message.Contains(TurrisDongle.TamperMessagePatern))
            {
                TamperNotification?.Invoke(this,
                    new TamperNotificationEventArgs(message.Contains(TurrisDongle.ActActivePatern)));
            }
            await Task.Run(() => ProcessMessage(message));
        }

        protected internal abstract void ProcessMessage(string message);

        protected internal virtual async Task InitializeAsync()
        {
        }

        protected internal abstract void OnDispose();

        public void Dispose()
        {
            OnDispose();
        }

        public byte GetIndex()
        {
            return (byte) Array.FindIndex(_turrisDongle.GetRegisteredDeviceMap(), address => address.Equals(AddressString));
        }
    }
    public delegate void LowBatteryNotificationEventHandler(object sender, LowBatteryNotificationEventArgs e);

    public class LowBatteryNotificationEventArgs : EventArgs
    {
        public LowBatteryNotificationEventArgs()
        {
        }
    }
    public delegate void TamperNotificationEventHandler(object sender, TamperNotificationEventArgs e);

    public class TamperNotificationEventArgs : EventArgs
    {
        public bool IsCircuitClosed { get; }
        public TamperNotificationEventArgs(bool isCircuitClosed)
        {
            IsCircuitClosed = isCircuitClosed;
        }
    }
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}