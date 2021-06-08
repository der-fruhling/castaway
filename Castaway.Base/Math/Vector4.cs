using System;

namespace Castaway.Math
{
    public struct Vector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Vector4 operator *(Vector4 a, Vector4 b)
        {
            return new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

        public static Vector4 operator /(Vector4 a, Vector4 b)
        {
            return new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        }

        public static Vector4 operator +(Vector4 a, float b)
        {
            return new(a.X + b, a.Y + b, a.Z + b, a.W + b);
        }

        public static Vector4 operator -(Vector4 a, float b)
        {
            return new(a.X - b, a.Y - b, a.Z - b, a.W - b);
        }

        public static Vector4 operator *(Vector4 a, float b)
        {
            return new(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Vector4 operator /(Vector4 a, float b)
        {
            return new(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }

        public static Vector4 operator -(Vector4 v) => new(-v.X, -v.Y, -v.Z, -v.W);

        public static implicit operator ReadOnlySpan<float>(Vector4 v)
        {
            return new[] {v.X, v.Y, v.Z, v.W};
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";
        }
    }
}