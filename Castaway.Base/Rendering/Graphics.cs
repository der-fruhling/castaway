#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Castaway.Math;

namespace Castaway.Rendering
{
    public abstract class Graphics : IDisposable
    {
        protected static ThreadLocal<Window?> CurrentlyBound = new(() => null);
        public Window? Window;

        public List<float> FrameTimes = new();
        public float FrameTime => FrameTimes.Any() ? FrameTimes.Sum() / FrameTimes.Count : 0;
        public float FrameChange => FrameTime / ExpectedFrameTime;
        public float ExpectedFrameTime = 1f / 60f;

        public static Graphics Current => CurrentlyBound.Value!.GL;

        protected internal static void BindWindow(Window window)
        {
            if (CurrentlyBound.Value == window) return;
            window.IBind();
            CurrentlyBound.Value = window;
        }

        protected void BindWindow() => BindWindow(Window!);

        public ShaderObject? BoundShader { get; set; }
        public FramebufferObject? BoundFramebuffer { get; set; }
        public Drawable? BoundDrawable { get; set; }
        public readonly TextureObject?[] BoundTextures = new TextureObject?[32];

        public abstract string Name { get; }

        public readonly Dictionary<BufferTarget, BufferObject?> BoundBuffers = new()
        {
            [BufferTarget.VertexArray] = null,
            [BufferTarget.ElementArray] = null
        };

        public abstract void WindowInit(Window window);

        public abstract BufferObject NewBuffer(BufferTarget target, float[]? data = null);
        public abstract TextureObject NewTexture(int width, int height, Color color);
        public abstract TextureObject NewTexture(int width, int height, float[]? data = null);
        public abstract TextureObject NewTexture(Bitmap bitmap);
        public abstract SeparatedShaderObject NewSepShader(ShaderStage stage, string source);
        public abstract ShaderObject NewShader(params SeparatedShaderObject[] objects);
        public abstract FramebufferObject NewFramebuffer();

        public abstract void UnbindBuffer(BufferTarget target);
        public abstract void UnbindTexture(int number);
        public abstract void UnbindShader();
        public abstract void UnbindFramebuffer();

        public abstract object NativeRepresentation(RenderObject renderObject);

        public abstract void FinishFrame(Window window);
        public abstract void StartFrame();

        public abstract void Draw(ShaderObject @object, Drawable buffer);

        public abstract void SetUniform(ShaderObject p, string name, float f);
        public abstract void SetUniform(ShaderObject p, string name, float x, float y);
        public abstract void SetUniform(ShaderObject p, string name, float x, float y, float z);
        public abstract void SetUniform(ShaderObject p, string name, float x, float y, float z, float w);
        public abstract void SetUniform(ShaderObject p, string name, int i);
        public abstract void SetUniform(ShaderObject p, string name, int x, int y);
        public abstract void SetUniform(ShaderObject p, string name, int x, int y, int z);
        public abstract void SetUniform(ShaderObject p, string name, int x, int y, int z, int w);
        public abstract void SetUniform(ShaderObject p, string name, Vector2 v);
        public abstract void SetUniform(ShaderObject p, string name, Vector3 v);
        public abstract void SetUniform(ShaderObject p, string name, Vector4 v);
        public abstract void SetUniform(ShaderObject p, string name, Matrix2 m);
        public abstract void SetUniform(ShaderObject p, string name, Matrix3 m);
        public abstract void SetUniform(ShaderObject p, string name, Matrix4 m);
        public abstract void SetUniform(ShaderObject p, string name, TextureObject t);
        public abstract void SetUniform(ShaderObject p, string name, FramebufferObject t);

