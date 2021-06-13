#nullable enable
using System;
using System.Drawing;
using Castaway.Assets;
using Castaway.Math;

namespace Castaway.Rendering
{
    public abstract class Graphics<
        TWindow,
        TBuffer,
        TShader,
        TProgram,
        TTexture,
        TFramebuffer,
        TDrawable
    > : IDisposable
    {
        public abstract TWindow CreateWindowWindowed(string title, int width, int height, bool visible = true);
        public abstract TWindow CreateWindowFullscreen(string title, bool visible = true);
        public abstract TBuffer CreateBuffer(BufferTarget target);
        public abstract TShader CreateShader(ShaderStage stage, string source);
        public abstract TShader CreateShader(ShaderStage stage, Asset source);
        public abstract TProgram CreateProgram(params TShader[] shaders);
        public abstract TTexture CreateTexture(Bitmap image);
        public abstract TTexture CreateTexture(Asset image);
        public abstract TFramebuffer CreateFramebuffer(TWindow window);

        public abstract void Destroy(params TWindow[] windows);
        public abstract void Destroy(params TBuffer[] buffers);
        public abstract void Destroy(params TShader[] shaders);
        public abstract void Destroy(params TProgram[] programs);
        public abstract void Destroy(params TTexture[] textures);
        public abstract void Destroy(params TFramebuffer[] framebuffers);

        public abstract void Bind(TWindow window);
        public abstract void Bind(TBuffer buffer);
        public abstract void Bind(TProgram program);
        public abstract void Bind(TTexture texture, int number);
        public abstract void Bind(TFramebuffer framebuffer);
        public abstract void UnbindFramebuffer();

        public abstract void FinishFrame(TWindow window);
        public abstract void StartFrame();

        public abstract void Upload<T>(TBuffer buffer, T[] data) where T : unmanaged;
        public abstract void Draw(TProgram program, TDrawable buffer);

        public abstract void CreateInput(TProgram p, VertexInputType inputType, string name);
        public abstract void CreateOutput(TProgram p, uint color, string name);
        public abstract void BindUniform(TProgram p, string name, UniformType type = UniformType.Custom);

        public abstract void FinishProgram(ref TProgram p);
        public abstract string UniformRef(TProgram p, UniformType type);

        public virtual string UniformRef(TProgram p, UniformType type, int index) =>
            UniformRef(p, type).Replace("$INDEX", index.ToString());

        public abstract void SetUniform(TProgram p, string name, float f);
        public abstract void SetUniform(TProgram p, string name, float x, float y);
        public abstract void SetUniform(TProgram p, string name, float x, float y, float z);
        public abstract void SetUniform(TProgram p, string name, float x, float y, float z, float w);
        public abstract void SetUniform(TProgram p, string name, int i);
        public abstract void SetUniform(TProgram p, string name, int x, int y);
        public abstract void SetUniform(TProgram p, string name, int x, int y, int z);
        public abstract void SetUniform(TProgram p, string name, int x, int y, int z, int w);
        public abstract void SetUniform(TProgram p, string name, Vector2 v);
        public abstract void SetUniform(TProgram p, string name, Vector3 v);
        public abstract void SetUniform(TProgram p, string name, Vector4 v);
        public abstract void SetUniform(TProgram p, string name, Matrix2 m);
        public abstract void SetUniform(TProgram p, string name, Matrix3 m);
        public abstract void SetUniform(TProgram p, string name, Matrix4 m);
        public abstract void SetUniform(TProgram p, string name, TTexture t);
        public abstract void SetUniform(TProgram p, string name, TFramebuffer t);

        public virtual void SetUniform(TProgram p, UniformType name, float f) => SetUniform(p, UniformRef(p, name), f);
        public virtual void SetUniform(TProgram p, UniformType name, float x, float y) => SetUniform(p, UniformRef(p, name), x, y);
        public virtual void SetUniform(TProgram p, UniformType name, float x, float y, float z) => SetUniform(p, UniformRef(p, name), x, y, z);
        public virtual void SetUniform(TProgram p, UniformType name, float x, float y, float z, float w) => SetUniform(p, UniformRef(p, name), x, y, z, w);
        public virtual void SetUniform(TProgram p, UniformType name, int i) => SetUniform(p, UniformRef(p, name), i);
        public virtual void SetUniform(TProgram p, UniformType name, int x, int y) => SetUniform(p, UniformRef(p, name), x, y);
        public virtual void SetUniform(TProgram p, UniformType name, int x, int y, int z) => SetUniform(p, UniformRef(p, name), x, y, z);
        public virtual void SetUniform(TProgram p, UniformType name, int x, int y, int z, int w) => SetUniform(p, UniformRef(p, name), x, y, z, w);
        public virtual void SetUniform(TProgram p, UniformType name, Vector2 v) => SetUniform(p, UniformRef(p, name), v);
        public virtual void SetUniform(TProgram p, UniformType name, Vector3 v) => SetUniform(p, UniformRef(p, name), v);
        public virtual void SetUniform(TProgram p, UniformType name, Vector4 v) => SetUniform(p, UniformRef(p, name), v);
        public virtual void SetUniform(TProgram p, UniformType name, Matrix2 m) => SetUniform(p, UniformRef(p, name), m);
        public virtual void SetUniform(TProgram p, UniformType name, Matrix3 m) => SetUniform(p, UniformRef(p, name), m);
        public virtual void SetUniform(TProgram p, UniformType name, Matrix4 m) => SetUniform(p, UniformRef(p, name), m);
        public virtual void SetUniform(TProgram p, UniformType name, TTexture t) => SetUniform(p, UniformRef(p, name), t);
        public virtual void SetUniform(TProgram p, UniformType name, TFramebuffer t) => SetUniform(p, UniformRef(p, name), t);

        public virtual void SetUniform(TProgram p, int i, UniformType name, float f) => SetUniform(p, UniformRef(p, name, i), f);
        public virtual void SetUniform(TProgram p, int i, UniformType name, float x, float y) => SetUniform(p, UniformRef(p, name, i), x, y);
        public virtual void SetUniform(TProgram p, int i, UniformType name, float x, float y, float z) => SetUniform(p, UniformRef(p, name, i), x, y, z);
        public virtual void SetUniform(TProgram p, int i, UniformType name, float x, float y, float z, float w) => SetUniform(p, UniformRef(p, name, i), x, y, z, w);
        public virtual void SetUniform(TProgram p, int j, UniformType name, int i) => SetUniform(p, UniformRef(p, name, j), i);
        public virtual void SetUniform(TProgram p, int i, UniformType name, int x, int y) => SetUniform(p, UniformRef(p, name, i), x, y);
        public virtual void SetUniform(TProgram p, int i, UniformType name, int x, int y, int z) => SetUniform(p, UniformRef(p, name, i), x, y, z);
        public virtual void SetUniform(TProgram p, int i, UniformType name, int x, int y, int z, int w) => SetUniform(p, UniformRef(p, name, i), x, y, z, w);
        public virtual void SetUniform(TProgram p, int i, UniformType name, Vector2 v) => SetUniform(p, UniformRef(p, name, i), v);
        public virtual void SetUniform(TProgram p, int i, UniformType name, Vector3 v) => SetUniform(p, UniformRef(p, name, i), v);
        public virtual void SetUniform(TProgram p, int i, UniformType name, Vector4 v) => SetUniform(p, UniformRef(p, name, i), v);
        public virtual void SetUniform(TProgram p, int i, UniformType name, Matrix2 m) => SetUniform(p, UniformRef(p, name, i), m);
        public virtual void SetUniform(TProgram p, int i, UniformType name, Matrix3 m) => SetUniform(p, UniformRef(p, name, i), m);
        public virtual void SetUniform(TProgram p, int i, UniformType name, Matrix4 m) => SetUniform(p, UniformRef(p, name, i), m);
        public virtual void SetUniform(TProgram p, int i, UniformType name, TTexture t) => SetUniform(p, UniformRef(p, name, i), t);
        public virtual void SetUniform(TProgram p, int i, UniformType name, TFramebuffer t) => SetUniform(p, UniformRef(p, name, i), t);

        public abstract void Clear();
        public abstract void SetClearColor(float r, float g, float b);

        public abstract void SetWindowSize(TWindow window, int width, int height);
        public abstract void SetWindowTitle(TWindow window, string title);
        public abstract (int Width, int Height) GetWindowSize(TWindow window);
        public abstract bool WindowShouldBeOpen(TWindow window);
        public abstract void ShowWindow(TWindow window);
        public abstract void HideWindow(TWindow window);

        [Obsolete("Use specific destroy methods.")]
        public abstract void Destroy(params object[] things);
        [Obsolete("Use specific bind methods.")]
        public abstract void Bind(params object[] things);
        
        public abstract void Dispose();
    }
}