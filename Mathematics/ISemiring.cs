using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Mathematics
{
    /// <summary>
    /// I'm going to hell for this.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISemiring<T>
    {
        // Equality
        bool Equals(T a, T b);

        // Arithmetic
        T Add(T a, T b);
        T Multiply(T a, T b);

        // Identities
        T Zero { get; }
        T One { get; }

        bool IsZero(T x);
        bool IsOne(T x);

        // Identities
        T AdditiveIdentity { get; }
        T MultiplicativeIdentity { get; }

        bool IsAdditiveIdentity(T x);
        bool IsMultiplicativeIdentity(T x);

        string ToString(T x);
    }
}
