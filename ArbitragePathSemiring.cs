using AnarchocapitalismBot.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot
{
    public class ArbitragePathSemiring : Semiring<ArbitragePath>
    {
        public ArbitragePathSemiring() { }

        // ISemiring<decimal>
        // Equality
        public override bool Equals(ArbitragePath a, ArbitragePath b) => a == b;

        // Arithmetic
        /// <summary>
        /// Return argmax(x => x.multiplier)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public override ArbitragePath Add(ArbitragePath a, ArbitragePath b)
        {
            bool aInteresting = true;
            bool bInteresting = true;

            if (a.Currencies.Count > 2)
            {
                for (int i = 1; i < a.Currencies.Count - 1; i++)
                {
                    if (a.Currencies[a.Currencies.Count - 1] == a.Currencies[i])
                    {
                        aInteresting = false;
                        break;
                    }
                }
            }
            if (b.Currencies.Count > 2)
            {
                for (int i = 1; i < b.Currencies.Count - 1; i++)
                {
                    if (b.Currencies[b.Currencies.Count - 1] == b.Currencies[i])
                    {
                        bInteresting = false;
                        break;
                    }
                }
            }

            if ( aInteresting && !bInteresting) { return a; }
            if (!aInteresting &&  bInteresting) { return b; }

            return ArbitragePath.Best(a, b);
        }

        public override ArbitragePath Multiply(ArbitragePath a, ArbitragePath b) => ArbitragePath.Compose(a, b);

        // Identities
        public override ArbitragePath AdditiveIdentity       => new ArbitragePath(0, new List<string>());
        public override ArbitragePath MultiplicativeIdentity => new ArbitragePath(1, new List<string>());

        public static ISemiring<ArbitragePath> Instance => new ArbitragePathSemiring();
    }
}
