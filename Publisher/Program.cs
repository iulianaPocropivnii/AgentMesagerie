using System;
using System.Text;
using Common;
using Newtonsoft.Json;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Publisher is running...");
            var publisherSocket = new PublisherSocket();
            publisherSocket.Connect(Settings.BROKER_IP, Settings.BROKIER_PORT);

            if (publisherSocket.IsConnected)
            {
                while(true)
                {
                    var payload = new Payload();
                    Console.WriteLine("Enter the topic: ");
                    payload.Topic = Console.ReadLine().ToLower();

                    Console.WriteLine("Enter the message: ");
                    payload.Message = Console.ReadLine();

                    var payloadString = JsonConvert.SerializeObject(payload);
                    byte[] data = Encoding.UTF8.GetBytes(payloadString);

                    publisherSocket.Send(data);
                }
            }

            Console.ReadLine();
        }
    }
}