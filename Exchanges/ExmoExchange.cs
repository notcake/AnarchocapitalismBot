using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    [Exchange]
    public partial class ExmoExchange : IExchange
    {
        // IExchange
        public string Name => "EXMO";

        // Connection
        public bool Connected    { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IExchangeCurrencies Currencies { get; private set; } = null;

        // Trading pairs
        public decimal FeePercentage => 0.2m;
        private TradingPairType[,] tradingPairs = null;
        public TradingPairType[,] TradingPairs => (TradingPairType[,])this.tradingPairs.Clone();

        // ExmoExchange
        public ExmoExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            Dictionary<string, object> tradingPairs = await Json.DeserializeUrl<Dictionary<string, object>>("https://api.exmo.com/v1/ticker/");
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

        // Trading pairs
        public async Task<Ticker[,]> GetTicker()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            Dictionary<string, ExmoExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, ExmoExchange.TickerEntry>>("https://api.exmo.com/v1/ticker/");
            return Util.GetTicker(tradingPairs, this.Currencies);
        }

        public async Task<Exchanges.OrderBook> GetOrderBook((string, string) tradingPair)
        {
            if (!this.Connected) { throw new InvalidOperationException(); }
            
            Dictionary<string, ExmoExchange.OrderBook> orderBook = await Json.DeserializeUrl<Dictionary<string, ExmoExchange.OrderBook>>("https://api.exmo.com/v1/order_book/?pair=" + tradingPair.Item1 + "_" + tradingPair.Item2);
            return new Exchanges.OrderBook
            {
                Asks = orderBook[tradingPair.Item1 + "_" + tradingPair.Item2].Ask.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).ToArray(),
                Bids = orderBook[tradingPair.Item1 + "_" + tradingPair.Item2].Bid.Select(x => new OrderBookEntry { Price = x[0], Quantity = x[1] }).ToArray()
            };
        }
    }
}
