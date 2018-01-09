using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    [Exchange]
    public partial class ExxExchange : IExchange
    {
        // IExchange
        public string Name => "EXX";

        // Connection
        public bool Connected { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IExchangeCurrencies Currencies { get; private set; } = null;

        // Trading pairs
        public decimal FeePercentage => 0.1m;
        private TradingPairType[,] tradingPairs = null;
        public TradingPairType[,] TradingPairs => (TradingPairType[,])this.tradingPairs.Clone();

        // ExxExchange
        public ExxExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            Dictionary<string, object> tradingPairs = await Json.DeserializeUrl<Dictionary<string, object>>("https://api.exx.com/data/v1/tickers");
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

            Dictionary<string, ExxExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, ExxExchange.TickerEntry>>("https://api.exx.com/data/v1/tickers");
            Ticker[,] ticker = Util.GetTicker(tradingPairs, this.Currencies);

            // QASH/ETH is completely illiquid, so ignore it
            ticker[this.Currencies.IndexOf("QASH"), this.Currencies.IndexOf("ETH")] = new Ticker();
            ticker[this.Currencies.IndexOf("ETH"), this.Currencies.IndexOf("QASH")] = new Ticker();

            return ticker;
        }

        public async Task<Exchanges.OrderBook> GetOrderBook((string, string) tradingPair)
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            ExxExchange.OrderBook orderBook = await Json.DeserializeUrl<ExxExchange.OrderBook>("https://api.exx.com/data/v1/depth?currency=" + tradingPair.Item1.ToLower() + "_" + tradingPair.Item2.ToLower());
            return new Exchanges.OrderBook
            {
                Asks = orderBook.Ask.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).Reverse().ToArray(),
                Bids = orderBook.Bid.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).ToArray()
            };
        }
    }
}
