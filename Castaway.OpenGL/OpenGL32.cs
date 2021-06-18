using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using Castaway.Math;
using Castaway.OpenGL.Input;
using Castaway.OpenGL.Native;
using Castaway.Rendering;
using GLFW;
using Graphics = Castaway.Rendering.Graphics;
using Window = Castaway.Rendering.Window;

namespace Castaway.OpenGL
{
    [Implements("OpenGL-3.2")]
    public class OpenGL32 : Graphics
    {
        public OpenGL32()
        {
            GL.Init();
            Glfw.Init();
        }

        ~OpenGL32() => DisposeGL();

        private void DisposeGL()
        {
            Glfw.Terminate();
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeGL();
        }

        public override string Name => "OpenGL-3.2";

        public override void WindowInit(Window window)
        {
            BuiltinShaders.Init();
            InputSystem.Init();
            GL.Enable(GLC.GL_DEPTH_TEST);
            GL.Enable(GLC.GL_CULL_FACE);
        }

        public override BufferObject NewBuffer(BufferTarget target, float[]? data = null) => new Buffer(target, data ?? Array.Empty<float>());

        public override TextureObject NewTexture(int width, int height, Color color)
        {
            var data = new float[width * height * 3];
            var r = color.R / (float) byte.MaxValue;
            var g = color.G / (float) byte.MaxValue;
            var b = color.B / (float) byte.MaxValue;
            
            for (int i = 0; i < width * height * 3; i += 3)
            {
                data[i + 0] = r;
                data[i + 1] = g;
                data[i + 2] = b;
            }

            return new Texture(width, height, data);
        }

        public override TextureObject NewTexture(int width, int height, float[]? data = null)
        {
            return new Texture(width, height, data);
        }

        public override TextureObject NewTexture(Bitmap bitmap)
        {
            var data = new float[bitmap.Width * bitmap.Height * 3];

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    var k = i * j * 3;
                    var p = bitmap.GetPixel(i, j);
                    data[k + 0] = p.R / (float) byte.MaxValue;
                    data[k + 1] = p.G / (float) byte.MaxValue;
                    data[k + 2] = p.B / (float) byte.MaxValue;
                }
            }

