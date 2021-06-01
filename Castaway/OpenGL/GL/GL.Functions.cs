using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using GLFW;

namespace Castaway.OpenGL
{
    public static unsafe partial class GL
    {
        private static readonly Dictionary<GLF, IntPtr> fn = new();

        private static IntPtr Load(GLF f) =>
            !fn.ContainsKey(f) || fn[f] == IntPtr.Zero
                ? fn[f] = Glfw.GetProcAddress(Enum.GetName(f)) 
                : fn[f];

        public static void GenBuffers(int count, out uint[] output)
        {
            output = new uint[count];
            if(count == 0) return;
            var f = (delegate*<int, uint*, void>) Load(GLF.glGenBuffers);
            fixed(uint* p = output) f(count, p);
        }

        public static void CreateBuffers(int count, out uint[] output)
        {
            output = new uint[count];
            if(count == 0) return;
            var f = (delegate*<int, uint*, void>) Load(GLF.glCreateBuffers);
            fixed(uint* p = output) f(count, p);
        }

        public static void DeleteBuffers(int count, uint[] buffers)
        {
            if(count == 0) return;
            var f = (delegate*<int, uint*, void>) Load(GLF.glDeleteBuffers);
            fixed (uint* p = buffers) f(count, p);
        }

        public static bool IsBuffer(uint obj)
        {
            if (obj == 0) return false;
            var f = (delegate*<uint, bool>) Load(GLF.glIsBuffer);
            return f(obj);
        }

        public static uint CreateShader(ShaderStage stage)
        {
            var f = (delegate*<uint, uint>) Load(GLF.glCreateShader);
            return f(EnumValue(stage));
        }

        public static void DeleteShader(uint shader)
        {
            var f = (delegate*<uint, void>) Load(GLF.glDeleteShader);
            f(shader);
        }

        public static uint CreateProgram()
        {
            var f = (delegate*<uint>) Load(GLF.glCreateProgram);
            return f();
        }

        public static void DeleteProgram(uint program)
        {
            var f = (delegate*<uint, void>) Load(GLF.glDeleteProgram);
            f(program);
        }

        public static void ShaderSource(uint shader, params string[] sourceCode)
        {
            var arrayArray = (byte**) Marshal.AllocHGlobal(sizeof(byte*) * sourceCode.Length);
            Debug.Assert(arrayArray != null, nameof(arrayArray) + " != null");
            for (var i = 0; i < sourceCode.Length; i++)
                arrayArray[i] = (byte*) Marshal.StringToHGlobalAnsi(sourceCode[i]);
            var lengths = sourceCode.Select(s => s.Length).ToArray();

            var f = (delegate*<uint, int, byte**, int*, void>) Load(GLF.glShaderSource);
            fixed (int* lp = lengths) f(shader, sourceCode.Length, arrayArray, lp);
            
            for(var i = 0; i < sourceCode.Length; i++)
                Marshal.FreeHGlobal((IntPtr) arrayArray[i]);
            Marshal.FreeHGlobal((IntPtr) arrayArray);
        }

        public static int GetShader(uint shader, ShaderQuery query)
        {
            int v;
            var f = (delegate*<uint, uint, int*, void>) Load(GLF.glGetShaderiv);
            f(shader, EnumValue(query), &v);
            return v;
        }

        public static void CompileShader(uint shader)
        {
            var f = (delegate*<uint, void>) Load(GLF.glCompileShader);
            f(shader);
        }

        public static void AttachShader(uint program, uint shader)
        {
            var f = (delegate*<uint, uint, void>) Load(GLF.glAttachShader);
            f(program, shader);
        }

        public static void DetachShader(uint program, uint shader)
        {
            var f = (delegate*<uint, uint, void>) Load(GLF.glDetachShader);
            f(program, shader);
        }

        public static void LinkProgram(uint program)
        {
            var f = (delegate*<uint, void>) Load(GLF.glLinkProgram);
            f(program);
        }

        public static int GetProgram(uint program, ProgramQuery query)
        {
            int v;
            var f = (delegate*<uint, uint, int*, void>) Load(GLF.glGetProgramiv);
            f(program, EnumValue(query), &v);
            return v;
        }

        public static void UseProgram(uint program)
        {
            var f = (delegate*<uint, void>) Load(GLF.glUseProgram);
            f(program);
        }

        public static int GetAttribLocation(uint program, string name)
        {
            var nameB = Marshal.StringToHGlobalAnsi(name);
            var f = (delegate*<uint, IntPtr, int>) Load(GLF.glGetAttribLocation);
            var @return = f(program, nameB);
            Marshal.FreeHGlobal(nameB);
            return @return;
        }

        public static void VertexAttribPointer(int attrib, int size, GLC type, bool normalize, int stride, nint pointer)
        {
            var f = (delegate*<int, int, GLC, GLC, int, nint, void>) Load(GLF.glVertexAttribPointer);
            f(attrib, size, type, normalize ? GLC.GL_TRUE : GLC.GL_FALSE, stride, pointer);
        }

        public static void EnableVertexAttrib(int attrib)
        {
            var f = (delegate*<int, void>) Load(GLF.glEnableVertexAttribArray);
            f(attrib);
        }

