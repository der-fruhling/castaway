using System;
using static System.MathF;

namespace Castaway.Math
{
    public class Matrix4
    {
        public float[] Array { get; } = new float[4 * 4];
        
        public float A { get => Array[0 ]; set => Array[0 ] = value; }
        public float B { get => Array[1 ]; set => Array[1 ] = value; }
        public float C { get => Array[2 ]; set => Array[2 ] = value; }
        public float D { get => Array[3 ]; set => Array[3 ] = value; }
        public float E { get => Array[4 ]; set => Array[4 ] = value; }
        public float F { get => Array[5 ]; set => Array[5 ] = value; }
        public float G { get => Array[6 ]; set => Array[6 ] = value; }
        public float H { get => Array[7 ]; set => Array[7 ] = value; }
        public float I { get => Array[8 ]; set => Array[8 ] = value; }
        public float J { get => Array[9 ]; set => Array[9 ] = value; }
        public float K { get => Array[10]; set => Array[10] = value; }
        public float L { get => Array[11]; set => Array[11] = value; }
        public float M { get => Array[12]; set => Array[12] = value; }
        public float N { get => Array[13]; set => Array[13] = value; }
        public float O { get => Array[14]; set => Array[14] = value; }
        public float P { get => Array[15]; set => Array[15] = value; }

        /// <summary>
        /// Identity matrix.
        /// </summary>
        public static Matrix4 Identity => new Matrix4(new float[] {1,0,0,0,
                                                                        0,1,0,0,
                                                                        0,0,1,0,
                                                                        0,0,0,1});

        public Matrix4(float[] floats)
        {
            if (floats.Length != 4 * 4)
                throw new ApplicationException("Cannot initialize Matrix4 with an array that isn't 4*4 long");
            floats.CopyTo(Array, 0);
        }
        
        private Matrix4() { }
        
        /// <summary>
        /// Adds the values of 2 matrices.
        /// </summary>
        /// <param name="a">Left.</param>
        /// <param name="b">Right.</param>
        /// <returns>New matrix.</returns>
        public static Matrix4 operator +(Matrix4 a, Matrix4 b)
        {
            var m = new Matrix4();
            
            for (var i = 0; i < m.Array.Length; i++) 
                m.Array[i] = a.Array[i] + b.Array[i];

            return m;
        }
        
        /// <summary>
        /// Subtracts the values of 2 matrices.
        /// </summary>
        /// <param name="a">Left.</param>
        /// <param name="b">Right.</param>
        /// <returns>New matrix.</returns>
        public static Matrix4 operator -(Matrix4 a, Matrix4 b)
        {
            var m = new Matrix4();
            
            for (var i = 0; i < m.Array.Length; i++) 
                m.Array[i] = a.Array[i] - b.Array[i];

            return m;
        }

        /// <summary>
        /// Multiplies a matrix by a scalar.
        /// </summary>
        /// <param name="a">Left.</param>
        /// <param name="b">Right.</param>
        /// <returns>New matrix.</returns>
        public static Matrix4 operator *(Matrix4 a, float b)
        {
            var m = new Matrix4();
            
            for (var i = 0; i < m.Array.Length; i++) 
                m.Array[i] = a.Array[i] * b;

            return m;
        }

        /// <summary>
        /// Applies this matrix to a vector
        /// </summary>
        /// <param name="a">Left.</param>
        /// <param name="b">Right.</param>
        /// <returns>Magicked vector.</returns>
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

