using System;

namespace Castaway.Math
{
    public class Matrix4
    {
        private readonly float[] _array = new float[4 * 4];
        
        public float A { get => _array[0];  set => _array[0] = value;  }
        public float B { get => _array[1];  set => _array[1] = value;  }
        public float C { get => _array[2];  set => _array[2] = value;  }
        public float D { get => _array[3];  set => _array[3] = value;  }
        public float E { get => _array[4];  set => _array[4] = value;  }
        public float F { get => _array[5];  set => _array[5] = value;  }
        public float G { get => _array[6];  set => _array[6] = value;  }
        public float H { get => _array[7];  set => _array[7] = value;  }
        public float I { get => _array[8];  set => _array[8] = value;  }
        public float J { get => _array[9];  set => _array[9] = value;  }
        public float K { get => _array[10]; set => _array[10] = value; }
        public float L { get => _array[11]; set => _array[11] = value; }
        public float M { get => _array[12]; set => _array[12] = value; }
        public float N { get => _array[13]; set => _array[13] = value; }
        public float O { get => _array[14]; set => _array[14] = value; }
        public float P { get => _array[15]; set => _array[15] = value; }

        public static Matrix4 Identity => new Matrix4(new float[] {1,0,0,0,
                                                                        0,1,0,0,
                                                                        0,0,1,0,
                                                                        0,0,0,1});

        public Matrix4(float[] floats)
        {
            if (floats.Length != 4 * 4)
                throw new ApplicationException("Cannot initialize Matrix4 with an array that isn't 4*4 long");
            floats.CopyTo(_array, 0);
        }
        
        private Matrix4() { }
        
        public static Matrix4 operator +(Matrix4 a, Matrix4 b)
        {
            var m = new Matrix4();
            
            for (var i = 0; i < m._array.Length; i++) 
                m._array[i] = a._array[i] + b._array[i];

            return m;
        }

        public static Matrix4 operator -(Matrix4 a, Matrix4 b)
        {
            var m = new Matrix4();
            
            for (var i = 0; i < m._array.Length; i++) 
                m._array[i] = a._array[i] - b._array[i];

            return m;
        }

        public static Matrix4 operator *(Matrix4 a, float b)
        {
            var m = new Matrix4();
            
            for (var i = 0; i < m._array.Length; i++) 
                m._array[i] = a._array[i] * b;

            return m;
        }

        public static Vector4 operator *(Matrix4 a, Vector4 b)
        {
            return new Vector4
            {
                X = a.A * b.X + a.B * b.Y + a.C * b.Z + a.D * b.W,
                Y = a.E * b.X + a.F * b.Y + a.G * b.Z + a.H * b.W,
                Z = a.I * b.X + a.J * b.Y + a.K * b.Z + a.L * b.W,
                W = a.M * b.X + a.N * b.Y + a.O * b.Z + a.P * b.W
            };
        }

        public static Vector3 operator *(Matrix4 a, Vector3 b)
        {
            var b1 = new Vector4(b, 1);
            var v = a * b1;
            return new Vector3(v.X, v.Y, v.Z);
        }

        private bool Equals(Matrix4 other)
        {
            return Equals(_array, other._array);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Matrix4) obj);
        }

        public override int GetHashCode()
        {
            return _array != null ? _array.GetHashCode() : 0;
        }

        public Matrix4 Translate(Vector3 v)
        {
            return new Matrix4(_array)
            {
                D = v.X,
                H = v.Y,
                L = v.Z,
                P = 1
            };
        }
    }
}