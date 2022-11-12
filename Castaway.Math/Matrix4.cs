using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Castaway.Math;

public struct Matrix4
{
    public Vector4 X;
    public Vector4 Y;
    public Vector4 Z;
    public Vector4 W;

    public double[] Array => new[]
    {
        X.X, Y.X, Z.X, W.X,
        X.Y, Y.Y, Z.Y, W.Y,
        X.Z, Y.Z, Z.Z, W.Z,
        X.W, Y.W, Z.W, W.W
    };

    public float[] ArrayF => Array.Select(n => (float) n).ToArray();
    public int[] ArrayI => Array.Select(n => (int) n).ToArray();
    public uint[] ArrayU => Array.Select(n => (uint) n).ToArray();

    public Matrix4(Vector4 x, Vector4 y, Vector4 z, Vector4 w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Matrix4(
        double xx, double xy, double xz, double xw,
        double yx, double yy, double yz, double yw,
        double zx, double zy, double zz, double zw,
        double wx, double wy, double wz, double ww)
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

    [SuppressMessage("ReSharper", "ArgumentsStyleOther")]
    public static Matrix4 operator *(Matrix4 a, Matrix4 b)
    {
        return new(
            xx: a.X.X * b.X.X + a.X.Y * b.Y.X + a.X.Z * b.Z.X + a.X.W * b.W.X,
            xy: a.X.X * b.X.Y + a.X.Y * b.Y.Y + a.X.Z * b.Z.Y + a.X.W * b.W.Y,
            xz: a.X.X * b.X.Z + a.X.Y * b.Y.Z + a.X.Z * b.Z.Z + a.X.W * b.W.Z,
            xw: a.X.X * b.X.W + a.X.Y * b.Y.W + a.X.Z * b.Z.W + a.X.W * b.W.W,
            yx: a.Y.X * b.X.X + a.Y.Y * b.Y.X + a.Y.Z * b.Z.X + a.Y.W * b.W.X,
            yy: a.Y.X * b.X.Y + a.Y.Y * b.Y.Y + a.Y.Z * b.Z.Y + a.Y.W * b.W.Y,
            yz: a.Y.X * b.X.Z + a.Y.Y * b.Y.Z + a.Y.Z * b.Z.Z + a.Y.W * b.W.Z,
            yw: a.Y.X * b.X.W + a.Y.Y * b.Y.W + a.Y.Z * b.Z.W + a.Y.W * b.W.W,
            zx: a.Z.X * b.X.X + a.Z.Y * b.Y.X + a.Z.Z * b.Z.X + a.Z.W * b.W.X,
            zy: a.Z.X * b.X.Y + a.Z.Y * b.Y.Y + a.Z.Z * b.Z.Y + a.Z.W * b.W.Y,
            zz: a.Z.X * b.X.Z + a.Z.Y * b.Y.Z + a.Z.Z * b.Z.Z + a.Z.W * b.W.Z,
            zw: a.Z.X * b.X.W + a.Z.Y * b.Y.W + a.Z.Z * b.Z.W + a.Z.W * b.W.W,
            wx: a.W.X * b.X.X + a.W.Y * b.Y.X + a.W.Z * b.Z.X + a.W.W * b.W.X,
            wy: a.W.X * b.X.Y + a.W.Y * b.Y.Y + a.W.Z * b.Z.Y + a.W.W * b.W.Y,
            wz: a.W.X * b.X.Z + a.W.Y * b.Y.Z + a.W.Z * b.Z.Z + a.W.W * b.W.Z,
            ww: a.W.X * b.X.W + a.W.Y * b.Y.W + a.W.Z * b.Z.W + a.W.W * b.W.W);
    }

    public static Matrix4 Scale(double x, double y, double z, double w = 1)
    {
        var a = Ident;
        a.X.X = x;
        a.Y.Y = y;
        a.Z.Z = z;
        a.W.W = w;
        return a;
    }

    public static Matrix4 Scale(Vector3 v)
    {
        return Scale(v.X, v.Y, v.Z);
    }

    public static Matrix4 Scale(Vector4 v)
    {
        return Scale(v.X, v.Y, v.Z, v.W);
    }

    public static Matrix4 Translate(double x, double y, double z)
    {
        var a = Ident;
        a.X.W = x;
        a.Y.W = y;
        a.Z.W = z;
        return a;
    }

    public static Matrix4 Translate(Vector3 v)
    {
        return Translate(v.X, v.Y, v.Z);
    }

    public bool Equals(Matrix4 other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
    }

    public override bool Equals(object? obj)
    {
        return obj is Matrix4 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    public override string ToString()
    {
        return $"{nameof(X)}: ({X}), {nameof(Y)}: ({Y}), {nameof(Z)}: ({Z}), {nameof(W)}: ({W})";
    }
}