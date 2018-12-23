using Network;
using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Client!");

            Discover discover = new Discover();
            discover.DeviceFoundEvent += DeviceFoundHandler;
            discover.BeginSearch();

            Console.ReadLine();
        }

        private static void DeviceFoundHandler(object sender, DeviceFoundEventArgs args)
        {
            Console.WriteLine($"Found device: {args.Usn}, {args.DescriptionLocation}");
        }
    }
}
