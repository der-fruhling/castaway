using Castaway.Native;

namespace Castaway.Math
{
    public abstract class Vector : Uniform
    {
        public float this[int i]
        {
            get => Data[i];
            set => Data[i] = value;
        }
        
        public abstract float[] Data { get; }
        
        public static implicit operator float(Vector v) => v[0];
    }

    public sealed class Vector2 : Vector
    {
        public override float[] Data { get; } = new float[2];
        
        public float X { get => this[0]; set => this[0] = value; }
        public float Y { get => this[1]; set => this[1] = value; }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(Vector3 v) : this(v.X, v.Y) { }
        
        public override unsafe void Upload(int uniform)
        {
            fixed(float* p = Data) CawNative.cawSetUniformF2(uniform, p, 1);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);
        public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.X / b.X, a.Y / b.Y);
        
        public static implicit operator Vector2(Vector3 v) => new(v.X, v.Y);
        public static implicit operator Vector2(Vector4 v) => new(v.X, v.Y);
    }

    public sealed class Vector3 : Vector
    {
        public override float[] Data { get; } = new float[3];
        
        public float X { get => this[0]; set => this[0] = value; }
        public float Y { get => this[1]; set => this[1] = value; }
        public float Z { get => this[2]; set => this[2] = value; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public Vector3(Vector2 v, float z) : this(v.X, v.Y, z) { }
        public Vector3(Vector4 v) : this(v.X, v.Y, v.Z) { }
        
        public override unsafe void Upload(int uniform)
        {
            fixed(float* p = Data) CawNative.cawSetUniformF3(uniform, p, 1);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static implicit operator Vector3(Vector2 v) => new(v, 0);
        public static implicit operator Vector3(Vector4 v) => new(v);
    }

    public sealed class Vector4 : Vector
    {
        public override float[] Data { get; } = new float[4];
        
        public float X { get => this[0]; set => this[0] = value; }
        public float Y { get => this[1]; set => this[1] = value; }
        public float Z { get => this[2]; set => this[2] = value; }
        public float W { get => this[3]; set => this[3] = value; }

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        
        public Vector4(Vector2 v, float z, float w) : this(v.X, v.Y, z, w) { }
        public Vector4(Vector3 v, float w) : this(v.X, v.Y, v.Z, w) { }
        
        public override unsafe void Upload(int uniform)
        {
            fixed(float* p = Data) CawNative.cawSetUniformF4(uniform, p, 1);
        }

        public static Vector4 operator +(Vector4 a, Vector4 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Vector4 operator -(Vector4 a, Vector4 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Vector4 operator *(Vector4 a, Vector4 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        public static Vector4 operator /(Vector4 a, Vector4 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

        public static implicit operator Vector4(Vector2 v) => new(v, 0, 0);
        public static implicit operator Vector4(Vector3 v) => new(v, 0);
    }
}