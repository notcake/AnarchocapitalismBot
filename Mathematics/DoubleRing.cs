using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Mathematics
{
    public class DoubleRing : Semiring<double>
    {
        public DoubleRing() { }

        // ISemiring<double>
        // Equality
        public override bool Equals(double a, double b) => a == b;

        // Arithmetic
        public override double Add(double a, double b) => a + b;
        public override double Multiply(double a, double b) => a * b;

        // Identities
        public override double AdditiveIdentity => 0;
        public override double MultiplicativeIdentity => 0;

        public override string ToString(double x) => x.ToString();

        public static ISemiring<double> Instance => new DoubleRing();
    }
}
