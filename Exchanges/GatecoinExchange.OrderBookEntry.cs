using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public partial class GatecoinExchange
    {
        public struct OrderBookEntry
        {
            [JsonProperty("price")]
            public decimal Price;
            [JsonProperty("volume")]
            public decimal Volume;
        }
    }
}
