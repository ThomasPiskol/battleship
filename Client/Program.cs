using Network;
using System;

namespace Client
{
    internal class Program
    {
#pragma warning disable RCS1163 // Unused parameter.
        private static void Main(string[] args)
#pragma warning restore RCS1163 // Unused parameter.
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
