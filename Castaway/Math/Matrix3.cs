using System;

namespace Castaway.Math
{
    public struct Matrix3
    {
        public Vector3 X;
        public Vector3 Y;
        public Vector3 Z;

        public float[] Array => new[]
        {
            X.X, Y.X, Z.X,
            X.Y, Y.Y, Z.Y,
            X.Z, Y.Z, Z.Z
        };

        public Matrix3(Vector3 x, Vector3 y, Vector3 z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Matrix3(
            float xx, float xy, float xz,
            float yx, float yy, float yz,
            float zx, float zy, float zz)
            : this(new Vector3(xx, xy, xz),
                new Vector3(yx, yy, yz),
                new Vector3(zx, zy, zz))
        {
        }

        public static Matrix3 Ident => new(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1);

        public static Matrix3 operator +(Matrix3 a, Matrix3 b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Matrix3 operator -(Matrix3 a, Matrix3 b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Matrix3 operator *(Matrix3 a, float b)
        {
            return new(a.X * b, a.Y * b, a.Z * b);
        }

        public static bool operator ==(Matrix3 left, Matrix3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix3 left, Matrix3 right)
        {
            return !left.Equals(right);
        }

        public static Vector3 operator *(Matrix3 a, Vector3 b)
        {
            return new(
                a.X.X * b.X + a.X.Y * b.Y + a.X.Z * b.Z,
                a.Y.X * b.X + a.Y.Y * b.Y + a.Y.Z * b.Z,
                a.Z.X * b.X + a.Z.Y * b.Y + a.Z.Z * b.Z);
        }

        public static Matrix3 operator *(Matrix3 a, Matrix3 b)
        {
            return new(
                a.X.X * b.X.X + a.X.Y * b.X.Y + a.X.Z * b.X.Z,
                a.X.X * b.Y.X + a.X.Y * b.Y.Y + a.X.Z * b.Y.Z,
                a.X.X * b.Z.X + a.X.Y * b.Z.Y + a.X.Z * b.Z.Z,
                a.Y.X * b.X.X + a.Y.Y * b.X.Y + a.Y.Z * b.X.Z,
                a.Y.X * b.Y.X + a.Y.Y * b.Y.Y + a.Y.Z * b.Y.Z,
                a.Y.X * b.Z.X + a.Y.Y * b.Z.Y + a.Y.Z * b.Z.Z,
                a.Z.X * b.X.X + a.Z.Y * b.X.Y + a.Z.Z * b.X.Z,
                a.Z.X * b.Y.X + a.Z.Y * b.Y.Y + a.Z.Z * b.Y.Z,
                a.Z.X * b.Z.X + a.Z.Y * b.Z.Y + a.Z.Z * b.Z.Z);
        }

        public static Matrix3 Scale(float x, float y, float z = 1)
        {
            var a = Ident;
            a.X.X = x;
            a.Y.Y = y;
            a.Z.Z = z;
            return a;
        }

        public static Matrix3 Translate(float x, float y)
        {
            var a = Ident;
            a.X.Z = x;
            a.Y.Z = y;
            return a;
        }

        public bool Equals(Matrix3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {{{X}}}, {nameof(Y)}: {{{Y}}}, {nameof(Z)}: {{{Z}}}";
        }
    }
}