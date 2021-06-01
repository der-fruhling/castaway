using System;
using System.Runtime.InteropServices;
using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public class GLBuffer : IBuffer, IOpenGLObject
    {
        public uint Number { get; set; }
        public OpenGLType Type => OpenGLType.Buffer;
        public bool IsValid => Validate();
        public GL.BufferTarget Target;

        private bool Revalidate = true;
        private bool LastValidate;
        
        public GLBuffer(GL.BufferTarget target, uint number)
        {
            Target = target;
            Number = number;
        }

        public bool Validate()
        {
            if (!Revalidate) return LastValidate;
            Revalidate = false;
            return LastValidate = GL.IsBuffer(Number);
        }

        public void MarkDirty()
        {
            Revalidate = true;
        }

        public void Upload(Span<byte> data)
        {
            GL.BindBuffer(Target, Number);
            GL.BufferData(Target, data.Length, data, GLC.GL_STATIC_DRAW);
        }

        public void Upload(Span<float> data)
        {
            var mem = Marshal.AllocHGlobal(sizeof(float) * data.Length);
            var ary = new byte[data.Length * sizeof(float)];
            Marshal.Copy(data.ToArray(), 0, mem, data.Length);
            Marshal.Copy(mem, ary, 0, ary.Length);
            Marshal.FreeHGlobal(mem);
            Upload(ary);
        }
    }
}