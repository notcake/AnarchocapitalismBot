using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public partial class ExmoExchange
    {
        public struct OrderBook
        {
            [JsonProperty("ask_quantity")]
            public decimal AskQuantity;
            [JsonProperty("ask_amount")]
            public decimal AskAmount;
            [JsonProperty("ask_top")]
            public decimal AskTop;
            [JsonProperty("bid_quantity")]
            public decimal BidQuantity;
            [JsonProperty("bid_amount")]
            public decimal BidAmount;
            [JsonProperty("bid_top")]
            public decimal BidTop;

            [JsonProperty("ask")]
            public decimal[][] Ask;
            [JsonProperty("bid")]
            public decimal[][] Bid;
        }
    }
}
