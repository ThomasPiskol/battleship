using System;
using System.Net;
using Rssdp;
using Storage;

namespace Network
{
    public class PublishServer
    {
        private SsdpDevicePublisher m_Publisher;

        private IStorageManager m_StorageManager;

        private SimpleHTTPServer m_HTTPServer;

        public PublishServer() : this(new StorageManager())
        {        
            
        }

        public PublishServer(IStorageManager storageManager)
        {
            m_StorageManager = storageManager;
            m_HTTPServer = new SimpleHTTPServer();
        }

        public void Publish()
        {
            // As this is a sample, we are only setting the minimum required properties.
            SsdpRootDevice deviceDefinition = new SsdpRootDevice()
            {
                CacheLifetime = TimeSpan.FromMinutes(30), //How long SSDP clients can cache this info.
                //Location = new Uri("http://mydevice/descriptiondocument.xml"), // Must point to the URL that serves your devices UPnP description document. 
                DeviceTypeNamespace = "my-namespace",
                DeviceType = "BattleShip.Server",
                FriendlyName = "Battleship Server",
                Manufacturer = "Me",
                ModelName = "MyCustomDevice",
                Uuid = m_StorageManager.InstanceID.ToString() // This must be a globally unique value that survives reboots etc. Get from storage or embedded hardware etc.
            };

            m_HTTPServer.Start(deviceDefinition);

            if (m_Publisher == null)
            {
                m_Publisher = new SsdpDevicePublisher();
            }

            m_Publisher.AddDevice(deviceDefinition);
        }

        public void Stop()
        {
            m_HTTPServer.Stop();
        }
    }
}
