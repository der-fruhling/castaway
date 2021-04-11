using System;
using System.Diagnostics.CodeAnalysis;
using Castaway.Native;
using static Castaway.Native.GLFW;

namespace Castaway.Window
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public unsafe class GLFWWindow
    {
        public void* GLFW { get; }
        
        public GLFWWindow(void* glfw)
        {
            GLFW = glfw;
            Use();
        }

        public static GLFWWindow Windowed(int width, int height, string title)
        {
            return new GLFWWindow(glfwCreateWindow(width, height, title, null, null));
        }

        public void Use() => glfwMakeContextCurrent(GLFW);

        public bool ShouldClose
        {
            get => glfwWindowShouldClose(GLFW) > 0;
            set => glfwSetWindowShouldClose(GLFW, value ? 1 : 0);
        }

        public GLFWKeyCallback KeyCallback
        {
            set => glfwSetKeyCallback(GLFW, value);
        }

        public void Finish()
        {
            glfwSwapBuffers(GLFW);
            glfwPollEvents();
        }

        public static void Init()
        {
            if (glfwInit() != 1) throw new ApplicationException("GLFW failed to initialize");
        }
    }
}