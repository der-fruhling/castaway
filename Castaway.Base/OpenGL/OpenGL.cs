using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Castaway.Assets;
using Castaway.Math;
using Castaway.Native;
using Castaway.OpenGL.Input;
using Castaway.Rendering;
using GLFW;
using static Castaway.OpenGL.GLC;

namespace Castaway.OpenGL
{
    public class OpenGL : Graphics<
        Window, 
        Buffer, 
        Shader, 
        ShaderProgram, 
        Texture, 
        Framebuffer, 
        IDrawable
    >
    {
        private static OpenGL? _global;

        public static OpenGL Setup()
        {
            if (_global != null) throw new InvalidOperationException("Cannot Setup() after Setup()");
            return _global = new OpenGL();
        }

        public static void Unsetup()
        {
            (_global ?? throw new InvalidOperationException("Cannot Unsetup() before Setup()"))
                .Dispose();
        }

        public static OpenGL Get()
        {
            return _global ?? throw new InvalidOperationException("Cannot Get() before Setup()");
        }
        
        public OpenGL()
        {
            GL.Init();
            Glfw.Init();
        }

        ~OpenGL() => DisposeGL();

        private void DisposeGL()
        {
            Glfw.Terminate();
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeGL();
        }

        public Window? BoundWindow { get; private set; }
        public ShaderProgram? BoundProgram { get; private set; }
        public Framebuffer? BoundFramebuffer { get; private set; }

        /// <summary>
        /// Creates a new window, in windowed form.
        /// </summary>
        /// <param name="title">Title to be displayed at the top of the window.</param>
        /// <param name="width">Width of the window, in pixels.</param>
        /// <param name="height">Height of the window, in pixels.</param>
        /// <param name="visible">Should the window be visible?</param>
        /// <returns>New <see cref="Window"/> object pointing to the newly
        /// created window.</returns>
        /// <seealso cref="Bind(Castaway.OpenGL.Window)"/>
        /// <seealso cref="Destroy(Castaway.OpenGL.Window[])"/>
        public override Window CreateWindowWindowed(string title, int width, int height, bool visible = true)
        {
            Window w = new();
            Glfw.DefaultWindowHints();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 2);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, true);
            Glfw.WindowHint(Hint.Visible, visible);
            w.GlfwWindow = Glfw.CreateWindow(width, height, title, Monitor.None, GLFW.Window.None);
            Bind(w);
            InputSystem.Init();
            BuiltinShaders.Init();
            return w;
        }

        /// <summary>
        /// Creates a new window, in fullscreen. If you use this to create
        /// your window, it could also be a good idea to add an in-game
        /// way to close the window.
        /// </summary>
        /// <param name="title">Title to be displayed in some places.
        /// Fullscreen windows do not display the title at their top.</param>
        /// <param name="visible">Should the window be visible?</param>
        /// <returns>New <see cref="Window"/> object pointing to the newly
        /// created window.</returns>
        /// <seealso cref="Bind(Castaway.OpenGL.Window)"/>
        /// <seealso cref="Destroy(Castaway.OpenGL.Window[])"/>
        public override Window CreateWindowFullscreen(string title, bool visible = true)
        {
            Window w = new();
            Glfw.DefaultWindowHints();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 2);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, true);
            Glfw.WindowHint(Hint.Visible, visible);
            var v = Glfw.GetVideoMode(Glfw.PrimaryMonitor);
            w.GlfwWindow = Glfw.CreateWindow(v.Width, v.Height, title, Glfw.PrimaryMonitor, GLFW.Window.None);
            Bind(w);
            InputSystem.Init();
            BuiltinShaders.Init();
            return w;
        }

        /// <summary>
        /// Creates a new data buffer, for use in drawing.
        /// </summary>
        /// <param name="target">What this new buffer will be used for.</param>
        /// <returns>New <see cref="Buffer"/> object.</returns>
        /// <seealso cref="Bind(Buffer)"/>
        /// <seealso cref="Destroy(Buffer[])"/>
        /// <seealso cref="Draw"/>
        /// <seealso cref="Upload"/>
        public override Buffer CreateBuffer(BufferTarget target)
        {
            var b = new Buffer {Number = GL.CreateBuffer(), Target = target};
            Bind(b);
            return b;
        }

        /// <summary>
        /// Creates a new shader from <paramref name="source"/> to fulfill
        /// <paramref name="stage"/>. Shader objects cannot be used by themselves -
        /// they first need to be linked into a <see cref="ShaderProgram"/> using
        /// <see cref="CreateProgram"/>to be used in rendering.
        /// </summary>
        /// <param name="stage">The stage this shader fills. At least one
        /// <see cref="ShaderStage.Vertex"/> and one
        /// <see cref="ShaderStage.Fragment"/> shader are required to
        /// link a program.</param>
        /// <param name="source">Source code for this shader, in GLSL (OpenGL
        /// Shading Language).</param>
        /// <returns>New <see cref="Shader"/> object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if an invalid stage is passed to this method.
        /// </exception>
        /// <exception cref="GraphicsException">
        /// Thrown if the shader fails to compile. The log will be printed
        /// above the stacktrace.
        /// </exception>
        public override Shader CreateShader(ShaderStage stage, string source)
        {
            Shader s = new()
            {
                SourceCode = source, Stage = stage, Number = GL.CreateShader(stage switch
                {
                    ShaderStage.Vertex => GL.ShaderStage.VertexShader,
                    ShaderStage.Fragment => GL.ShaderStage.FragmentShader,
                    _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
                })
            };
            GL.ShaderSource(s.Number, source);
            GL.CompileShader(s.Number);

            string log;
            if ((log = s.CompileLog).Any())
            {
                Console.Error.WriteLine(log);
                Console.Error.Flush();
            }

            if (!s.CompileSuccess)
                throw new GraphicsException("Failed to compile shader.");

            return s;
        }

        /// <summary>
        /// <p>
        /// Creates a shader from the text contained in an <see cref="Asset"/>.
        /// The asset must be convertable to <see cref="string"/>.
        /// </p>
        /// <p>
        /// Creates a new shader from <paramref name="source"/> to fulfill
        /// <paramref name="stage"/>. Shader objects cannot be used by themselves -
        /// they first need to be linked into a <see cref="ShaderProgram"/> using
        /// <see cref="CreateProgram"/>to be used in rendering.
        /// </p>
        /// </summary>
        /// <seealso cref="CreateShader(Castaway.Rendering.ShaderStage,string)"/>
        /// <inheritdoc cref="CreateShader(Castaway.Rendering.ShaderStage,string)" />
        public override Shader CreateShader(ShaderStage stage, Asset source)
        {
            return CreateShader(stage, source.Type.To<string>(source));
        }

        /// <summary>
        /// Creates a new program from a series of shaders. The program cannot
        /// be used before <see cref="FinishProgram"/> is called.
        /// </summary>
        /// <param name="shaders">
        /// Shaders to link, created from
        /// <see cref="CreateShader(Castaway.Rendering.ShaderStage,string)"/>
        /// </param>
        /// <returns>New <i>incomplete</i> <see cref="ShaderProgram"/> object.</returns>
        /// <seealso cref="CreateInput"/>
        /// <seealso cref="CreateOutput"/>
        /// <seealso cref="BindUniform"/>
        /// <seealso cref="FinishProgram"/>
        public override ShaderProgram CreateProgram(params Shader[] shaders)
        {
            ShaderProgram p = new()
            {
                Shaders = shaders.Select(s => s.Number).ToArray(),
                Number = GL.CreateProgram(),
                Inputs = new Dictionary<string, VertexInputType>(),
                Outputs = new Dictionary<string, uint>(),
                UniformBindings = new Dictionary<string, UniformType>(),
                UniformLocations = new Dictionary<string, int>()
            };
            foreach (var s in shaders) GL.AttachShader(p.Number, s.Number);
            GL.GenerateVertexArrays(1, out var a);
            p.VAO = a[0];

            return p;
        }

        /// <summary>
        /// Creates a new texture from a bitmap.
        /// </summary>
        /// <param name="image">Bitmap to load from.</param>
        /// <returns>New <see cref="Texture"/> object.</returns>
        public override Texture CreateTexture(Bitmap image)
        {
            List<float> data = new();
            for (var i = image.Height - 1; i >= 0; i--)
            for (var j = 0; j < image.Width; j++)
            {
                var c = image.GetPixel(j, i);
                data.AddRange(new float[] {c.R, c.G, c.B, c.A}.Select(f => f / byte.MaxValue));
            }

            GL.GenTextures(1, out var a);
            Texture t = new() {Number = a[0]};
            Bind(t);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int) GL_CLAMP_TO_EDGE);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int) GL_CLAMP_TO_EDGE);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int) GL_LINEAR);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int) GL_LINEAR);
            GL.TexImage2D(GL_TEXTURE_2D, GL_ZERO, GL_RGBA, image.Width, image.Height, GL_RGBA, GL_FLOAT,
                data.ToArray());
            return t;
        }

        /// <summary>
        /// Creates a new texture from an image loaded from an
        /// <see cref="Asset"/>.
        /// </summary>
        /// <inheritdoc cref="CreateTexture(System.Drawing.Bitmap)"/>
        public override Texture CreateTexture(Asset image)
        {
            return CreateTexture(image.Type.To<Bitmap>(image));
        }

        /// <summary>
        /// Creates a new framebuffer, which can be used to implement
        /// post-processing.
        /// </summary>
        /// <param name="window">Window to use as the size of the framebuffer.
        /// </param>
        /// <returns>New <see cref="Framebuffer"/> object.</returns>
        /// <remarks>Unlike other create methods, this one unbinds the object
        /// it creates after it is created.</remarks>
        public override Framebuffer CreateFramebuffer(Window window)
        {
            var (width, height) = GetWindowSize(window);
            GL.GenFramebuffers(1, out var a);
            Framebuffer f = new() {Number = a[0]};
            GL.BindFramebuffer(GL_FRAMEBUFFER, f.Number);

            GL.GenTextures(1, out a);
            GL.BindTexture(GL_TEXTURE_2D, a[0]);
            GL.TexImage2D(GL_TEXTURE_2D, GL_ZERO, GL_RGB, width, height, GL_RGB, GL_FLOAT, null);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int) GL_CLAMP_TO_EDGE);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int) GL_CLAMP_TO_EDGE);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int) GL_NEAREST);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int) GL_NEAREST);
            GL.FramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, a[0], 0);
            f.Texture = new Texture {Number = a[0]};

            GL.GenRenderbuffers(1, out a);
            GL.BindRenderbuffer(GL_RENDERBUFFER, a[0]);
            GL.RenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, width, height);
            GL.FramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, a[0]);

            UnbindFramebuffer(); // just in case
            return f;
        }

        /// <summary>
        /// Destroys any amount of <see cref="Window"/> objects. If the object
        /// is bound, it is automatically unbound. 
        /// </summary>
        /// <param name="windows">Objects to destroy.</param>
        public override void Destroy(params Window[] windows)
        {
            for (var i = 0; i < windows.Length; i++)
            {
                Bind(windows[i]);
                BuiltinShaders.Destroy();
                Glfw.DestroyWindow(windows[i].GlfwWindow);
                windows[i].Destroyed = true;
                if (windows[i] == BoundWindow) BoundWindow = null;
            }
        }

        /// <summary>
        /// Destroys any amount of <see cref="Buffer"/> objects. If the object
        /// is bound, it is automatically unbound.
        /// </summary>
        /// <param name="buffers">Objects to destroy.</param>
        public override void Destroy(params Buffer[] buffers)
        {
            GL.DeleteBuffers(buffers.Length, buffers.Select(b => b.Number).ToArray());
            for (var i = 0; i < buffers.Length; i++)
            {
                buffers[i].Destroyed = true;
            }
        }

        /// <summary>
        /// Destroys any amount of <see cref="Shader"/> objects.
        /// </summary>
        /// <param name="shaders">Objects to destroy.</param>
        public override void Destroy(params Shader[] shaders)
        {
            for (var i = 0; i < shaders.Length; i++)
            {
                GL.DeleteShader(shaders[i].Number);
                shaders[i].Destroyed = true;
            }
        }

        /// <summary>
        /// Destroys any amount of <see cref="ShaderProgram"/> objects.
        /// If the object is bound, it is automatically unbound.
        /// </summary>
        /// <param name="programs">Objects to destroy.</param>
        public override void Destroy(params ShaderProgram[] programs)
        {
            for (var i = 0; i < programs.Length; i++)
            {
                GL.DeleteProgram(programs[i].Number);
                programs[i].Destroyed = true;
                if (programs[i] == BoundProgram) BoundProgram = null;
            }
        }

        /// <summary>
        /// Destroys any amount of <see cref="Texture"/> objects.
        /// If the object is bound, it is automagically unbound.
        /// </summary>
        /// <param name="textures">Objects to destroy.</param>
        public override void Destroy(params Texture[] textures)
        {
            GL.DeleteTextures(textures.Length, textures.Select(t => t.Number).ToArray());
            for (var i = 0; i < textures.Length; i++)
            {
                textures[i].Destroyed = true;
            }
        }

        /// <summary>
        /// Destroys any amount of <see cref="Framebuffer"/> objects.
        /// If the object is bound, it is automatically unbound.
        /// </summary>
        /// <param name="framebuffers"></param>
        public override void Destroy(params Framebuffer[] framebuffers)
        {
            GL.DeleteFramebuffers(framebuffers.Length, framebuffers.Select(f => f.Number).ToArray());
            Destroy(framebuffers.Select(f => f.Texture).ToArray());
            foreach (var f in framebuffers)
                if (f == BoundFramebuffer)
                    BoundFramebuffer = null;
        }

        /// <summary>
        /// Binds a window, making it's context active.
        /// </summary>
        /// <param name="window">Window to bind.</param>
        /// <seealso cref="BoundWindow"/>
        public override void Bind(Window window)
        {
            Glfw.MakeContextCurrent(window.GlfwWindow);
            BoundWindow = window;
        }

        /// <summary>
        /// Binds a buffer to it's target. Multiple buffers can be bound to
        /// different targets.
        /// </summary>
        /// <param name="buffer">Buffer to bind.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the buffer's target is out of range, which should never
        /// happen, but it's always good to make sure.
        /// </exception>
        public override void Bind(Buffer buffer)
        {
            GL.BindBuffer(buffer.Target switch
            {
                BufferTarget.VertexArray => GL.BufferTarget.ArrayBuffer,
                BufferTarget.ElementArray => GL.BufferTarget.ElementArrayBuffer,
                _ => throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Target, "Buffer target out of range.")
            }, buffer.Number);
        }

        /// <summary>
        /// Binds a <i>complete</i> shader program to be used in rendering
        /// operations.
        /// </summary>
        /// <param name="program">Program to bind.</param>
        public override void Bind(ShaderProgram program)
        {
            if (!program.LinkSuccess) throw new InvalidOperationException("Cannot bind program that isn't linked");
            GL.UseProgram(program.Number);
            GL.BindVertexArray(program.VAO);
            BoundProgram = program;
        }

        /// <summary>
        /// Binds a texture.
        /// </summary>
        /// <param name="texture">Texture to bind.</param>
        [Obsolete("Use numbered overload.")]
        public override void Bind(Texture texture)
        {
            GL.BindTexture(GL_TEXTURE_2D, texture.Number);
        }

        /// <summary>
        /// Binds a texture to a specific slot.
        /// </summary>
        /// <param name="texture">Texture to bind.</param>
        /// <param name="number">Texture number to bind to.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the texture
        /// number is out of range.</exception>
        public override void Bind(Texture texture, [Range(0, 31)] int number)
        {
            GL.ActiveTexture(number switch
            {
                0 =>  GL_TEXTURE0,   1 =>  GL_TEXTURE1,   2 =>  GL_TEXTURE2,
                3 =>  GL_TEXTURE3,   4 =>  GL_TEXTURE4,   5 =>  GL_TEXTURE5,
                6 =>  GL_TEXTURE6,   7 =>  GL_TEXTURE7,   8 =>  GL_TEXTURE8,
                9 =>  GL_TEXTURE9,   10 => GL_TEXTURE10,  11 => GL_TEXTURE11,
                12 => GL_TEXTURE12,  13 => GL_TEXTURE13,  14 => GL_TEXTURE14,
                15 => GL_TEXTURE15,  16 => GL_TEXTURE16,  17 => GL_TEXTURE17,
                18 => GL_TEXTURE18,  19 => GL_TEXTURE19,  20 => GL_TEXTURE20,
                21 => GL_TEXTURE21,  22 => GL_TEXTURE22,  23 => GL_TEXTURE23,
                24 => GL_TEXTURE24,  25 => GL_TEXTURE25,  26 => GL_TEXTURE26,
                27 => GL_TEXTURE27,  28 => GL_TEXTURE28,  29 => GL_TEXTURE29,
                30 => GL_TEXTURE30,  31 => GL_TEXTURE31,
                _ => throw new ArgumentOutOfRangeException(nameof(number), number, "Need number < 32, but >= 0")
            });
            GL.BindTexture(GL_TEXTURE_2D, texture);
            texture.BindingPoint = (uint)number;
        }

        /// <summary>
        /// Binds a framebuffer. Once bound, <b>all rendering operations will
        /// be redirected to this framebuffer!</b> This is why
        /// <see cref="UnbindFramebuffer"/> is provided, so that you can
        /// return to normal at any time.
        /// </summary>
        /// <param name="framebuffer"></param>
        public override void Bind(Framebuffer framebuffer)
        {
            GL.BindFramebuffer(GL_FRAMEBUFFER, framebuffer.Number);
            BoundFramebuffer = framebuffer;
        }
        
        /// <summary>
        /// Unbinds a framebuffer, allowing draw operations to write to the
        /// window instead.
        /// </summary>
        /// <seealso cref="Bind(Castaway.OpenGL.Framebuffer)"/>
        public override void UnbindFramebuffer()
        {
            GL.BindFramebuffer(GL_FRAMEBUFFER, 0);
        }

        /// <summary>
        /// Should be called at the end of the frame to actually put things
        /// onto the screen. Also polls for events like keyboard key pressing
        /// and mouse moving.
        /// </summary>
        /// <param name="window">Window of to render to.</param>
        public override void FinishFrame(Window window)
        {
            Glfw.SwapBuffers(window.GlfwWindow);
            InputSystem.Clear();
            Glfw.PollEvents();
            InputSystem.Gamepad.Read();
        }

        /// <summary>
        /// Should be called at the start of the frame.
        /// </summary>
        public override void StartFrame()
        {
            Clear();
        }

        /// <summary>
        /// Uploads an array of floats to a <see cref="Buffer"/>.
        /// </summary>
        /// <param name="buffer">Buffer to upload the data to.</param>
        /// <param name="data">Data to upload.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the buffers target is out of range, for the same reason
        /// that <see cref="Bind(Castaway.OpenGL.Buffer)"/> does.
        /// </exception>
        public override void Upload<T>(Buffer buffer, T[] data)
        {
            byte[] bytes;
            unsafe
            {
                fixed (T* p = data)
                {
                    bytes = new byte[data.Length * sizeof(T)];
                    Marshal.Copy((IntPtr) p, bytes, 0, bytes.Length);
                }
            }

            Bind(buffer);
            GL.BufferData(buffer.Target switch
            {
                BufferTarget.VertexArray => GL.BufferTarget.ArrayBuffer,
                BufferTarget.ElementArray => GL.BufferTarget.ElementArrayBuffer,
                _ => throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Target, "Bad target")
            }, bytes.Length, bytes, GL_STATIC_DRAW);
        }

        /// <summary>
        /// Draws some number of vertices from a buffer.
        /// </summary>
        /// <param name="program">Shader program to use. The object is not
        /// bound by this method.</param>
        /// <param name="drawable">Object to draw. The object <i>is</i> bound by
        /// this method.</param>
        /// <exception cref="InvalidOperationException">Thrown if the drawable
        /// does not have a vertex array attached.</exception>
        public override void Draw(ShaderProgram program, IDrawable drawable)
        {
            if (!drawable.VertexArray.HasValue)
                throw new InvalidOperationException("Drawables must have an attached vertex array.");
            if (drawable.ElementArray.HasValue)
            {
                // Draw Element Buffer
                GL.BindBuffer(GL.BufferTarget.ElementArrayBuffer, drawable.ElementArray.Value.Number);
                GL.BindBuffer(GL.BufferTarget.ArrayBuffer, drawable.VertexArray.Value.Number);
                if (drawable.VertexArray.Value.SetupProgram != program.Number)
                    program.InputBinder.Apply(drawable.VertexArray.Value);
                GL.DrawElements(GL_TRIANGLES, drawable.VertexCount, GL_UNSIGNED_INT, 0);
            }
            else
            {
                GL.BindBuffer(GL.BufferTarget.ArrayBuffer, drawable.VertexArray.Value.Number);
                if (drawable.VertexArray.Value.SetupProgram != program.Number)
                    program.InputBinder.Apply(drawable.VertexArray.Value);
                GL.DrawArrays(GL_TRIANGLES, 0, drawable.VertexCount);
            }
        }

        /// <summary>
        /// Registers a new vertex input, defined like
        /// <c>in &lt;type&gt; &lt;name&gt;;</c>
        /// in your vertex shader.
        /// </summary>
        /// <param name="p">Program to add the input to.</param>
        /// <param name="inputType">Type of the input to create.</param>
        /// <param name="name">Name of the input to create.</param>
        public override void CreateInput(ShaderProgram p, VertexInputType inputType, string name)
        {
            p.Inputs[name] = inputType;
        }

        /// <summary>
        /// Registers a new fragment output, defined like
        /// <c>out vec4 &lt;name&gt;;</c>
        /// in your fragment shader. Using types other than <c>vec4</c> is not
        /// supported by Castaway, but might work anyways.
        /// </summary>
        /// <param name="p">Program to add the output to.</param>
        /// <param name="color">Color number of the output to create. Still not
        /// sure what this means.</param>
        /// <!--
        /// TODO Find out what this means.
        /// -->
        /// <param name="name">Name of the output to create.</param>
        public override void CreateOutput(ShaderProgram p, uint color, string name)
        {
            p.Outputs[name] = color;
        }

        /// <summary>
        /// Registers a new uniform value, defined like
        /// <c>uniform &lt;type&gt; &lt;name&gt;;</c>
        /// in any shader. Uniform values can be modified at any time while the
        /// program containing them is bound.
        /// </summary>
        /// <param name="p">Program to add the uniform to.</param>
        /// <param name="name">Name of the uniform to add.</param>
        /// <param name="type">Type of the uniform to add. Defaults to
        /// <see cref="UniformType.Custom"/>.</param>
        public override void BindUniform(ShaderProgram p, string name, UniformType type = UniformType.Custom)
        {
            p.UniformBindings[name] = type;
        }
        
        [Obsolete("Removing inputs, outputs, and uniforms will be removed in the future.")]
        public override void RemoveInput(ShaderProgram p, string name)
        {
            p.Inputs.Remove(name);
        }

        [Obsolete("Removing inputs, outputs, and uniforms will be removed in the future.")]
        public override void RemoveOutput(ShaderProgram p, string name)
        {
            p.Outputs.Remove(name);
        }

        [Obsolete("Removing inputs, outputs, and uniforms will be removed in the future.")]
        public override void UnbindUniform(ShaderProgram p, string name)
        {
            p.UniformBindings.Remove(name);
        }

        /// <summary>
        /// Finishes up an <i>incomplete</i> program, allowing it to be used
        /// for rendering. All inputs, outputs, and uniforms should be
        /// defined <i>before</i> this method is called. This method also
        /// binds the object.
        /// </summary>
        /// <param name="p">Reference to the program to complete.</param>
        /// <exception cref="GraphicsException">
        /// Thrown if the program fails to link. A log will be printed above
        /// the stacktrace.
        /// </exception>
        public override void FinishProgram(ref ShaderProgram p)
        {
            foreach (var (name, color) in p.Outputs)
                GL.BindFragDataLocation(p.Number, color, name);

            GL.LinkProgram(p.Number);

            string log;
            if ((log = p.LinkLog).Any())
            {
                Console.Error.WriteLine(log);
                Console.Error.Flush();
            }

            if (!p.LinkSuccess)
                throw new GraphicsException("Failed to link shader program.");

            foreach (var s in p.Shaders) GL.DeleteShader(s);
            p.InputBinder = new ShaderInputBinder(p);

            Bind(p);
            foreach (var (name, _) in p.UniformBindings)
                p.UniformLocations[name] = GL.GetUniformLocation(p.Number, name);
        }

        /// <summary>
        /// Gets the name of the uniform with the specified type.
        /// </summary>
        /// <param name="p">Shader program to search.</param>
        /// <param name="type">Type to find.</param>
        /// <returns>Name of the uniform <paramref name="type"/> references</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the specified type has multiple uniforms bound to it -or-
        /// There are no <see cref="BindUniform">bound uniforms</see> in the program. -or-
        /// There is no such uniform of <paramref name="type"/> -or-
        /// <paramref name="type"/> is <see cref="UniformType.Custom"/>
        /// </exception>
        public override string UniformRef(ShaderProgram p, UniformType type)
        {
            if (type == UniformType.Custom)
                throw new InvalidOperationException("Cannot reference custom uniform binding.");
            return p.UniformBindings.ContainsValue(type) 
                ? p.UniformBindings.Single(pair => pair.Value == type).Key 
                : "";
        }

        /// <summary>
        /// Sets a value in a uniform. The program must already be bound for
        /// this to work properly.
        /// </summary>
        public override void SetUniform(ShaderProgram p, string name, float f)
        {
            if(name.Any()) GL.SetUniform(p.UniformLocations[name], 1, new[] {f});
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, float x, float y)
        {
            if(name.Any()) GL.SetUniformVector2(p.UniformLocations[name], 1, new[] {x, y});
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, float x, float y, float z)
        {
            if(name.Any()) GL.SetUniformVector3(p.UniformLocations[name], 1, new[] {x, y, z});
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, float x, float y, float z, float w)
        {
            if(name.Any()) GL.SetUniformVector4(p.UniformLocations[name], 1, new[] {x, y, z, w});
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, int i)
        {
            if(name.Any()) GL.SetUniform(p.UniformLocations[name], 1, new[] {i});
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, int x, int y)
        {
            if(name.Any()) GL.SetUniformVector2(p.UniformLocations[name], 1, new[] {x, y});
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, int x, int y, int z)
        {
            if(name.Any()) GL.SetUniformVector3(p.UniformLocations[name], 1, new[] {x, y, z});
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, int x, int y, int z, int w)
        {
            if(name.Any()) GL.SetUniformVector4(p.UniformLocations[name], 1, new[] {x, y, z, w});
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, Vector2 v)
        {
            SetUniform(p, name, v.X, v.Y);
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, Vector3 v)
        {
            SetUniform(p, name, v.X, v.Y, v.Z);
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, Vector4 v)
        {
            SetUniform(p, name, v.X, v.Y, v.Z, v.W);
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, Matrix2 m)
        {
            if(name.Any()) GL.SetUniformMatrix2(p.UniformLocations[name], 1, false, m.Array);
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, Matrix3 m)
        {
            if(name.Any()) GL.SetUniformMatrix3(p.UniformLocations[name], 1, false, m.Array);
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, Matrix4 m)
        {
            if(name.Any()) GL.SetUniformMatrix4(p.UniformLocations[name], 1, false, m.Array);
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        /// <seealso cref="Bind(Castaway.OpenGL.Texture, int)"/>
        public override void SetUniform(ShaderProgram p, string name, Texture t)
        {
            SetUniform(p, name, (int)t.BindingPoint);
        }

        /// <inheritdoc cref="SetUniform(Castaway.OpenGL.ShaderProgram,string,float)"/>
        public override void SetUniform(ShaderProgram p, string name, Framebuffer t)
        {
            SetUniform(p, name, t.Texture);
        }

        /// <summary>
        /// Clears the color, depth, and stencil buffers in the current render
        /// target.
        /// </summary>
        public override void Clear()
        {
            GL.Clear();
        }

        /// <summary>
        /// Sets the color that <see cref="Color"/> clears to.
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public override void SetClearColor(float r, float g, float b)
        {
            GL.ClearColor(r, g, b, 1);
        }

        /// <summary>
        /// Sets the size of the specified window.
        /// </summary>
        /// <param name="window">Window to set the size of.</param>
        /// <param name="width">The new width of the window.</param>
        /// <param name="height">The new height of the window.</param>
        public override void SetWindowSize(Window window, int width, int height)
        {
            Glfw.SetWindowSize(window.GlfwWindow, width, height);
        }

        /// <summary>
        /// Sets the title of the specified window.
        /// </summary>
        /// <param name="window">Window to set the title of.</param>
        /// <param name="title">The new title of the window.</param>
        public override void SetWindowTitle(Window window, string title)
        {
            Glfw.SetWindowTitle(window.GlfwWindow, title);
        }

        /// <summary>
        /// Gets the size of the specified window.
        /// </summary>
        /// <param name="window">Window to get the size from.</param>
        /// <returns>Size of the specified window.</returns>
        public override (int Width, int Height) GetWindowSize(Window window)
        {
            Glfw.GetWindowSize(window.GlfwWindow, out var w, out var h);
            return (w, h);
        }

        /// <summary>
        /// Checks if the user closed the window.
        /// </summary>
        /// <param name="window">Window to check.</param>
        /// <returns><c>true</c> if should still be open, <c>false</c>
        /// otherwise</returns>
        public override bool WindowShouldBeOpen(Window window)
        {
            return !Glfw.WindowShouldClose(window.GlfwWindow);
        }

        /// <summary>
        /// Sets the window to visible.
        /// </summary>
        /// <param name="window">Window to visibleize.</param>
        public override void ShowWindow(Window window)
        {
            Glfw.ShowWindow(window.GlfwWindow);
        }

        /// <summary>
        /// Sets the window to invisible.
        /// </summary>
        /// <param name="window">Window to invisibleize.</param>
        public override void HideWindow(Window window)
        {
            Glfw.HideWindow(window.GlfwWindow);
        }

        /// <summary>
        /// Destroys a collection of objects with varying types. Supported
        /// types are <see cref="Window"/>, <see cref="Buffer"/>,
        /// <see cref="Shader"/>, <see cref="ShaderProgram"/>,
        /// <see cref="Texture"/>, and <see cref="Framebuffer"/>.
        /// </summary>
        /// <param name="things">The things to destroy.</param>
        /// <exception cref="InvalidOperationException">Thrown if an
        /// invalid thing was passed.</exception>
        public override void Destroy(params object[] things)
        {
            foreach (var thing in things)
            {
                switch (thing)
                {
                    case Window window: 
                        Destroy(window); 
                        break;
                    case Buffer buffer: 
                        Destroy(buffer);
                        break;
                    case Shader shader:
                        Destroy(shader);
                        break;
                    case ShaderProgram program:
                        Destroy(program);
                        break;
                    case Texture texture:
                        Destroy(texture);
                        break;
                    case Framebuffer framebuffer:
                        Destroy(framebuffer);
                        break;
                    default:
                        throw new InvalidOperationException($"Cannot destroy objects of type {thing.GetType().FullName}");
                }
            }
        }

        /// <summary>
        /// Binds a collection of things with varying types. Supported types
        /// are <see cref="Window"/>, <see cref="Buffer"/>,
        /// <see cref="ShaderProgram"/>, <see cref="Texture"/>, and
        /// <see cref="Framebuffer"/>.
        /// </summary>
        /// <param name="things">Things to bind.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if an invalid thing was passes or if multiple of a single
        /// type was found.
        /// </exception>
        public override void Bind(params object[] things)
        {
            var bw = false;
            var bp = false;
            var bt = false;
            var bf = false;
            foreach (var thing in things)
            {
                switch (thing)
                {
                    case Window window:
                        if (bw) throw new InvalidOperationException("Cannot bind multiple windows.");
                        Bind(window);
                        bw = true;
                        break;
                    case Buffer buffer: 
                        Bind(buffer);
                        break;
                    case ShaderProgram program:
                        if (bp) throw new InvalidOperationException("Cannot bind multiple programs.");
                        Bind(program);
                        bp = true;
                        break;
                    case Texture texture:
                        if (bt) throw new InvalidOperationException("Cannot bind multiple textures.");
                        Bind(texture);
                        bt = true;
                        break;
                    case Framebuffer framebuffer:
                        if (bf) throw new InvalidOperationException("Cannot bind multiple framebuffers.");
                        Bind(framebuffer);
                        bf = true;
                        break;
                    default:
                        throw new InvalidOperationException($"Cannot bind objects of type {thing.GetType().FullName}");
                }
            }
        }
    }
}