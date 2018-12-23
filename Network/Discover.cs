using Rssdp;
using System;

namespace Network
{
    public class Discover
    {
        // Define m_DeviceLocator as a field so it doesn't get GCed after the method ends, and it can
        // continue to listen for notifications until it is explicitly stopped 
        // (with a call to m_DeviceLocator.StopListeningForNotifications();)
        private SsdpDeviceLocator m_DeviceLocator;

        public delegate void DeviceFoundEventHandler(object sender, DeviceFoundEventArgs args);

        public event DeviceFoundEventHandler DeviceFoundEvent;

        // Call this method from somewhere in your code to start the search.
        public void BeginSearch()
        {
            m_DeviceLocator = new SsdpDeviceLocator();

            // (Optional) Set the filter so we only see notifications for devices we care about 
            // (can be any search target value i.e device type, uuid value etc - any value that appears in the 
            // DiscoverdSsdpDevice.NotificationType property or that is used with the searchTarget parameter of the Search method).
            m_DeviceLocator.NotificationFilter = "upnp:rootdevice";

            // Connect our event handler so we process devices as they are found
            m_DeviceLocator.DeviceAvailable += deviceLocator_DeviceAvailable;

            // Enable listening for notifications (optional)
            m_DeviceLocator.StartListeningForNotifications();

            // Perform a search so we don't have to wait for devices to broadcast notifications 
            // again to get any results right away (notifications are broadcast periodically).
            m_DeviceLocator.SearchAsync();            
        }

        public void EndSearch()
        {
            if(m_DeviceLocator == null)
            {
                return;
            }

            m_DeviceLocator.DeviceAvailable -= deviceLocator_DeviceAvailable;
            m_DeviceLocator.StopListeningForNotifications();

        }

        // Process each found device in the event handler
        async void deviceLocator_DeviceAvailable(object sender, DeviceAvailableEventArgs e)
        {
            if(DeviceFoundEvent == null)
            {
                return;
            }
            //Device data returned only contains basic device details and location of full device description.
            //Console.WriteLine("Found " + e.DiscoveredDevice.Usn + " at " + e.DiscoveredDevice.DescriptionLocation.ToString());

            //Can retrieve the full device description easily though.
            try
            {
                SsdpDevice fullDevice = await e.DiscoveredDevice.GetDeviceInfo();
            }
            catch(Exception ex)
            {
                // ToDo: Error Handling
                return;
            }
            
            
            DeviceFoundEventArgs eventArgs = new DeviceFoundEventArgs();
            eventArgs.Usn = e.DiscoveredDevice.Usn;
            eventArgs.DescriptionLocation = e.DiscoveredDevice.DescriptionLocation.ToString();
            //eventArgs.FriendlyName = fullDevice.FriendlyName;

            DeviceFoundEvent(this, eventArgs);
        }

    }
}
