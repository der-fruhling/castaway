using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.OpenGL.Native;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    internal sealed class Buffer : BufferObject
    {
        public bool Destroyed { get; set; }
        public uint Number { get; set; }

        public uint SetupProgram;
        public BufferTarget Target;
        public override string Name => $"{Target}->{Number}({Valid})";
        public override bool Valid => GL.IsBuffer(Number) && !Destroyed;
        
        public override void Bind()
        {
            if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
            gl.BindBuffer(Target, Number);
            gl.BoundBuffers[Target] = this;
        }

        public override void Unbind()
        {
            if (Graphics.Current is not OpenGLImpl gl) throw new InvalidOperationException("Need OpenGL >= 3.2");
            gl.UnbindBuffer(Target);
            gl.BoundBuffers[Target] = null;
        }

        public Buffer(BufferTarget target, uint number)
        {
            Target = target;
            Number = number;
        }

        public Buffer(BufferTarget target, uint number, IEnumerable<float> data) : this(target, number) => Upload(data);
        public Buffer(BufferTarget target, uint number, IEnumerable<uint> data) : this(target, number) => Upload(data);
        public Buffer(BufferTarget target, uint number, IEnumerable<int> data) : this(target, number) => Upload(data);
        public Buffer(BufferTarget target, uint number, IEnumerable<double> data) : this(target, number) => Upload(data);
        
        public Buffer(BufferTarget target) : this(target, GL.CreateBuffer()) { }
        public Buffer(BufferTarget target, IEnumerable<float> data) : this(target, GL.CreateBuffer(), data) { }
        public Buffer(BufferTarget target, IEnumerable<uint> data) : this(target, GL.CreateBuffer(), data) { }
        public Buffer(BufferTarget target, IEnumerable<int> data) : this(target, GL.CreateBuffer(), data) { }
        public Buffer(BufferTarget target, IEnumerable<double> data) : this(target, GL.CreateBuffer(), data) { }

        public bool Equals(Buffer other)
        {
            return SetupProgram == other.SetupProgram && Target == other.Target && Destroyed == other.Destroyed &&
                   Number == other.Number;
        }

        public override bool Equals(object? obj)
        {
            return obj is Buffer other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SetupProgram, (int) Target, Destroyed, Number);
        }

        public static bool operator ==(Buffer left, Buffer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Buffer left, Buffer right)
        {
            return !left.Equals(right);
        }
        
        public override void Dispose()
        {
            GL.DeleteBuffers(1, new []{Number});
        }

        public override void Upload(IEnumerable<byte> bytes)
        {
            var target = Target switch
            {
                BufferTarget.VertexArray => GL.BufferTarget.ArrayBuffer,
                BufferTarget.ElementArray => GL.BufferTarget.ElementArrayBuffer,
                _ => throw new ArgumentOutOfRangeException()
            };
            GL.GetInt(Target switch
            {
                BufferTarget.VertexArray => GLC.GL_ARRAY_BUFFER_BINDING,
                BufferTarget.ElementArray => GLC.GL_ELEMENT_ARRAY_BUFFER_BINDING,
                _ => throw new ArgumentOutOfRangeException()
            });
            GL.BindBuffer(target, Number);
            var b = bytes.ToArray();
            GL.BufferData(target, b.Length, b, GLC.GL_STATIC_DRAW);
        }
    }
}