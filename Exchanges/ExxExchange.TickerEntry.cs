using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace AnarchocapitalismBot.Exchanges
{
    public partial class ExxExchange
    {
        private struct TickerEntry : ITickerEntry
        {
            [JsonProperty("sell")]
            public decimal Sell;
            [JsonProperty("buy")]
            public decimal Buy;

            [JsonProperty("last")]
            public decimal Last;

            // ITickerEntry
            decimal ITickerEntry.HighestBidPrice => this.Buy;
            decimal ITickerEntry.LowestAskPrice  => this.Sell;
            decimal ITickerEntry.LastTradePrice  => this.Last;
        }
    }
}
