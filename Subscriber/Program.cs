using System;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Subscriber is running...");
            var subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(Common.Settings.BROKER_IP, Common.Settings.BROKIER_PORT);

            Console.WriteLine("Enter comma-separated topics or press 'q' to exit::");
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.Trim().ToLower() == "q") break;

                var topics = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var topic in topics)
                {
                    subscriberSocket.Subscribe(topic.ToLower());
                }
            }

            subscriberSocket.Close();
        }
    }
}
