using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace AnarchocapitalismBot.Exchanges
{
    public partial class GatecoinExchange
    {
        private struct TickerEntry : ITickerEntry
        {
            [JsonProperty("currencyPair")]
            public string CurrencyPair;

            [JsonProperty("bid")]
            public decimal Bid;
            [JsonProperty("ask")]
            public decimal Ask;

            [JsonProperty("last")]
            public decimal Last;

            [JsonProperty("volume")]
            public decimal Volume;

            // ITickerEntry
            decimal ITickerEntry.HighestBidPrice => this.Bid;
            decimal ITickerEntry.LowestAskPrice  => this.Ask;
            decimal ITickerEntry.LastTradePrice  => this.Last;

            decimal ITickerEntry.Volume24Hours   => this.Volume;
        }
    }
}
