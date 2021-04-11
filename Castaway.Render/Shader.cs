using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Castaway.Native;
using static Castaway.Render.VertexAttribInfo.AttribValue;

namespace Castaway.Render
{
    public struct VertexAttribInfo
    {
        public enum AttribValue
        {
            Position,
            Color,
            Normal,
            Texture,
        
            Colour = Color
        }

        public AttribValue Value;
        public string Name;
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ShaderHandle
    {
        internal readonly uint Index;
        internal readonly uint GLProgram;
        internal readonly uint GLFrag;
        internal readonly uint GLVert;
        internal readonly VertexAttribInfo[] Attributes;

        internal ShaderHandle(uint index, uint glProgram, uint glFrag, uint glVert, VertexAttribInfo[] attributes)
        {
            Index = index;
            GLProgram = glProgram;
            GLFrag = glFrag;
            GLVert = glVert;
            Attributes = attributes;
        }

        public bool Active => ShaderManager.ActiveHandle == this;

        public void Use() => ShaderManager.Use(this);
        public void Destroy() => ShaderManager.Destroy(this);
        public void BindFragmentLocation(uint location, string name) =>
            ShaderManager.BindFragmentLocation(this, location, name);

        protected bool Equals(ShaderHandle other)
        {
            return Index == other.Index && 
                   GLProgram == other.GLProgram && 
                   GLFrag == other.GLFrag && 
                   GLVert == other.GLVert &&
                   Attributes == other.Attributes;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ShaderHandle) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, GLProgram, GLFrag, GLVert, Attributes);
        }

        public static bool operator ==(ShaderHandle left, ShaderHandle right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ShaderHandle left, ShaderHandle right)
        {
            return !Equals(left, right);
        }
    }
    
    public static unsafe class ShaderManager
    {
        public static ShaderHandle ActiveHandle { get; internal set; }
        
        private static List<uint> _used = new List<uint>();

        internal static uint CreateProgram(params uint[] shaders)
        {
            var p = GL.CreateProgram();
            foreach (var shader in shaders)
            {
                GL.AttachToProgram(p, shader);
            }
            GL.LinkProgram(p);
            return p;
        }

        internal static void Use(uint program)
        {
            GL.UseProgram(program);
        }

        public static void Use(ShaderHandle handle)
        {
            Use(handle.GLProgram);
            ActiveHandle = handle;
        }

        internal static uint CreateShader(uint type, string source)
        {
            var shader = GL.CreateShader(type);
            
            var ptr = stackalloc char[source.Length];
            for (var i = 0; i < source.Length; i++) ptr[i] = source[i];
            GL.ShaderSource(shader, 1, &ptr, null);
            
            GL.CompileShader(shader);
            
            uint logLen;
            var log = stackalloc char[8192];
            GL.GetShaderInfo(shader, 8192, &logLen, log);
            var t = type switch
            {
                GL.VERTEX_SHADER => "Vertex",
                GL.FRAGMENT_SHADER => "Fragment",
                _ => "Weird"
            };
            if (logLen > 0)
            {
                
                Console.WriteLine($"{t} shader log:");
                Console.WriteLine(Marshal.PtrToStringAnsi(new IntPtr(log), (int) logLen));
            }
            
            int val;
            GL.GetShaderValue(shader, GL.COMPILE_STATUS, &val);
            if (val != 1)
            {
                throw new ApplicationException($"{t} shader failed to compile.");
            }

            return shader;
        }

        public static ShaderHandle CreateShader(string vert, string frag, params VertexAttribInfo[] attributes)
        {
            var v = CreateShader(GL.VERTEX_SHADER, vert);
            var f = CreateShader(GL.FRAGMENT_SHADER, frag);
            var p = CreateProgram(v, f);
            
            var free = uint.MaxValue;
            for (uint i = 0; i < 256; i++)
            {
                if (_used.Contains(i)) continue;
                free = i;
                break;
            }
            if (free >= 256) throw new ApplicationException("Ran out of shader slots.");
            _used.Add(free);
            
            return new ShaderHandle(free, p, f, v, attributes);
        }

        internal static void DeleteShaders(params uint[] shaders)
        {
            foreach(var s in shaders) GL.DeleteShader(s);
        }

        internal static void DeletePrograms(params uint[] programs)
        {
            foreach(var p in programs) GL.DeleteProgram(p);
        }

        internal static void DetachShaders(uint program, params uint[] shaders)
        {
            foreach (var s in shaders) GL.DetachFromProgram(program, s);
        }

        public static void Destroy(ShaderHandle handle)
        {
            DetachShaders(handle.GLProgram, handle.GLVert, handle.GLFrag);
            DeletePrograms(handle.GLProgram);
            DeleteShaders(handle.GLVert, handle.GLFrag);
        }

        public static void SetupAttributes(ShaderHandle handle)
        {
            var sizes = new int[handle.Attributes.Length];
            for (var i = 0; i < handle.Attributes.Length; i++)
            {
                sizes[i] = handle.Attributes[i].Value switch
                {
                    Position => 3,
                    Color => 4,
                    Normal => 3,
                    Texture => 3,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            var all = sizes.Aggregate((a, b) => a + b);
            for (var i = 0; i < handle.Attributes.Length; i++)
            {
                var attr = GL.GetAttributeLocation(handle.GLProgram, handle.Attributes[i].Name);
                GL.SetAttribPointer(attr, sizes[i], GL.FLOAT, 0, 
                    (uint) (all * sizeof(float)), 
                    (uint) (sizes[..i].Aggregate((a, b) => a + b) * sizeof(float)));
                GL.EnableAttribute(attr);
            }
        }

        public static void BindFragmentLocation(ShaderHandle handle, uint location, string name)
            => GL.BindFragLocation(handle.GLProgram, location, name);
    }
}
