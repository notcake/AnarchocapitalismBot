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
        public async Task<decimal[,]> GetSpotPrices()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            Dictionary<string, ExxExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, ExxExchange.TickerEntry>>("https://api.exx.com/data/v1/tickers");
            return Util.GetSpotPrices(tradingPairs, this.Currencies, 0.001m);
        }
    }
}
