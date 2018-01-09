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
            [JsonProperty("quoteVolume")]
            public decimal QuoteVolume;

            [JsonProperty("percentChange")]
            public decimal PercentageChange;

            // ITickerEntry
            decimal ITickerEntry.HighestBidPrice => Math.Min(this.HighestBid, this.LowestAsk); // fixup the shitty data returned to be slightly more accurate
            decimal ITickerEntry.LowestAskPrice  => Math.Max(this.HighestBid, this.LowestAsk); // fixup the shitty data returned to be slightly more accurate
            decimal ITickerEntry.LastTradePrice  => this.Last;

			decimal ITickerEntry.Volume24Hours   => this.QuoteVolume; // wrong way around
        }
    }
}
