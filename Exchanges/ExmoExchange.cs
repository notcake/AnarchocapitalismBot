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
        public async Task<decimal[,]> GetSpotPrices()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            Dictionary<string, ExmoExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, ExmoExchange.TickerEntry>>("https://api.exmo.com/v1/ticker/");
            return Util.GetSpotPrices(tradingPairs, this.Currencies, 0.002m);
        }
    }
}
