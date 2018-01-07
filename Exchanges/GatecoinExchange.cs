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
    public partial class GatecoinExchange : IExchange
    {
        // IExchange
        public string Name => "Gatecoin";

        // Connection
        public bool Connected { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IReadOnlyList<string> SupportedCurrencies { get; private set; } = null;
        private Dictionary<string, uint> SupportedCurrencyIndices = null;
        private Matrix<bool> supportedCurrencyPairs = null;

        // PoloniexExchange
        private JsonSerializer JsonSerializer = new JsonSerializer();

        public GatecoinExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            WebRequest httpWebRequest = HttpWebRequest.Create("https://api.gatecoin.com/Public/LiveTickers");

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                // Parse response
                GatecoinExchange.Tickers tickers = this.JsonSerializer.Deserialize<GatecoinExchange.Tickers>(jsonReader);
                (this.SupportedCurrencies, this.SupportedCurrencyIndices, this.supportedCurrencyPairs) = Util.GetSupportedCurrenciesFromTradingPairs(tickers.TickerEntries.Select(x => x.CurrencyPair.Substring(0, 3) + "_" + x.CurrencyPair.Substring(4)));
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

            WebRequest httpWebRequest = HttpWebRequest.Create("https://api.gatecoin.com/Public/LiveTickers");

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                // Parse response
                GatecoinExchange.Tickers tickers = this.JsonSerializer.Deserialize<GatecoinExchange.Tickers>(jsonReader);
                Dictionary<string, ITickerEntry> tradingPairs = new Dictionary<string, ITickerEntry>();
                foreach (GatecoinExchange.TickerEntry tickerEntry in tickers.TickerEntries)
                {
                    tradingPairs[tickerEntry.CurrencyPair.Substring(0, 3) + "_" + tickerEntry.CurrencyPair.Substring(4)] = tickerEntry;
                }
                return Util.GetSpotPrices(tradingPairs, this.SupportedCurrencies, this.SupportedCurrencyIndices, 0.0013m);
            }
        }
    }
}
