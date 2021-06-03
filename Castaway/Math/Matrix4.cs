using System;

namespace Castaway.Math
{
    public struct Matrix4
    {
        public Vector4 X;
        public Vector4 Y;
        public Vector4 Z;
        public Vector4 W;

        public float[] Array => new[]
        {
            X.X, Y.X, Z.X, W.X,
            X.Y, Y.Y, Z.Y, W.Y,
            X.Z, Y.Z, Z.Z, W.Z,
            X.W, Y.W, Z.W, W.W
        };

        public Matrix4(Vector4 x, Vector4 y, Vector4 z, Vector4 w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Matrix4(
            float xx, float xy, float xz, float xw,
            float yx, float yy, float yz, float yw,
            float zx, float zy, float zz, float zw,
            float wx, float wy, float wz, float ww)
            : this(new Vector4(xx, xy, xz, xw),
                new Vector4(yx, yy, yz, yw),
                new Vector4(zx, zy, zz, zw),
                new Vector4(wx, wy, wz, ww))
        {
        }

        public static Matrix4 Ident => new(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);

        public static Matrix4 operator +(Matrix4 a, Matrix4 b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Matrix4 operator -(Matrix4 a, Matrix4 b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Matrix4 operator *(Matrix4 a, float b)
        {
            return new(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static bool operator ==(Matrix4 left, Matrix4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix4 left, Matrix4 right)
        {
            return !left.Equals(right);
        }

        public static Vector4 operator *(Matrix4 a, Vector4 b)
        {
            return new(
                a.X.X * b.X + a.X.Y * b.Y + a.X.Z * b.Z + a.X.W * b.W,
                a.Y.X * b.X + a.Y.Y * b.Y + a.Y.Z * b.Z + a.Y.W * b.W,
                a.Z.X * b.X + a.Z.Y * b.Y + a.Z.Z * b.Z + a.Z.W * b.W,
                a.W.X * b.X + a.W.Y * b.Y + a.W.Z * b.Z + a.W.W * b.W);
        }

        public static Matrix4 operator *(Matrix4 a, Matrix4 b)
        {
            return new(
                a.X.X * b.X.X + a.X.Y * b.X.Y + a.X.Z * b.X.Z + a.X.W * b.X.W,
                a.X.X * b.Y.X + a.X.Y * b.Y.Y + a.X.Z * b.Y.Z + a.X.W * b.Y.W,
                a.X.X * b.Z.X + a.X.Y * b.Z.Y + a.X.Z * b.Z.Z + a.X.W * b.Z.W,
                a.X.X * b.W.X + a.X.Y * b.W.Y + a.X.Z * b.W.Z + a.X.W * b.W.W,
                a.Y.X * b.X.X + a.Y.Y * b.X.Y + a.Y.Z * b.X.Z + a.Y.W * b.X.W,
                a.Y.X * b.Y.X + a.Y.Y * b.Y.Y + a.Y.Z * b.Y.Z + a.Y.W * b.Y.W,
                a.Y.X * b.Z.X + a.Y.Y * b.Z.Y + a.Y.Z * b.Z.Z + a.Y.W * b.Z.W,
                a.Y.X * b.W.X + a.Y.Y * b.W.Y + a.Y.Z * b.W.Z + a.Y.W * b.W.W,
                a.Z.X * b.X.X + a.Z.Y * b.X.Y + a.Z.Z * b.X.Z + a.Z.W * b.X.W,
                a.Z.X * b.Y.X + a.Z.Y * b.Y.Y + a.Z.Z * b.Y.Z + a.Z.W * b.Y.W,
                a.Z.X * b.Z.X + a.Z.Y * b.Z.Y + a.Z.Z * b.Z.Z + a.Z.W * b.Z.W,
                a.Z.X * b.W.X + a.Z.Y * b.W.Y + a.Z.Z * b.W.Z + a.Z.W * b.W.W,
                a.W.X * b.X.X + a.W.Y * b.X.Y + a.W.Z * b.X.Z + a.W.W * b.X.W,
                a.W.X * b.Y.X + a.W.Y * b.Y.Y + a.W.Z * b.Y.Z + a.W.W * b.Y.W,
                a.W.X * b.Z.X + a.W.Y * b.Z.Y + a.W.Z * b.Z.Z + a.W.W * b.Z.W,
                a.W.X * b.W.X + a.W.Y * b.W.Y + a.W.Z * b.W.Z + a.W.W * b.W.W);
        }

        public static Matrix4 Scale(float x, float y, float z, float w = 1)
        {
            var a = Ident;
            a.X.X = x;
            a.Y.Y = y;
            a.Z.Z = z;
            a.W.W = w;
            return a;
        }

        public static Matrix4 Translate(float x, float y, float z)
        {
            var a = Ident;
            a.X.W = x;
            a.Y.W = y;
            a.Z.W = z;
            return a;
        }

        public bool Equals(Matrix4 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix4 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";
        }
    }
}