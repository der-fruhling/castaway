using System;

namespace Castaway.Math
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3 operator +(Vector3 a, float b) => new(a.X + b, a.Y + b, a.Z + b);
        public static Vector3 operator -(Vector3 a, float b) => new(a.X - b, a.Y - b, a.Z - b);
        public static Vector3 operator *(Vector3 a, float b) => new(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator /(Vector3 a, float b) => new(a.X / b, a.Y / b, a.Z / b);
        public static implicit operator ReadOnlySpan<float>(Vector3 v) => new[] {v.X, v.Y, v.Z};

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
        }
    }
}