#nullable enable
using System;
using System.Collections.Generic;
using Castaway.Rendering;
using GLFW;
using static Castaway.Rendering.DrawBufferConstants;

namespace Castaway.OpenGL
{
    public class GLGraphics : IGraphics
    {
        private IProgram? _currentProgram;
        
        public GLGraphics()
        {
            Glfw.Init();
            GL.Init();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Glfw.Terminate();
        }

        public IWindow CreateWindow(string title, int width, int height)
        {
            var w = new GLWindow();
            w.Open(title, width, height, true);
            w.Use();
            GL.GenerateVertexArrays(1, out var arrays);
            w.Vao = arrays[0];
            GL.BindVertexArray(w.Vao);
            return w;
        }

        public void FinishFrame(IWindow w)
        {
            if (w is not GLWindow o) throw new InvalidOperationException("Not an OpenGL Window.");
            o.FinishFrame();
        }

        public void StartFrame(IWindow w)
        {
            if (w is not GLWindow o) throw new InvalidOperationException("Not an OpenGL Window.");
            GL.BindVertexArray(o.Vao);
        }

        public void Destroy(IBuffer buf)
        {
            if (buf is not GLBuffer o) throw new InvalidOperationException("Not an OpenGL buffer instance.");
            if (!buf.IsValid) throw new InvalidOperationException("Cannot destroy invalid buffer.");
            GL.DeleteBuffers(1, new []{o.Number});
        }

        public void Use(IBuffer buf)
        {
            if (buf is not GLBuffer o) throw new InvalidOperationException("Not an OpenGL buffer instance.");
            if (!o.Validate()) throw new InvalidOperationException("Cannot modify invalid object.");
            GL.BindBuffer(o.Target, o.Number);
        }

        public DrawBuffer CreateDrawBuffer(IBuffer buf, int vertexCount)
        {
            if (buf is not GLBuffer o) throw new InvalidOperationException("Not an OpenGL buffer instance.");
            if (!o.Validate()) throw new InvalidOperationException("Cannot modify invalid object.");
            if (_currentProgram is not {IsValid: true, IsLinked: true}) throw new InvalidOperationException("This operation requires a valid and finished program to be bound.");
            var p = _currentProgram as GLProgram;
            GL.BindBuffer(GL.BufferTarget.ArrayBuffer, o.Number);

            List<(VertexInputType, int, int, int)> bindings = new();
            foreach (var (name, type) in _currentProgram!.VertexInputs)
            {
                var l = GL.GetAttribLocation(p!.Number, name);
                GL.EnableVertexAttrib(l);
                switch (type)
                {
                    case VertexInputType.PositionXY:
                        bindings.Add((type, PositionX, 2, l));
                        break;
                    case VertexInputType.PositionXYZ:
                        bindings.Add((type, PositionX, 3, l));
                        break;
                    case VertexInputType.ColorG:
                        bindings.Add((type, ColorG, 1, l));
                        break;
                    case VertexInputType.ColorRGB:
                        bindings.Add((type, ColorR, 3, l));
                        break;
                    case VertexInputType.ColorRGBA:
                        bindings.Add((type, ColorR, 4, l));
                        break;
                    case VertexInputType.ColorBGRA:
                        bindings.Add((type, ColorR, (int) GLC.GL_BGRA, l));
                        break;
                    case VertexInputType.NormalXY:
                        bindings.Add((type, NormalX, 2, l));
                        break;
                    case VertexInputType.NormalXYZ:
                        bindings.Add((type, NormalX, 3, l));
                        break;
                    case VertexInputType.TextureU:
                        bindings.Add((type, TextureU, 1, l));
                        break;
                    case VertexInputType.TextureUV:
                        bindings.Add((type, TextureU, 2, l));
                        break;
                    case VertexInputType.TextureUVT:
                        bindings.Add((type, TextureU, 3, l));
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid vertex input type {type} ({(long) type})");
                }
            }

            return new DrawBuffer(buf, bindings.ToArray(), vertexCount);
        }

