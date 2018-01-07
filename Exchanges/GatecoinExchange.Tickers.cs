using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649

namespace AnarchocapitalismBot.Exchanges
{
    public partial class GatecoinExchange
    {
        private struct Tickers
        {
            [JsonProperty("responseStatus")]
            public object ResponseStatus;

            [JsonProperty("tickers")]
            public GatecoinExchange.TickerEntry[] TickerEntries;
        }
    }
}
