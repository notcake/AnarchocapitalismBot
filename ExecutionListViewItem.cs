using AnarchocapitalismBot.Exchanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot
{
    public struct ExecutionListViewItem
    {
        public IExchange Exchange { get; set; }

        public (string, string) TradingPair { get; set; }
        public string TradingPairName => this.TradingPair.Item1 + "/" + this.TradingPair.Item2;
        public Ticker Ticker { get; set; }
        public OrderBook OrderBook { get; set; }
        public OrderBookEntry BuyOrder  { get; set; }
        public OrderBookEntry SellOrder { get; set; }

        public TradeType Type { get; set; }
        public decimal Price { get; set; }

        public string SourceCurrency      => this.Type == TradeType.Buy ? this.TradingPair.Item2 : this.TradingPair.Item1;
        public string DestinationCurrency => this.Type == TradeType.Buy ? this.TradingPair.Item1 : this.TradingPair.Item2;
        public decimal SourceQuantity { get; set; }
        public decimal DestinationQuantity { get; set; }
    }
}
