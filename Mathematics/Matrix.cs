using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot.Mathematics
{
    public class Matrix<T>
    {
        private ISemiring<T> Semiring;
        public readonly uint Width;
        public readonly uint Height;

        private T[,] Elements;

        public Matrix(ISemiring<T> semiring, uint height, uint width)
        {
            this.Semiring = semiring;
            this.Height = height;
            this.Width  = width;
            this.Elements = new T[this.Height, this.Width];
        }

        public Matrix<T> Clone(Matrix<T> result = null)
        {
            if (result != null)
            {
                Debug.Assert(result.Width  == this.Width);
                Debug.Assert(result.Height == this.Height);
            }

            result = result ?? new Matrix<T>(this.Semiring, this.Height, this.Width);
            result.Semiring = this.Semiring;

            for (uint y = 0; y < this.Height; y++)
            {
                for (uint x = 0; x < this.Width; x++)
                {
                    result.Elements[y, x] = this.Elements[y, x];
                }
            }

            return result;
        }

        public Matrix<T> Add(Matrix<T> b, Matrix<T> result = null)
        {
            Debug.Assert(this.Width  == b.Width);
            Debug.Assert(this.Height == b.Height);

            if (result != null)
            {
                Debug.Assert(result.Width  == this.Width);
                Debug.Assert(result.Height == this.Height);
            }

            result = result ?? new Matrix<T>(this.Semiring, this.Height, this.Width);
            result.Semiring = this.Semiring;

            for (uint y = 0; y < this.Height; y++)
            {
                for (uint x = 0; x < this.Width; x++)
                {
                    result.Elements[y, x] = this.Semiring.Add(this.Elements[y, x], b.Elements[y, x]);
                }
            }

            return result;
        }

        public Matrix<T> Multiply(Matrix<T> b, Matrix<T> result = null)
        {
            Debug.Assert(this.Width == b.Height);

            if (result != null)
            {
                Debug.Assert(result.Width  == b.Width);
                Debug.Assert(result.Height == this.Height);
            }

            result = result ?? new Matrix<T>(this.Semiring, this.Height, b.Width);
            result.Semiring = this.Semiring;

            for (uint y = 0; y < this.Height; y++)
            {
                for (uint x = 0; x < b.Width; x++)
                {
                    T element = this.Semiring.Zero;
                    for (uint k = 0; k < this.Width; k++)
                    {
                        element = this.Semiring.Add(element, this.Semiring.Multiply(this.Elements[y, k], b.Elements[k, x]));
                    }

                    result.Elements[y, x] = element;
                }
            }

            return result;
        }

        public Matrix<U> Map<U>(ISemiring<U> semiring, Func<T, U> f, Matrix<U> result = null)
        {
            if (result != null)
            {
                Debug.Assert(result.Width == this.Width);
                Debug.Assert(result.Height == this.Height);
            }

            result = result ?? new Matrix<U>(semiring, this.Height, this.Width);
            result.Semiring = semiring;

            for (uint y = 0; y < this.Height; y++)
            {
                for (uint x = 0; x < this.Width; x++)
                {
                    result.Elements[y, x] = f(this.Elements[y, x]);
                }
            }

            return result;
        }

        public Matrix<U> Map<U>(ISemiring<U> semiring, Func<uint, uint, T, U> f, Matrix<U> result = null)
        {
            if (result != null)
            {
                Debug.Assert(result.Width == this.Width);
                Debug.Assert(result.Height == this.Height);
            }

            result = result ?? new Matrix<U>(semiring, this.Height, this.Width);
            result.Semiring = semiring;

            for (uint y = 0; y < this.Height; y++)
            {
                for (uint x = 0; x < this.Width; x++)
                {
                    result.Elements[y, x] = f(y, x, this.Elements[y, x]);
                }
            }

            return result;
        }

        public T this[uint y, uint x]
        {
            get { return this.Elements[y, x]; }
            set { this.Elements[y, x] = value; }
        }

        public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b) => a.Add(b);
        public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b) => a.Multiply(b);

        public static Matrix<T> Fill(ISemiring<T> ring, uint height, uint width, T value)
        {
            Matrix<T> matrix = new Matrix<T>(ring, height, width);
            for (uint y = 0; y < matrix.Height; y++)
            {
                for (uint x = 0; x < matrix.Width; x++)
                {
                    matrix.Elements[y, x] = value;
                }
            }

            return matrix;
        }

        public static Matrix<T> Zero(ISemiring<T> ring, uint height, uint width)
        {
            return Matrix<T>.Fill(ring, height, width, ring.Zero);
        }
    }
}
