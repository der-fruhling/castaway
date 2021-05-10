using System;
using System.Runtime.InteropServices;
using static Castaway.Native.CawNative;

namespace Castaway.Native
{
    public sealed class CastawayError : Exception
    {
        public CastawayError(error e) : base($"Castaway error: {e}") { }
    }
    
    // ReSharper disable once InconsistentNaming
    public sealed class OpenGLError : Exception
    {
        public OpenGLError(string s) : base($"OpenGL error: {s}") { }
    }
    
    public static unsafe class ErrorHandlers
    {
        public delegate void ErrorHandler(error e);
        
        // ReSharper disable once InconsistentNaming
        public delegate void GLErrorHandler(uint gl, string str);

        public static void SetDefault() =>
            Set(e => throw new CastawayError(e),
                (_, s) => throw new OpenGLError(s));
        
        public static void Set(ErrorHandler errorHandler, GLErrorHandler glErrorHandler)
        {
            cawSetErrorHandler((error, gl) =>
            {
                if (error != error.gl_error) errorHandler(error);
                else glErrorHandler(gl, Marshal.PtrToStringAnsi(new IntPtr(cawErrorString(gl))));
            });
        }
    }
}