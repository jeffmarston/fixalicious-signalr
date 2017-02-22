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

    public class FixMessage
    {
        public string clOrdId { get; set; }
        public string symbol { get; set; }
        public int fillStatus { get; set; }
        public int lastShares { get; set; }
        public int cumQty { get; set; }
        public decimal lastPx { get; set; }
        public decimal avgPx { get; set; }
        public int leavesQty { get; set; }
        public int side { get; set; }

        public override string ToString()
        {
            return $"{symbol} - {clOrdId}";
        }
    }


}
