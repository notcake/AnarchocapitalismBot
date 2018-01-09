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
        private struct TickerEntry : ITickerEntry
        {
            [JsonProperty("symbol")]
            public string Symbol;

            [JsonProperty("bidPrice")]
            public decimal BidPrice;
            [JsonProperty("bidQty")]
            public decimal BidQuantity;
            [JsonProperty("askPrice")]
            public decimal AskPrice;
            [JsonProperty("askQty")]
            public decimal AskQuantity;

            // ITickerEntry
            decimal ITickerEntry.HighestBidPrice => this.BidPrice;
            decimal ITickerEntry.LowestAskPrice  => this.AskPrice;
            decimal ITickerEntry.LastTradePrice  => 0.5m * (this.BidPrice + this.AskPrice);

            decimal ITickerEntry.Volume24Hours   => -999999;
        }
    }
}
