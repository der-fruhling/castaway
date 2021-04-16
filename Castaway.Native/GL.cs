using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Castaway.Math;

using static System.Runtime.InteropServices.UnmanagedType;

namespace Castaway.Native
{
    /// <summary>
    /// Various native functions relating to OpenGL.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static unsafe class GL
    {
        #region Internal
        
        [DllImport("libGLX.so")]
        private static extern IntPtr glXGetProcAddress([MarshalAs(LPStr)] string name);
        
        [DllImport("libGLU.so")]
        private static extern IntPtr gluErrorString(uint err);

        private static T Fn<T>(string name)
        {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return Marshal.GetDelegateForFunctionPointer<T>(Environment.OSVersion.Platform switch
            {
                PlatformID.Unix => glXGetProcAddress(name),
                _ => throw new ApplicationException("Castaway does not yet support your platform.")
            });
        }
        
        #endregion
        
        #region Variables
        
        public const uint POINTS = 0;
        public const uint LINES = 1;
        public const uint LINE_LOOP = 2;
        public const uint LINE_STRIP = 3;
        public const uint TRIANGLES = 4;
        public const uint TRIANGLE_STRIP = 5;
        public const uint TRIANGLE_FAN = 6;
        public const uint QUADS = 7;
        public const uint QUAD_STRIP = 8;
        public const uint POLYGON = 9;

        public const uint DEPTH_BUFFER_BIT   = 0b000000100000000;
        public const uint ACCUM_BUFFER_BIT   = 0b000001000000000;
        public const uint STENCIL_BUFFER_BIT = 0b000010000000000;
        public const uint COLOR_BUFFER_BIT   = 0b100000000000000;

        public const uint VERTEX_SHADER = 0x8B31;
        public const uint FRAGMENT_SHADER = 0x8B30;

        public const uint ARRAY_BUFFER = 0x8892;
        public const uint ELEMENT_ARRAY_BUFFER = 0x8893;

        public const uint STREAM_DRAW = 0x88E0;
        public const uint STATIC_DRAW = 0x88E4;
        public const uint DYNAMIC_DRAW = 0x88E8;

        public const uint FLOAT = 0x1406;

        public const uint COMPILE_STATUS = 0x8B81;
        
        public const uint DEPTH_TEST = 0x0B71;

        public const uint TEXTURE_2D = 0x0DE1;
        public const uint REPEAT = 0x2901;
        public const uint MIRRORED_REPEAT = 0x8370;
        public const uint CLAMP_TO_EDGE = 0x812F;
        public const uint CLAMP_TO_BORDER = 0x812D;
        public const uint TEXTURE_WRAP_S = 0x2802;
        public const uint TEXTURE_WRAP_T = 0x2803;
        public const uint NEAREST = 0x2600;
        public const uint LINEAR = 0x2601;
        public const uint TEXTURE_MIN_FILTER = 0x2801;
        public const uint TEXTURE_MAG_FILTER = 0x2800;
        public const uint RGB = 0x1907;
        public const uint RGBA = 0x1908;
        public const uint TEXTURE0 = 0x84C0;
        public const uint TEXTURE31 = 0x84DF;
        
        #endregion
        
        #region Delegates
        
        private delegate void none();
        private delegate void uint1(uint a);
        private delegate void uint2(uint a, uint b);
        private delegate void uint3(uint a, uint b, uint c);
        private delegate void uint4(uint a, uint b, uint c, uint d);
        private delegate void int1(int a);
        private delegate void int2(int a, int b);
        private delegate void int3(int a, int b, int c);
        private delegate void int4(int a, int b, int c, int d);
        private delegate void float1(float a);
        private delegate void float2(float a, float b);
        private delegate void float3(float a, float b, float c);
        private delegate void float4(float a, float b, float c, float d);
        private delegate void double1(double a);
        private delegate void double2(double a, double b);
        private delegate void double3(double a, double b, double c);
        private delegate void double4(double a, double b, double c, double d);
        private delegate void gen(uint c, uint* o);
        private delegate uint create1(uint m);
        private delegate uint create0();
        private delegate bool isChecker(uint o);
        
        private delegate void bufferData(uint p, uint s, void* d, uint m);
        private delegate void shaderSource(uint s, uint c, IntPtr* src, uint* len);
        private delegate void getShaderiv(uint s, uint t, int* p);
        private delegate void bindFragDataLocation(uint p, uint o, [MarshalAs(LPStr)] string name);
        private delegate int getAttribLocation(uint p, [MarshalAs(LPStr)] string name);
        private delegate void vertexAttribPointer(int i, int s, uint t, uint n, uint stride, [MarshalAs(U8)] ulong ptr);
        private delegate void getInfoLog(uint s, uint l, uint* ol, IntPtr log);
        private delegate void drawArrays(uint m, int f, uint c);

        private delegate void uniform1f(int l, float a);
        private delegate void uniform2f(int l, float a, float b);
        private delegate void uniform3f(int l, float a, float b, float c);
        private delegate void uniform4f(int l, float a, float b, float c, float d);
        private delegate void uniform1i(int l, int a);
        private delegate void uniform2i(int l, int a, int b);
        private delegate void uniform3i(int l, int a, int b, int c);
        private delegate void uniform4i(int l, int a, int b, int c, int d);
        private delegate void uniformMf(int l, uint c, bool t, float* v);
        private delegate int getUniformLocation(uint p, [MarshalAs(LPStr)] string name);

        private delegate void texParameterf(uint t, uint n, float p);
        private delegate void texParameteri(uint t, uint n, int p);
        private delegate void texImage2D(uint t, int l, uint i, uint w, uint h, int b, uint f, uint t1, void* d);
        
        #endregion
        
        #region Functions
        
        #region Old OpenGL Drawing
        
        public static void Begin(uint a)
        {
            Fn<uint1>("glBegin")(a);
            CheckError();
        }

        public static void End()
        {
            Fn<none>("glEnd")();
            CheckError();
        }

        public static void Vertex(float x, float y)
        {
            Fn<float2>("glVertex2f")(x, y);
            CheckError();
        }

        public static void Vertex(float x, float y, float z)
        {
            Fn<float3>("glVertex3f")(x, y, z);
            CheckError();
        }

        public static void Vertex(float x, float y, float z, float w)
        {
            Fn<float4>("glVertex4f")(x, y, z, w);
            CheckError();
        }

        public static void Color(float r, float g, float b)
        {
            Fn<float3>("glColor3f")(r, g, b);
            CheckError();
        }

        public static void Color(float r, float g, float b, float a)
        {
            Fn<float4>("glColor4f")(r, g, b, a);
            CheckError();
        }

        public static void TexCoord(float x, float y)
        {
            Fn<float2>("glTexCoord2f")(x, y);
            CheckError();
        }

        public static void TexCoord(float x, float y, float z)
        {
            Fn<float3>("glTexCoord3f")(x, y, z);
            CheckError();
        }
        #endregion

        public static void Clear(uint a)
        {
            Fn<uint1>("glClear")(a);
            CheckError();
        }

        public static void ClearColor(float r, float g, float b, float a)
        {
            Fn<float4>("glClearColor")(r, g, b, a);
            CheckError();
        }

        public static void ClearColor(Vector4 v) => ClearColor(v.R, v.G, v.B, v.A);

        #region Buffers
        
        public static void GenBuffers(uint c, uint* p)
        {
            Fn<gen>("glGenBuffers")(c, p);
            CheckError();
        }

        public static void BindBuffer(uint p, uint b)
        {
            Fn<uint2>("glBindBuffer")(p, b);
            CheckError();
        }

        public static void BufferData(uint p, uint s, void* d, uint m)
        {
            Fn<bufferData>("glBufferData")(p, s, d, m);
            CheckError();
        }
        
        #endregion

        public static void DrawArrays(uint m, int f, uint c)
        {
            Fn<drawArrays>("glDrawArrays")(m, f, c);
            CheckError();
        }

        #region Shaders
        
        #region Create / Delete
        public static uint CreateShader(uint m)
        {
            var a = Fn<create1>("glCreateShader")(m);
            CheckError();
            return a;
        }

        public static void DeleteShader(uint s)
        {
            Fn<uint1>("glDeleteShader")(s);
            CheckError();
        }
        #endregion

        public static void ShaderSource(uint s, uint c, IntPtr* src, uint* len)
        {
            Fn<shaderSource>("glShaderSource")(s, c, src, len);
            CheckError();
        }

        public static void CompileShader(uint s)
        {
            Fn<uint1>("glCompileShader")(s);
            CheckError();
        }

        public static void GetShaderValue(uint s, uint t, int* p)
        {
            Fn<getShaderiv>("glGetShaderiv")(s, t, p);
            CheckError();
        }

        public static void GetShaderInfo(uint s, uint l, uint* ol, IntPtr log)
        {
            Fn<getInfoLog>("glGetShaderInfoLog")(s, l, ol, log);
            CheckError();
        }
        
        #endregion

        #region Programs
        
        #region Create / Delete
        public static uint CreateProgram()
        {
            var a = Fn<create0>("glCreateProgram")();
            CheckError();
            return a;
        }

        public static void DeleteProgram(uint p)
        {
            Fn<uint1>("glDeleteProgram")(p);
            CheckError();
        }
        #endregion

        #region Attach / Detach
        public static void AttachToProgram(uint p, uint s)
        {
            Fn<uint2>("glAttachShader")(p, s);
            CheckError();
        }

        public static void DetachFromProgram(uint p, uint s)
        {
            Fn<uint2>("glDetachShader")(p, s);
            CheckError();
        }
        #endregion

        #region Link / Use
        public static void LinkProgram(uint p)
        {
            Fn<uint1>("glLinkProgram")(p);
            CheckError();
        }

        public static void UseProgram(uint p)
        {
            Fn<uint1>("glUseProgram")(p);
            CheckError();
        }
        #endregion
        
        public static void GetProgramInfo(uint p, uint l, uint* ol, IntPtr log)
        {
            Fn<getInfoLog>("glGetProgramInfoLog")(p, l, ol, log);
            CheckError();
        }
        
        public static void BindFragLocation(uint p, uint o, string name)
        {
            Fn<bindFragDataLocation>("glBindFragDataLocation")(p, o, name);
            CheckError();
        }

        #region Attributes
        public static int GetAttributeLocation(uint p, string name)
        {
            var a = Fn<getAttribLocation>("glGetAttribLocation")(p, name);
            CheckError();
            return a;
        }

        public static void SetAttribPointer(int i, int s, uint t, uint n, uint stride, ulong ptr)
        {
            Fn<vertexAttribPointer>("glVertexAttribPointer")(i, s, t, n, stride, ptr);
            CheckError();
        }

        public static void EnableAttribute(int a)
        {
            Fn<int1>("glEnableVertexAttribArray")(a);
            CheckError();
        }
        #endregion

        public static bool IsProgram(uint o)
        {
            var a = Fn<isChecker>("glIsProgram")(o);
            CheckError();
            return a;
        }

        #region Uniforms
        public static void SetUniform(int l, params float[] data)
        {
            if (data.Length > 4) throw new ApplicationException("Too many parameters. (max: 4)");
            switch (data.Length)
            {
                case 1: Fn<uniform1f>("glUniform1f")(l, data[0]); break;
                case 2: Fn<uniform2f>("glUniform2f")(l, data[0], data[1]); break;
                case 3: Fn<uniform3f>("glUniform3f")(l, data[0], data[1], data[2]); break;
                case 4: Fn<uniform4f>("glUniform4f")(l, data[0], data[1], data[2], data[3]); break;
            }
            CheckError();
        }
        
        public static void SetUniform(int l, params int[] data)
        {
            if (data.Length > 4) throw new ApplicationException("Too many parameters. (max: 4)");
            switch (data.Length)
            {
                case 1: Fn<uniform1i>("glUniform1i")(l, data[0]); break;
                case 2: Fn<uniform2i>("glUniform2i")(l, data[0], data[1]); break;
                case 3: Fn<uniform3i>("glUniform3i")(l, data[0], data[1], data[2]); break;
                case 4: Fn<uniform4i>("glUniform4i")(l, data[0], data[1], data[2], data[3]); break;
            }
            CheckError();
        }

        public static void SetUniform(int l, Matrix4 m)
        {
            var a = m.Array;
            fixed (float* p = a) Fn<uniformMf>("glUniformMatrix4fv")(l, 1, true, p);
            CheckError();
        }

        public static int GetUniformLocation(uint p, string name)
        {
            var a = Fn<getUniformLocation>("glGetUniformLocation")(p, name);
            CheckError();
            return a;
        }
        #endregion
        
        #endregion

        #region Enable / Disable

        public static void Enable(uint c)
                {
                    Fn<uint1>("glEnable")(c);
                    CheckError();
                }
        
                public static void Disable(uint c)
                {
                    Fn<uint1>("glDisable")(c);
                    CheckError();
                }

        #endregion

        #region Textures
        
        public static void GenTextures(uint count, uint* output)
        {
            Fn<gen>("glGenTextures")(count, output);
            CheckError();
        }
        
        public static void BindTexture(uint mode, uint texture)
        {
            Fn<uint2>("glBindTexture")(mode, texture);
            CheckError();
        }

        public static void TextureParam(uint target, uint name, float param)
        {
            Fn<texParameterf>("glTexParameterf")(target, name, param);
            CheckError();
        }

        public static void TextureParam(uint target, uint name, int param)
        {
            Fn<texParameteri>("glTexParameteri")(target, name, param);
            CheckError();
        }

        public static void LoadTexture2D(uint target, int level, uint internalFormat, uint width, uint height, int border,
            uint format, uint type, void* data)
        {
            Fn<texImage2D>("glTexImage2D")(
                target, level, internalFormat, width, height, border,
                format, type, data);
            CheckError();
        }

        #endregion

        public static uint GetError() => Fn<create0>("glGetError")();
        
        #region Helpers

        public static void CheckError()
        {
            var e = GetError();
            if (e != 0)
            {
                throw new ApplicationException($"OpenGL Error {e} ({Marshal.PtrToStringAnsi(gluErrorString(e))})");
            }
        }
        
        public static void Vertex(Vector2 v) => Vertex(v.X, v.Y);
        public static void Vertex(Vector3 v) => Vertex(v.X, v.Y, v.Z);
        public static void Vertex(Vector4 v) => Vertex(v.X, v.Y, v.Z, v.W);
        public static void Color(Vector3 v) => Color(v.R, v.G, v.B);
        public static void Color(Vector4 v) => Color(v.R, v.G, v.B, v.A);
        public static void TexCoord(Vector2 v) => TexCoord(v.X, v.Y);
        public static void TexCoord(Vector3 v) => TexCoord(v.X, v.Y, v.Z);
        public static void Clear(params uint[] a) => Clear(a.Aggregate((o, i) => o | i));

        #endregion

        #endregion
    }
}