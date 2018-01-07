using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace AnarchocapitalismBot.Exchanges
{
    public partial class PoloniexExchange
    {
		private struct TickerEntry
        {
            [JsonProperty("lowestAsk")]
            public decimal LowestAsk;
            [JsonProperty("highestBid")]
            public decimal HighestBid;

            [JsonProperty("last")]
            public decimal LastTrade;

            [JsonProperty("percentChange")]
            public decimal PercentageChange;
        }
    }
}
