using System;
using System.Diagnostics.CodeAnalysis;

namespace Castaway.Math
{
    [SuppressMessage("ReSharper", "ArgumentsStyleOther")]
    public readonly struct Quaternion
    {
        public readonly double W, X, Y, Z;

        public Vector3 XYZ => new(X, Y, Z);

        public Quaternion(double w, double x, double y, double z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        public Quaternion(double w, Vector3 v) : this(w, v.X, v.Y, v.Z)
        {
        }

        public static Quaternion operator +(Quaternion a, Quaternion b)
        {
            return new(
                a.W + b.W, a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new(
                w: a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z,
                x: a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                y: a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z,
                z: a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X);
        }

        public static double Inner(Quaternion a, Quaternion b)
        {
            return a.W * b.W + a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public Quaternion Conjugate()
        {
            return new(W, -X, -Y, -Z);
        }

        public double Norm()
        {
            return System.Math.Sqrt(W * W + X * X + Y * Y + Z * Z);
        }

        public Quaternion Normalize()
        {
            var norm = Norm();
            return new Quaternion(W / norm, X / norm, Y / norm, Z / norm);
        }

        public Matrix3 ToMatrix()
        {
            return new(
                xx: 1 - 2 * (Y * Y + Z * Z),
                xy: 2 * (X * Y - W * Z),
                xz: 2 * (W * Y + X * Z),
                yx: 2 * (X * Y + W * Z),
                yy: 1 - 2 * (X * X + Z * Z),
                yz: 2 * (-W * X + Y * Z),
                zx: 2 * (-W * Y + X * Z),
                zy: 2 * (W * X + Y * Z),
                zz: 1 - 2 * (X * X + Y * Y));
        }

        public Matrix4 ToMatrix4()
        {
            var m = ToMatrix();
            return new Matrix4(
                new Vector4(m.X, 0),
                new Vector4(m.Y, 0),
                new Vector4(m.Z, 0),
                new Vector4(0, 0, 0, 1));
        }

        public static Quaternion Rotation(double x, double y, double z)
        {
            var cosYaw = System.Math.Cos(z * .5);
            var sinYaw = System.Math.Sin(z * .5);
            var cosPitch = System.Math.Cos(y * .5);
            var sinPitch = System.Math.Sin(y * .5);
            var cosRoll = System.Math.Cos(x * .5);
            var sinRoll = System.Math.Sin(x * .5);
            return new Quaternion(
                w: cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw,
                x: sinRoll * cosPitch * cosYaw + cosRoll * sinPitch * sinYaw,
                y: cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw,
                z: cosRoll * cosPitch * sinYaw + sinRoll * sinPitch * cosYaw);
        }

        public static Quaternion Rotation(Vector3 vector)
        {
            return Rotation(vector.X, vector.Y, vector.Z);
        }

        public static Quaternion DegreesRotation(float x, float y, float z)
        {
            return Rotation(MathEx.ToRadians(x), MathEx.ToRadians(y), MathEx.ToRadians(z));
        }

        public static Quaternion DegreesRotation(Vector3 v)
        {
            return Rotation(MathEx.ToRadians(v));
        }

        public Vector3 ToEulerAngles()
        {
            var vector = new Vector3();

            var xa = 2 * (W * X + Y * Z);
            var xb = 1 - 2 * (X * X + Y * Y);
            vector.X = System.Math.Atan2(xa, xb);

            var ya = 2 * (W * Y - Z * X);
            vector.Y = System.Math.Abs(ya) >= 1
                ? System.Math.CopySign(MathF.PI / 2, ya)
                : System.Math.Asin(ya);

            var za = 2 * (W * Z + X * Y);
            var zb = 1 - 2 * (Y * Y + Z * Z);
            vector.Z = System.Math.Atan2(za, zb);

            return vector;
        }

        public static Quaternion Rotation(Vector3 axis, float angle)
        {
            return new(
                w: MathF.Cos(angle / 2),
                x: MathF.Sin(angle / 2) * axis.X,
                y: MathF.Sin(angle / 2) * axis.Y,
                z: MathF.Sin(angle / 2) * axis.Z);
        }

        public static Quaternion DegreesRotation(Vector3 axis, float angle)
        {
            return Rotation(axis, MathEx.ToRadians(angle));
        }

        public static Vector3 operator *(Quaternion a, Vector3 b)
        {
            var t = Vector3.Cross(a.XYZ * 2, b);
            var v = b + t * a.W + Vector3.Cross(a.XYZ, t);
            return v;
        }
    }
}