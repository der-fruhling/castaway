using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Castaway.Math;
using Castaway.Native;
using static Castaway.Render.VertexAttribInfo.AttribValue;

namespace Castaway.Render
{
    /// <summary>
    /// Structure containing information about attributes passed down to vertex
    /// shaders.
    /// </summary>
    /// <seealso cref="AttribValue"/>
    /// <seealso cref="ShaderHandle"/>
    public struct VertexAttribInfo
    {
        /// <summary>
        /// Determines what type of value will be passed to a specific
        /// attribute.
        /// </summary>
        public enum AttribValue
        {
            /// <summary>
            /// Type: <see cref="Vector3"/>
            /// </summary>
            Position,
            
            /// <summary>
            /// Type: <see cref="Vector4"/>
            /// </summary>
            Color,
            
            /// <summary>
            /// Type: <see cref="Vector3"/>
            /// </summary>
            Normal,
            
            /// <summary>
            /// Type: <see cref="Vector3"/>
            /// </summary>
            Texture,
        
            /// <inheritdoc cref="Color"/>
            /// <remarks>Alias to <see cref="Color"/>.</remarks>
            Colour = Color
        }

        public AttribValue Value;
        public string Name;

        public VertexAttribInfo(AttribValue value, string name)
        {
            Value = value;
            Name = name;
        }
    }
    
    /// <summary>
    /// Handle storing OpenGL's values for a specific shader.
    /// </summary>
    /// <seealso cref="ShaderManager"/>
    /// <seealso cref="ShaderManager.CreateShader(string,string,VertexAttribInfo[])"/>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ShaderHandle
    {
        internal readonly uint Index;
        internal readonly uint GLProgram;
        internal readonly uint GLFrag;
        internal readonly uint GLVert;
        internal readonly VertexAttribInfo[] Attributes;
        public int TModel = -1, TView = -1, TProjection = -1;

        internal ShaderHandle(uint index, uint glProgram, uint glFrag, uint glVert, VertexAttribInfo[] attributes)
        {
            Index = index;
            GLProgram = glProgram;
            GLFrag = glFrag;
            GLVert = glVert;
            Attributes = attributes;
        }

        /// <seealso cref="ShaderManager.ActiveHandle"/>
        public bool Active => ShaderManager.ActiveHandle == this;

        /// <seealso cref="ShaderManager.Use"/>
        public void Use() => ShaderManager.Use(this);
        
        /// <seealso cref="ShaderManager.Destroy"/>
        public void Destroy() => ShaderManager.Destroy(this);
        
        /// <seealso cref="ShaderManager.BindFragmentLocation"/>
        public void BindFragmentLocation(uint location, string name) =>
            ShaderManager.BindFragmentLocation(this, location, name);
        
        /// <seealso cref="ShaderManager.FinishLinkingProgram"/>
        public void Finish() => ShaderManager.FinishLinkingProgram(GLProgram);

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

        public void SetTModel(Matrix4 m)
        {
            if(TModel == -1) return;
            GL.SetUniform(TModel, m);
        }

        public void SetTView(Matrix4 m)
        {
            if(TView == -1) return;
            GL.SetUniform(TView, m);
        }

        public void SetTProjection(Matrix4 m)
        {
            if(TProjection == -1) return;
            GL.SetUniform(TProjection, m);
        }
    }
    
    /// <summary>
    /// Manages shaders. <c>public</c> methods should take
    /// <see cref="ShaderHandle"/>, <c>internal</c> methods may take
    /// <see cref="uint"/>.
    /// </summary>
    public static unsafe class ShaderManager
    {
        /// <summary>
        /// The currently active <see cref="ShaderHandle"/> instance. Can be
        /// used to determine which shader is active. <b>Not initialized
        /// on startup.</b>
        /// </summary>
        public static ShaderHandle ActiveHandle { get; internal set; }
        
        private static readonly List<uint> Used = new List<uint>();

        /// <summary>
        /// Creates a new shader program from an array of shader objects. This
        /// method does <b>not</b> link the program afterwords, to allow the
        /// <see cref="BindFragmentLocation"/> method to be used.
        /// </summary>
        /// <param name="shaders">Array of valid OpenGL shader objects.</param>
        /// <returns>New program, with all <paramref name="shaders"/> attached.
        /// </returns>
        /// <seealso cref="FinishLinkingProgram"/>
        /// <seealso cref="CreateShader(uint,string)"/>
        /// <seealso cref="GL.CreateProgram"/>
        internal static uint CreateProgram(params uint[] shaders)
        {
            var p = GL.CreateProgram();
            foreach (var shader in shaders)
            {
                GL.AttachToProgram(p, shader);
            }
            return p;
        }