        public virtual void SetUniform(ShaderObject p, UniformType name, float f) => SetUniform(p, p.GetUniform(name)!, f);
        public virtual void SetUniform(ShaderObject p, UniformType name, float x, float y) => SetUniform(p, p.GetUniform(name)!, x, y);
        public virtual void SetUniform(ShaderObject p, UniformType name, float x, float y, float z) => SetUniform(p, p.GetUniform(name)!, x, y, z);
        public virtual void SetUniform(ShaderObject p, UniformType name, float x, float y, float z, float w) => SetUniform(p, p.GetUniform(name)!, x, y, z, w);
        public virtual void SetUniform(ShaderObject p, UniformType name, int i) => SetUniform(p, p.GetUniform(name)!, i);
        public virtual void SetUniform(ShaderObject p, UniformType name, int x, int y) => SetUniform(p, p.GetUniform(name)!, x, y);
        public virtual void SetUniform(ShaderObject p, UniformType name, int x, int y, int z) => SetUniform(p, p.GetUniform(name)!, x, y, z);
        public virtual void SetUniform(ShaderObject p, UniformType name, int x, int y, int z, int w) => SetUniform(p, p.GetUniform(name)!, x, y, z, w);
        public virtual void SetUniform(ShaderObject p, UniformType name, Vector2 v) => SetUniform(p, p.GetUniform(name)!, v);
        public virtual void SetUniform(ShaderObject p, UniformType name, Vector3 v) => SetUniform(p, p.GetUniform(name)!, v);
        public virtual void SetUniform(ShaderObject p, UniformType name, Vector4 v) => SetUniform(p, p.GetUniform(name)!, v);
        public virtual void SetUniform(ShaderObject p, UniformType name, Matrix2 m) => SetUniform(p, p.GetUniform(name)!, m);
        public virtual void SetUniform(ShaderObject p, UniformType name, Matrix3 m) => SetUniform(p, p.GetUniform(name)!, m);
        public virtual void SetUniform(ShaderObject p, UniformType name, Matrix4 m) => SetUniform(p, p.GetUniform(name)!, m);
        public virtual void SetUniform(ShaderObject p, UniformType name, TextureObject t) => SetUniform(p, p.GetUniform(name)!, t);
        public virtual void SetUniform(ShaderObject p, UniformType name, FramebufferObject t) => SetUniform(p, p.GetUniform(name)!, t);

        public virtual void SetUniform(ShaderObject p, int i, UniformType name, float f) => SetUniform(p, p.GetUniform(name, i)!, f);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, float x, float y) => SetUniform(p, p.GetUniform(name, i)!, x, y);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, float x, float y, float z) => SetUniform(p, p.GetUniform(name, i)!, x, y, z);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, float x, float y, float z, float w) => SetUniform(p, p.GetUniform(name, i)!, x, y, z, w);
        public virtual void SetUniform(ShaderObject p, int j, UniformType name, int i) => SetUniform(p, p.GetUniform(name, j)!, i);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, int x, int y) => SetUniform(p, p.GetUniform(name, i)!, x, y);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, int x, int y, int z) => SetUniform(p, p.GetUniform(name, i)!, x, y, z);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, int x, int y, int z, int w) => SetUniform(p, p.GetUniform(name, i)!, x, y, z, w);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, Vector2 v) => SetUniform(p, p.GetUniform(name, i)!, v);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, Vector3 v) => SetUniform(p, p.GetUniform(name, i)!, v);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, Vector4 v) => SetUniform(p, p.GetUniform(name, i)!, v);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, Matrix2 m) => SetUniform(p, p.GetUniform(name, i)!, m);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, Matrix3 m) => SetUniform(p, p.GetUniform(name, i)!, m);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, Matrix4 m) => SetUniform(p, p.GetUniform(name, i)!, m);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, TextureObject t) => SetUniform(p, p.GetUniform(name, i)!, t);
        public virtual void SetUniform(ShaderObject p, int i, UniformType name, FramebufferObject t) => SetUniform(p, p.GetUniform(name, i)!, t);

        public abstract void Clear();
        public abstract void SetClearColor(float r, float g, float b);

        public abstract void Dispose();
    }
}