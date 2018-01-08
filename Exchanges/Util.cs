using AnarchocapitalismBot.Mathematics;
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
        public static (IExchangeCurrencies, Matrix<bool>) GetSupportedCurrenciesFromTradingPairs(IEnumerable<string> tradingPairs, char separator = '_')
        {
            // Generate list of currencies
            HashSet<string> currencySet = new HashSet<string>();
            foreach (string currencyPairName in tradingPairs)
            {
                string[] currencyIds = currencyPairName.ToUpper().Split('_');
                currencySet.Add(currencyIds[0]);
                currencySet.Add(currencyIds[1]);
            }

            IExchangeCurrencies currencies = new ExchangeCurrencies(currencySet);
            
            // Generate supported pairs
            Matrix<bool> supportedCurrencyPairs = Matrix<bool>.Fill(BooleanRing.Instance, (uint)currencies.Count, (uint)currencies.Count, false);
            foreach (string currencyPairName in tradingPairs)
            {
                string[] currencyIds = currencyPairName.ToUpper().Split('_');
                supportedCurrencyPairs[(uint)currencies.IndexOf(currencyIds[0]), (uint)currencies.IndexOf(currencyIds[1])] = true;
                supportedCurrencyPairs[(uint)currencies.IndexOf(currencyIds[1]), (uint)currencies.IndexOf(currencyIds[0])] = true;
            }

            return (currencies, supportedCurrencyPairs);
        }

        public static Matrix<decimal> GetSpotPrices<TickerEntryT>(IReadOnlyDictionary<string, TickerEntryT> tradingPairs, IExchangeCurrencies currencies, decimal feeFraction, char separator = '_')
            where TickerEntryT : ITickerEntry
        {
            Matrix<decimal> prices = Matrix<decimal>.Fill(DecimalRing.Instance, (uint)currencies.Count, (uint)currencies.Count, 0);

            foreach (KeyValuePair<string, TickerEntryT> pair in tradingPairs)
            {
                if (pair.Value.Volume24Hours == 0) { continue; }

                string[] currencyIds = pair.Key.ToUpper().Split('_');
                uint index0 = (uint)currencies.IndexOf(currencyIds[0]);
                uint index1 = (uint)currencies.IndexOf(currencyIds[1]);

                // Debug.Assert(pair.Value.HighestBidPrice <= pair.Value.LowestAskPrice);

                // c0_c1, c0/c1 = ask, c1 -> c0
                prices[index0, index1] = pair.Value.HighestBidPrice * (1m - feeFraction); // apply fees

                // c0_c1, c1/c0 = bid, c0 -> c1
                if (pair.Value.LowestAskPrice != 0)
                {
                    prices[index1, index0] = 1m / pair.Value.LowestAskPrice * (1m - feeFraction); // apply fees
                }
            }

            return prices;
        }
    }
}
