using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    [Exchange]
    public partial class GateIoExchange : IExchange
    {
        // IExchange
        public string Name => "gate.io";

        // Connection
        public bool Connected    { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IExchangeCurrencies Currencies { get; private set; } = null;

        // Trading pairs
        public decimal FeePercentage => 0.2m;
        private TradingPairType[,] tradingPairs = null;
        public TradingPairType[,] TradingPairs => (TradingPairType[,])this.tradingPairs.Clone();

        // GateIoExchange
        public GateIoExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            string[] tradingPairs = await Json.DeserializeUrl<string[]>("http://data.gate.io/api2/1/pairs");
            (this.Currencies, this.tradingPairs) = Util.GetSupportedCurrenciesFromTradingPairs(tradingPairs);
            
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

        // Trading pairs
        public async Task<Ticker[,]> GetTicker()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            Dictionary<string, GateIoExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, GateIoExchange.TickerEntry>>("http://data.gate.io/api2/1/tickers");
            return Util.GetTicker(tradingPairs, this.Currencies);
        }

        public async Task<Exchanges.OrderBook> GetOrderBook((string, string) tradingPair)
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            GateIoExchange.OrderBook orderBook = await Json.DeserializeUrl<GateIoExchange.OrderBook>("http://data.gate.io/api2/1/orderBook/" + tradingPair.Item1.ToLower() + "_" + tradingPair.Item2.ToLower());
            return new Exchanges.OrderBook
            {
                Asks = orderBook.Ask.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).Reverse().ToArray(),
                Bids = orderBook.Bid.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).ToArray()
            };
        }
    }
}
