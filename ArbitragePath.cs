using AnarchocapitalismBot.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot
{
    public struct ArbitragePath : IEquatable<ArbitragePath>
    {
        public decimal Multiplier;
        public List<string> Currencies;

        public ArbitragePath(decimal multiplier, List<string> currencies)
        {
            this.Multiplier = multiplier;
            this.Currencies = currencies;
        }

        // Object
        public override bool Equals(object obj)
        {
            return obj is ArbitragePath ? this.Equals((ArbitragePath)obj) : false;
        }

        public override int GetHashCode()
        {
            return this.Multiplier.GetHashCode();
        }

        // IEquatable<ArbitragePath>
        public bool Equals(ArbitragePath other)
        {
            return this.Multiplier == other.Multiplier && this.Currencies.SequenceEqual(other.Currencies);
        }

        // ArbitragePath
        public ArbitragePath Compose(ArbitragePath b)
        {
            ArbitragePath result = new ArbitragePath(this.Multiplier * b.Multiplier, new List<string>());

            result.Currencies.AddRange(this.Currencies);
            if (this.Currencies.Count > 0 && b.Currencies.Count > 0)
            {
                result.Currencies.RemoveAt(result.Currencies.Count - 1);
            }
            result.Currencies.AddRange(b.Currencies);

            return result;
        }

        public bool HasStrictSubcycle
        {
            get
            {
                if (this.Currencies.Count <= 2) { return false; }

                for (int i = 1; i < this.Currencies.Count - 1; i++)
                {
                    if (this.Currencies[this.Currencies.Count - 1] == this.Currencies[i])
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static ArbitragePath Best(ArbitragePath a, ArbitragePath b)
        {
            if (a.Multiplier > b.Multiplier) { return a; }
            else if (a.Multiplier < b.Multiplier) { return b; }
            else
            {
                return a.Currencies.Count < b.Currencies.Count ? a : b;
            }
        }

        public static ArbitragePath Compose(ArbitragePath a, ArbitragePath b) => b.Compose(a);

        public static bool operator ==(ArbitragePath a, ArbitragePath b) => a.Equals(b);
        public static bool operator !=(ArbitragePath a, ArbitragePath b) => !a.Equals(b);

        public static ISemiring<ArbitragePath> Semiring => ArbitragePathSemiring.Instance;
    }
}
