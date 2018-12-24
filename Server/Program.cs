using System;
using Network;

namespace Server
{
    internal class Program
    {
#pragma warning disable RCS1163 // Unused parameter.
        private static void Main(string[] args)
#pragma warning restore RCS1163 // Unused parameter.
        {
            Console.WriteLine("Hello Server!");

            PublishServer publisher = new PublishServer();
            publisher.Publish();

            Console.ReadLine();
        }
    }
}
