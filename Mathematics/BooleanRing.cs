using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Mathematics
{
    public class BooleanRing : Semiring<bool>
    {
        public BooleanRing() { }

        // ISemiring<bool>
        // Equality
        public override bool Equals(bool a, bool b) => a == b;

        // Arithmetic
        public override bool Add(bool a, bool b)      => a != b;
        public override bool Multiply(bool a, bool b) => a && b;

        // Identities
        public override bool AdditiveIdentity       => false;
        public override bool MultiplicativeIdentity => true;

        public static ISemiring<bool> Instance => new BooleanRing();
    }
}
