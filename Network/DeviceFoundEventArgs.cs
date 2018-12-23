using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class DeviceFoundEventArgs : EventArgs
    {
        public string Usn { get; set; }

        public string DescriptionLocation { get; set; }

        public string FriendlyName { get; set; }
    }
}
