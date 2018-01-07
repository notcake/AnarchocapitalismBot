using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Mathematics
{
    public class DecimalRing : Semiring<decimal>
    {
        public DecimalRing() { }

        // ISemiring<decimal>
        // Equality
        public override bool Equals(decimal a, decimal b) => a == b;

        // Arithmetic
        public override decimal Add(decimal a, decimal b)      => a + b;
        public override decimal Multiply(decimal a, decimal b) => a * b;

        // Identities
        public override decimal AdditiveIdentity       => 0;
        public override decimal MultiplicativeIdentity => 1;

        public static ISemiring<decimal> Instance => new DecimalRing();
    }
}
