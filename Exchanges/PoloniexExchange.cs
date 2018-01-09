using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    [Exchange]
    public partial class PoloniexExchange : IExchange
    {
        // IExchange
        public string Name => "Poloniex";

        // Connection
        public bool Connected    { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IExchangeCurrencies Currencies { get; private set; } = null;

        // Trading pairs
        public decimal FeePercentage => 0.25m;
        private TradingPairType[,] tradingPairs = null;
        public TradingPairType[,] TradingPairs => (TradingPairType[,])this.tradingPairs.Clone();

        // PoloniexExchange
        public PoloniexExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            Dictionary<string, object> tradingPairs = await Json.DeserializeUrl<Dictionary<string, object>>("https://poloniex.com/public?command=returnTicker");
            (this.Currencies, this.tradingPairs) = Util.GetSupportedCurrenciesFromTradingPairs(tradingPairs.Keys);
            
            this.Connected = true;

            return true;
        }

        public Task<bool> ConnectReadWrite()
        {
            throw new NotImplementedException();
        }

        public Task Disconnect()
        {
            this.Connected = false;

            this.Currencies = null;
            this.tradingPairs = null;

            return Task.CompletedTask;
        }

        // Currencies
        public async Task<Ticker[,]> GetTicker()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }
            
            Dictionary<string, PoloniexExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, PoloniexExchange.TickerEntry>>("https://poloniex.com/public?command=returnTicker");
            return Util.GetTicker(tradingPairs, this.Currencies);
        }

        public async Task<Exchanges.OrderBook> GetOrderBook((string, string) tradingPair)
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            PoloniexExchange.OrderBook orderBook = await Json.DeserializeUrl<PoloniexExchange.OrderBook>("https://poloniex.com/public?command=returnOrderBook&depth=10&currencyPair=" + tradingPair.Item1 + "_" + tradingPair.Item2);
            return new Exchanges.OrderBook
            {
                Asks = orderBook.Ask.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).Reverse().ToArray(),
                Bids = orderBook.Bid.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).ToArray()
            };
        }
    }
}
