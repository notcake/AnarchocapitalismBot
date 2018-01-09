using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Mathematics
{
    public class NullRing<T> : Semiring<T>
    {
        public NullRing() { }

        // ISemiring<T>
        // Equality
        public override bool Equals(T a, T b) => a.Equals(b);

        // Arithmetic
        public override T Add(T a, T b)      => default(T);
        public override T Multiply(T a, T b) => default(T);

        // Identities
        public override T AdditiveIdentity       => default(T);
        public override T MultiplicativeIdentity => default(T);

        public static ISemiring<T> Instance => new NullRing<T>();
    }
}
