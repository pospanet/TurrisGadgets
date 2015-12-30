using System;

namespace Pospa.NET.TurrisGadgets
{
    public class JablotronDevice
    {
        protected readonly byte _type;
        protected readonly ushort _address;

        internal JablotronDevice(byte type, ushort address)
        {
            _type = type;
            _address = address;
        }

        public int Address => _type*65536 + _address;

        internal static JablotronDevicType GetDevicType(byte deviceType)
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

        public static JablotronDevice Create(byte type, ushort address)
        {
            switch(GetDevicType(type))
            {
                case JablotronDevicType.AC_82:
                    return new AC_82(type, address);
                case JablotronDevicType.AC_88:
                    return new AC_88(type, address);
                case JablotronDevicType.JA_80L:
                    return new AC_80L(type, address);
                case JablotronDevicType.JA_81M:
                    return new AC_81M(type, address);
                case JablotronDevicType.JA_82SH:
                    return new AC_82SH(type, address);
                case JablotronDevicType.JA_83M:
                    return new AC_83M(type, address);
                case JablotronDevicType.JA_83P:
                    return new AC_83P(type, address);
                case JablotronDevicType.JA_85ST:
                    return new AC_85ST(type, address);
                case JablotronDevicType.RC_86K:
                    return new AC_86K(type, address);
                case JablotronDevicType.RC_86K_2nd:
                    return new AC_86K_2nd(type, address);
                case JablotronDevicType.TP_82N:
                    return new AC_82N(type, address);
                default:
                    return new JablotronDevice(type, address);

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
    }
}