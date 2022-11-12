#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GLFW;

namespace Castaway.OpenGL.Native;

public static unsafe partial class GL
{
    private static readonly Dictionary<GLF, IntPtr> fn = new();

    private static IntPtr Load(GLF f)
    {
        return !fn.ContainsKey(f) || fn[f] == IntPtr.Zero
            ? fn[f] = Glfw.GetProcAddress(Enum.GetName(f))
            : fn[f];
    }

    public static void GenBuffers(int count, out uint[] output)
    {
        output = new uint[count];
        if (count == 0) return;
        var f = (delegate*<int, uint*, void>) Load(GLF.glGenBuffers);
        fixed (uint* p = output)
        {
            f(count, p);
        }
    }

    public static void CreateBuffers(int count, out uint[] output)
    {
        output = new uint[count];
        if (count == 0) return;
        var f = (delegate*<int, uint*, void>) Load(GLF.glCreateBuffers);
        fixed (uint* p = output)
        {
            f(count, p);
        }
    }

    public static void DeleteBuffers(int count, uint[] buffers)
    {
        if (count == 0) return;
        var f = (delegate*<int, uint*, void>) Load(GLF.glDeleteBuffers);
        fixed (uint* p = buffers)
        {
            f(count, p);
        }
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
        fixed (int* lp = lengths)
        {
            f(shader, sourceCode.Length, arrayArray, lp);
        }

        for (var i = 0; i < sourceCode.Length; i++)
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
        {
            f(EnumValue(target), size, p, (int) usage);
        }
    }

    public static void DrawArrays(GLC mode, int first, int count)
    {
        var f = (delegate*<GLC, int, int, void>) Load(GLF.glDrawArrays);
        f(mode, first, count);
    }

    public static void Clear()
    {
        var f = (delegate*<int, void>) Load(GLF.glClear);
        f((int) GLC.GL_COLOR_BUFFER_BIT | (int) GLC.GL_DEPTH_BUFFER_BIT | (int) GLC.GL_STENCIL_BUFFER_BIT);
    }

    public static void ClearColor(float r, float g, float b, float a)
    {
        var f = (delegate*<float, float, float, float, void>) Load(GLF.glClearColor);
        f(r, g, b, a);
    }