        /// <summary>
        /// Completes a program. After calling this method, the
        /// <see cref="BindFragmentLocation"/> method should not be called.
        ///
        /// Shaders cannot be used before finished.
        /// </summary>
        /// <param name="program"></param>
        /// <seealso cref="BindFragmentLocation"/>
        /// <seealso cref="CreateProgram"/>
        /// <seealso cref="ShaderHandle.Finish"/>
        /// <seealso cref="GL.LinkProgram"/>
        internal static void FinishLinkingProgram(uint program)
        {
            GL.LinkProgram(program);
            var ptr = Marshal.AllocHGlobal(8192);
            GL.GetProgramInfo(program, 8192, null, ptr);
            Console.Write(Marshal.PtrToStringAnsi(ptr));
        }

        /// <summary>
        /// Enables an OpenGL shader program. This method should not be used
        /// directly, use <see cref="Use(ShaderHandle)"/> instead.
        /// </summary>
        /// <param name="program">Program to use.</param>
        /// <seealso cref="Use(ShaderHandle)"/>
        /// <seealso cref="GL.UseProgram"/>
        internal static void Use(uint program)
        {
            GL.UseProgram(program);
        }

        /// <summary>
        /// Enables a shader program. <b>The program must be
        /// <see cref="ShaderHandle.Finish">linked</see> before it can be
        /// used.</b>
        /// </summary>
        /// <param name="handle">Handle to enable.</param>
        /// <seealso cref="Use(uint)"/>
        /// <seealso cref="ActiveHandle"/>
        /// <seealso cref="GL.UseProgram"/>
        /// <seealso cref="ShaderHandle.Use"/>
        public static void Use(ShaderHandle handle)
        {
            Use(handle.GLProgram);
            ActiveHandle = handle;
        }

        /// <summary>
        /// Creates a new OpenGL shader that can be linked into a program.
        /// </summary>
        /// <param name="type">Type of the shader to create. Should be either
        /// <see cref="GL.VERTEX_SHADER"/> or <see cref="GL.FRAGMENT_SHADER"/>.
        /// </param>
        /// <param name="source">GLSL source of the shader.</param>
        /// <returns>New OpenGL shader object.</returns>
        /// <exception cref="ApplicationException">Thrown if an error occurs
        /// during compilation.</exception>
        internal static uint CreateShader(uint type, string source)
        {
            var shader = GL.CreateShader(type);

            var len = (uint) source.Length;
            var ptr = Marshal.StringToHGlobalAnsi(source);
            GL.ShaderSource(shader, 1, &ptr, &len);
            
            GL.CompileShader(shader);
            
            var log = Marshal.AllocHGlobal(8192);
            GL.GetShaderInfo(shader, 8192, null, log);
            var logStr = Marshal.PtrToStringAnsi(log);
            var t = type switch
            {
                GL.VERTEX_SHADER => "Vertex",
                GL.FRAGMENT_SHADER => "Fragment",
                _ => "Weird"
            };
            if (!string.IsNullOrEmpty(logStr))
            {
                Console.WriteLine($"{t} shader log:");
                Console.WriteLine(logStr);
            }
            
            int val;
            GL.GetShaderValue(shader, GL.COMPILE_STATUS, &val);
            if (val != 1)
            {
                throw new ApplicationException($"{t} shader failed to compile.");
            }

            return shader;
        }

        /// <summary>
        /// Creates a new <see cref="ShaderHandle"/> from vertex and fragment
        /// sources.
        /// </summary>
        /// <param name="vert">Source code for the vertex shader.</param>
        /// <param name="frag">Source code for the fragment shader.</param>
        /// <param name="attributes">Vertex attributes to use in
        /// <see cref="SetupAttributes"/></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Thrown if more than 256
        /// shaders are used. No sane person should encounter this limit.
        /// </exception>
        public static ShaderHandle CreateShader(string vert, string frag, VertexAttribInfo[] attributes)
        {
            var v = CreateShader(GL.VERTEX_SHADER, vert);
            var f = CreateShader(GL.FRAGMENT_SHADER, frag);
            var p = CreateProgram(v, f);
            
            var free = uint.MaxValue;
            for (uint i = 0; i < 256; i++)
            {
                if (Used.Contains(i)) continue;
                free = i;
                break;
            }
            if (free >= 256) throw new ApplicationException("Ran out of shader slots.");
            Used.Add(free);
            
            return new ShaderHandle(free, p, f, v, attributes);
        }

