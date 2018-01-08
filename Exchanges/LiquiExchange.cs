using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnarchocapitalismBot.Mathematics;

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
        public async Task<decimal[,]> GetSpotPrices()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            // The Liqui API is really shit and fails half the time.
            // This won't ever work reliably.
            Dictionary<string, LiquiExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, LiquiExchange.TickerEntry>>("https://api.liqui.io/api/3/ticker/" + this.AllTradingPairs + "?ignore_invalid=1");
            return Util.GetSpotPrices(tradingPairs, this.Currencies, 0.0025m);
        }
    }
}
