using System.Web.Http;
using Microsoft.AspNet.SignalR;
using System.Net.Http;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Wask.Lib.Model;
using Wask.Lib.Utils;
using System.ServiceProcess;
using ServiceStack.Redis;
using Newtonsoft.Json;

namespace Wask.Lib.SignalR
{
    /// <summary>
    /// Service Info OWIN Controller
    /// </summary>
    [RoutePrefix(Constants.ServiceInfoChannel)]
    public class ServiceStatusController : ApiController
    {
        //private ServiceWatcher _serviceWatcher;

        public ServiceStatusController()
        {
            //_serviceWatcher = ServiceWatcher.Instance;
        }
        
        /// <summary>
        /// Get all known services and their current status
        /// </summary>
        /// <remarks>
        /// The possible values for status include:
        /// - Running
        /// - Stopped
        /// - [empty] This indicates that the service is not installed.
        ///  
        ///     GET /service
        /// 
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [Route("service")]
        [HttpGet]
        public IEnumerable<FixMessage> GetServices()
        {
            //List<FixMessage> services = _serviceWatcher.GetServices();
            return null; // services;
        }

        [Route("service/{profile}")]
        [HttpGet]
        public IEnumerable<FixMessage> GetServices(string profile)
        {
            string redisDoc;
            using (var redis = new RedisClient("localhost", 6379))
            {
                redisDoc = redis.GetValue(profile);
            }

            List<FixMessage> messages = JsonConvert.DeserializeObject<List<FixMessage>>(redisDoc);

            return messages;
        }
        
        /// <summary>
        /// Start or Stop a service
        /// </summary>
        /// <remarks>
        /// Use the service name as the ID.  
        /// Possible actions include:
        /// - start
        /// - stop
        ///  
        ///     POST '/serviceInfo/service/Mainline%20Audit?action=start'
        /// 
        /// </remarks>
        /// <response code="200">Returns if the action completed successfully</response>
        /// <response code="500">Returns if the action was unsuccessful</response>
        [Route("service/{name}")]
        [HttpPost]
        public IHttpActionResult PostService(string name, string action)
        {
            var returnMsg = "";

            List<FixMessage> messages = new List<FixMessage>() {
                new FixMessage() { symbol="GOOG", clOrdId="abc", lastShares=100, lastPx=12.3m },
                new FixMessage() { symbol="MSFT", clOrdId="def", lastShares=2200, lastPx=22.3m },
                new FixMessage() { symbol="AAPL", clOrdId="ghi", lastShares=4500, lastPx=132.1m }
            };
            string json = JsonConvert.SerializeObject(messages);

            using (var redis = new RedisClient("localhost", 6379))
            {
                redis.SetValue(name, json);
            }
            
            Console.WriteLine(returnMsg);
            return Ok(returnMsg);
        }
    }
}
