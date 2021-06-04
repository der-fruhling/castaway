using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public struct Mesh
    {
        public struct Vertex
        {
            public Vector3 Position;
            public Vector4 Color;
            public Vector3 Normal;
            public Vector3 Texture;
            
            public Vertex(Vector3 position, Vector4 color, Vector3 normal, Vector3 texture)
            {
                Position = position;
                Color = color;
                Normal = normal;
                Texture = texture;
            }
        }

        public Vertex[] Vertices;
        public uint[] Elements;

        public Mesh(Vertex[] vertices, uint[] elements)
        {
            Vertices = vertices;
            Elements = elements;
        }

        private float[] ConstructVertexArray(ShaderProgram program)
        {
            var size = VertexSize(program.Inputs.Values);
            var value = new float[size * Vertices.Length];
            
            var j = 0;
            foreach (var v in Vertices)
            {
                foreach (var input in program.Inputs.Values)
                {
                    switch (input)
                    {
                        case VertexInputType.PositionXY:
                            value[j++] = v.Position.X;
                            value[j++] = v.Position.Y;
                            break;
                        case VertexInputType.PositionXYZ:
                            value[j++] = v.Position.X;
                            value[j++] = v.Position.Y;
                            value[j++] = v.Position.Z;
                            break;
                        case VertexInputType.ColorG:
                            value[j++] = v.Color.Y;
                            break;
                        case VertexInputType.ColorRGB:
                            value[j++] = v.Color.X;
                            value[j++] = v.Color.Y;
                            value[j++] = v.Color.Z;
                            break;
                        case VertexInputType.ColorRGBA:
                            value[j++] = v.Color.X;
                            value[j++] = v.Color.Y;
                            value[j++] = v.Color.Z;
                            value[j++] = v.Color.W;
                            break;
                        case VertexInputType.ColorBGRA:
                            value[j++] = v.Color.Z;
                            value[j++] = v.Color.Y;
                            value[j++] = v.Color.X;
                            value[j++] = v.Color.W;
                            break;
                        case VertexInputType.NormalXY:
                            value[j++] = v.Normal.X;
                            value[j++] = v.Normal.Y;
                            break;
                        case VertexInputType.NormalXYZ:
                            value[j++] = v.Normal.X;
                            value[j++] = v.Normal.Y;
                            value[j++] = v.Normal.Z;
                            break;
                        case VertexInputType.TextureS:
                            value[j++] = v.Texture.X;
                            break;
                        case VertexInputType.TextureST:
                            value[j++] = v.Texture.X;
                            value[j++] = v.Texture.Y;
                            break;
                        case VertexInputType.TextureSTV:
                            value[j++] = v.Texture.X;
                            value[j++] = v.Texture.Y;
                            value[j++] = v.Texture.Z;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(program), input, "Invalid input type.");
                    }
                }
            }

            return value;
        }

        private int VertexSize(ICollection<VertexInputType> values)
        {
            return values.Sum(v => v switch
            {
                VertexInputType.PositionXY => 2,
                VertexInputType.PositionXYZ => 3,
                VertexInputType.ColorG => 1,
                VertexInputType.ColorRGB => 3,
                VertexInputType.ColorRGBA => 4,
                VertexInputType.ColorBGRA => 4,
                VertexInputType.NormalXY => 2,
                VertexInputType.NormalXYZ => 3,
                VertexInputType.TextureS => 1,
                VertexInputType.TextureST => 2,
                VertexInputType.TextureSTV => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(values), v, null)
            });
        }

        public ElementDrawable ConstructFor(OpenGL g, ShaderProgram program)
        {
            var vertexBuffer = g.CreateBuffer(BufferTarget.VertexArray);
            g.Upload(vertexBuffer, ConstructVertexArray(program));

            var elementBuffer = g.CreateBuffer(BufferTarget.ElementArray);
            g.Upload(elementBuffer, Elements);

            return new ElementDrawable(vertexBuffer, elementBuffer, Elements.Length);
        }
    }
}