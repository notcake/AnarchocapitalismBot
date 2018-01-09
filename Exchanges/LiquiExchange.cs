using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    // Disable this one for now since the API barely works
    // [Exchange]
    public partial class LiquiExchange : IExchange
    {
        // IExchange
        public string Name => "Liqui";

        // Connection
        public bool Connected    { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IExchangeCurrencies Currencies { get; private set; } = null;

        // Trading pairs
        public decimal FeePercentage => 0.25m;
        private TradingPairType[,] tradingPairs = null;
        public TradingPairType[,] TradingPairs => (TradingPairType[,])this.tradingPairs.Clone();

        // LiquiExchange
        private string AllTradingPairs;

        public LiquiExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            LiquiExchange.Info info = await Json.DeserializeUrl<LiquiExchange.Info>("https://api.liqui.io/api/3/info");
            (this.Currencies, this.tradingPairs) = Util.GetSupportedCurrenciesFromTradingPairs(info.Pairs.Keys);

            this.AllTradingPairs = string.Join("-", info.Pairs.Keys);
            
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

            // The Liqui API is really shit and fails half the time.
            // This won't ever work reliably.
            Dictionary<string, LiquiExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, LiquiExchange.TickerEntry>>("https://api.liqui.io/api/3/ticker/" + this.AllTradingPairs + "?ignore_invalid=1");
            return Util.GetTicker(tradingPairs, this.Currencies);
        }

        public async Task<Exchanges.OrderBook> GetOrderBook((string, string) tradingPair)
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            Dictionary<string, LiquiExchange.OrderBook> orderBook = await Json.DeserializeUrl< Dictionary<string, LiquiExchange.OrderBook>>("https://api.liqui.io/api/3/depth/" + tradingPair.Item1.ToLower() + "_" + tradingPair.Item2.ToLower());
            return new Exchanges.OrderBook
            {
                Asks = orderBook[tradingPair.Item1.ToLower() + "_" + tradingPair.Item2.ToLower()].Ask.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).ToArray(),
                Bids = orderBook[tradingPair.Item1.ToLower() + "_" + tradingPair.Item2.ToLower()].Bid.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).ToArray()
            };
        }
    }
}
