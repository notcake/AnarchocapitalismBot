using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AnarchocapitalismBot.Mathematics;
using Newtonsoft.Json;

namespace AnarchocapitalismBot.Exchanges
{
    [Exchange]
    public partial class LiquiExchange : IExchange
    {
        // IExchange
        public string Name => "Liqui";

        // Connection
        public bool Connected { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IReadOnlyList<string> SupportedCurrencies { get; private set; } = null;
        private Dictionary<string, uint> SupportedCurrencyIndices = null;
        private Matrix<bool> supportedCurrencyPairs = null;

        // LiquiExchange
        private JsonSerializer JsonSerializer = new JsonSerializer();
        private string AllTradingPairs;

        public LiquiExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            WebRequest httpWebRequest = HttpWebRequest.Create("https://api.liqui.io/api/3/info");

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                // Parse response
                LiquiExchange.Info info = this.JsonSerializer.Deserialize<LiquiExchange.Info>(jsonReader);
                (this.SupportedCurrencies, this.SupportedCurrencyIndices, this.supportedCurrencyPairs) = Util.GetSupportedCurrenciesFromTradingPairs(info.Pairs.Keys);

                this.AllTradingPairs = string.Join("-", info.Pairs.Keys);
            }

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

            this.SupportedCurrencies = null;
            this.SupportedCurrencyIndices = null;
            this.supportedCurrencyPairs = null;

            return Task.CompletedTask;
        }

        // Currencies
        public Matrix<bool> SupportedCurrencyPairs => this.supportedCurrencyPairs.Clone();

        public async Task<Matrix<decimal>> GetSpotPrices()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            // The Liqui API is really shit and fails half the time.
            // This won't ever work reliably.
            WebRequest httpWebRequest = HttpWebRequest.Create("https://api.liqui.io/api/3/ticker/" + this.AllTradingPairs + "?ignore_invalid=1");

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                // Parse response
                Dictionary<string, LiquiExchange.TickerEntry> tradingPairs = this.JsonSerializer.Deserialize<Dictionary<string, LiquiExchange.TickerEntry>>(jsonReader);
                return Util.GetSpotPrices(tradingPairs, this.SupportedCurrencies, this.SupportedCurrencyIndices, 0.0025m);
            }
        }
    }
}
