using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Base;
using Castaway.Rendering;
using Castaway.Rendering.Structures;
using Serilog;

namespace Castaway.OpenGL
{
    public static class OpenGLEx
    {
        private static readonly ILogger Logger = CastawayGlobal.GetLogger();

        #region Mesh

        private static float[] ConstructVertexArray(this Mesh mesh, ShaderObject shader)
        {
            var size = VertexSize(shader.GetInputs().Select(shader.GetInput).ToList());
            var value = new double[size * mesh.Vertices.Length];

            var j = 0;
            foreach (var v in mesh.Vertices)
            foreach (var input in shader.GetInputs().Select(shader.GetInput))
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
#pragma warning disable 618
                    case VertexInputType.ColorBGRA:
                        value[j++] = v.Color.Z;
                        value[j++] = v.Color.Y;
                        value[j++] = v.Color.X;
                        value[j++] = v.Color.W;
                        break;
#pragma warning restore 618
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
                        throw new ArgumentOutOfRangeException(nameof(shader), input, "Invalid input type.");
                }

            return value.Select(n => (float) n).ToArray();
        }

        private static int VertexSize(ICollection<VertexInputType> values)
        {
            return values.Sum(v => v switch
            {
                VertexInputType.PositionXY => 2,
                VertexInputType.PositionXYZ => 3,
                VertexInputType.ColorG => 1,
                VertexInputType.ColorRGB => 3,
                VertexInputType.ColorRGBA => 4,
#pragma warning disable 618
                VertexInputType.ColorBGRA => 4,
#pragma warning restore 618
                VertexInputType.NormalXY => 2,
                VertexInputType.NormalXYZ => 3,
                VertexInputType.TextureS => 1,
                VertexInputType.TextureST => 2,
                VertexInputType.TextureSTV => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(values), v, null)
            });
        }

        public static Drawable ConstructFor(this Mesh mesh, ShaderObject shader)
        {
            var vertexBuffer = new Buffer(BufferTarget.VertexArray, mesh.ConstructVertexArray(shader));
            var elementBuffer = new Buffer(BufferTarget.ElementArray, mesh.Elements);

            return Graphics.Current switch
            {
                OpenGLImpl => new VertexArrayDrawable(mesh.Elements.Length, vertexBuffer, elementBuffer),
                _ => new Drawable(mesh.Elements.Length, vertexBuffer, elementBuffer)
            };
        }

        [Obsolete("Use " + nameof(ConstructFor) + " instead")]
        public static Drawable ConstructUnoptimisedFor(this Mesh mesh, ShaderObject shader)
        {
            var vertexBuffer = new Buffer(BufferTarget.VertexArray, mesh.ConstructVertexArray(shader));
            var elementBuffer = new Buffer(BufferTarget.ElementArray, mesh.Elements);

            return new Drawable(mesh.Elements.Length, vertexBuffer, elementBuffer);
        }

        #endregion
    }
}