        public void Draw(DrawBuffer d)
        {
            if (d.Buffer is not GLBuffer o) throw new InvalidOperationException("Not an OpenGL draw buffer instance.");
            if (!o.Validate()) throw new InvalidOperationException("Cannot draw invalid object.");
            
            GL.BindBuffer(GL.BufferTarget.ArrayBuffer, o.Number);
            foreach (var (_, index, size, attr) in d.Bindings)
            {
                GL.VertexAttribPointer(attr, size, GLC.GL_FLOAT, false, Size * sizeof(float), index * sizeof(float));
            }
            GL.DrawArrays(GLC.GL_TRIANGLES, 0, d.VertexCount);
        }

        public IShader CreateShader(ShaderStage stage, string source)
        {
            var n = GL.CreateShader(stage switch
            {
                ShaderStage.Vertex => GL.ShaderStage.VertexShader,
                ShaderStage.Fragment => GL.ShaderStage.FragmentShader,
                _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
            });
            GL.ShaderSource(n, source);
            GL.CompileShader(n);
            if (GL.GetShader(n, GL.ShaderQuery.CompileStatus) == 0)
            {
                // TODO Add compile log.
                throw new GraphicsException($"{stage} shader failed to compile.");
            }

            return new GLShader(n);
        }

        public void Destroy(IShader shader)
        {
            if (shader is not GLShader o) throw new InvalidOperationException("Object is not an OpenGL shader instance.");
            if (!shader.IsValid) throw new InvalidOperationException("Cannot destroy invalid object.");
            GL.DeleteShader(o.Number);
        }

        public IProgram CreateProgram(params IShader[] shaders)
        {
            foreach (var s in shaders)
            {
                if (s is not GLShader) throw new InvalidOperationException($"Object {s} is not an OpenGL shader instance.");
                if (!s.IsValid) throw new InvalidOperationException("Cannot attach invalid shader.");
            }
            var n = GL.CreateProgram();
            foreach (var s in shaders) GL.AttachShader(n, (s as GLShader)!.Number);
            return new GLProgram(n) { Shaders = shaders };
        }

        public void Destroy(IProgram program)
        {
            throw new NotImplementedException();
        }

        public void Use(IProgram program)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot use an invalid program.");
            
            GL.UseProgram(o.Number);
            _currentProgram = o;
        }

        public void CreateInput(IProgram program, VertexInputType inputType, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot create an input on an invalid program.");

            o.VertexInputsI[name] = inputType;
        }

        public void CreateOutput(IProgram program, uint color, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot create an output on an invalid program.");

            o.FragmentOutputsI[name] = color;
        }

        public void CreateUniform(IProgram program, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot create a uniform on an invalid program.");

            o.UniformsI[name] = -1;
        }

        public void RemoveInput(IProgram program, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot delete an input on an invalid program.");

            o.VertexInputsI.Remove(name);
        }

        public void RemoveOutput(IProgram program, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot delete an output on an invalid program.");

            o.FragmentOutputsI.Remove(name);
        }

        public void RemoveUniform(IProgram program, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot delete a uniform on an invalid program.");

            o.UniformsI.Remove(name);
        }

        public void FinishProgram(IProgram program)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot finish invalid program.");

            foreach (var (n, c) in program.FragmentOutputs) 
                GL.BindFragDataLocation(o.Number, c, n!);

            GL.LinkProgram(o.Number);
            if (GL.GetProgram(o.Number, GL.ProgramQuery.LinkStatus) == 0)
            {
                // TODO Add link log.
                throw new GraphicsException($"Program (with {program.Shaders.Length} failed to link.");
            }

            o.IsLinked = true;
            Use(o);
        }

        public void Clear()
        {
            GL.Clear();
        }

        public void SetClearColor(float r, float g, float b)
        {
            GL.ClearColor(r, g, b, 1);
        }

        public IBuffer CreateBuffer(BufferTarget target)
        {
            return new GLBuffer(target switch
            {
                BufferTarget.VertexArray => GL.BufferTarget.ArrayBuffer,
                BufferTarget.ElementArray => GL.BufferTarget.ElementArrayBuffer,
                _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
            }, GL.CreateBuffer());
        }
    }
}