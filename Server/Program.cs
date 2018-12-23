using System;
using Network;


namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Server!");

            PublishServer publisher = new PublishServer();
            publisher.Publish();

            Console.ReadLine();
        }
    }
}