    public static void GenVertexArrays(int count, out uint[] arrays)
    {
        arrays = new uint[count];
        var f = (delegate*<int, uint*, void>) Load(GLF.glGenVertexArrays);
        fixed (uint* p = arrays)
        {
            f(count, p);
        }
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
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector3(int location, int count, float[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform3fv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector2(int location, int count, float[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform2fv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniform(int location, int count, float[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform1fv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector4(int location, int count, double[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform4dv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector3(int location, int count, double[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform3dv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector2(int location, int count, double[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform2dv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniform(int location, int count, double[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform1dv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector4(int location, int count, int[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform4iv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector3(int location, int count, int[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform3iv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector2(int location, int count, int[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform2iv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector4(int location, int count, uint[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform4uiv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector3(int location, int count, uint[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform3uiv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformVector2(int location, int count, uint[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform2uiv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniform(int location, int count, int[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform1iv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniform(int location, int count, uint[] values)
    {
        var f = (delegate*<int, int, void*, void>) Load(GLF.glUniform1uiv);
        fixed (void* p = values)
        {
            f(location, count, p);
        }
    }

    public static void SetUniformMatrix4(int location, int count, bool normalize, float[] values)
    {
        var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix4fv);
        fixed (void* p = values)
        {
            f(location, count, normalize, p);
        }
    }

    public static void SetUniformMatrix3(int location, int count, bool normalize, float[] values)
    {
        var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix3fv);
        fixed (void* p = values)
        {
            f(location, count, normalize, p);
        }
    }

    public static void SetUniformMatrix2(int location, int count, bool transpose, float[] values)
    {
        var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix2fv);
        fixed (void* p = values)
        {
            f(location, count, transpose, p);
        }
    }

    public static void SetUniformMatrix4(int location, int count, bool normalize, double[] values)
    {
        var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix4dv);
        fixed (void* p = values)
        {
            f(location, count, normalize, p);
        }
    }

    public static void SetUniformMatrix3(int location, int count, bool normalize, double[] values)
    {
        var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix3dv);
        fixed (void* p = values)
        {
            f(location, count, normalize, p);
        }
    }

    public static void SetUniformMatrix2(int location, int count, bool transpose, double[] values)
    {
        var f = (delegate*<int, int, bool, void*, void>) Load(GLF.glUniformMatrix2dv);
        fixed (void* p = values)
        {
            f(location, count, transpose, p);
        }
    }

    public static void GetShaderInfoLog(uint shader, out int length, out string log)
    {
        var lengthB = Marshal.AllocHGlobal(sizeof(int));
        var logB = Marshal.AllocHGlobal(1 << 16);
        var f = (delegate*<uint, int, IntPtr, IntPtr, void>) Load(GLF.glGetShaderInfoLog);
        f(shader, 1 << 16, lengthB, logB);
        length = Marshal.ReadInt32(lengthB);
        log = Marshal.PtrToStringAnsi(logB)!;
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
        log = Marshal.PtrToStringAnsi(logB)!;
        Marshal.FreeHGlobal(lengthB);
        Marshal.FreeHGlobal(logB);
    }

    public static void GenTextures(int count, out uint[] textures)
    {
        textures = new uint[count];
        var f = (delegate*<int, uint*, void>) Load(GLF.glGenTextures);
        fixed (uint* pTextures = textures)
        {
            f(count, pTextures);
        }
    }

    public static void BindTexture(GLC where, uint texture)
    {
        var f = (delegate*<GLC, uint, void>) Load(GLF.glBindTexture);
        f(where, texture);
    }

    public static void TexParameter(GLC where, GLC param, int value)
    {
        var f = (delegate*<GLC, GLC, int, void>) Load(GLF.glTexParameteri);
        f(where, param, value);
    }

    public static void TexParameter(GLC where, GLC param, float[] value)
    {
        var f = (delegate*<GLC, GLC, float*, void>) Load(GLF.glTexParameterfv);
        fixed (float* pValue = value)
        {
            f(@where, param, pValue);
        }
    }

    public static void TexImage2D(GLC where, GLC level, GLC internalFormat, int width, int height,
        GLC format, GLC type, float[]? data)
    {
        var f = (delegate*<GLC, GLC, GLC, int, int, int, GLC, GLC, float*, void>) Load(GLF.glTexImage2D);
        if (data != null)
            fixed (float* pData = data)
            {
                f(@where, level, internalFormat, width, height, 0, format, type, pData);
            }
        else
            f(@where, level, internalFormat, width, height, 0, format, type, null);
    }

    public static void GenFramebuffers(int count, out uint[] framebuffers)
    {
        framebuffers = new uint[count];
        var f = (delegate*<int, uint*, void>) Load(GLF.glGenFramebuffers);
        fixed (uint* pFramebuffers = framebuffers)
        {
            f(count, pFramebuffers);
        }
    }

    public static void BindFramebuffer(GLC where, uint framebuffer)
    {
        var f = (delegate*<GLC, uint, void>) Load(GLF.glBindFramebuffer);
        f(where, framebuffer);
    }

    public static void DeleteFramebuffers(int count, params uint[] framebuffers)
    {
        var f = (delegate*<int, uint*, void>) Load(GLF.glDeleteFramebuffers);
        fixed (uint* pFramebuffers = framebuffers)
        {
            f(count, pFramebuffers);
        }
    }

    public static void FramebufferTexture2D(GLC where, GLC attachment, GLC target, uint texture, int level)
    {
        var f = (delegate*<GLC, GLC, GLC, uint, int, void>) Load(GLF.glFramebufferTexture2D);
        f(where, attachment, target, texture, level);
    }

    public static void GenRenderbuffers(int count, out uint[] renderbuffers)
    {
        renderbuffers = new uint[count];
        var f = (delegate*<int, uint*, void>) Load(GLF.glGenRenderbuffers);
        fixed (uint* pRenderbuffers = renderbuffers)
        {
            f(count, pRenderbuffers);
        }
    }

    public static void BindRenderbuffer(GLC where, uint renderbuffer)
    {
        var f = (delegate*<GLC, uint, void>) Load(GLF.glBindRenderbuffer);
        f(where, renderbuffer);
    }

    public static void RenderbufferStorage(GLC where, GLC what, int width, int height)
    {
        var f = (delegate*<GLC, GLC, int, int, void>) Load(GLF.glRenderbufferStorage);
        f(where, what, width, height);
    }

    public static void FramebufferRenderbuffer(GLC where, GLC attachment, GLC target, uint renderbuffer)
    {
        var f = (delegate*<GLC, GLC, GLC, uint, void>) Load(GLF.glFramebufferRenderbuffer);
        f(where, attachment, target, renderbuffer);
    }

    public static void DeleteTextures(int count, params uint[] textures)
    {
        var f = (delegate*<int, uint*, void>) Load(GLF.glDeleteTextures);
        fixed (uint* pTextures = textures)
        {
            f(count, pTextures);
        }
    }

    public static void ActiveTexture(GLC texture)
    {
        var f = (delegate*<GLC, void>) Load(GLF.glActiveTexture);
        f(texture);
    }

    public static void DrawElements(GLC mode, int count, GLC type, nuint indices)
    {
        var f = (delegate*<GLC, int, GLC, void*, void>) Load(GLF.glDrawElements);
        f(mode, count, type, (void*) indices);
    }

    public static void Enable(GLC cap)
    {
        var f = (delegate*<GLC, void>) Load(GLF.glEnable);
        f(cap);
    }

    public static void Viewport(int x, int y, int w, int h)
    {
        var f = (delegate*<int, int, int, int, void>) Load(GLF.glViewport);
        f(x, y, w, h);
    }

    public static string GetString(GLC e)
    {
        var f = (delegate*<GLC, byte*>) Load(GLF.glGetString);
        var bytes = f(e);
        if (bytes == (void*) 0) return string.Empty;
        var str = "";
        var i = 0;
        while (bytes[i] != 0) str += Encoding.UTF8.GetString(new[] {bytes[i++]});
        return str;
    }

    public static int GetInt(GLC e)
    {
        int v;
        var f = (delegate*<GLC, int*, void>) Load(GLF.glGetIntegerv);
        f(e, &v);
        return v;
    }

    public static bool IsTexture(uint obj)
    {
        var f = (delegate*<uint, bool>) Load(GLF.glIsTexture);
        return f(obj);
    }

    public static void GenerateMipmap(GLC e)
    {
        var f = (delegate*<GLC, void>) Load(GLF.glGenerateMipmap);
        f(e);
    }

    public static void DeleteVertexArrays(params uint[] vaos)
    {
        var f = (delegate*<int, uint*, void>) Load(GLF.glDeleteVertexArrays);
        fixed (uint* p = vaos)
        {
            f(vaos.Length, p);
        }
    }

    public static void BindImageTexture(uint unit, uint texture, int level, bool layered, int layer, GLC access,
        GLC format)
    {
        var f = (delegate*<uint, uint, int, bool, int, GLC, GLC, void>) Load(GLF.glBindImageTexture);
        f(unit, texture, level, layered, layer, access, format);
    }
}