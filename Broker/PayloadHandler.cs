using System;
using System.Text;
using System.Linq;
using Common;
using Newtonsoft.Json;

namespace Broker
{
    class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes, ConnectionInfo connectionInfo)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);

            if (payloadString.StartsWith("subscribe#"))
            {
                var topic = payloadString.Split(new[] { "subscribe#" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(topic))
                {
                    if (connectionInfo.Topics == null)
                        connectionInfo.Topics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    connectionInfo.Topics.Add(topic);              
                    ConnectionsStorage.AddOrUpdateConnection(connectionInfo);
                }
            }

            else
            {
                Payload payload = JsonConvert.DeserializeObject<Payload>(payloadString);
                PayloadStorage.AddPayload(payload);
            }
        }
    }
}
