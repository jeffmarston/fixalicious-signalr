using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using Wask.Lib.BranchProfile;
using Wask.Lib.Model;

namespace Wask.Lib.Utils
{
    public static class ServiceUtils
    {
        public static void StartService(string svcName)
        {
            var serviceController = new ServiceController { ServiceName = svcName };
            if (serviceController.Status == ServiceControllerStatus.Stopped)
            {
                serviceController.Start();
            }
        }

        public static void StopService(string svcName)
        {
            var serviceController = new ServiceController { ServiceName = svcName };
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                serviceController.Stop();
            }
        }

        public static string GetServiceStatus(string svcName)
        {
            ServiceController svcController = ServiceController.GetServices().ToList().Find(o => o.ServiceName == svcName);
            if (svcController == null)
                return "";

            switch (svcController.Status)
            {
                case ServiceControllerStatus.StopPending:
                    return "Stop Pending";
                case ServiceControllerStatus.Stopped:
                    return "Stopped";
                case ServiceControllerStatus.StartPending:
                    return "Start Pending";
                case ServiceControllerStatus.Running:
                    return "Running";
            }
            return "";
        }

        private const string serverBin = @"\Server\Bin";

        private static string GetServicePrefix(string dirRoot)
        {
            string[] pieces = dirRoot.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            return (pieces.Length > 0) ? pieces[pieces.Length - 1] : string.Empty;
        }
        private static string GenerateServiceName(string prefix, string filename)
        {
            string p = string.IsNullOrEmpty(prefix) ? string.Empty : prefix + " ";

            string s = filename;
            if (s.StartsWith("Eze.", StringComparison.InvariantCultureIgnoreCase))
            {
                s = s.Substring(4);
            }

            if (s.EndsWith(".Svc.exe", StringComparison.InvariantCultureIgnoreCase))
            {
                s = s.Substring(0, s.Length - 8);
            }
            s = s.Replace(".", " ");
            return p + s;
        }

        public static List<Service> GetAllEzeServices()
        {
            var dirs = ProfileDal.Profiles.Select(o => o.path).ToArray();
            var services = new List<Service>();

            foreach (var dir in dirs)
            {
                //Get all win services
                var editorPath = dir + serverBin;
                if (Directory.Exists(editorPath))
                {
                    foreach (string path in Directory.GetFiles(editorPath))
                    {
                        var filename = Path.GetFileName(path.Replace("\"", ""));

                        if (filename.StartsWith("eze.", StringComparison.CurrentCultureIgnoreCase) 
                            && filename.EndsWith(".svc.exe", StringComparison.CurrentCultureIgnoreCase))
                        {
                            var svcName = GenerateServiceName(GetServicePrefix(dir), filename);
                            services.Add(new Service() { path = path, name = svcName });
                        }
                    }
                }
            }

            //var handles = ServiceController.GetServices().Where( o=> o.ServiceName.StartsWith("Mainline") )
            //    .Select(o => o.ServiceHandle).ToList();

            //ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
            //ManagementObjectCollection collection = searcher.Get();

            //foreach (ManagementObject obj in collection)
            //{
            //    string name = obj["Name"] as string;
            //    string pathName = obj["PathName"] as string;
            //    uint processId = (uint)obj["ProcessId"];

            //    var filename = Path.GetFileName(pathName.Replace("\"", ""));
            //    if (filename.ToLower().StartsWith("eze."))
            //    {
            //        var p = Process.GetProcessById((int)processId);
            //        services.Add(new Service() { name = name, pid = p.Id });
            //    }
            //}

            return services;
        }

        public static void InstallAndStart(string svcName, string svcDesc, string svcPath)
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller { Parent = processInstaller };

            var context = new InstallContext("install.log", null);
            context.Parameters["assemblypath"] = svcPath;
            serviceInstaller.Context = context;
            serviceInstaller.ServiceName = svcName;
            serviceInstaller.DisplayName = svcDesc;
            serviceInstaller.Description = svcDesc;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.Parent = processInstaller;
            serviceInstaller.Install(new ListDictionary());

            var serviceController = new ServiceController { ServiceName = svcName };
            serviceController.Start();
        }
    }
}