using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    [Exchange]
    public partial class BinanceExchange : IExchange
    {
        // IExchange
        public string Name => "Binance";

        // Connection
        public bool Connected    { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IExchangeCurrencies Currencies { get; private set; } = null;

        // Trading pairs
        public decimal FeePercentage => 0.1m;
        private TradingPairType[,] tradingPairs = null;
        public TradingPairType[,] TradingPairs => (TradingPairType[,])this.tradingPairs.Clone();
        
        // BinanceExchange
        public BinanceExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            BinanceExchange.TickerEntry[] tickers = await Json.DeserializeUrl< BinanceExchange.TickerEntry[] >("https://www.binance.com/api/v1/ticker/allBookTickers");
            (this.Currencies, this.tradingPairs) = Util.GetSupportedCurrenciesFromTradingPairs(tickers.Select(x => this.ParseTradingPair(x.Symbol)));
            
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

            BinanceExchange.TickerEntry[] tickers = await Json.DeserializeUrl<BinanceExchange.TickerEntry[]>("https://www.binance.com/api/v1/ticker/allBookTickers");
            Dictionary<string, ITickerEntry> tradingPairs = new Dictionary<string, ITickerEntry>();
            foreach (BinanceExchange.TickerEntry tickerEntry in tickers)
            {
                (string, string) tradingPair = this.ParseTradingPair(tickerEntry.Symbol);
                tradingPairs[tradingPair.Item1 + "_" + tradingPair.Item2] = tickerEntry;
            }
            return Util.GetTicker(tradingPairs, this.Currencies);
        }

        public async Task<Exchanges.OrderBook> GetOrderBook((string, string) tradingPair)
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            BinanceExchange.OrderBook orderBook = await Json.DeserializeUrl<BinanceExchange.OrderBook>("https://www.binance.com/api/v1/depth?symbol=" + tradingPair.Item1 + tradingPair.Item2);
            return new Exchanges.OrderBook
            {
                Asks = orderBook.Ask.Select(x => new OrderBookEntry { Price = decimal.Parse((string)x[0]), Quantity = decimal.Parse((string)x[1]) }).Reverse().ToArray(),
                Bids = orderBook.Bid.Select(x => new OrderBookEntry { Price = decimal.Parse((string)x[0]), Quantity = decimal.Parse((string)x[1]) }).ToArray()
            };
        }

        private (string, string) ParseTradingPair(string tradingPair)
        {
            if (tradingPair.StartsWith("BTC") ||
                tradingPair.StartsWith("ETH") ||
                tradingPair.StartsWith("BNB"))
            {
                return (tradingPair.Substring(0, 3), tradingPair.Substring(3));
            }
            else if (tradingPair.StartsWith("USDT"))
            {
                return (tradingPair.Substring(0, 4), tradingPair.Substring(4));
            }
            else if (tradingPair.EndsWith("BTC") ||
                     tradingPair.EndsWith("ETH") ||
                     tradingPair.EndsWith("BNB"))
            {
                return (tradingPair.Substring(0, tradingPair.Length - 3), tradingPair.Substring(tradingPair.Length - 3));
            }
            else if (tradingPair.EndsWith("USDT"))
            {
                return (tradingPair.Substring(0, tradingPair.Length - 4), tradingPair.Substring(tradingPair.Length - 4));
            }
            else if (tradingPair == "123456")
            {
                return ("123", "456");
            }

            throw new ArgumentException(nameof(tradingPair));
        }
    }
}
