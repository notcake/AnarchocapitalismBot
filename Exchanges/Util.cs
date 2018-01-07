using AnarchocapitalismBot.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public static class Util
    {
        public static (IReadOnlyList<string>, Dictionary<string, uint>, Matrix<bool>) GetSupportedCurrenciesFromTradingPairs(IEnumerable<string> tradingPairs, char separator = '_')
        {
            // Generate list of currencies
            HashSet<string> currencySet = new HashSet<string>();
            foreach (string currencyPairName in tradingPairs)
            {
                string[] currencyIds = currencyPairName.Split('_');
                currencySet.Add(currencyIds[0]);
                currencySet.Add(currencyIds[1]);
            }

            List<string> supportedCurrencies = currencySet.ToList();
            supportedCurrencies.Sort();

            Dictionary<string, uint> supportedCurrencyIndices = new Dictionary<string, uint>();
            for (int i = 0; i < supportedCurrencies.Count; i++)
            {
                supportedCurrencyIndices[supportedCurrencies[i]] = (uint)i;
            }

            // Generate supported pairs
            Matrix<bool> supportedCurrencyPairs = Matrix<bool>.Fill(BooleanRing.Instance, (uint)supportedCurrencies.Count, (uint)supportedCurrencies.Count, false);
            foreach (string currencyPairName in tradingPairs)
            {
                string[] currencyIds = currencyPairName.Split('_');
                supportedCurrencyPairs[supportedCurrencyIndices[currencyIds[0]], supportedCurrencyIndices[currencyIds[1]]] = true;
                supportedCurrencyPairs[supportedCurrencyIndices[currencyIds[1]], supportedCurrencyIndices[currencyIds[0]]] = true;
            }

            return (supportedCurrencies, supportedCurrencyIndices, supportedCurrencyPairs);
        }

        public static Matrix<decimal> GetSpotPrices<TickerEntryT>(IReadOnlyDictionary<string, TickerEntryT> tradingPairs, IReadOnlyList<string> supportedCurrencies, IReadOnlyDictionary<string, uint> supportedCurrencyIndices, decimal feeFraction, char separator = '_')
            where TickerEntryT : ITickerEntry
        {
            Matrix<decimal> prices = Matrix<decimal>.Fill(DecimalRing.Instance, (uint)supportedCurrencies.Count, (uint)supportedCurrencies.Count, 0);

            foreach (KeyValuePair<string, TickerEntryT> pair in tradingPairs)
            {
                string[] currencyIds = pair.Key.Split('_');
                uint index0 = supportedCurrencyIndices[currencyIds[0]];
                uint index1 = supportedCurrencyIndices[currencyIds[1]];

                // c0_c1, c0/c1 = ask, c1 -> c0
                prices[index0, index1] = pair.Value.HighestBidPrice * (1m - feeFraction); // apply fees

                // c0_c1, c1/c0 = bid, c0 -> c1
                prices[index1, index0] = 1m / pair.Value.LowestAskPrice * (1m - feeFraction); // apply fees
            }

            return prices;
        }
    }
}
