using Microsoft.AspNet.SignalR;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private void ListenToRedis(object sender, DoWorkEventArgs e)
        {
            //using (var redisConsumer = new RedisClient("localhost", 6379))
            //using (var subscription = redisConsumer.CreateSubscription())
            //{
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

                _context.Clients.Group(Constants.TransactionChannel).OnEvent(Constants.TransactionChannel, new ChannelEvent
                {
                    ChannelName = Constants.TransactionChannel,
                    Name = "BAX",
                    Data = "---=== oooOOOooo ===---"
                });


            };

            subscription.SubscribeToChannels("refresh"); //blocking
        }
        //}
    }
}
