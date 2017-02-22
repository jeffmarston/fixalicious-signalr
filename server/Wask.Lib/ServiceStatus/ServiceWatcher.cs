using Wask.Lib.Utils;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using Wask.Lib.SignalR;
using Microsoft.AspNet.SignalR;

namespace Wask.Lib.Model
{
    public class ServiceWatcher
    {
        private List<Service> _knownStates = new List<Service>();
        private List<Service> _servicesToMonitor;
        private Timer _pollingTimer;
        private IHubContext _context;
        private string _channel = Constants.ServiceInfoChannel;
        

        internal static void Init(List<Service> services)
        {
            _instance = new ServiceWatcher(services);
        }

        private static ServiceWatcher _instance;
        public static ServiceWatcher Instance
        {
            get { return _instance; }
        }

        private ServiceWatcher(List<Service> servicesToMonitor)
        {
            _servicesToMonitor = servicesToMonitor;
            foreach (var svc in _servicesToMonitor)
            {
                svc.status = ServiceUtils.GetServiceStatus(svc.name);
                _knownStates.Add(svc);
            }
            _context = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            _pollingTimer = new Timer(DoPoll, null, 0, 3000);
        }

        private void DoPoll(object state)
        {
            foreach (var svc in _servicesToMonitor)
            {
                var newStatus = ServiceUtils.GetServiceStatus(svc.name);
                var prevState = _knownStates.Find(o => o.name == svc.name);
                if (prevState != null && prevState.status != newStatus)
                {
                    Console.WriteLine($"Publish: [{svc.name}] is now {newStatus}.");
                    prevState.status = newStatus;                    
                    PublishServiceChange("serviceInfo.status", new List<Service>() { svc });
                }
            }
        }
        private void PublishServiceChange(string eventName, List<Service> deltas)
        {
            _context.Clients.Group(_channel).OnEvent(Constants.ServiceInfoChannel, new ChannelEvent
            {
                ChannelName = Constants.ServiceInfoChannel,
                Name = eventName,
                Data = deltas
            });
        }

        public List<Service> GetServices()
        {
            return _knownStates;
        }
    }
}