        /// <summary>
        /// Shorthand for <c>a * new Vector4(b, 1)</c>.
        /// </summary>
        /// <inheritdoc cref="op_Multiply(Castaway.Math.Matrix4,Castaway.Math.Vector4)"/>
        public static Vector3 operator *(Matrix4 a, Vector3 b)
        {
            var b1 = new Vector4(b, 1);
            var v = a * b1;
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Matrix4 operator *(Matrix4 a, Matrix4 b)
        {
            return new Matrix4
            {
                A = a.A * b.A  +  a.B * b.E  +  a.C * b.I  +  a.D * b.M,
                B = a.A * b.B  +  a.B * b.F  +  a.C * b.J  +  a.D * b.N,
                C = a.A * b.C  +  a.B * b.G  +  a.C * b.K  +  a.D * b.O,
                D = a.A * b.D  +  a.B * b.H  +  a.C * b.L  +  a.D * b.P,
                
                E = a.E * b.A  +  a.F * b.E  +  a.G * b.I  +  a.H * b.M,
                F = a.E * b.B  +  a.F * b.F  +  a.G * b.J  +  a.H * b.N,
                G = a.E * b.C  +  a.F * b.G  +  a.G * b.K  +  a.H * b.O,
                H = a.E * b.D  +  a.F * b.H  +  a.G * b.L  +  a.H * b.P,
                
                I = a.I * b.A  +  a.J * b.E  +  a.K * b.I  +  a.L * b.M,
                J = a.I * b.B  +  a.J * b.F  +  a.K * b.J  +  a.L * b.N,
                K = a.I * b.C  +  a.J * b.G  +  a.K * b.K  +  a.L * b.O,
                L = a.I * b.D  +  a.J * b.H  +  a.K * b.L  +  a.L * b.P,
                
                M = a.M * b.A  +  a.N * b.E  +  a.O * b.I  +  a.P * b.M,
                N = a.M * b.B  +  a.N * b.F  +  a.O * b.J  +  a.P * b.N,
                O = a.M * b.C  +  a.N * b.G  +  a.O * b.K  +  a.P * b.O,
                P = a.M * b.D  +  a.N * b.H  +  a.O * b.L  +  a.P * b.P
            };
        }

        private bool Equals(Matrix4 other)
        {
            return Equals(Array, other.Array);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Matrix4) obj);
        }

        public override int GetHashCode()
        {
            return Array != null ? Array.GetHashCode() : 0;
        }

        /// <summary>
        /// Translates this matrix by <paramref name="v"/>
        /// </summary>
        /// <param name="v">Vector to translate by.</param>
        /// <returns>New matrix.</returns>
        public static Matrix4 Translate(Vector3 v) =>
            new Matrix4(Identity.Array) {D = v.X, H = v.Y, L = v.Z, P = 1};

        /// <summary>
        /// Scales this matrix by <paramref name="v"/>
        /// </summary>
        /// <param name="v">Vector to scale by.</param>
        /// <returns>New matrix.</returns>
        public static Matrix4 Scale(Vector3 v) =>
            new Matrix4(Identity.Array) {A = v.X, F = v.Y, K = v.Z, P = 1};

        public static Matrix4 RotateX(float x) =>
            new Matrix4(Identity.Array) {F = Cos(x), G = -Sin(x), J = Sin(x), K = Cos(x)};

        public static Matrix4 RotateY(float x) =>
            new Matrix4(Identity.Array) {A = Cos(x), C = Sin(x), I = -Sin(x), K = Cos(x)};

        public static Matrix4 RotateZ(float x) => 
            new Matrix4(Identity.Array) {A = Cos(x), B = -Sin(x), E = Sin(x), F = Cos(x)};

        public static Matrix4 Rotate(Vector3 v) => RotateZ(v.Z) * RotateY(v.Y) * RotateX(v.X);

        public static Matrix4 RotateXDeg(float x) => RotateX(x * (PI / 180f));
        public static Matrix4 RotateYDeg(float x) => RotateY(x * (PI / 180f));
        public static Matrix4 RotateZDeg(float x) => RotateZ(x * (PI / 180f));
        public static Matrix4 RotateDeg(Vector3 v) => RotateZDeg(v.Z) * RotateYDeg(v.Y) * RotateXDeg(v.X);

        public void Print()
        {
            Console.WriteLine($"{A},{B},{C},{D}\n" +
                              $"{E},{F},{G},{H}\n" +
                              $"{I},{J},{K},{L}\n" +
                              $"{M},{N},{O},{P}");
        }

        public Matrix4 Transpose() => new Matrix4(new[]
        {
            A,E,I,M,
            B,F,J,N,
            C,G,K,O,
            D,H,L,P
        });
    }
}