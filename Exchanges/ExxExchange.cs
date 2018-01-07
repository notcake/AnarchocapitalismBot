﻿using System;
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
    public partial class ExxExchange : IExchange
    {
        // IExchange
        public string Name => "EXX";

        // Connection
        public bool Connected { get; private set; } = false;
        public bool TradingReady { get; private set; } = false;

        // Currencies
        public IReadOnlyList<string> SupportedCurrencies { get; private set; } = null;
        private Dictionary<string, uint> SupportedCurrencyIndices = null;
        private Matrix<bool> supportedCurrencyPairs = null;

        // ExxExchange
        private JsonSerializer JsonSerializer = new JsonSerializer();

        public ExxExchange() { }

        // IExchange
        // Connection
        public async Task<bool> ConnectReadOnly()
        {
            if (this.Connected) { return true; }

            WebRequest httpWebRequest = HttpWebRequest.Create("https://api.exx.com/data/v1/tickers");

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                // Parse response
                Dictionary<string, object> tradingPairs = this.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonReader);
                (this.SupportedCurrencies, this.SupportedCurrencyIndices, this.supportedCurrencyPairs) = Util.GetSupportedCurrenciesFromTradingPairs(tradingPairs.Keys);
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

            WebRequest httpWebRequest = HttpWebRequest.Create("https://api.exx.com/data/v1/tickers");

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                // Parse response
                Dictionary<string, ExxExchange.TickerEntry> tradingPairs = this.JsonSerializer.Deserialize<Dictionary<string, ExxExchange.TickerEntry>>(jsonReader);
                return Util.GetSpotPrices(tradingPairs, this.SupportedCurrencies, this.SupportedCurrencyIndices, 0.001m);
            }
        }
    }
}