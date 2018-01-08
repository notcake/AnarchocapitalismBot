using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    [Exchange]
    public partial class GatecoinExchange : IExchange
    {
        // IExchange
        public string Name => "Gatecoin";

        // Connection
        public bool Connected    { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IExchangeCurrencies Currencies { get; private set; } = null;

        // Trading pairs
        private TradingPairType[,] tradingPairs = null;
        public TradingPairType[,] TradingPairs => (TradingPairType[,])this.tradingPairs.Clone();

        // GatecoinExchange
        public GatecoinExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            GatecoinExchange.Tickers tickers = await Json.DeserializeUrl<GatecoinExchange.Tickers>("https://api.gatecoin.com/Public/LiveTickers");
            (this.Currencies, this.tradingPairs) = Util.GetSupportedCurrenciesFromTradingPairs(tickers.TickerEntries.Select(x => x.CurrencyPair.Substring(0, 3) + "_" + x.CurrencyPair.Substring(4)));

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
        public async Task<decimal[,]> GetSpotPrices()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            GatecoinExchange.Tickers tickers = await Json.DeserializeUrl<GatecoinExchange.Tickers>("https://api.gatecoin.com/Public/LiveTickers");
            Dictionary<string, ITickerEntry> tradingPairs = new Dictionary<string, ITickerEntry>();
            foreach (GatecoinExchange.TickerEntry tickerEntry in tickers.TickerEntries)
            {
                tradingPairs[tickerEntry.CurrencyPair.Substring(0, 3) + "_" + tickerEntry.CurrencyPair.Substring(4)] = tickerEntry;
            }
            return Util.GetSpotPrices(tradingPairs, this.Currencies, 0.0013m);
        }
    }
}
