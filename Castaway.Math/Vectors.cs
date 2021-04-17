namespace Castaway.Math
{
    public class Vector
    {
        public float[] Array { get; }

        public Vector(int size)
        {
            Array = new float[size];
        }
    }

    public class Vector2 : Vector
    {
        public float X { get => Array[0]; set => Array[0] = value; }
        public float Y { get => Array[1]; set => Array[1] = value; }

        /// <summary>
        /// Vector with all components set to zero.
        /// </summary>
        public static readonly Vector2 Zero = new Vector2(0, 0);
        
        public Vector2() : base(2) { }

        public Vector2(float x, float y) : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Computes the dot product with this vector and
        /// <paramref name="b"/>.
        /// </summary>
        /// <param name="b">Other vector.</param>
        /// <returns>Dot product.</returns>
        public float Dot(Vector2 b)
        {
            var a = this;
            return a.X * b.X + 
                   a.Y * b.Y;
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float i) => new Vector2(
            CMath.Lerp(a.X, b.X, i),
            CMath.Lerp(a.Y, b.Y, i));
    }

    public class Vector3 : Vector
    {
        public float X { get => Array[0]; set => Array[0] = value; }
        public float Y { get => Array[1]; set => Array[1] = value; }
        public float Z { get => Array[2]; set => Array[2] = value; }
        
        public float R => X;
        public float G => Y;
        public float B => Z;
        
        /// <summary>
        /// Vector with all components set to zero.
        /// </summary>
        public static Vector3 Zero => new Vector3(0, 0, 0);
        
        public Vector3() : base(3) { }

        public Vector3(float x, float y, float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public Vector3(Vector2 v, float z = 0) : this(v.X, v.Y, z) {}

        /// <summary>
        /// Computes the cross product with this vector and
        /// <paramref name="b"/>.
        /// </summary>
        /// <param name="b">Other vector.</param>
        /// <returns>Cross product.</returns>
        public Vector3 Cross(Vector3 b)
        {
            var a = this;
            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
        }
        
        /// <summary>
        /// Computes the dot product with this vector and
        /// <paramref name="b"/>.
        /// </summary>
        /// <param name="b">Other vector.</param>
        /// <returns>Dot product.</returns>
        public float Dot(Vector3 b)
        {
            var a = this;
            return a.X * b.X + 
                   a.Y * b.Y + 
                   a.Z * b.Z;
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
            => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b)
            => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 a, Vector3 b)
            => new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3 operator /(Vector3 a, Vector3 b)
            => new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3 operator +(Vector3 a, float b)
            => new Vector3(a.X + b, a.Y + b, a.Z + b);
        public static Vector3 operator -(Vector3 a, float b)
            => new Vector3(a.X - b, a.Y - b, a.Z - b);
        public static Vector3 operator *(Vector3 a, float b)
            => new Vector3(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator /(Vector3 a, float b)
            => new Vector3(a.X / b, a.Y / b, a.Z / b);

        public static Vector3 operator -(Vector3 a)
            => new Vector3(-a.X, -a.Y, -a.Z);
        
        public static Vector3 Lerp(Vector3 a, Vector3 b, float i) => new Vector3(
            CMath.Lerp(a.X, b.X, i),
            CMath.Lerp(a.Y, b.Y, i),
            CMath.Lerp(a.Z, b.Z, i));
    }

    public class Vector4 : Vector
    {
        public float X { get => Array[0]; set => Array[0] = value; }
        public float Y { get => Array[1]; set => Array[1] = value; }
        public float Z { get => Array[2]; set => Array[2] = value; }
        public float W { get => Array[3]; set => Array[3] = value; }
        
        public float R => X;
        public float G => Y;
        public float B => Z;
        public float A => W;
        
        /// <summary>
        /// Vector with all components set to zero.
        /// </summary>
        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);

        public Vector4() : base(4) { }

        public Vector4(float x, float y, float z, float w) : this()
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        
        public Vector4(Vector2 v, float z = 0, float w = 0) : this(v.X, v.Y, z, w) {}
        public Vector4(Vector3 v, float w = 0) : this(v.X, v.Y, v.Z, w) {}

        /// <summary>
        /// Computes the dot product with this vector and
        /// <paramref name="b"/>.
        /// </summary>
        /// <param name="b">Other vector.</param>
        /// <returns>Dot product.</returns>
        public float Dot(Vector4 b)
        {
            var a = this;
            return a.X * b.X + 
                   a.Y * b.Y + 
                   a.Z * b.Z + 
                   a.W * b.W;
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";
        }
        
        public static Vector4 Lerp(Vector4 a, Vector4 b, float i) => new Vector4(
            CMath.Lerp(a.X, b.X, i),
            CMath.Lerp(a.Y, b.Y, i),
            CMath.Lerp(a.Z, b.Z, i),
            CMath.Lerp(a.W, b.W, i));
    }
}
