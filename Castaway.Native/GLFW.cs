using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.UnmanagedType;

namespace Castaway.Native
{
    /// <summary>
    /// Various native functions relating to GLFW.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static unsafe class GLFW
    {
        public const uint FOCUSED = 0x00020001;
        public const uint ICONIFIED = 0x00020002;
        public const uint RESIZABLE = 0x00020003;
        public const uint VISIBLE = 0x00020004;
        public const uint DECORATED = 0x00020005;
        public const uint AUTO_ICONIFY = 0x00020006;
        public const uint FLOATING = 0x00020007;
        public const uint MAXIMISED = 0x00020008;
        // todo many more things nede to add here.
        
        private const string l = "/usr/local/lib/libglfw.so";
        
        [DllImport(l)] 
        public static extern int glfwInit();
        
        [DllImport(l)] 
        public static extern void glfwTerminate();
        
        [DllImport(l)] 
        public static extern int glfwGetError(char** @string);
        
        [DllImport(l)] 
        public static extern void glfwInitHint(int hint, int value);
        
        [DllImport(l)] 
        public static extern void glfwWindowHint(int hint, int value);

        [DllImport(l)]
        public static extern void* glfwCreateWindow(int w, int h, [MarshalAs(LPStr)] string title, void* monitor,
            void* share);

        [DllImport(l)]
        public static extern void glfwSwapBuffers(void* window);

        [DllImport(l)]
        public static extern void glfwPollEvents();

        [DllImport(l)]
        public static extern void glfwMakeContextCurrent(void* window);

        [DllImport(l)]
        public static extern int glfwWindowShouldClose(void* window);

        [DllImport(l)]
        public static extern void glfwSetWindowShouldClose(void* window, int value);

        public delegate void GLFWKeyCallback(void* window, int key, int scancode, int action, int mods);
        [DllImport(l)]
        public static extern void glfwSetKeyCallback(void* window, GLFWKeyCallback callback);

        [DllImport(l)]
        public static extern void glfwGetWindowSize(void* window, int* width, int* height);
    }
}