using System;

namespace Castaway.Math
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public static readonly Vector3 Right = new(1, 0, 0);
        public static readonly Vector3 Up = new(0, 1, 0);
        public static readonly Vector3 Backward = new(0, 0, 1);
        public static readonly Vector3 Left = -Right;
        public static readonly Vector3 Down = -Up;
        public static readonly Vector3 Forward = -Backward;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector2 v, float z) : this(v.X, v.Y, z) {}
        
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Vector3 operator +(Vector3 a, float b)
        {
            return new(a.X + b, a.Y + b, a.Z + b);
        }

        public static Vector3 operator -(Vector3 a, float b)
        {
            return new(a.X - b, a.Y - b, a.Z - b);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vector3 operator -(Vector3 v) => new(-v.X, -v.Y, -v.Z);

        public static implicit operator ReadOnlySpan<float>(Vector3 v)
        {
            return new[] {v.X, v.Y, v.Z};
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
        }

        public float Magnitude()
        {
            return MathF.Sqrt(X * X + Y * Y + Z * Z);
        }

        public static Vector3 Cross(Vector3 a, Vector3 b) => new(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X);

        public static float Dot(Vector3 a, Vector3 b) =>
            a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }
}