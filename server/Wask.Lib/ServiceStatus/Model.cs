using Newtonsoft.Json;
using System.Collections.Generic;

namespace Wask.Lib.Model
{
    public class ConfigObject
    {
        public static ConfigObject CreateMachineList(string json)
        {
            ConfigObject config = JsonConvert.DeserializeObject<ConfigObject>(json);
            return config;
        }
    }

    public class Profile
    {
        public string name { get; set; }
        public string path { get; set; }
    }

    public class Restart
    {
        public int restartTier { get; set; }
        public bool supportsPause { get; set; }
        public bool daily { get; set; }
        public string timeout { get; set; }
    }

    public class Service
    {
        public string path { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string instance { get; set; }
        public bool enabled { get; set; }
        public string exe { get; set; }
        public int pid { get; set; }
        public Restart restart { get; set; }

        public override string ToString()
        {
            return $"{name} - {status}";
        }
    }


}
