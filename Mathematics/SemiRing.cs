using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Mathematics
{
    public abstract class Semiring<T> : ISemiring<T>
    {
        // ISemiring<T>
        // Equality
        public abstract bool Equals(T a, T b);

        // Arithmetic
        public abstract T Add(T a, T b);
        public abstract T Multiply(T a, T b);

        // Identities
        public T Zero => this.AdditiveIdentity;
        public T One  => this.MultiplicativeIdentity;

        public bool IsZero(T x) => this.IsAdditiveIdentity(x);
        public bool IsOne(T x)  => this.IsMultiplicativeIdentity(x);

        // Identities
        public abstract T AdditiveIdentity       { get; }
        public abstract T MultiplicativeIdentity { get; }

        public bool IsAdditiveIdentity(T x)       => this.Equals(this.Zero, x);
        public bool IsMultiplicativeIdentity(T x) => this.Equals(this.One,  x);

        public abstract string ToString(T x);
    }
}
