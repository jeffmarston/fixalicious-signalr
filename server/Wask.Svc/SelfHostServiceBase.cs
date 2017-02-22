using System;
using System.ServiceProcess;
using Microsoft.Owin.Hosting;
using Wask.Lib;
using Microsoft.AspNet.SignalR.Client;

namespace Wask.Svc
{
    public partial class SelfHostServiceBase : ServiceBase
    {
        private IDisposable _webapi;

        public SelfHostServiceBase()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string baseAddress = "http://localhost:9000/";
            
            _webapi = WebApp.Start(new StartOptions(baseAddress), (appBuilder) =>
            {
                new Startup(@"..\client").Configuration(appBuilder);
            });

            var hubConnection = new HubConnection(baseAddress);
            IHubProxy eventHubProxy = hubConnection.CreateHubProxy("EventHub");
            hubConnection.Start().Wait();
        }

        protected override void OnStop()
        {
            _webapi?.Dispose();
        }
    }
}
