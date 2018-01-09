using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace AnarchocapitalismBot.Exchanges
{
    public partial class BinanceExchange
    {
        public struct OrderBook
        {
            [JsonProperty("lastUpdateId")]
            public ulong LastUpdateId;

            [JsonProperty("asks")]
            public object[][] Ask;
            [JsonProperty("bids")]
            public object[][] Bid;
        }
    }
}
