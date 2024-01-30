using System;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace WifiBrute
{


    public static class MAC
    {
       public static void Spoof()
       {
            foreach (var adapter in GetWirelessNetworkAdapters())
            {
                string newMacAddress = GenerateRandomMacAddress();
                ChangeMacAddress(adapter.Id, newMacAddress);
            }
        }

        static string GenerateRandomMacAddress()
        {
            var random = new Random();
            byte[] macAddr = new byte[6];
            random.NextBytes(macAddr);
            macAddr[0] = (byte)(macAddr[0] & (byte)254); // Убираем мультикаст бит
            return BitConverter.ToString(macAddr).Replace("-", ":");
        }

        static NetworkInterface[] GetWirelessNetworkAdapters()
        {
            return Array.FindAll(NetworkInterface.GetAllNetworkInterfaces(),
                                 adapter => adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
        }

        static void ChangeMacAddress(string adapterId, string newMac)
        {
            string regPath = $@"SYSTEM\CurrentControlSet\Control\Class\{{4d36e972-e325-11ce-bfc1-08002be10318}}\{adapterId}";
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(regPath, true))
            {
                if (regKey != null)
                {
                    regKey.SetValue("NetworkAddress", newMac, RegistryValueKind.String);
                }
            }
        }
    }

}
