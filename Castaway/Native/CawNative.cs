#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.UnmanagedType;

namespace Castaway.Native
{
    [SuppressMessage("ReSharper", "CA1401")]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantUnsafeContext")]
    public static class CawNative
    {
        private const string Lib = "./.CastawayNativeCode";

        public enum error : uint
        {
            ok = 0,
            gl_error = 1,
            shader_compile_failed = 2,
            out_of_attr_slots = 3,
            null_invalid = 4,
            invalid_storage_location = 5,
            action_failed = 6
        }

        public enum vertex_attr_type : byte
        {
            position2 = 0,
            position3 = 1,
            position4 = 2,
            color3 = 3,
            color4 = 4,
            normal3 = 5,
            tex_coords2 = 6,
            tex_coords3 = 7
        }

        public enum buffer_storage_type : byte
        {
            vert_triangles = 0
        }

        public unsafe struct window
        {
            public void* _glfw;
            public uint _vao;
        }
        
        public unsafe struct vertex_attr
        {
            public vertex_attr_type type;
            public byte* name;
        }
        
        public unsafe struct window_conf
        {
            public uint width;
            public uint height;
            private byte* _title;
            public bool fullscreen;
            public uint* glv;
            public bool forwardCompat;

            public string title
            {
                get => Marshal.PtrToStringAnsi(new IntPtr(_title))!;
                set => _title = (byte*) Marshal.StringToHGlobalAnsi(value).ToPointer();
            }
        }

        public struct vertex
        {
            public float x, y, z;
            public float u, v, t;
            public float nx, ny, nz;
            public float r, g, b, a;

            public vertex(float x, float y, float z, float u, float v, float t, float nx, float ny, float nz, float r, float g, float b, float a)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.u = u;
                this.v = v;
                this.t = t;
                this.nx = nx;
                this.ny = ny;
                this.nz = nz;
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }
        }

        public unsafe struct shader
        {
            public uint program, vertex, fragment;
            public int attrCount;
            public vertex_attr* attrs;
        }

        public struct buffer
        {
            public uint gl;
            public buffer_storage_type storageType;
        }
        
        public struct vertex_attr_spec
        {
            public vertex_attr_type type;
            public uint size;
        }

        public delegate void error_handler([MarshalAs(U4)] error e, uint gl);

        internal static void Init()
        {
            var asm = typeof(CawNative).Assembly;
            var rsc = asm.GetManifestResourceStream($"Castaway._native.{Environment.OSVersion.Platform}");
            if (rsc == null) 
                throw new ApplicationException($"Castaway Native does not support {Environment.OSVersion.Platform}");
            var ary = new byte[rsc.Length];
            rsc.Read(ary);
            var t = File.WriteAllBytesAsync(Lib, ary);
            AppDomain.CurrentDomain.ProcessExit += Destroy;
            t.Wait();
            if (t.IsCompletedSuccessfully) return;
            AppDomain.CurrentDomain.ProcessExit -= Destroy;
            throw new ApplicationException($"Failed to write Castaway Native temporary file for platform {Environment.OSVersion.Platform}");
        }

        private static void Destroy(object? sender, EventArgs e)
        {
            File.Delete(Lib);
        }

        [DllImport(Lib)] public static extern unsafe void cawInit();
        [DllImport(Lib)] public static extern unsafe void cawSetError([MarshalAs(U4)] error e);
        [DllImport(Lib)] public static extern unsafe void cawSetErrorGL(uint gl);
        [DllImport(Lib)] [return: MarshalAs(U4)] public static extern unsafe error cawGetError();
        [DllImport(Lib)] public static extern unsafe void cawSetErrorHandler(error_handler h);
        [DllImport(Lib)] public static extern unsafe void cawStartFrame();
        [DllImport(Lib)] public static extern unsafe void cawEndFrame(window* w);
        [DllImport(Lib)] public static extern unsafe void cawSetTrace(bool v);
        [DllImport(Lib)] public static extern unsafe void cawWriteTrace([MarshalAs(LPStr)] string s);
        [DllImport(Lib)] public static extern unsafe void cawVersion(int* m, int* d, int* y, int* b);
        [DllImport(Lib)] public static extern unsafe byte* cawVersionStr();

        [DllImport(Lib)] public static extern unsafe window_conf* cawNewWindowConf();
        [DllImport(Lib)] public static extern unsafe void cawDestroyWindowConf(window_conf* c);
        [DllImport(Lib)] public static extern unsafe window cawOpenWindow(window_conf* c);
        [DllImport(Lib)] public static extern unsafe void cawCloseWindow(window* w);
        [DllImport(Lib)] public static extern unsafe bool cawWindowShouldClose(window* w);
        [DllImport(Lib)] public static extern unsafe uint* cawGLVersionAny();
        [DllImport(Lib)] public static extern unsafe uint* cawGLVersion2(uint major, uint minor);
        [DllImport(Lib)] public static extern unsafe void cawFinishRender(window* w);
        [DllImport(Lib)] public static extern unsafe byte* cawErrorString(uint e);

        [DllImport(Lib)] public static extern unsafe void cawClearScreen();
        [DllImport(Lib)] public static extern unsafe void cawSetClearColor(float r, float g, float b);
        
        [DllImport(Lib)] public static extern unsafe shader* cawCreateShader();
        [DllImport(Lib)] public static extern unsafe void cawDestroyShader(shader* shdr);
        [DllImport(Lib)] public static extern unsafe void cawSetVertexShader(shader* shdr, [MarshalAs(LPStr)] string src, byte* log, int *logSize);
        [DllImport(Lib)] public static extern unsafe void cawSetFragmentShader(shader* shdr, [MarshalAs(LPStr)] string src, byte* log, int *logSize);
        [DllImport(Lib)] public static extern unsafe void cawLinkShader(shader* shdr, byte* log, int *logSize);
        [DllImport(Lib)] public static extern unsafe void cawAddVertexAttr(shader *shdr, [MarshalAs(U1)] vertex_attr_type attr, byte* name);
        [DllImport(Lib)] public static extern unsafe void cawSetFragmentOutput(shader *shdr, int buf, byte* name);
        [DllImport(Lib)] public static extern unsafe void cawApplyVertexAttrs(shader *shdr);
        [DllImport(Lib)] public static extern unsafe vertex_attr_spec* cawGetAttrs(shader *shdr);
        [DllImport(Lib)] public static extern unsafe void cawUseShader(shader *shdr);
        [DllImport(Lib)] public static extern unsafe int cawGetUniform(shader *shdr, [MarshalAs(LPStr)] string name);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF1(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF2(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF3(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF4(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF2x2(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF3x3(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF4x4(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF2x3(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF2x4(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF3x2(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF3x4(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF4x2(int u, float* f, int count);
        [DllImport(Lib)] public static extern unsafe void cawSetUniformF4x3(int u, float* f, int count);
        
        [DllImport(Lib)] public static extern unsafe buffer cawNewBuffer([MarshalAs(U1)] buffer_storage_type storageType);
        [DllImport(Lib)] public static extern unsafe void cawDeleteBuffer(buffer buf);
        [DllImport(Lib)] public static extern unsafe void cawUpdateBufferData(buffer buf, ulong size, void* data);
        [DllImport(Lib)] public static extern unsafe void cawDraw(buffer v, uint count);
        [DllImport(Lib)] public static extern unsafe void cawDrawElements(buffer v, uint *e, uint count);
    }
}