        /// <summary>
        /// Deletes an array of shaders. Shaders should be detached from their
        /// programs before being deleted.
        /// </summary>
        /// <param name="shaders">Shaders to delete.</param>
        internal static void DeleteShaders(params uint[] shaders)
        {
            foreach(var s in shaders) GL.DeleteShader(s);
        }

        /// <summary>
        /// Deletes a array of programs.
        /// </summary>
        /// <param name="programs">Programs to delete.</param>
        internal static void DeletePrograms(params uint[] programs)
        {
            foreach(var p in programs) GL.DeleteProgram(p);
        }

        /// <summary>
        /// Detaches an array of shaders from the specified program. OpenGL may
        /// throw an error if it finds that a shader is not already attached.
        /// </summary>
        /// <param name="program">Program to detach from.</param>
        /// <param name="shaders">Shaders to detach.</param>
        internal static void DetachShaders(uint program, params uint[] shaders)
        {
            foreach (var s in shaders) GL.DetachFromProgram(program, s);
        }

        /// <summary>
        /// Destroys a shader program, freeing all resources used.
        /// </summary>
        /// <param name="handle">Handle to detach.</param>
        public static void Destroy(ShaderHandle handle)
        {
            DetachShaders(handle.GLProgram, handle.GLVert, handle.GLFrag);
            DeletePrograms(handle.GLProgram);
            DeleteShaders(handle.GLVert, handle.GLFrag);
            Used.Remove(handle.Index);
        }

        /// <summary>
        /// Calls various OpenGL functions to set up vertex attributes. It uses
        /// the attributes stored in <see cref="ShaderHandle.Attributes"/>.
        /// </summary>
        /// <param name="handle">Handle to get attributes from.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid
        /// attribute value is encountered.</exception>
        public static void SetupAttributes(ShaderHandle handle)
        {
            handle.Use();
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
                    (ulong) (i == 0 ? 0 : sizes[..i].Aggregate((a, b) => a + b) * sizeof(float)));
                GL.EnableAttribute(attr);
            }
        }

        /// <summary>
        /// Sets a uniform value.
        /// </summary>
        /// <param name="handle">Handle of the shader with the uniform.</param>
        /// <param name="name">Name of the uniform variable.</param>
        /// <param name="floats">1..4 floats.</param>
        public static void SetUniform(ShaderHandle handle, string name, params float[] floats)
        {
            var loc = GL.GetUniformLocation(handle.GLProgram, name);
            GL.SetUniform(loc, floats);
        }
        
        /// <summary>
        /// Sets a uniform value.
        /// </summary>
        /// <param name="handle">Handle of the shader with the uniform.</param>
        /// <param name="name">Name of the uniform variable.</param>
        /// <param name="ints">1..4 ints.</param>
        public static void SetUniform(ShaderHandle handle, string name, params int[] ints)
        {
            var loc = GL.GetUniformLocation(handle.GLProgram, name);
            GL.SetUniform(loc, ints);
        }

        /// <summary>
        /// Sets a uniform value.
        /// </summary>
        /// <param name="handle">Handle of the shader with the uniform.</param>
        /// <param name="name">Name of the uniform variable.</param>
        /// <param name="matrix">Matrix value.</param>
        public static void SetUniform(ShaderHandle handle, string name, Matrix4 matrix)
        {
            var loc = GL.GetUniformLocation(handle.GLProgram, name);
            GL.SetUniform(loc, matrix);
        }

        /// <summary>
        /// Binds an output in the fragment shader to a location.
        /// </summary>
        /// <param name="handle">Handle to bind for.</param>
        /// <param name="location">Location to bind to.</param>
        /// <param name="name">Name of the output.</param>
        public static void BindFragmentLocation(ShaderHandle handle, uint location, string name)
            => GL.BindFragLocation(handle.GLProgram, location, name);
    }
}
