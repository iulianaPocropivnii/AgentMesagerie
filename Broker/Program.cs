using System;
using Common;

namespace Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Broker is running...");

            BrokerSocket socket = new BrokerSocket();
            socket.Start(Settings.BROKER_IP, Settings.BROKIER_PORT);

            var worker = new MessageWorker();
            Task.Factory.StartNew(worker.DoSendmessaeWork, TaskCreationOptions.LongRunning);

            Console.ReadLine();
        }
    }
}
