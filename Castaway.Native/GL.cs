using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Castaway.Math;

using static System.Runtime.InteropServices.UnmanagedType;

namespace Castaway.Native
{
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
        
        #endregion
        
        #region Functions
        
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

        public static void Clear(uint a)
        {
            Fn<uint1>("glClear")(a);
            CheckError();
        }

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

        public static void DrawArrays(uint m, int f, uint c)
        {
            Fn<drawArrays>("glDrawArrays")(m, f, c);
            CheckError();
        }

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

        public static bool IsProgram(uint o)
        {
            var a = Fn<isChecker>("glIsProgram")(o);
            CheckError();
            return a;
        }

        // GetError should NOT check for errors after it executes, because
        // that would be a recursive loop.
        public static uint GetError() => Fn<create0>("glGetError")();
        
        #region Helpers

        public static void CheckError()
        {
            var e = GetError();
            if (e != 0)
            {
                /*throw new ApplicationException*/ Console.WriteLine($"OpenGL Error {e} ({Marshal.PtrToStringAnsi(gluErrorString(e))})");
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