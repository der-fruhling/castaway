using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Castaway.Assets;
using Castaway.Math;
using Castaway.Rendering;
using GLFW;
using static Castaway.OpenGL.GLC;
using Image = System.Drawing.Image;

namespace Castaway.OpenGL
{
    public class OpenGL : IGraphics<Window, Buffer, Shader, ShaderProgram, Texture, Framebuffer>
    {
        public OpenGL()
        {
            GL.Init();
            Glfw.Init();
        }
        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Glfw.Terminate();
        }

        public Window? BoundWindow { get; private set; }
        public Buffer? BoundBuffer { get; private set; }
        public ShaderProgram? BoundProgram { get; private set; }
        public Texture? BoundTexture { get; private set; }
        public Framebuffer? BoundFramebuffer { get; private set; }

        public Window CreateWindowWindowed(string title, int width, int height)
        {
            Window w = new();
            Glfw.DefaultWindowHints();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 4);
            Glfw.WindowHint(Hint.ContextVersionMinor, 5);
            w.GlfwWindow = Glfw.CreateWindow(width, height, title, Monitor.None, GLFW.Window.None);
            Bind(w);
            return w;
        }

        public Window CreateWindowFullscreen(string title)
        {
            Window w = new();
            Glfw.DefaultWindowHints();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 4);
            Glfw.WindowHint(Hint.ContextVersionMinor, 5);
            var v = Glfw.GetVideoMode(Glfw.PrimaryMonitor);
            w.GlfwWindow = Glfw.CreateWindow(v.Width, v.Height, title, Glfw.PrimaryMonitor, GLFW.Window.None);
            return w;
        }

        public Buffer CreateBuffer(BufferTarget target)
        {
            var b = new Buffer {Number = GL.CreateBuffer(), Target = target};
            Bind(b);
            return b;
        }

        public Shader CreateShader(ShaderStage stage, string source)
        {
            Shader s = new() {SourceCode = source, Stage = stage, Number = GL.CreateShader(stage switch
            {
                ShaderStage.Vertex => GL.ShaderStage.VertexShader,
                ShaderStage.Fragment => GL.ShaderStage.FragmentShader,
                _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
            })};
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

        public Shader CreateShader(ShaderStage stage, Asset source)
        {
            return CreateShader(stage, source.Type.To<string>(source));
        }

        public ShaderProgram CreateProgram(params Shader[] shaders)
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
            
            Bind(p);
            return p;
        }

        public Texture CreateTexture(Bitmap image)
        {
            List<float> data = new();
            for (var i = image.Height - 1; i >= 0; i--)
            {
                for (var j = 0; j < image.Width; j++)
                {
                    var c = image.GetPixel(j, i);
                    data.AddRange(new float[]{c.R, c.G, c.B, c.A}.Select(f => f / byte.MaxValue));
                }
            }

            GL.GenTextures(1, out var a);
            Texture t = new() {Number = a[0]};
            Bind(t);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int)GL_CLAMP_TO_EDGE);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int)GL_CLAMP_TO_EDGE);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_LINEAR);
            GL.TexImage2D(GL_TEXTURE_2D, GL_ZERO, GL_RGBA, image.Width, image.Height, GL_RGBA, GL_FLOAT, data.ToArray());
            return t;
        }

        public Texture CreateTexture(Asset image)
        {
            return CreateTexture(image.Type.To<Bitmap>(image));
        }

        public Framebuffer CreateFramebuffer(Window window)
        {
            var (width, height) = GetWindowSize(window);
            GL.GenFramebuffers(1, out var a);
            Framebuffer f = new() {Number = a[0]};
            GL.BindFramebuffer(GL_FRAMEBUFFER, f.Number);
            
            GL.GenTextures(1, out a);
            GL.BindTexture(GL_TEXTURE_2D, a[0]);
            GL.TexImage2D(GL_TEXTURE_2D, GL_ZERO, GL_RGB, width, height, GL_RGB, GL_FLOAT, null);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int)GL_CLAMP_TO_EDGE);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int)GL_CLAMP_TO_EDGE);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_NEAREST);
            GL.TexParameter(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_NEAREST);
            GL.FramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, a[0], 0);
            f.Texture = new Texture {Number = a[0]};
            
            GL.GenRenderbuffers(1, out a);
            GL.BindRenderbuffer(GL_RENDERBUFFER, a[0]);
            GL.RenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, width, height);
            GL.FramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, a[0]);

            UnbindFramebuffer(); // just in case
            return f;
        }

        public void Destroy(params Window[] windows)
        {
            for (var i = 0; i < windows.Length; i++)
            {
                Glfw.DestroyWindow(windows[i].GlfwWindow);
                windows[i].Destroyed = true;
                if (windows[i] == BoundWindow) BoundWindow = null;
            }
        }

        public void Destroy(params Buffer[] buffers)
        {
            GL.DeleteBuffers(buffers.Length, buffers.Select(b => b.Number).ToArray());
            for (var i = 0; i < buffers.Length; i++)
            {
                buffers[i].Destroyed = true;
                if (buffers[i] == BoundBuffer) BoundBuffer = null;
            }
        }

        public void Destroy(params Shader[] shaders)
        {
            for (var i = 0; i < shaders.Length; i++)
            {
                GL.DeleteShader(shaders[i].Number);
                shaders[i].Destroyed = true;
            }
        }

        public void Destroy(params ShaderProgram[] programs)
        {
            for (var i = 0; i < programs.Length; i++)
            {
                GL.DeleteProgram(programs[i].Number);
                programs[i].Destroyed = true;
                if (programs[i] == BoundProgram) BoundProgram = null;
            }
        }

        public void Destroy(params Texture[] textures)
        {
            GL.DeleteTextures(textures.Length, textures.Select(t => t.Number).ToArray());
            for (var i = 0; i < textures.Length; i++)
            {
                textures[i].Destroyed = true;
                if (textures[i] == BoundTexture) BoundTexture = null;
            }
        }

        public void Destroy(params Framebuffer[] framebuffers)
        {
            GL.DeleteFramebuffers(framebuffers.Length, framebuffers.Select(f => f.Number).ToArray());
            Destroy(framebuffers.Select(f => f.Texture).ToArray());
            foreach(var f in framebuffers)
            {
                if (f == BoundFramebuffer) BoundFramebuffer = null;
            }
        }

        public void Bind(Window window)
        {
            Glfw.MakeContextCurrent(window.GlfwWindow);
            BoundWindow = window;
        }

        public void Bind(Buffer buffer)
        {
            GL.BindBuffer(buffer.Target switch
            {
                BufferTarget.VertexArray => GL.BufferTarget.ArrayBuffer,
                BufferTarget.ElementArray => GL.BufferTarget.ElementArrayBuffer,
                _ => throw new ArgumentOutOfRangeException(nameof(buffer), buffer.Target, "Buffer target out of range.")
            }, buffer.Number);
            BoundBuffer = buffer;
        }

        public void Bind(ShaderProgram program)
        {
            GL.UseProgram(program.Number);
            GL.BindVertexArray(program.VAO);
            BoundProgram = program;
        }

        public void Bind(Texture texture)
        {
            GL.BindTexture(GL_TEXTURE_2D, texture.Number);
            BoundTexture = texture;
        }

        public void Bind(Framebuffer framebuffer)
        {
            GL.BindFramebuffer(GL_FRAMEBUFFER, framebuffer.Number);
            BoundFramebuffer = framebuffer;
        }

        public void UnbindFramebuffer()
        {
            GL.BindFramebuffer(GL_FRAMEBUFFER, 0);
        }

        public void FinishFrame(Window window)
        {
            Glfw.SwapBuffers(window.GlfwWindow);
            Glfw.PollEvents();
        }

        public void StartFrame(Window window)
        {
            Clear();
        }

        public void Upload(Buffer buffer, float[] data)
        {
            byte[] bytes;
            unsafe
            {
                fixed (float* p = data)
                {
                    bytes = new byte[data.Length * sizeof(float)];
                    Marshal.Copy((IntPtr)p, bytes, 0, bytes.Length);
                }
            }
            Bind(buffer);
            GL.BufferData(buffer.Target switch
            {
                BufferTarget.VertexArray => GL.BufferTarget.ArrayBuffer,
                BufferTarget.ElementArray => GL.BufferTarget.ElementArrayBuffer,
                _ => throw new ArgumentOutOfRangeException()
            }, bytes.Length, bytes, GL_STATIC_DRAW);
        }

        public void Draw(ShaderProgram program, Buffer buffer, int vertexCount)
        {
            if(buffer.SetupProgram != program.Number)
                program.InputBinder.Apply(buffer);
            GL.DrawArrays(GL_TRIANGLES, 0, vertexCount);
        }

        public void CreateInput(ShaderProgram p, VertexInputType inputType, string name)
        {
            p.Inputs[name] = inputType;
        }

        public void CreateOutput(ShaderProgram p, uint color, string name)
        {
            p.Outputs[name] = color;
        }

        public void BindUniform(ShaderProgram p, string name, UniformType type = UniformType.Custom)
        {
            p.UniformBindings[name] = type;
        }

        public void RemoveInput(ShaderProgram p, string name)
        {
            p.Inputs.Remove(name);
        }

        public void RemoveOutput(ShaderProgram p, string name)
        {
            p.Outputs.Remove(name);
        }

        public void UnbindUniform(ShaderProgram p, string name)
        {
            p.UniformBindings.Remove(name);
        }

        public void FinishProgram(ref ShaderProgram p)
        {
            foreach(var (name, color) in p.Outputs)
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
            
            foreach(var s in p.Shaders) GL.DeleteShader(s);
            p.InputBinder = new ShaderInputBinder(p);
            
            Bind(p);
            foreach (var (name, _) in p.UniformBindings)
                p.UniformLocations[name] = GL.GetUniformLocation(p.Number, name);
        }

        public void SetUniform(ShaderProgram p, string name, float f)
        {
            GL.SetUniform(p.UniformLocations[name], 1, new []{f});
        }

        public void SetUniform(ShaderProgram p, string name, float x, float y)
        {
            GL.SetUniformVector2(p.UniformLocations[name], 1, new []{x, y});
        }

        public void SetUniform(ShaderProgram p, string name, float x, float y, float z)
        {
            GL.SetUniformVector3(p.UniformLocations[name], 1, new []{x, y, z});
        }

        public void SetUniform(ShaderProgram p, string name, float x, float y, float z, float w)
        {
            GL.SetUniformVector4(p.UniformLocations[name], 1, new []{x, y, z, w});
        }

        public void SetUniform(ShaderProgram p, string name, int i)
        {
            GL.SetUniform(p.UniformLocations[name], 1, new []{i});
        }

        public void SetUniform(ShaderProgram p, string name, int x, int y)
        {
            GL.SetUniformVector2(p.UniformLocations[name], 1, new []{x, y});
        }

        public void SetUniform(ShaderProgram p, string name, int x, int y, int z)
        {
            GL.SetUniformVector3(p.UniformLocations[name], 1, new []{x, y, z});
        }

        public void SetUniform(ShaderProgram p, string name, int x, int y, int z, int w)
        {
            GL.SetUniformVector4(p.UniformLocations[name], 1, new []{x, y, z, w});
        }

        public void SetUniform(ShaderProgram p, string name, Vector2 v)
        {
            SetUniform(p, name, v.X, v.Y);
        }

        public void SetUniform(ShaderProgram p, string name, Vector3 v)
        {
            SetUniform(p, name, v.X, v.Y, v.Z);
        }

        public void SetUniform(ShaderProgram p, string name, Vector4 v)
        {
            SetUniform(p, name, v.X, v.Y, v.Z, v.W);
        }

        public void SetUniform(ShaderProgram p, string name, Matrix2 m)
        {
            GL.SetUniformMatrix2(p.UniformLocations[name], 1, false, m.Array);
        }

        public void SetUniform(ShaderProgram p, string name, Matrix3 m)
        {
            GL.SetUniformMatrix3(p.UniformLocations[name], 1, false, m.Array);
        }

        public void SetUniform(ShaderProgram p, string name, Matrix4 m)
        {
            GL.SetUniformMatrix4(p.UniformLocations[name], 1, false, m.Array);
        }

        public void Clear()
        {
            GL.Clear();
        }

        public void SetClearColor(float r, float g, float b)
        {
            GL.ClearColor(r, g, b, 1);
        }

        public void SetWindowSize(Window window, int width, int height)
        {
            Glfw.SetWindowSize(window.GlfwWindow, width, height);
        }

        public void SetWindowTitle(Window window, string title)
        {
            Glfw.SetWindowTitle(window.GlfwWindow, title);
        }

        public (int Width, int Height) GetWindowSize(Window window)
        {
            Glfw.GetWindowSize(window.GlfwWindow, out var w, out var h);
            return (w, h);
        }

        public bool WindowShouldBeOpen(Window window)
        {
            return !Glfw.WindowShouldClose(window.GlfwWindow);
        }
    }
}