            return new Texture(bitmap.Width, bitmap.Height, data);
        }

        public override SeparatedShaderObject NewSepShader(ShaderStage stage, string source) => new ShaderPart(stage, source, "???");

        public override ShaderObject NewShader(params SeparatedShaderObject[] objects) => new Shader(objects);

        public override FramebufferObject NewFramebuffer() => new Framebuffer();

        public override object NativeRepresentation(RenderObject renderObject)
        {
            dynamic d = renderObject;
            try
            {
                return d.Number is uint u ? u : uint.MaxValue;
            }
            catch (MissingMemberException e)
            {
                if (renderObject is not Buffer or Texture or ShaderPart or Shader or Framebuffer)
                    throw new InvalidOperationException($"{renderObject.GetType().Name} is not an OpenGL type.", e);
                throw;
            }
        }
        
        private readonly Stopwatch _stopwatch = new();

        public override void FinishFrame(Window window)
        {
            window.SwapBuffers();
            _stopwatch.Stop();
            FrameTimes.Insert(0, (float) _stopwatch.Elapsed.TotalSeconds);
            const int MaxTimes = 60;
            if (FrameTimes.Count > MaxTimes) FrameTimes.RemoveRange(MaxTimes, FrameTimes.Count - MaxTimes);
        }
        
        /// <summary>
        /// Should be called at the start of the frame.
        /// </summary>
        public override void StartFrame()
        {
            _stopwatch.Restart();
            Clear();
            Glfw.PollEvents();
            if(InputSystem.Gamepad.Valid) InputSystem.Gamepad.Read();
        }

        public override void Draw(ShaderObject shader, Drawable buffer)
        {
            if (buffer.VertexArray == null) throw new InvalidOperationException("Drawables must have a vertex array.");
            if (buffer.VertexArray is not Buffer v)
                throw new InvalidOperationException($"Cannot use vertex buffer of type {buffer.VertexArray?.GetType()}");
            if (shader is not Shader s)
                throw new InvalidOperationException($"Cannot use shader of type {shader.GetType().FullName}");

            if (buffer is VertexArrayDrawable vaoDraw)
            {
                BindVAO(vaoDraw.VAO);
                if (!vaoDraw.SetUp)
                {
                    s.Binder!.Apply(v);
                    vaoDraw.SetUp = true;
                }
                if (vaoDraw.ElementArray != null) 
                    GL.DrawElements(GLC.GL_TRIANGLES, buffer.VertexCount, GLC.GL_UNSIGNED_INT, 0);
                else
                    GL.DrawArrays(GLC.GL_TRIANGLES, 0, buffer.VertexCount);
                UnbindVAO();
            }
            else
            {
                var vao = CreateVAO();
                BindVAO(vao);
                buffer.VertexArray.Bind();
                s.Binder!.Apply(v);
                if (buffer.ElementArray != null)
                {
                    if (buffer.ElementArray is not Buffer)
                        throw new InvalidOperationException($"Cannot use element buffer of type {buffer.ElementArray?.GetType()}");
                    buffer.ElementArray.Bind();
                    GL.DrawElements(GLC.GL_TRIANGLES, buffer.VertexCount, GLC.GL_UNSIGNED_INT, 0);
                }
                else
                {
                    GL.DrawArrays(GLC.GL_TRIANGLES, 0, buffer.VertexCount);
                }
                DeleteVAOs(vao);
            }
        }

        public override void SetUniform(ShaderObject p, string name, float f)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniform(GL.GetUniformLocation(s.Number, name), 1, new []{f});
        }

        public override void SetUniform(ShaderObject p, string name, float x, float y)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector2(GL.GetUniformLocation(s.Number, name), 1, new []{x, y});
        }

        public override void SetUniform(ShaderObject p, string name, float x, float y, float z)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector3(GL.GetUniformLocation(s.Number, name), 1, new []{x, y, z});
        }

        public override void SetUniform(ShaderObject p, string name, float x, float y, float z, float w)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector4(GL.GetUniformLocation(s.Number, name), 1, new []{x, y, z, w});
        }

        public override void SetUniform(ShaderObject p, string name, int i)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniform(GL.GetUniformLocation(s.Number, name), 1, new []{i});
        }

        public override void SetUniform(ShaderObject p, string name, int x, int y)
        {
            
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector2(GL.GetUniformLocation(s.Number, name), 1, new []{x, y});
        }

        public override void SetUniform(ShaderObject p, string name, int x, int y, int z)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector3(GL.GetUniformLocation(s.Number, name), 1, new []{x, y, z});
        }

        public override void SetUniform(ShaderObject p, string name, int x, int y, int z, int w)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformVector4(GL.GetUniformLocation(s.Number, name), 1, new []{x, y, z, w});
        }

        public override void SetUniform(ShaderObject p, string name, Vector2 v)
        {
            SetUniform(p, name, v.X, v.Y);
        }

        public override void SetUniform(ShaderObject p, string name, Vector3 v)
        {
           SetUniform(p, name, v.X, v.Y, v.Z);
        }

        public override void SetUniform(ShaderObject p, string name, Vector4 v)
        {
            SetUniform(p, name, v.X, v.Y, v.Z, v.W);
        }

        public override void SetUniform(ShaderObject p, string name, Matrix2 m)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformMatrix2(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
        }

        public override void SetUniform(ShaderObject p, string name, Matrix3 m)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformMatrix3(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
        }

        public override void SetUniform(ShaderObject p, string name, Matrix4 m)
        {
            if (p is not Shader s)
                throw new InvalidOperationException($"Need OpenGL object types only, not {p.GetType()}");
            GL.SetUniformMatrix4(GL.GetUniformLocation(s.Number, name), 1, false, m.Array);
        }

        public override void SetUniform(ShaderObject p, string name, TextureObject t)
        {
            if (t is not Texture tex) throw new InvalidOperationException("Need to use OpenGL objects only");
            SetUniform(p, name, tex.Number);
        }

        public override void SetUniform(ShaderObject p, string name, FramebufferObject t)
        {
            if (t.Color is not Texture tex) throw new InvalidOperationException("Need to use OpenGL objects only");
            SetUniform(p, name, tex.Number);
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

        internal virtual uint[] NewBuffers(int count)
        {
            GL.GenBuffers(count, out var a);
            return a;
        }

        internal virtual uint[] NewTextures(int count)
        {
            GL.GenTextures(count, out var a);
            return a;
        }

        internal virtual uint NewShader(ShaderStage stage) =>
            GL.CreateShader(stage switch
            {
                ShaderStage.Vertex => GL.ShaderStage.VertexShader,
                ShaderStage.Fragment => GL.ShaderStage.FragmentShader,
                _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
            });

        internal virtual uint NewProgram() => GL.CreateProgram();

        internal virtual void BindBuffer(BufferTarget target, uint b) => GL.BindBuffer(target switch
        {
            BufferTarget.VertexArray => GL.BufferTarget.ArrayBuffer,
            BufferTarget.ElementArray => GL.BufferTarget.ElementArrayBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        }, b);

        public override void UnbindBuffer(BufferTarget target) => BindBuffer(target, 0);

        internal virtual void MakeActiveTexture([Range(0, 31)] int number)
        {
            GL.ActiveTexture(GLC.GL_TEXTURE0 + number);
        }

        internal virtual void BindTexture(uint t)
        {
            GL.BindTexture(GLC.GL_TEXTURE_2D, t);
        }

        internal virtual void UnbindTexture()
        {
            GL.BindTexture(GLC.GL_TEXTURE_2D, 0);
        }
        
        internal virtual void BindTexture(int number, uint t)
        {
            MakeActiveTexture(number);
            BindTexture(t);
        }

        public override void UnbindTexture(int number)
        {
            MakeActiveTexture(number);
            UnbindTexture();
        }

        internal virtual void BindShader(uint p)
        {
            GL.UseProgram(p);
        }

        internal virtual void BindFramebuffer(uint number)
        {
            GL.BindFramebuffer(GLC.GL_FRAMEBUFFER, number);
        }

        public override void UnbindFramebuffer()
        {
            GL.BindFramebuffer(GLC.GL_FRAMEBUFFER, 0);
        }

        public override void UnbindShader()
        {
            GL.UseProgram(0);
        }

        internal virtual uint CreateVAO()
        {
            GL.GenVertexArrays(1, out var a);
            return a[0];
        }

        internal virtual uint[] CreateVAOs(int count)
        {
            GL.GenVertexArrays(count, out var a);
            return a;
        }

        internal virtual void BindVAO(uint n) => GL.BindVertexArray(n);
        internal virtual void UnbindVAO() => GL.BindVertexArray(0);
        internal virtual void DeleteVAOs(params uint[] vaos) => GL.DeleteVertexArrays(vaos);
    }
}