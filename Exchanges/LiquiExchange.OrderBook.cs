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
        public struct OrderBook
        {
            [JsonProperty("asks")]
            public decimal[][] Ask;
            [JsonProperty("bids")]
            public decimal[][] Bid;
        }
    }
}
