using System.Web.Http;
using System;
using System.Collections.Generic;
using Wask.Lib.Model;
using ServiceStack.Redis;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Wask.Lib.SignalR
{
    /// <summary>
    /// Transaction OWIN Controller
    /// </summary>
    [RoutePrefix(Constants.TransactionChannel)]
    public class TransactionController : ApiController
    {
        public TransactionController()
        {
        }
        
        /// <summary>
        /// Get all FIX transactions
        /// </summary>
        /// <response code="200">If the items are successfuly returned</response>
        [Route("{session}")]
        [HttpGet]
        public IEnumerable<Transaction> Get(string session)
        {
            byte[][] redisDocs;
            using (var redis = new RedisClient("localhost", 6379))
            {
                redisDocs = redis.LRange("session:"+session, 0, -1);
            }
            if (redisDocs == null)
            {
                return null;
            }

            List<Transaction> transactions = new List<Transaction>();
            foreach(var doc in redisDocs)
            {
                string str = Encoding.ASCII.GetString(doc);
                transactions.Add(JsonConvert.DeserializeObject<Transaction>(str));
            }
            return transactions;
        }


        /// <summary>
        /// Send a new transaction
        /// </summary>
        /// <remarks>
        /// TODO figure out sending the POST body  
        /// 
        ///     POST '/transaction?symbol=ibm&lastShares=200'
        /// 
        /// </remarks>
        /// <response code="200">Returns if the action completed successfully</response>
        /// <response code="500">Returns if the action was unsuccessful</response>
        [Route("{session}")]
        [HttpPost]
        public IHttpActionResult Post(string session, [FromBody] JObject payload)
        {
            Console.WriteLine("####################");
            Console.WriteLine(payload);
            Console.WriteLine("####################");
            

            var message = new FixMessage();

            message.clOrdId = getFromPayload(payload, "ClOrdID (11)");
            message.symbol = getFromPayload(payload, "Symbol (55)");
            message.lastPx = Convert.ToDecimal(getFromPayload(payload, "Price (44)"));
            
            message.lastShares = 2000;
            message.leavesQty = 0;

            try
            {
                SendRequest("http://localhost:9999/"+session, message);

                //Fake - Fixalicious should do this
                Transaction tx = new Transaction();
                tx.direction = "sent";
                tx.id = RedisSubscription.GetNextId();
                tx.message = JsonConvert.SerializeObject(message, Formatting.None);

                using (var redis = new RedisClient("localhost", 6379))
                {
                    string json = JsonConvert.SerializeObject(tx, Formatting.None);
                    redis.RPush("session:" + session, Encoding.ASCII.GetBytes(json));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Ok();
        }
        
        private string getFromPayload(JObject payload, string key)
        {
            return (payload.GetValue(key) as JValue).Value.ToString();
        }

        private void SendRequest(string url, object fixMsg)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            HttpResponseMessage response = client.PostAsJsonAsync(url, fixMsg).Result;
            response.EnsureSuccessStatusCode();
        }


        /// <summary>
        /// Delete all transactions for a given session
        /// </summary>
        [Route("{session}")]
        [HttpDelete]
        public IHttpActionResult Delete(string session)
        {
            using (var redis = new RedisClient("localhost", 6379))
            {
                redis.Remove("session:" + session);
            }
            return Ok();
        }
    }
}
