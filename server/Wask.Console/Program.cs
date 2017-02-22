using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Hosting;
using Wask.Lib;
using System;
using System.Net.Http;

namespace Wask.App
{
    class Program
    {
        static void Main()
        {
            string baseAddress = "http://localhost:9000/";

            // Start API host 
            Console.WriteLine("Open the application at: " + baseAddress);
            Console.WriteLine("Or browse the API documentation at: " + baseAddress + "swagger");

            using (WebApp.Start(new StartOptions(baseAddress), (appBuilder) =>
                {
                    new Startup(@"..\client").Configuration(appBuilder);
                }))
            {
                // Wire up SignalR
                var hubConnection = new HubConnection(baseAddress);
                IHubProxy eventHubProxy = hubConnection.CreateHubProxy("EventHub");
                hubConnection.Start().Wait();

                //HttpClient client = new HttpClient();
                //var r = client.GetAsync("http://localhost:9004").Result;

                Console.WriteLine("Press <enter> to stop server");
                Console.ReadLine();
            }

        }
    }
}
