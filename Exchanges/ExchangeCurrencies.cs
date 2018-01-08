using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Exchanges
{
    public class ExchangeCurrencies : IExchangeCurrencies
    {
        private List<string> currencies = new List<string>();
        private Dictionary<string, int> currencyIndices = new Dictionary<string, int>();

        public ExchangeCurrencies() { }

        public ExchangeCurrencies(IEnumerable<string> currencies)
        {
            this.currencies = new List<string>(currencies.OrderBy(x => x));
            for (int i = 0; i < this.currencies.Count; i++)
            {
                this.currencyIndices[this.currencies[i]] = i;
            }
        }

        // IEnumerable
        IEnumerator IEnumerable.GetEnumerator() => this.currencies.GetEnumerator();

        // IEnumerable<string>
        public IEnumerator<string> GetEnumerator() => this.currencies.GetEnumerator();

        // ICollection<string>
        public int Count => this.currencies.Count;

        // IReadOnlyList<string>
        public string this[int index] => this.currencies[index];

        // IExchangeCurrencies
        public int IndexOf(string currency)
        {
            if (!this.currencyIndices.ContainsKey(currency)) { return -1; }

            return this.currencyIndices[currency];
        }

        // ExchangeCurrencies

    }
}
