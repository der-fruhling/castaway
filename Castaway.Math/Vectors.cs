namespace Castaway.Math
{
    public class Vector
    {
        public float[] Array { get; set; }
        private int _size;

        protected Vector(int size)
        {
            Array = new float[size];
            _size = size;
        }
    }

    public class Vector2 : Vector
    {
        public float X { get => Array[0]; set => Array[0] = value; }
        public float Y { get => Array[1]; set => Array[1] = value; }

        public static readonly Vector2 Zero = new Vector2(0, 0);
        
        public Vector2() : base(2) { }

        public Vector2(float x, float y) : this()
        {
            X = x;
            Y = y;
        }

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
    }

    public class Vector3 : Vector
    {
        public float X { get => Array[0]; set => Array[0] = value; }
        public float Y { get => Array[1]; set => Array[1] = value; }
        public float Z { get => Array[2]; set => Array[2] = value; }
        
        public float R => X;
        public float G => Y;
        public float B => Z;
        
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        
        public Vector3() : base(3) { }

        public Vector3(float x, float y, float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public Vector3(Vector2 v, float z) : this(v.X, v.Y, z) {}

        public Vector3 Cross(Vector3 b)
        {
            var a = this;
            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
        }

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
        
        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);

        public Vector4() : base(4) { }

        public Vector4(float x, float y, float z, float w) : this()
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        
        public Vector4(Vector2 v, float z, float w) : this(v.X, v.Y, z, w) {}
        public Vector4(Vector3 v, float w) : this(v.X, v.Y, v.Z, w) {}

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
    }
}
