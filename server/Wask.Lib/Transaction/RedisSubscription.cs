using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wask.Lib.Model;
using Wask.Lib.SignalR;

namespace Wask.Lib
{
    public class RedisSubscription : IDisposable
    {
        private RedisClient redisConsumer;
        private IRedisSubscription subscription;
        private IHubContext _context;

        public RedisSubscription()
        {
            redisConsumer = new RedisClient("localhost", 6379);
            subscription = redisConsumer.CreateSubscription();
            _context = GlobalHost.ConnectionManager.GetHubContext<EventHub>();

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += ListenToRedis;

            bw.RunWorkerAsync();

        }

        public void Dispose()
        {
            subscription.UnSubscribeFromAllChannels();
            subscription.Dispose();
            redisConsumer.Dispose();
            Console.WriteLine("RedisSubscription Disposed");
        }

        public static int GetNextId()
        {
            return idIncrement++;
        }

        private static int idIncrement = 0;

        private void ListenToRedis(object sender, DoWorkEventArgs e)
        {
            subscription.OnSubscribe = channel =>
            {
                Console.WriteLine("Subscribed to '{0}'", channel);
            };
            subscription.OnUnSubscribe = channel =>
            {
                Console.WriteLine("UnSubscribed from '{0}'", channel);
            };
            subscription.OnMessage = (channel, msg) =>
            {
                Console.WriteLine("Received '{0}' from channel '{1}'", msg, channel);

                var jObj = ParseFixText(msg);
                //if ((jObj.GetValue("MsgType (35)") as JValue).Value.ToString() == "D")
                {
                    Transaction tx = new Transaction();
                    tx.direction = "Receieved";
                    tx.id = GetNextId();
                    tx.message = jObj.ToString();
                    Console.WriteLine("JSON = " + tx.message);

                    _context.Clients.Group(Constants.TransactionChannel).OnEvent(Constants.TransactionChannel, new ChannelEvent
                    {
                        ChannelName = Constants.TransactionChannel,
                        Name = channel,
                        Data = tx
                    });
                }
            };

            subscription.SubscribeToChannels("BAXA"); //blocking
        }

        private JObject ParseFixText(string text)
        {
            var jObject = new JObject();

            var lines = text.Split('\n');
            foreach(var line in lines)
            {
                int colonIdx = line.IndexOf(":");
                if (colonIdx > 0)
                {
                    jObject.Add(line.Substring(0, colonIdx).Trim(), line.Substring(colonIdx+1).Trim());
                }
            }

            return jObject;
        }
    }
}
