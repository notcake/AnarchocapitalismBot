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
        public bool SubcyclesFiltered { get; }

        public ArbitragePathSemiring(bool filterSubcycles)
        {
            this.SubcyclesFiltered = filterSubcycles;
        }

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
            if (this.SubcyclesFiltered)
            {
                bool aInteresting = !a.HasStrictSubcycle;
                bool bInteresting = !b.HasStrictSubcycle;

                if (aInteresting && !bInteresting) { return a; }
                if (!aInteresting && bInteresting) { return b; }
            }

            return ArbitragePath.Best(a, b);
        }

        public override ArbitragePath Multiply(ArbitragePath a, ArbitragePath b)
        {
            // Don't bother building paths if the reuslt is a 0.
            if (a.Multiplier == 0) { return a; }
            if (b.Multiplier == 0) { return b; }

            return ArbitragePath.Compose(a, b);
        }

        // Identities
        public override ArbitragePath AdditiveIdentity       => new ArbitragePath(0, new List<string>());
        public override ArbitragePath MultiplicativeIdentity => new ArbitragePath(1, new List<string>());
    }
}
