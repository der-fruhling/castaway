using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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

        private static readonly Dictionary<string, object> Functions = new Dictionary<string, object>();

        private static T Fn<T>(string name)
        {
            if (!Functions.ContainsKey(name))
            {
                // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                Functions[name] = Marshal.GetDelegateForFunctionPointer<T>(Environment.OSVersion.Platform switch
                {
                    PlatformID.Unix => glXGetProcAddress(name),
                    _ => throw new ApplicationException("Castaway does not yet support your platform.")
                });
            }
            return (T) Functions[name];
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
        private delegate void uniformfv(int l, uint c, float* a);
        private delegate void uniformiv(int l, uint c, int* a);
        private delegate int getUniformLocation(uint p, [MarshalAs(LPStr)] string name);

        private delegate void texParameterf(uint t, uint n, float p);
        private delegate void texParameteri(uint t, uint n, int p);
        private delegate void texImage2D(uint t, int l, uint i, uint w, uint h, int b, uint f, uint t1, void* d);
        
        #endregion
        
        #region Functions
        
        #region Old OpenGL Drawing
        
        public static void glBegin(uint a)
        {
            Fn<uint1>("glBegin")(a);
            CheckError();
        }

        public static void glEnd()
        {
            Fn<none>("glEnd")();
            CheckError();
        }

        public static void glVertex2f(float x, float y)
        {
            Fn<float2>("glVertex2f")(x, y);
            CheckError();
        }

        public static void glVertex3f(float x, float y, float z)
        {
            Fn<float3>("glVertex3f")(x, y, z);
            CheckError();
        }

        public static void glVertex4f(float x, float y, float z, float w)
        {
            Fn<float4>("glVertex4f")(x, y, z, w);
            CheckError();
        }

        public static void glColor3f(float r, float g, float b)
        {
            Fn<float3>("glColor3f")(r, g, b);
            CheckError();
        }

        public static void glColor4f(float r, float g, float b, float a)
        {
            Fn<float4>("glColor4f")(r, g, b, a);
            CheckError();
        }

        public static void glTexCoord2f(float x, float y)
        {
            Fn<float2>("glTexCoord2f")(x, y);
            CheckError();
        }

        public static void glTexCoord3f(float x, float y, float z)
        {
            Fn<float3>("glTexCoord3f")(x, y, z);
            CheckError();
        }
        #endregion

        public static void glClear(uint a)
        {
            Fn<uint1>("glClear")(a);
            CheckError();
        }

        public static void glClearColor(float r, float g, float b, float a)
        {
            Fn<float4>("glClearColor")(r, g, b, a);
            CheckError();
        }

        public static void glClearColor(Vector4 v) => glClearColor(v.R, v.G, v.B, v.A);

        #region Buffers
        
        public static void glGenBuffers(uint c, uint* p)
        {
            Fn<gen>("glGenBuffers")(c, p);
            CheckError();
        }

        public static void glBindBuffer(uint p, uint b)
        {
            Fn<uint2>("glBindBuffer")(p, b);
            CheckError();
        }

        public static void glBufferData(uint p, uint s, void* d, uint m)
        {
            Fn<bufferData>("glBufferData")(p, s, d, m);
            CheckError();
        }

        public static void glDeleteBuffers(uint c, uint* p)
        {
            Fn<gen>("glDeleteBuffers")(c, p);
            CheckError();
        }
        
        #endregion

        public static void glDrawArrays(uint m, int f, uint c)
        {
            Fn<drawArrays>("glDrawArrays")(m, f, c);
            CheckError();
        }

        #region Shaders
        
        #region Create / Delete
        public static uint glCreateShader(uint m)
        {
            var a = Fn<create1>("glCreateShader")(m);
            CheckError();
            return a;
        }

        public static void glDeleteShader(uint s)
        {
            Fn<uint1>("glDeleteShader")(s);
            CheckError();
        }
        #endregion

        public static void glShaderSource(uint s, uint c, IntPtr* src, uint* len)
        {
            Fn<shaderSource>("glShaderSource")(s, c, src, len);
            CheckError();
        }

        public static void glCompileShader(uint s)
        {
            Fn<uint1>("glCompileShader")(s);
            CheckError();
        }

        public static void glGetShaderiv(uint s, uint t, int* p)
        {
            Fn<getShaderiv>("glGetShaderiv")(s, t, p);
            CheckError();
        }

        public static void glGetShaderInfoLog(uint s, uint l, uint* ol, IntPtr log)
        {
            Fn<getInfoLog>("glGetShaderInfoLog")(s, l, ol, log);
            CheckError();
        }
        
        #endregion

        #region Programs
        
        #region Create / Delete
        public static uint glCreateProgram()
        {
            var a = Fn<create0>("glCreateProgram")();
            CheckError();
            return a;
        }

        public static void glDeleteProgram(uint p)
        {
            Fn<uint1>("glDeleteProgram")(p);
            CheckError();
        }
        #endregion

        #region Attach / Detach
        public static void glAttachShader(uint p, uint s)
        {
            Fn<uint2>("glAttachShader")(p, s);
            CheckError();
        }

        public static void glDetachShader(uint p, uint s)
        {
            Fn<uint2>("glDetachShader")(p, s);
            CheckError();
        }
        #endregion

        #region Link / Use
        public static void glLinkProgram(uint p)
        {
            Fn<uint1>("glLinkProgram")(p);
            CheckError();
        }

        public static void glUseProgram(uint p)
        {
            Fn<uint1>("glUseProgram")(p);
            CheckError();
        }
        #endregion
        
        public static void glGetProgramInfoLog(uint p, uint l, uint* ol, IntPtr log)
        {
            Fn<getInfoLog>("glGetProgramInfoLog")(p, l, ol, log);
            CheckError();
        }
        
        public static void glBindFragDataLocation(uint p, uint o, string name)
        {
            Fn<bindFragDataLocation>("glBindFragDataLocation")(p, o, name);
            CheckError();
        }

        #region Attributes
        public static int glGetAttribLocation(uint p, string name)
        {
            var a = Fn<getAttribLocation>("glGetAttribLocation")(p, name);
            CheckError();
            return a;
        }

        public static void glVertexAttribPointer(int i, int s, uint t, uint n, uint stride, ulong ptr)
        {
            Fn<vertexAttribPointer>("glVertexAttribPointer")(i, s, t, n, stride, ptr);
            CheckError();
        }

        public static void glEnableVertexAttribArray(int a)
        {
            Fn<int1>("glEnableVertexAttribArray")(a);
            CheckError();
        }
        #endregion

        public static bool glIsProgram(uint o)
        {
            var a = Fn<isChecker>("glIsProgram")(o);
            CheckError();
            return a;
        }

        #region Uniforms
        public static void glSetUniform(int l, int len, params float[] data)
        {
            fixed (float* p = data)
            {
                switch (len)
                {
                    case 1: Fn<uniformfv>("glUniform1fv")(l, (uint) (data.Length / 1), p); break;
                    case 2: Fn<uniformfv>("glUniform2fv")(l, (uint) (data.Length / 2), p); break;
                    case 3: Fn<uniformfv>("glUniform3fv")(l, (uint) (data.Length / 3), p); break;
                    case 4: Fn<uniformfv>("glUniform4fv")(l, (uint) (data.Length / 4), p); break;
                }
            }
            CheckError();
        }
        
        public static void glSetUniform(int l, int len, params int[] data)
        {
            fixed (int* p = data)
            {
                switch (len)
                {
                    case 1: Fn<uniformiv>("glUniform1iv")(l, (uint) (data.Length / 1), p); break;
                    case 2: Fn<uniformiv>("glUniform2iv")(l, (uint) (data.Length / 2), p); break;
                    case 3: Fn<uniformiv>("glUniform3iv")(l, (uint) (data.Length / 3), p); break;
                    case 4: Fn<uniformiv>("glUniform4iv")(l, (uint) (data.Length / 4), p); break;
                }
            }
            CheckError();
        }

        public static void glSetUniform(int l, Matrix4 m)
        {
            var a = m.Array;
            fixed (float* p = a) Fn<uniformMf>("glUniformMatrix4fv")(l, 1, true, p);
            CheckError();
        }

        public static int glGetUniformLocation(uint p, string name)
        {
            var a = Fn<getUniformLocation>("glGetUniformLocation")(p, name);
            CheckError();
            return a;
        }
        #endregion
        
        #endregion

        #region Enable / Disable

        public static void glEnable(uint c)
        {
            Fn<uint1>("glEnable")(c);
            CheckError();
        }

        public static void glDisable(uint c)
        {
            Fn<uint1>("glDisable")(c);
            CheckError();
        }

        #endregion

        #region Textures
        
        public static void glGenTextures(uint count, uint* output)
        {
            Fn<gen>("glGenTextures")(count, output);
            CheckError();
        }
        
        public static void glBindTexture(uint mode, uint texture)
        {
            Fn<uint2>("glBindTexture")(mode, texture);
            CheckError();
        }

        public static void glTexParameterf(uint target, uint name, float param)
        {
            Fn<texParameterf>("glTexParameterf")(target, name, param);
            CheckError();
        }

        public static void glTexParameteri(uint target, uint name, int param)
        {
            Fn<texParameteri>("glTexParameteri")(target, name, param);
            CheckError();
        }

        public static void glTexImage2D(uint target, int level, uint internalFormat, uint width, uint height, int border,
            uint format, uint type, void* data)
        {
            Fn<texImage2D>("glTexImage2D")(
                target, level, internalFormat, width, height, border,
                format, type, data);
            CheckError();
        }

        #endregion

        public static uint glGetError() => Fn<create0>("glGetError")();
        
        #region Helpers

        private static uint _error;

        public static void CheckError()
        {
            _error = glGetError();
            if (_error != 0)
            {
                throw new ApplicationException($"OpenGL Error {_error} ({Marshal.PtrToStringAnsi(gluErrorString(_error))})");
            }
        }
        
        public static void glVertex(Vector2 v) => glVertex2f(v.X, v.Y);
        public static void glVertex(Vector3 v) => glVertex3f(v.X, v.Y, v.Z);
        public static void glVertex(Vector4 v) => glVertex4f(v.X, v.Y, v.Z, v.W);
        public static void glColor(Vector3 v) => glColor3f(v.R, v.G, v.B);
        public static void glColor(Vector4 v) => glColor4f(v.R, v.G, v.B, v.A);
        public static void glTexCoord(Vector2 v) => glTexCoord2f(v.X, v.Y);
        public static void glTexCoord(Vector3 v) => glTexCoord3f(v.X, v.Y, v.Z);
        public static void glClear(params uint[] a) => glClear(a.Aggregate((o, i) => o | i));

        #endregion

        #endregion
    }
}