using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public static class Util
    {
        public static (IExchangeCurrencies, TradingPairType[,]) GetSupportedCurrenciesFromTradingPairs(IEnumerable<string> tradingPairs, char separator = '_')
        {
            return Util.GetSupportedCurrenciesFromTradingPairs(tradingPairs.Select(x =>
            {
                string[] currencyIds = x.ToUpper().Split('_');
                return (currencyIds[0], currencyIds[1]);
            }));
        }

        public static (IExchangeCurrencies, TradingPairType[,]) GetSupportedCurrenciesFromTradingPairs(IEnumerable<(string, string)> tradingPairs)
        {
            // Generate list of currencies
            HashSet<string> currencySet = new HashSet<string>();
            foreach ((string, string) tradingPair in tradingPairs)
            {
                currencySet.Add(tradingPair.Item1);
                currencySet.Add(tradingPair.Item2);
            }

            IExchangeCurrencies currencies = new ExchangeCurrencies(currencySet);

            // Generate supported pairs
            TradingPairType[,] tradingPairTable = new TradingPairType[currencies.Count, currencies.Count];
            foreach ((string, string) tradingPair in tradingPairs)
            {
                tradingPairTable[currencies.IndexOf(tradingPair.Item1), currencies.IndexOf(tradingPair.Item2)] = TradingPairType.Buy;
                tradingPairTable[currencies.IndexOf(tradingPair.Item2), currencies.IndexOf(tradingPair.Item1)] = TradingPairType.Sell;
            }

            return (currencies, tradingPairTable);
        }

        public static Ticker[,] GetTicker<TickerEntryT>(IReadOnlyDictionary<string, TickerEntryT> tradingPairs, IExchangeCurrencies currencies, char separator = '_')
            where TickerEntryT : ITickerEntry
        {
            Ticker[,] ticker = new Ticker[currencies.Count, currencies.Count];

            foreach (KeyValuePair<string, TickerEntryT> pair in tradingPairs)
            {
                if (pair.Value.Volume24Hours == 0) { continue; }

                string[] currencyIds = pair.Key.ToUpper().Split('_');
                uint index0 = (uint)currencies.IndexOf(currencyIds[0]);
                uint index1 = (uint)currencies.IndexOf(currencyIds[1]);

                // Debug.Assert(pair.Value.HighestBidPrice <= pair.Value.LowestAskPrice);
                if (pair.Value.HighestBidPrice > pair.Value.LowestAskPrice)
                {
                    Debug.WriteLine(pair.Key + ": " + pair.Value.HighestBidPrice.ToString() + " to " + pair.Value.LowestAskPrice.ToString());
                }

                // c0_c1, c0/c1 = ask, c1 -> c0
                ticker[index0, index1] = new Ticker(pair.Value);

                // c0_c1, c1/c0 = bid, c0 -> c1
                if (pair.Value.LowestAskPrice != 0)
                {
                    ticker[index1, index0] = new Ticker(pair.Value).Flip();
                }
            }

            return ticker;
        }
    }
}
