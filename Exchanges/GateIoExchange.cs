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
        public async Task<decimal[,]> GetSpotPrices()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            Dictionary<string, GateIoExchange.TickerEntry> tradingPairs = await Json.DeserializeUrl<Dictionary<string, GateIoExchange.TickerEntry>>("http://data.gate.io/api2/1/tickers");
            return Util.GetSpotPrices(tradingPairs, this.Currencies, 0.002m);
        }
    }
}
