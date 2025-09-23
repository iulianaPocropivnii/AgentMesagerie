using System;
using Common;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Subscriber is running............");
            string topic;
            Console.WriteLine("Enter the topic you want to subscribe to: ");
            topic = Console.ReadLine().ToLower();
            var subscriberSocket = new SubscriberSocket(topic);
            subscriberSocket.Connect(Settings.BROKER_IP, Settings.BROKIER_PORT);
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}