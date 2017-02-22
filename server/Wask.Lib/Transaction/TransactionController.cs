using System.Web.Http;
using System;
using System.Collections.Generic;
using Wask.Lib.Model;
using ServiceStack.Redis;
using Newtonsoft.Json;

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
            string redisDoc;
            using (var redis = new RedisClient("localhost", 6379))
            {
                redisDoc = redis.GetValue(session);
            }
            if (redisDoc == null)
            {
                return null;
            }

            List<Transaction> messages = JsonConvert.DeserializeObject<List<Transaction>>(redisDoc);
            return messages;
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
        public IHttpActionResult Post(string session, string symbol)
        {
            var returnMsg = "";

            List<Transaction> messages = new List<Transaction>();
            messages.AddRange(Get(session) ?? new List<Transaction>());
            messages.Add(new Transaction() { direction="sent", symbol = symbol });
            string json = JsonConvert.SerializeObject(messages);

            using (var redis = new RedisClient("localhost", 6379))
            {
                redis.SetValue(session, json);
            }

            Console.WriteLine(returnMsg);
            return Ok(returnMsg);
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
                redis.Remove(session);
            }
            return Ok();
        }
    }
}
