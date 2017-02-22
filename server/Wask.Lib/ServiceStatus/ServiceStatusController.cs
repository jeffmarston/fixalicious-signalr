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

namespace Wask.Lib.SignalR
{
    /// <summary>
    /// Service Info OWIN Controller
    /// </summary>
    [RoutePrefix(Constants.ServiceInfoChannel)]
    public class ServiceStatusController : ApiController
    {
        private ServiceWatcher _serviceWatcher;

        public ServiceStatusController()
        {
            _serviceWatcher = ServiceWatcher.Instance;
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
        public IEnumerable<Service> GetServices()
        {
            List<Service> services = _serviceWatcher.GetServices();
            return services;
        }

        [Route("service/{profile}")]
        [HttpGet]
        public IEnumerable<Service> GetServices(string profile)
        {
            List<Service> services = _serviceWatcher.GetServices();
            return services.Where(o=> o.path.Contains(profile) );
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
            try
            {
                List<Service> services = _serviceWatcher.GetServices();
                var svc = services.Find(o => string.Compare(o.name, name, true) == 0);
                if (svc == null)
                {
                    return BadRequest("Service not found: " + name);
                }

                if (svc != null && action != null)
                {
                    if (action.ToLower() == "start")
                    {
                        ServiceUtils.StartService(svc.name);
                        returnMsg = "Starting Service: " + svc.name;
                    }
                    else if (action.ToLower() == "stop")
                    {
                        ServiceUtils.StopService(svc.name);
                        returnMsg = "Stoping Service: " + svc.name;
                    }
                    else if (action.ToLower() == "install")
                    {
                        ServiceUtils.InstallAndStart(svc.name, svc.name, svc.path);
                        returnMsg = "Installing Service: " + svc.name;
                    }
                    else
                    {
                        return BadRequest("Unsupported Action: " + action);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException.Message == "Access is denied")
                {
                    Debug.WriteLine("Access denied");
                    throw new HttpResponseException(
                        Request.CreateErrorResponse(HttpStatusCode.Unauthorized, e.InnerException.Message));
                }
            }
            Console.WriteLine(returnMsg);
            return Ok(returnMsg);
        }
    }
}
