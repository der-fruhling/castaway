namespace Castaway.Math
{
    public struct Vector4
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Vector4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(Vector3 v, double w) : this(v.X, v.Y, v.Z, w) {}
        public Vector4(Vector2 v, double z, double w) : this(v.X, v.Y, z, w) {}
        public Vector4(float x, float y, float z, float w) : this((double) x, y, z, w) {}
        public Vector4(uint x, uint y, uint z, uint w) : this((double) x, y, z, w) {}
        public Vector4(int x, int y, int z, int w) : this((double) x, y, z, w) {}

        public static Vector4 operator +(Vector4 a, Vector4 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Vector4 operator -(Vector4 a, Vector4 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Vector4 operator *(Vector4 a, Vector4 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        public static Vector4 operator /(Vector4 a, Vector4 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
        
        public static Vector4 operator +(Vector4 a, double b) => new(a.X + b, a.Y + b, a.Z + b, a.W + b);
        public static Vector4 operator -(Vector4 a, double b) => new(a.X - b, a.Y - b, a.Z - b, a.W - b);
        public static Vector4 operator *(Vector4 a, double b) => new(a.X * b, a.Y * b, a.Z * b, a.W * b);
        public static Vector4 operator /(Vector4 a, double b) => new(a.X / b, a.Y / b, a.Z / b, a.W / b);
        public static Vector4 operator +(Vector4 a, uint b) => new(a.X + b, a.Y + b, a.Z + b, a.W + b);
        public static Vector4 operator -(Vector4 a, uint b) => new(a.X - b, a.Y - b, a.Z - b, a.W - b);
        public static Vector4 operator *(Vector4 a, uint b) => new(a.X * b, a.Y * b, a.Z * b, a.W * b);
        public static Vector4 operator /(Vector4 a, uint b) => new(a.X / b, a.Y / b, a.Z / b, a.W / b);
        public static Vector4 operator +(Vector4 a, float b) => new(a.X + b, a.Y + b, a.Z + b, a.W + b);
        public static Vector4 operator -(Vector4 a, float b) => new(a.X - b, a.Y - b, a.Z - b, a.W - b);
        public static Vector4 operator *(Vector4 a, float b) => new(a.X * b, a.Y * b, a.Z * b, a.W * b);
        public static Vector4 operator /(Vector4 a, float b) => new(a.X / b, a.Y / b, a.Z / b, a.W / b);
        public static Vector4 operator +(Vector4 a, int b) => new(a.X + b, a.Y + b, a.Z + b, a.W + b);
        public static Vector4 operator -(Vector4 a, int b) => new(a.X - b, a.Y - b, a.Z - b, a.W - b);
        public static Vector4 operator *(Vector4 a, int b) => new(a.X * b, a.Y * b, a.Z * b, a.W * b);
        public static Vector4 operator /(Vector4 a, int b) => new(a.X / b, a.Y / b, a.Z / b, a.W / b);
        
        public static Vector4 operator -(Vector4 v) => new(-v.X, -v.Y, -v.Z, -v.W);

        public static explicit operator double[](Vector4 v) => new[] {v.X, v.Y, v.Z, v.W};
        public static explicit operator float[](Vector4 v) => new[] {(float) v.X, (float) v.Y, (float) v.Z, (float) v.W};
        public static explicit operator uint[](Vector4 v) => new[] {(uint) v.X, (uint) v.Y, (uint) v.Z, (uint) v.W};
        public static explicit operator int[](Vector4 v) => new[] {(int) v.X, (int) v.Y, (int) v.Z, (int) v.W};

        public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";
    }
}