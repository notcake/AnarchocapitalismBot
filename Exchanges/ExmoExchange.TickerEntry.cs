﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace AnarchocapitalismBot.Exchanges
{
    public partial class ExmoExchange
    {
        private struct TickerEntry : ITickerEntry
        {
            [JsonProperty("sell_price")]
            public decimal SellPrice;
            [JsonProperty("buy_price")]
            public decimal BuyPrice;

            [JsonProperty("last_trade")]
            public decimal LastTrade;

            [JsonProperty("vol")]
            public decimal Volume;

            // ITickerEntry
            decimal ITickerEntry.HighestBidPrice => this.BuyPrice;
            decimal ITickerEntry.LowestAskPrice  => this.SellPrice;
            decimal ITickerEntry.LastTradePrice  => this.LastTrade;

			decimal ITickerEntry.Volume24Hours   => this.Volume;
        }
    }
}
