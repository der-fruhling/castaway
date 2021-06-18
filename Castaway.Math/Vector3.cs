using System;

namespace Castaway.Math
{
    public struct Vector3 : IEquatable<Vector3>
    {
        public double X;
        public double Y;
        public double Z;

        public static readonly Vector3 Right = new(1, 0, 0);
        public static readonly Vector3 Up = new(0, 1, 0);
        public static readonly Vector3 Backward = new(0, 0, 1);
        public static readonly Vector3 Left = -Right;
        public static readonly Vector3 Down = -Up;
        public static readonly Vector3 Forward = -Backward;

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector2 v, double z) : this(v.X, v.Y, z) {}
        
        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static Vector3 operator +(Vector3 a, double b) => new(a.X + b, a.Y + b, a.Z + b);
        public static Vector3 operator -(Vector3 a, double b) => new(a.X - b, a.Y - b, a.Z - b);
        public static Vector3 operator *(Vector3 a, double b) => new(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator /(Vector3 a, double b) => new(a.X / b, a.Y / b, a.Z / b);
        public static Vector3 operator +(Vector3 a, uint b) => new(a.X + b, a.Y + b, a.Z + b);
        public static Vector3 operator -(Vector3 a, uint b) => new(a.X - b, a.Y - b, a.Z - b);
        public static Vector3 operator *(Vector3 a, uint b) => new(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator /(Vector3 a, uint b) => new(a.X / b, a.Y / b, a.Z / b);
        public static Vector3 operator +(Vector3 a, float b) => new(a.X + b, a.Y + b, a.Z + b);
        public static Vector3 operator -(Vector3 a, float b) => new(a.X - b, a.Y - b, a.Z - b);
        public static Vector3 operator *(Vector3 a, float b) => new(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator /(Vector3 a, float b) => new(a.X / b, a.Y / b, a.Z / b);
        public static Vector3 operator +(Vector3 a, int b) => new(a.X + b, a.Y + b, a.Z + b);
        public static Vector3 operator -(Vector3 a, int b) => new(a.X - b, a.Y - b, a.Z - b);
        public static Vector3 operator *(Vector3 a, int b) => new(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator /(Vector3 a, int b) => new(a.X / b, a.Y / b, a.Z / b);
        
        public static Vector3 operator -(Vector3 v) => new(-v.X, -v.Y, -v.Z);

        public static explicit operator double[](Vector3 v) => new[] {v.X, v.Y, v.Z};

        public static explicit operator float[](Vector3 v) => new[] {(float) v.X, (float) v.Y, (float) v.Z};

        public static explicit operator uint[](Vector3 v) => new[] {(uint) v.X, (uint) v.Y, (uint) v.Z};

        public static explicit operator int[](Vector3 v) => new[] {(int) v.X, (int) v.Y, (int) v.Z};

        public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";

        public double Magnitude() => System.Math.Sqrt(X * X + Y * Y + Z * Z);

        public static Vector3 Cross(Vector3 a, Vector3 b) => new(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X);

        public static double Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public bool Equals(Vector3 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        public override bool Equals(object? obj) => obj is Vector3 other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public static bool operator ==(Vector3 left, Vector3 right) => left.Equals(right);

        public static bool operator !=(Vector3 left, Vector3 right) => !left.Equals(right);
    }
}