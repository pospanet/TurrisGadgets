namespace Pospa.NET.TGManager
{
    public class NetworkWatchdog
    {
        public static bool IsNetworkAvailable => System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
    }
}
