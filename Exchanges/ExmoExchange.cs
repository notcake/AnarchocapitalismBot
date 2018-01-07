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
    public partial class ExmoExchange : IExchange
    {
        // IExchange
        public string Name => "EXMO";

        // Connection
        public bool Connected    { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IReadOnlyList<string> SupportedCurrencies { get; private set; } = null;
        private Dictionary<string, uint> CurrencyIndices = null;
        private Matrix<bool> supportedCurrencyPairs = null;

        // PoloniexExchange
        private JsonSerializer JsonSerializer = new JsonSerializer();

        public ExmoExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            WebRequest httpWebRequest = HttpWebRequest.Create("https://api.exmo.com/v1/ticker/");

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                // Parse response
                Dictionary<string, ExmoExchange.TickerEntry> currencyPairs = this.JsonSerializer.Deserialize<Dictionary<string, ExmoExchange.TickerEntry>>(jsonReader);

                // Generate list of currencies
                HashSet<string> currencySet = new HashSet<string>();
                foreach (string currencyPairName in currencyPairs.Keys)
                {
                    string[] currencyIds = currencyPairName.Split('_');
                    currencySet.Add(currencyIds[0]);
                    currencySet.Add(currencyIds[1]);
                }

                List<string> supportedCurrencies = currencySet.ToList();
                supportedCurrencies.Sort();
                this.SupportedCurrencies = supportedCurrencies;

                this.CurrencyIndices = new Dictionary<string, uint>();
                for (int i = 0; i < this.SupportedCurrencies.Count; i++)
                {
                    this.CurrencyIndices[this.SupportedCurrencies[i]] = (uint)i;
                }

                // Generate supported pairs
                this.supportedCurrencyPairs = Matrix<bool>.Fill(BooleanRing.Instance, (uint)this.SupportedCurrencies.Count, (uint)this.SupportedCurrencies.Count, false);
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
            this.CurrencyIndices = null;
            this.supportedCurrencyPairs = null;

            return Task.CompletedTask;
        }

        // Currencies
        public Matrix<bool> SupportedCurrencyPairs => this.supportedCurrencyPairs.Clone();

        public async Task<Matrix<decimal>> GetSpotPrices()
        {
            if (!this.Connected) { throw new InvalidOperationException(); }

            WebRequest httpWebRequest = HttpWebRequest.Create("https://api.exmo.com/v1/ticker/");

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                // Parse response
                Dictionary<string, ExmoExchange.TickerEntry> currencyPairs = this.JsonSerializer.Deserialize<Dictionary<string, ExmoExchange.TickerEntry>>(jsonReader);

                Matrix<decimal> prices = Matrix<decimal>.Fill(DecimalRing.Instance, (uint)this.SupportedCurrencies.Count, (uint)this.SupportedCurrencies.Count, 0);

                foreach (KeyValuePair<string, ExmoExchange.TickerEntry> pair in currencyPairs)
                {
                    string[] currencyIds = pair.Key.Split('_');
                    uint index0 = this.CurrencyIndices[currencyIds[0]];
                    uint index1 = this.CurrencyIndices[currencyIds[1]];

                    // c0_c1, c0/c1 = ask, c1 -> c0
                    prices[index0, index1] = pair.Value.BuyPrice;
                    prices[index0, index1] *= (1m - 0.002m); // apply fees

                    // c0_c1, c1/c0 = bid, c0 -> c1
                    prices[index1, index0] = 1 / pair.Value.SellPrice;
                    prices[index0, index1] *= (1m - 0.002m); // apply fees
                }

                return prices;
            }
        }
    }
}
