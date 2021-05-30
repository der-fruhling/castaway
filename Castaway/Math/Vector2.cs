using System;

namespace Castaway.Math
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.X / b.X, a.Y / b.Y);
        public static Vector2 operator +(Vector2 a, float b) => new(a.X + b, a.Y + b);
        public static Vector2 operator -(Vector2 a, float b) => new(a.X - b, a.Y - b);
        public static Vector2 operator *(Vector2 a, float b) => new(a.X * b, a.Y * b);
        public static Vector2 operator /(Vector2 a, float b) => new(a.X / b, a.Y / b);
        public static implicit operator ReadOnlySpan<float>(Vector2 v) => new[] {v.X, v.Y};
        public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);
        public static bool operator !=(Vector2 left, Vector2 right) => !left.Equals(right);

        public bool Equals(Vector2 other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object obj) => obj is Vector2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }
    }
}