using System;
using System.Collections.Generic;
using static Castaway.Rendering.DrawBufferConstants;

namespace Castaway.Rendering
{
    public class VertexArray
    {
        private List<float> _data = new();
        private float[] _current = {
            0, 0, 0,    // position
            1, 1, 1, 1, // color
            0, 0, 0,    // normal
            0, 0, 0     // texture
        };

        public VertexArray Position(float x, float y, float z = 0)
        {
            _current[PositionX] = x;
            _current[PositionY] = y;
            _current[PositionZ] = z;
            return this;
        }

        public VertexArray Color(float r, float g, float b, float a = 1)
        {
            _current[ColorR] = r;
            _current[ColorG] = g;
            _current[ColorB] = b;
            _current[ColorA] = a;
            return this;
        }

        public VertexArray Normal(float x, float y, float z)
        {
            _current[NormalX] = x;
            _current[NormalY] = y;
            _current[NormalZ] = z;
            return this;
        }

        public VertexArray Texture(float u, float v = 0, float t = 0)
        {
            _current[TextureU] = u;
            _current[TextureV] = v;
            _current[TextureT] = t;
            return this;
        }

        public VertexArray Next()
        {
            _data.AddRange(_current);
            _current = new float[] {
                0, 0, 0,    // position
                1, 1, 1, 1, // color
                0, 0, 0,    // normal
                0, 0, 0     // texture
            };
            return this;
        }
        
        public static implicit operator Span<float>(VertexArray a) => a._data.ToArray();
    }
}