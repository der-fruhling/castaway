#nullable enable
using System;
using System.Runtime.InteropServices;
using Castaway.Native;
using static Castaway.Native.CawNative;

namespace Castaway.Render
{
    public enum ShaderAttr : byte
    {
        Position2 = vertex_attr_type.position2,
        Position3 = vertex_attr_type.position3,
        Position4 = vertex_attr_type.position4,
        Color3 = vertex_attr_type.color3,
        Color4 = vertex_attr_type.color4,
        TexCoords2 = vertex_attr_type.tex_coords2,
        TexCoords3 = vertex_attr_type.tex_coords3,
    }
    
    public unsafe class Shader : IDisposable
    {
        private byte* _log;
        private int* _logSize;

        internal static Shader? Active;

        private void PrintLog(object? o, EventArgs eventArgs)
        {
            if (*_logSize > 0) Console.WriteLine(Marshal.PtrToStringAnsi(new IntPtr(_log)));
        }
        
        private Shader() {}

        public Shader(string vert, string frag)
        {
            _log = Memory.Alloc<byte>(8192);
            _logSize = Memory.Alloc<int>();
            
            Native = cawCreateShader();
            
            AppDomain.CurrentDomain.ProcessExit += PrintLog;
            
            *_logSize = 8192;
            cawSetVertexShader(Native, vert, _log, _logSize);
            PrintLog(null, EventArgs.Empty);
            
            *_logSize = 8192;
            cawSetFragmentShader(Native, frag, _log, _logSize);
            PrintLog(null, EventArgs.Empty);

            AppDomain.CurrentDomain.ProcessExit -= PrintLog;
        }

        private void ReleaseUnmanagedResources()
        {
            cawDestroyShader(Native);
            Memory.Free(_log, _logSize);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            ReleaseUnmanagedResources();
        }

        public string this[ShaderAttr attr]
        {
            set
            {
                var m = Marshal.StringToHGlobalAnsi(value);
                cawAddVertexAttr(Native, (vertex_attr_type) attr, (byte*) m);
                Marshal.FreeHGlobal(m);
            }
        }

        public string this[int buf]
        {
            set
            {
                var m = Marshal.StringToHGlobalAnsi(value);
                cawSetFragmentOutput(Native, buf, (byte*) m);
                Marshal.FreeHGlobal(m);
            }
        }

        public Uniform this[string name]
        {
            set
            {
                var u = cawGetUniform(Native, name);
                if(Active != this) Use();
                value.Upload(u);
            }
        }

        public void Use()
        {
            cawUseShader(Native);
            Active = this;
        }

        public void Link()
        {
            *_logSize = 8192;
            cawLinkShader(Native, _log, _logSize);
            PrintLog(null, EventArgs.Empty);
        }

        internal shader* Native { get; }

        protected bool Equals(Shader other)
        {
            return Native == other.Native;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Shader) obj);
        }

        public override int GetHashCode()
        {
            return unchecked((int) (long) Native);
        }

        public static bool operator ==(Shader? left, Shader? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Shader? left, Shader? right)
        {
            return !Equals(left, right);
        }
    }
}