        public static void BindBuffer(BufferTarget target, uint buffer)
        {
            var f = (delegate*<uint, uint, void>) Load(GLF.glBindBuffer);
            f(EnumValue(target), buffer);
        }

        public static bool IsShader(uint obj)
        {
            if (obj == 0) return false;
            var f = (delegate*<uint, bool>) Load(GLF.glIsShader);
            return f(obj);
        }
        
        public static bool IsProgram(uint obj)
        {
            if (obj == 0) return false;
            var f = (delegate*<uint, bool>) Load(GLF.glIsProgram);
            return f(obj);
        }

        public static void BindFragDataLocation(uint program, uint color, string name)
        {
            var nameB = Marshal.StringToHGlobalAnsi(name);
            var f = (delegate*<uint, uint, IntPtr, void>) Load(GLF.glBindFragDataLocation);
            f(program, color, nameB);
            Marshal.FreeHGlobal(nameB);
        }

        public static void BufferData(BufferTarget target, int size, Span<byte> data, GLC usage)
        {
            var f = (delegate*<uint, int, void*, int, void>) Load(GLF.glBufferData);
            fixed (void* p = &data.GetPinnableReference())
                f(EnumValue(target), size, p, (int) usage);
        }

        public static void DrawArrays(GLC mode, int first, int count)
        {
            var f = (delegate*<GLC, int, int, void>) Load(GLF.glDrawArrays);
            f(mode, first, count);
        }

        public static void Clear()
        {
            var f = (delegate*<int, void>) Load(GLF.glClear);
            f((int)GLC.GL_COLOR_BUFFER_BIT | (int)GLC.GL_DEPTH_BUFFER_BIT);
        }

        public static void ClearColor(float r, float g, float b, float a)
        {
            var f = (delegate*<float, float, float, float, void>) Load(GLF.glClearColor);
            f(r, g, b, a);
        }

        public static void GenerateVertexArrays(int count, out uint[] arrays)
        {
            arrays = new uint[count];
            var f = (delegate*<int, uint*, void>) Load(GLF.glGenVertexArrays);
            fixed (uint* p = arrays) f(count, p);
        }

        public static void BindVertexArray(uint a)
        {
            var f = (delegate*<uint, void>) Load(GLF.glBindVertexArray);
            f(a);
        }

        public static int GetUniformLocation(uint program, string name)
        {
            var nameB = Marshal.StringToHGlobalAnsi(name);
            var f = (delegate*<uint, IntPtr, int>) Load(GLF.glGetUniformLocation);
            var @return = f(program, nameB);
            Marshal.FreeHGlobal(nameB);
            return @return;
        }

        public static void SetUniformVector4(int location, int count, float[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform4fv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformVector3(int location, int count, float[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform3fv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformVector2(int location, int count, float[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform2fv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniform(int location, int count, float[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform1fv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformVector4(int location, int count, double[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform4dv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformVector3(int location, int count, double[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform3dv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformVector2(int location, int count, double[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform2dv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniform(int location, int count, double[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform1dv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformVector4(int location, int count, int[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform4iv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformVector3(int location, int count, int[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform3iv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformVector2(int location, int count, int[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform2iv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniform(int location, int count, int[] values)
        {
            var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform1iv);
            fixed (void* p = values) f(location, count, p);
        }

        public static void SetUniformMatrix4(int location, int count, bool normalize, float[] values)
        {
            var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix4fv);
            fixed (void* p = values) f(location, count, normalize, p);
        }

        public static void SetUniformMatrix3(int location, int count, bool normalize, float[] values)
        {
            var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix3fv);
            fixed (void* p = values) f(location, count, normalize, p);
        }

        public static void SetUniformMatrix2(int location, int count, bool transpose, float[] values)
        {
            var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix2fv);
            fixed (void* p = values) f(location, count, transpose, p);
        }

        public static void GetShaderInfoLog(uint shader, out int length, out string log)
        {
            var lengthB = Marshal.AllocHGlobal(sizeof(int));
            var logB = Marshal.AllocHGlobal(1 << 16);
            var f = (delegate*<uint, int, IntPtr, IntPtr, void>) Load(GLF.glGetShaderInfoLog);
            f(shader, 1 << 16, lengthB, logB);
            length = Marshal.ReadInt32(lengthB);
            log = Marshal.PtrToStringAnsi(logB);
            Marshal.FreeHGlobal(lengthB);
            Marshal.FreeHGlobal(logB);
        }

        public static void GetProgramInfoLog(uint shader, out int length, out string log)
        {
            var lengthB = Marshal.AllocHGlobal(sizeof(int));
            var logB = Marshal.AllocHGlobal(1 << 16);
            var f = (delegate*<uint, int, IntPtr, IntPtr, void>) Load(GLF.glGetProgramInfoLog);
            f(shader, 1 << 16, lengthB, logB);
            length = Marshal.ReadInt32(lengthB);
            log = Marshal.PtrToStringAnsi(logB);
            Marshal.FreeHGlobal(lengthB);
            Marshal.FreeHGlobal(logB);
        }
    }
}