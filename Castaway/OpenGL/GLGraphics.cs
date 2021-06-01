#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Math;
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
            GL.GetShaderInfoLog(n, out _, out var log);
            if (log.Any()) Console.Error.WriteLine($"{stage} shader log:\n{log}");
            if (GL.GetShader(n, GL.ShaderQuery.CompileStatus) != 0) return new GLShader(n);
            Console.Error.Flush();
            throw new GraphicsException($"{stage} shader failed to compile. Check above log.");

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
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot destroy an invalid program.");

            GL.DeleteProgram(o.Number);
            foreach(var s in o.Shaders)
                if(s is GLShader {IsValid: true} os)
                    GL.DeleteShader(os.Number);
        }

        public void Use(IProgram program)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot use an invalid program.");
            
            GL.UseProgram(o.Number);
            _currentProgram = o;
            o.MarkDirty();
        }

        public void CreateInput(IProgram program, VertexInputType inputType, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot create an input on an invalid program.");

            o.VertexInputsI[name] = inputType;
            o.MarkDirty();
        }

        public void CreateOutput(IProgram program, uint color, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot create an output on an invalid program.");

            o.FragmentOutputsI[name] = color;
            o.MarkDirty();
        }

        public void CreateUniform(IProgram program, string name, UniformType type)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot create a uniform on an invalid program.");

            o.UniformsI[name] = type;
            o.MarkDirty();
        }

        public void RemoveInput(IProgram program, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot delete an input on an invalid program.");

            o.VertexInputsI.Remove(name);
            o.MarkDirty();
        }

        public void RemoveOutput(IProgram program, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot delete an output on an invalid program.");

            o.FragmentOutputsI.Remove(name);
            o.MarkDirty();
        }

        public void RemoveUniform(IProgram program, string name)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot delete a uniform on an invalid program.");

            o.UniformsI.Remove(name);
            o.MarkDirty();
        }

        public void FinishProgram(IProgram program)
        {
            if (program is not GLProgram o) throw new InvalidOperationException("Object is not an OpenGL program instance.");
            if (!program.IsValid) throw new InvalidOperationException("Cannot finish invalid program.");

            foreach (var (n, c) in program.FragmentOutputs) 
                GL.BindFragDataLocation(o.Number, c, n!);

            GL.LinkProgram(o.Number);
            GL.GetProgramInfoLog(o.Number, out _, out var log);
            if (log.Any()) Console.Error.WriteLine($"Program link log:\n{log}");
            if (GL.GetProgram(o.Number, GL.ProgramQuery.LinkStatus) == 0)
                throw new GraphicsException($"Program (with {program.Shaders.Length} failed to link. Check above log.");

            o.IsLinked = true;
            Use(o);
            o.MarkDirty();
        }

        public void SetUniform(string name, object @object)
        {
            SetUniform(name, new[] {@object});
        }

        public void SetUniform(string name, object[] objects)
        {
            if (_currentProgram is not GLProgram o) throw new InvalidOperationException("Current program is not an OpenGL program instance.");
            if (!_currentProgram.IsValid) throw new InvalidOperationException("Cannot modify invalid program.");

            var loc = o.UniformLocations.ContainsKey(name)
                ? o.UniformLocations[name]
                : o.UniformLocations[name] = GL.GetUniformLocation(o.Number, name);
            
            switch (objects[0])
            {
                case float:
                    GL.SetUniform(loc, objects.Length, objects.Cast<float>().ToArray());
                    break;
                case int:
                    GL.SetUniform(loc, objects.Length, objects.Cast<int>().ToArray());
                    break;
                case double:
                    GL.SetUniform(loc, objects.Length, objects.Cast<double>().ToArray());
                    break;
                case Vector2:
                    GL.SetUniformVector2(loc, objects.Length, objects
                        .Cast<Vector2>()
                        .SelectMany(v => new[] {v.X, v.Y})
                        .ToArray());
                    break;
                case Vector3:
                    GL.SetUniformVector3(loc, objects.Length, objects
                        .Cast<Vector3>()
                        .SelectMany(v => new[] {v.X, v.Y, v.Z})
                        .ToArray());
                    break;
                case Vector4:
                    GL.SetUniformVector4(loc, objects.Length, objects
                        .Cast<Vector4>()
                        .SelectMany(v => new[] {v.X, v.Y, v.Z, v.W})
                        .ToArray());
                    break;
                case Matrix2:
                    GL.SetUniformMatrix2(loc, objects.Length, false, objects
                        .Cast<Matrix2>()
                        .SelectMany(m => new[] {m.X, m.Y})
                        .SelectMany(v => new[] {v.X, v.Y})
                        .ToArray());
                    break;
                case Matrix3:
                    GL.SetUniformMatrix3(loc, objects.Length, false, objects
                        .Cast<Matrix3>()
                        .SelectMany(m => new[] {m.X, m.Y, m.Z})
                        .SelectMany(v => new[] {v.X, v.Y, v.Z})
                        .ToArray());
                    break;
                case Matrix4:
                    GL.SetUniformMatrix4(loc, objects.Length, false, objects
                        .Cast<Matrix4>()
                        .SelectMany(m => new[] {m.X, m.Y, m.Z, m.W})
                        .SelectMany(v => new[] {v.X, v.Y, v.Z, v.W})
                        .ToArray());
                    break;
            }
            
            o.MarkDirty();
        }

        public void SetUniform(UniformType name, object @object)
        {
            SetUniform(name, new[] {@object});
        }

        public void SetUniform(UniformType name, object[] objects)
        {
            if (_currentProgram is not GLProgram o) throw new InvalidOperationException("Current program is not an OpenGL program instance.");
            if (!_currentProgram.IsValid) throw new InvalidOperationException("Cannot modify invalid program.");
            
            foreach (var (k, v) in o.Uniforms)
            {
                if (name != v) continue;
                SetUniform(k!, objects);
                return;
            }
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