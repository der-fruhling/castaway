using System;
using System.Linq;

namespace Castaway.Math;

public struct Matrix2
{
    public Vector2 X;
    public Vector2 Y;

    public double[] Array => new[]
    {
        X.X, Y.X,
        X.Y, Y.Y
    };

    public float[] ArrayF => Array.Select(n => (float) n).ToArray();
    public int[] ArrayI => Array.Select(n => (int) n).ToArray();
    public uint[] ArrayU => Array.Select(n => (uint) n).ToArray();

    public Matrix2(Vector2 x, Vector2 y)
    {
        X = x;
        Y = y;
    }

    public Matrix2(
        double xx, double xy,
        double yx, double yy) :
        this(new Vector2(xx, xy),
            new Vector2(yx, yy))
    {
    }

    public static Matrix2 Ident => new(
        1, 0,
        0, 1);

    public static Matrix2 operator +(Matrix2 a, Matrix2 b)
    {
        return new(a.X + b.X, a.Y + b.Y);
    }

    public static Matrix2 operator -(Matrix2 a, Matrix2 b)
    {
        return new(a.X - b.X, a.Y - b.Y);
    }

    public static Matrix2 operator *(Matrix2 a, float b)
    {
        return new(a.X * b, a.Y * b);
    }

    public static bool operator ==(Matrix2 left, Matrix2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Matrix2 left, Matrix2 right)
    {
        return !left.Equals(right);
    }

    public static Vector2 operator *(Matrix2 a, Vector2 b)
    {
        return new(
            a.X.X * b.X + a.X.Y * b.Y,
            a.Y.X * b.X + a.Y.Y * b.Y);
    }

    public static Matrix2 operator *(Matrix2 a, Matrix2 b)
    {
        return new(
            a.X.X * b.X.X + a.Y.X * b.X.Y,
            a.X.X * b.Y.X + a.Y.X * b.Y.Y,
            a.Y.X * b.X.X + a.Y.Y * b.X.Y,
            a.Y.X * b.Y.X + a.Y.Y * b.Y.Y);
    }

    public static Matrix2 Scale(float x, float y)
    {
        var a = Ident;
        a.X.X = x;
        a.Y.Y = y;
        return a;
    }

    public bool Equals(Matrix2 other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Matrix2 other && Equals(other);
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