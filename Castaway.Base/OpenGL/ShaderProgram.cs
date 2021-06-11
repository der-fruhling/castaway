using System;
using System.Collections.Generic;
using Castaway.Native;
using Castaway.Rendering;
using static Castaway.Rendering.IGraphicsObject;

namespace Castaway.OpenGL
{
    public struct ShaderProgram : IOpenGLObject
    {
        public ObjectType Type => ObjectType.ShaderProgram;
        public bool Destroyed { get; set; }
        public uint Number { get; set; }

        public uint[] Shaders;
        public bool LinkSuccess => GL.GetProgram(Number, GL.ProgramQuery.LinkStatus) == 1;
        public Dictionary<string, VertexInputType> Inputs;
        public Dictionary<string, uint> Outputs;
        public Dictionary<string, UniformType> UniformBindings;
        public ShaderInputBinder InputBinder;
        public uint VAO;

        public string LinkLog
        {
            get
            {
                GL.GetProgramInfoLog(Number, out _, out var ret);
                return ret;
            }
        }

        public bool Equals(ShaderProgram other)
        {
            return Equals(Shaders, other.Shaders) && Equals(Inputs, other.Inputs) && Equals(Outputs, other.Outputs) &&
                   Equals(UniformBindings, other.UniformBindings) &&
                   Equals(InputBinder, other.InputBinder) && VAO == other.VAO && Destroyed == other.Destroyed &&
                   Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is ShaderProgram other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Shaders);
            hashCode.Add(Inputs);
            hashCode.Add(Outputs);
            hashCode.Add(UniformBindings);
            hashCode.Add(InputBinder);
            hashCode.Add(VAO);
            hashCode.Add(Destroyed);
            hashCode.Add(Number);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(ShaderProgram left, ShaderProgram right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderProgram left, ShaderProgram right)
        {
            return !left.Equals(right);
        }
    }
}