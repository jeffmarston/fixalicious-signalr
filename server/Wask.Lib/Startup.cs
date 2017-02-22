using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.ServiceProcess;
using System.Web.Http;
using Wask.Lib.Model;
using Wask.Lib.Utils;

namespace Wask.Lib
{
    public class Startup
    {
        private string _staticFilePath;
        private List<Service> _services = new List<Service>();


        public Startup(string options)
        {
            _staticFilePath = options;
            ServiceWatcher.Init(ServiceUtils.GetAllEzeServices());
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            var defaultSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = true } }
            };

            JsonConvert.DefaultSettings = () => { return defaultSettings; };
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings = defaultSettings;

            config.Routes.MapHttpRoute(
                "api",
                "api/{controller}/{id}",
                defaults: new { controller = "Machines", id = RouteParameter.Optional }
                );

            config.MapHttpAttributeRoutes();

            config.EnableSwagger(c => {
                //c.InjectStylesheet(containingAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testStyles1.css");
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);

                c.IncludeXmlComments(commentsFile);

                c.SingleApiVersion("v1","API Documentation");       
            }).EnableSwaggerUi();

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            app.MapSignalR();

            // static content hosting
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = { "index.html" } },
                FileSystem = new PhysicalFileSystem(_staticFilePath)
            };
            app.UseFileServer(options);
        }
    }
}