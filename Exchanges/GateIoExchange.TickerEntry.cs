using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace AnarchocapitalismBot.Exchanges
{
    public partial class GateIoExchange
    {
		private struct TickerEntry : ITickerEntry
        {
            [JsonProperty("lowestAsk")]
            public decimal LowestAsk;
            [JsonProperty("highestBid")]
            public decimal HighestBid;

            [JsonProperty("last")]
            public decimal Last;

            [JsonProperty("baseVolume")]
            public decimal BaseVolume;

            [JsonProperty("percentChange")]
            public decimal PercentageChange;

            // ITickerEntry
            decimal ITickerEntry.HighestBidPrice => this.HighestBid;
            decimal ITickerEntry.LowestAskPrice  => this.LowestAsk;
            decimal ITickerEntry.LastTradePrice  => this.Last;

			decimal ITickerEntry.Volume24Hours   => this.BaseVolume;
        }
    }
}
