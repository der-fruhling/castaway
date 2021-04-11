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
        
        private delegate void bufferData(uint p, uint s, void* d, uint m);
        private delegate void shaderSource(uint s, uint c, char** src, uint* len);
        private delegate void getShaderiv(uint s, uint t, int* p);
        private delegate void bindFragDataLocation(uint p, uint o, [MarshalAs(LPStr)] string name);
        private delegate int getAttribLocation(uint p, [MarshalAs(LPStr)] string name);
        private delegate void vertexAttribPointer(int i, int s, uint t, uint n, uint stride, [MarshalAs(SysUInt)] uint ptr);
        private delegate void getShaderInfoLog(uint s, uint l, uint* ol, char* log);
        private delegate void drawArrays(uint m, int f, uint c);
        
        #endregion
        
        #region Functions
        
        public static void Begin(uint a) => Fn<uint1>("glBegin")(a);
        public static void End() => Fn<none>("glEnd")();
        public static void Vertex(float x, float y) => Fn<float2>("glVertex2f")(x, y);
        public static void Vertex(float x, float y, float z) => Fn<float3>("glVertex3f")(x, y, z);
        public static void Vertex(float x, float y, float z, float w) => Fn<float4>("glVertex4f")(x, y, z, w);
        public static void Color(float r, float g, float b) => Fn<float3>("glColor3f")(r, g, b);
        public static void Color(float r, float g, float b, float a) => Fn<float4>("glColor4f")(r, g, b, a);
        public static void TexCoord(float x, float y) => Fn<float2>("glTexCoord2f")(x, y);
        public static void TexCoord(float x, float y, float z) => Fn<float3>("glTexCoord3f")(x, y, z);
        
        public static void Clear(uint a) => Fn<uint1>("glClear")(a);
        
        public static void GenBuffers(uint c, uint* p) => Fn<gen>("glGenBuffers")(c, p);
        public static void BindBuffer(uint p, uint b) => Fn<uint2>("glBindBuffer")(p, b);
        public static void BufferData(uint p, uint s, void* d, uint m) => Fn<bufferData>("glBufferData")(p, s, d, m);
        public static void DrawArrays(uint m, int f, uint c) => Fn<drawArrays>("glDrawArrays")(m, f, c);
        
        public static uint CreateShader(uint m) => Fn<create1>("glCreateShader")(m);
        public static void DeleteShader(uint s) => Fn<uint1>("glDeleteShader")(s);
        public static void ShaderSource(uint s, uint c, char** src, uint* len) => Fn<shaderSource>("glShaderSource")(s, c, src, len);
        public static void CompileShader(uint s) => Fn<uint1>("glCompileShader")(s);
        public static void GetShaderValue(uint s, uint t, int* p) => Fn<getShaderiv>("glGetShaderiv")(s, t, p);
        public static void GetShaderInfo(uint s, uint l, uint* ol, char* log) => Fn<getShaderInfoLog>("glGetShaderInfoLog")(s, l, ol, log);

        public static uint CreateProgram() => Fn<create0>("glCreateProgram")();
        public static void DeleteProgram(uint p) => Fn<uint1>("glDeleteProgram")(p);
        public static void AttachToProgram(uint p, uint s) => Fn<uint2>("glAttachShader")(p, s);
        public static void DetachFromProgram(uint p, uint s) => Fn<uint2>("glDetachShader")(p, s);
        public static void LinkProgram(uint p) => Fn<uint1>("glLinkProgram")(p);
        public static void UseProgram(uint p) => Fn<uint1>("glUseProgram")(p);
        public static void BindFragLocation(uint p, uint o, string name) => Fn<bindFragDataLocation>("glBindFragDataLocation")(p, o, name);
        public static int GetAttributeLocation(uint p, string name) => Fn<getAttribLocation>("glGetAttribLocation")(p, name);
        public static void SetAttribPointer(int i, int s, uint t, uint n, uint stride, uint ptr) => Fn<vertexAttribPointer>("glVertexAttribPointer")(i, s, t, n, stride, ptr);
        public static void EnableAttribute(int a) => Fn<int1>("glEnableVertexAttribArray")(a);
        
        #region Helpers
        
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