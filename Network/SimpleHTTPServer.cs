using Rssdp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public class SimpleHTTPServer
    {
        private byte[] m_Payload;

        private HttpListener m_HTTPListener;

        public const string URL = "battleship/discover";

        public SimpleHTTPServer()
        {
            m_HTTPListener = new HttpListener();
        }

        public void Start(SsdpRootDevice rootDevice)
        {
            m_HTTPListener.Prefixes.Clear();
            int port = GetAvailablePort();
            m_HTTPListener.Prefixes.Add($"http://+:{port}/{URL}/");
            string hostname = GetHostName();
            rootDevice.UrlBase = new Uri($"http://{hostname}:{port}/{URL}/");
            rootDevice.Location = new Uri($"http://{hostname}:{port}/{URL}/descriptiondocument.xml");
            m_Payload = Encoding.UTF8.GetBytes(rootDevice.ToDescriptionDocument());

            Task.Run(StartAsync);
        }

        public async Task StartAsync()
        {
            
            try
            {
                m_HTTPListener.Start();
                while(m_HTTPListener.IsListening)
                {
                    HttpListenerContext context = await m_HTTPListener.GetContextAsync();
                    await ProcessRequestAsync(context);
                }
            }
            catch(Exception ex)
            {
                // ToDo: Error Handling
                throw;
            }
        }

        public void Stop()
        {
            m_HTTPListener.Stop();
        }


        internal async Task ProcessRequestAsync(HttpListenerContext context)
        {
            if(! context.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            await context.Response.OutputStream.WriteAsync(m_Payload, 0, m_Payload.Length);
            await context.Response.OutputStream.FlushAsync();
            context.Response.OutputStream.Close();
        }

        public static int GetAvailablePort()
        {
            int startPort = 65535;
            int endPort = 49152;
            
            for(int i = startPort; i >= endPort; i--)
            {
                if (!IsPortBlocked(i))
                {
                    return i;                    
                }
            }

            // ToDo: Error Handling
            return -1;
        }

        public static bool IsPortBlocked(int port)
        {
            bool isUsed = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endPoints = ipProperties.GetActiveTcpListeners();

            foreach(IPEndPoint endPoint in endPoints)
            {
                if(endPoint.Port == port)
                {
                    isUsed = true;
                    break;
                }
            }

            return isUsed;
        }


        internal string GetHostName()
        {
            UnicastIPAddressInformation localIP = FindLocalIp();
            if(localIP == null)
            {
                return Environment.MachineName;
            }

            if(localIP.Address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return $"[{localIP.Address.ToString()}]";
            }

            return localIP.Address.ToString();
        }

        internal UnicastIPAddressInformation FindLocalIp()
        {
            UnicastIPAddressInformation bestGuess = null;
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach(NetworkInterface network in networkInterfaces)
            {
                if(network.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                IPInterfaceProperties properties = network.GetIPProperties();

                if(properties.GatewayAddresses.Count == 0)
                {
                    continue;
                }

                foreach(UnicastIPAddressInformation address in properties.UnicastAddresses)
                {
                    if( address.Address.AddressFamily != AddressFamily.InterNetwork && 
                        address.Address.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        continue;
                    }

                    if (IPAddress.IsLoopback(address.Address))
                    {
                        continue;
                    }

                    if (!address.IsDnsEligible)
                    {
                        if(bestGuess == null)
                        {
                            bestGuess = address;
                            continue;
                        }
                    }

                    if(address.PrefixOrigin == PrefixOrigin.Dhcp || address.PrefixOrigin == PrefixOrigin.RouterAdvertisement)
                    {
                        if(bestGuess == null || !bestGuess.IsDnsEligible)
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
