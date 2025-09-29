using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Broker
{
    internal class MessageWorker
    {
        private const int TIME_TO_SLEEP = 500;
        public void DoSendmessaeWork()
        {
            while (true)
            {
                while (!PayloadStorage.IsEmpty())
                {
                    var payload = PayloadStorage.GetNext();
                    if (payload != null)
                    {
                        var connections = ConnectionsStorage.GetConnectionsByTopic(payload.Topic);
                        foreach (var connection in connections)
                        {
                            var payloadString = JsonConvert.SerializeObject(payload);
                            byte[] data = Encoding.UTF8.GetBytes(payloadString);
                            connection.Socket.Send(data);
                        }
                    }
                }
                Thread.Sleep(TIME_TO_SLEEP);
            }
        }
    }
}
