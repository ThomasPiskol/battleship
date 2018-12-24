using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Network
{
    public class GetIPAddress
    {
        public static string GetHostName()
        {
            UnicastIPAddressInformation localIP = FindLocalIp();
            if (localIP == null)
            {
                return Environment.MachineName;
            }

            if (localIP.Address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return $"[{localIP.Address}]";
            }

            return localIP.Address.ToString();
        }

        public static UnicastIPAddressInformation FindLocalIp()
        {
            UnicastIPAddressInformation bestGuess = null;
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                IPInterfaceProperties properties = network.GetIPProperties();

                if (properties.GatewayAddresses.Count == 0)
                {
                    continue;
                }

                foreach (UnicastIPAddressInformation address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork
                        && address.Address.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        continue;
                    }

                    if (IPAddress.IsLoopback(address.Address))
                    {
                        continue;
                    }

                    if (!address.IsDnsEligible && bestGuess == null)
                    {
                        bestGuess = address;
                        continue;
                    }

                    if (address.PrefixOrigin == PrefixOrigin.Dhcp || address.PrefixOrigin == PrefixOrigin.RouterAdvertisement)
                    {
                        if (bestGuess?.IsDnsEligible != true)
                        {
                            bestGuess = address;
                            continue;
                        }
                    }
                }
            }

            return bestGuess;
        }
    }
}
