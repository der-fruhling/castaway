using System;

namespace Castaway.Math;

public struct Vector2
{
	public double X;
	public double Y;

	public Vector2(double x, double y) : this()
	{
		X = x;
		Y = y;
	}

	public static Vector2 operator +(Vector2 a, Vector2 b)
	{
		return new Vector2(a.X + b.X, a.Y + b.Y);
	}

	public static Vector2 operator -(Vector2 a, Vector2 b)
	{
		return new Vector2(a.X - b.X, a.Y - b.Y);
	}

	public static Vector2 operator *(Vector2 a, Vector2 b)
	{
		return new Vector2(a.X * b.X, a.Y * b.Y);
	}

	public static Vector2 operator /(Vector2 a, Vector2 b)
	{
		return new Vector2(a.X / b.X, a.Y / b.Y);
	}

	public static Vector2 operator +(Vector2 a, double b)
	{
		return new Vector2(a.X + b, a.Y + b);
	}

	public static Vector2 operator -(Vector2 a, double b)
	{
		return new Vector2(a.X - b, a.Y - b);
	}

	public static Vector2 operator *(Vector2 a, double b)
	{
		return new Vector2(a.X * b, a.Y * b);
	}

	public static Vector2 operator /(Vector2 a, double b)
	{
		return new Vector2(a.X / b, a.Y / b);
	}

	public static Vector2 operator +(Vector2 a, uint b)
	{
		return new Vector2(a.X + b, a.Y + b);
	}

	public static Vector2 operator -(Vector2 a, uint b)
	{
		return new Vector2(a.X - b, a.Y - b);
	}

	public static Vector2 operator *(Vector2 a, uint b)
	{
		return new Vector2(a.X * b, a.Y * b);
	}

	public static Vector2 operator /(Vector2 a, uint b)
	{
		return new Vector2(a.X / b, a.Y / b);
	}

	public static Vector2 operator +(Vector2 a, float b)
	{
		return new Vector2(a.X + b, a.Y + b);
	}

	public static Vector2 operator -(Vector2 a, float b)
	{
		return new Vector2(a.X - b, a.Y - b);
	}

	public static Vector2 operator *(Vector2 a, float b)
	{
		return new Vector2(a.X * b, a.Y * b);
	}

	public static Vector2 operator /(Vector2 a, float b)
	{
		return new Vector2(a.X / b, a.Y / b);
	}

	public static Vector2 operator +(Vector2 a, int b)
	{
		return new Vector2(a.X + b, a.Y + b);
	}

	public static Vector2 operator -(Vector2 a, int b)
	{
		return new Vector2(a.X - b, a.Y - b);
	}

	public static Vector2 operator *(Vector2 a, int b)
	{
		return new Vector2(a.X * b, a.Y * b);
	}

	public static Vector2 operator /(Vector2 a, int b)
	{
		return new Vector2(a.X / b, a.Y / b);
	}

	public static Vector2 operator -(Vector2 v)
	{
		return new Vector2(-v.X, -v.Y);
	}

	public static explicit operator double[](Vector2 v)
	{
		return new[] { v.X, v.Y };
	}

	public static explicit operator float[](Vector2 v)
	{
		return new[] { (float)v.X, (float)v.Y };
	}

	public static explicit operator uint[](Vector2 v)
	{
		return new[] { (uint)v.X, (uint)v.Y };
	}

	public static explicit operator int[](Vector2 v)
	{
		return new[] { (int)v.X, (int)v.Y };
	}

	public static bool operator ==(Vector2 left, Vector2 right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Vector2 left, Vector2 right)
	{
		return !left.Equals(right);
	}

	public bool Equals(Vector2 other)
	{
		return X.Equals(other.X) && Y.Equals(other.Y);
	}

	public override bool Equals(object? obj)
	{
		return obj is Vector2 other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	public override string ToString()
	{
		return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
	}
}