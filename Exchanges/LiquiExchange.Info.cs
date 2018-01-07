using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace AnarchocapitalismBot.Exchanges
{
    public partial class LiquiExchange
    {
        private struct Info
        {
            [JsonProperty("server_time")]
            public uint ServerTime;

            [JsonProperty("pairs")]
            public Dictionary<string, object> Pairs;
        }
    }
}
