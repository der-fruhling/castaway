using System;
using System.Diagnostics.CodeAnalysis;
using Castaway.Native;
using static Castaway.Native.GLFW;

namespace Castaway.Window
{
    /// <summary>
    /// Window managed by GLFW.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public unsafe class GLFWWindow
    {
        public static GLFWWindow Current;
        
        /// <summary>
        /// Pointer to the <c>GLFWwindow</c> structure created by
        /// <see cref="Native.GLFW.glfwCreateWindow"/>.
        /// </summary>
        public void* GLFW { get; }

        private GLFWWindow(void* glfw)
        {
            GLFW = glfw;
            Use();
        }

        /// <summary>
        /// Creates a new windowed window.
        /// </summary>
        /// <param name="width">Width of the new window.</param>
        /// <param name="height">Height of the new window.</param>
        /// <param name="title">Title of the new window.</param>
        /// <returns>The new window.</returns>
        public static GLFWWindow Windowed(int width, int height, string title)
        {
            return new GLFWWindow(glfwCreateWindow(width, height, title, null, null));
        }

        /// <summary>
        /// Makes this GLFW context current.
        /// </summary>
        public void Use() => glfwMakeContextCurrent(GLFW);

        /// <summary>
        /// Determines whether this window was closed by pressing the X. Can
        /// also be set.
        /// </summary>
        public bool ShouldClose
        {
            get => glfwWindowShouldClose(GLFW) > 0;
            set => glfwSetWindowShouldClose(GLFW, value ? 1 : 0);
        }

        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        private GLFWKeyCallback _keyCallback;
        
        /// <summary>
        /// Sets the key callback for this window.
        /// </summary>
        public GLFWKeyCallback KeyCallback
        {
            set
            {
                glfwSetKeyCallback(GLFW, value);
                _keyCallback = value;
            }
        }

        /// <summary>
        /// Should be called after drawing is finished, to actually put stuff
        /// on the screen.
        /// </summary>
        public void Finish()
        {
            glfwSwapBuffers(GLFW);
            glfwPollEvents();
        }

        /// <summary>
        /// Initializes GLFW.
        /// </summary>
        /// <exception cref="ApplicationException">Thrown if GLFW didn't want
        /// to initialize.</exception>
        public static void Init()
        {
            if (glfwInit() != 1) throw new ApplicationException("GLFW failed to initialize");
        }

        public void GetWindowSize(out int width, out int height)
        {
            int w, h;
            glfwGetWindowSize(GLFW, &w, &h);
            width = w;
            height = h;
        }
    }
}