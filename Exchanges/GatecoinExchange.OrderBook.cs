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
        public struct OrderBook
        {
            [JsonProperty("asks")]
            public OrderBookEntry[] Ask;
            [JsonProperty("bids")]
            public OrderBookEntry[] Bid;
        }
    }
}
