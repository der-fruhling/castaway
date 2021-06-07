using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Castaway.Math;
using Castaway.Rendering;
using static Castaway.Rendering.VertexInputType;

namespace Castaway.OpenGL
{
    [Obsolete("Use Mesh instead.", true)]
    public class VertexArrayConstructor
    {
        private ShaderProgram _program;
        private List<float[]> _data = new();
        private List<float> _bufferData = new();

        public VertexArrayConstructor(ShaderProgram program)
        {
            _program = program;
        }

        public void New()
        {
            _data = new List<float[]> {new float[13]};
            _bufferData.Clear();
        }

        public void Next()
        {
            List<float> realData = new();
            foreach (var (_, type) in _program.Inputs)
                switch (type)
                {
                    case PositionXY:
                        realData.AddRange(_data.Last()[..2]);
                        break;
                    case PositionXYZ:
                        realData.AddRange(_data.Last()[..3]);
                        break;
                    case ColorG:
                        realData.Add(_data.Last()[4]);
                        break;
                    case ColorRGB:
                        realData.AddRange(_data.Last()[3..6]);
                        break;
                    case ColorRGBA:
                    case VertexInputType.ColorBGRA:
                        realData.AddRange(_data.Last()[3..7]);
                        break;
                    case NormalXY:
                        realData.AddRange(_data.Last()[7..9]);
                        break;
                    case NormalXYZ:
                        realData.AddRange(_data.Last()[7..10]);
                        break;
                    case TextureS:
                        realData.Add(_data.Last()[10]);
                        break;
                    case TextureST:
                        realData.AddRange(_data.Last()[10..12]);
                        break;
                    case TextureSTV:
                        realData.AddRange(_data.Last()[10..13]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            _data.Add(new float[13]);
            _bufferData.AddRange(realData);
        }

        public VertexArrayConstructor Put(int i, params float[] data)
        {
            data.CopyTo(_data.Last(), i);
            return this;
        }

        public VertexArrayConstructor Position(float x, float y)
        {
            return Put(0, x, y);
        }

        public VertexArrayConstructor Position(float x, float y, float z)
        {
            return Put(0, x, y, z);
        }

        public VertexArrayConstructor Position(Vector2 v)
        {
            return Put(0, v.X, v.Y);
        }

        public VertexArrayConstructor Position(Vector2 xy, float z)
        {
            return Put(0, xy.X, xy.Y, z);
        }

        public VertexArrayConstructor Position(Vector3 v)
        {
            return Put(0, v.X, v.Y, v.Z);
        }

        public VertexArrayConstructor Color3(float r, float g, float b)
        {
            return Put(3, r, g, b);
        }

        public VertexArrayConstructor Color(float r, float g, float b, float a)
        {
            return Put(3, r, g, b, a);
        }

        public VertexArrayConstructor Color3(Color c)
        {
            return Put(3, (float) c.R / byte.MaxValue, (float) c.G / byte.MaxValue, (float) c.B / byte.MaxValue);
        }

        public VertexArrayConstructor Color(Color c)
        {
            return Put(3, (float) c.R / byte.MaxValue, (float) c.G / byte.MaxValue, (float) c.B / byte.MaxValue,
                (float) c.A / byte.MaxValue);
        }

        public VertexArrayConstructor ColorBGRA(Color c)
        {
            return Put(3, (float) c.B / byte.MaxValue, (float) c.G / byte.MaxValue, (float) c.R / byte.MaxValue,
                (float) c.A / byte.MaxValue);
        }

        public VertexArrayConstructor Normal(float x, float y, float z)
        {
            return Put(7, x, y, z);
        }

        public VertexArrayConstructor Normal(Vector3 v)
        {
            return Put(7, v.X, v.Y, v.Z);
        }

        public VertexArrayConstructor Texture(float u)
        {
            return Put(10, u);
        }

        public VertexArrayConstructor Texture(float u, float v)
        {
            return Put(10, u, v);
        }

        public VertexArrayConstructor Texture(float u, float v, float t)
        {
            return Put(10, u, v, t);
        }

        public VertexArrayConstructor Texture(Vector2 v)
        {
            return Put(10, v.X, v.Y);
        }

        public VertexArrayConstructor Texture(Vector3 v)
        {
            return Put(10, v.X, v.Y, v.Z);
        }
    }
}