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

    public class Session
    {
        public string name { get; set; }
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
    }

    public class Transaction
    {
        public string id { get; set; }
        public string direction { get; set; }
        public string message { get; set; }

        public override string ToString()
        {
            return $"{symbol} - {clOrdId}";
        }
    }


}
