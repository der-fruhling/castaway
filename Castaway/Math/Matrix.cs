using System;
using Castaway.Native;
using static System.Math;
using static Castaway.Native.CawNative;

namespace Castaway.Math
{
    public class Matrix : Uniform
    {
        private readonly int _width, _height;
        private readonly float[] _data;
        
        public Matrix(int w, int h)
        {
            _data = new float[w * h];
            _width = w;
            _height = h;
            for (var i = 0; i < Min(w, h); i++) this[i, i] = 1;
        }
        
        public Matrix(int s) : this(s, s) {}

        public float this[int x, int y]
        {
            get => _data[y * _width + x];
            set => _data[y * _width + x] = value;
        }

        public override unsafe void Upload(int uniform)
        {
            fixed (float* ptr = _data)
            {
                switch (_width, _height)
                {
                    case (2, 2): cawSetUniformF2x2(uniform, ptr, 1); break;
                    case (2, 3): cawSetUniformF2x3(uniform, ptr, 1); break;
                    case (2, 4): cawSetUniformF2x4(uniform, ptr, 1); break;
                    
                    case (3, 2): cawSetUniformF3x2(uniform, ptr, 1); break;
                    case (3, 3): cawSetUniformF3x3(uniform, ptr, 1); break;
                    case (3, 4): cawSetUniformF3x4(uniform, ptr, 1); break;
                    
                    case (4, 2): cawSetUniformF4x2(uniform, ptr, 1); break;
                    case (4, 3): cawSetUniformF4x3(uniform, ptr, 1); break;
                    case (4, 4): cawSetUniformF4x4(uniform, ptr, 1); break;
                    
                    default: throw new InvalidOperationException($"Cannot upload a matrix of size {_width}x{_height}");
                }
            }
        }
    }
}