#nullable enable
using System;
using System.Drawing;
using Castaway.Assets;
using Castaway.Math;

namespace Castaway.Rendering
{
    public interface IGraphics<
        TWindow,
        TBuffer,
        TShader,
        TProgram,
        TTexture,
        TFramebuffer
    > : IDisposable
    {
        TWindow CreateWindowWindowed(string title, int width, int height);
        TWindow CreateWindowFullscreen(string title);
        TBuffer CreateBuffer(BufferTarget target);
        TShader CreateShader(ShaderStage stage, string source);
        TShader CreateShader(ShaderStage stage, Asset source);
        TProgram CreateProgram(params TShader[] shaders);
        TTexture CreateTexture(Bitmap image);
        TTexture CreateTexture(Asset image);
        TFramebuffer CreateFramebuffer(TWindow window);

        void Destroy(params TWindow[] windows);
        void Destroy(params TBuffer[] buffers);
        void Destroy(params TShader[] shaders);
        void Destroy(params TProgram[] programs);
        void Destroy(params TTexture[] textures);
        void Destroy(params TFramebuffer[] framebuffers);

        void Bind(TWindow window);
        void Bind(TBuffer buffer);
        void Bind(TProgram program);
        void Bind(TTexture texture);
        void Bind(TFramebuffer framebuffer);
        void UnbindFramebuffer();

        void FinishFrame(TWindow window);
        void StartFrame(TWindow window);

        void Upload(TBuffer buffer, float[] data);
        void Draw(TProgram program, TBuffer buffer, int vertexCount);

        void CreateInput(TProgram p, VertexInputType inputType, string name);
        void CreateOutput(TProgram p, uint color, string name);
        void BindUniform(TProgram p, string name, UniformType type);
        [Obsolete("Removing inputs, outputs, and uniforms will be removed in the future.")]
        void RemoveInput(TProgram p, string name);
        [Obsolete("Removing inputs, outputs, and uniforms will be removed in the future.")]
        void RemoveOutput(TProgram p, string name);
        [Obsolete("Removing inputs, outputs, and uniforms will be removed in the future.")]
        void UnbindUniform(TProgram p, string name);
        void FinishProgram(ref TProgram p);

        void SetUniform(TProgram p, string name, float f);
        void SetUniform(TProgram p, string name, float x, float y);
        void SetUniform(TProgram p, string name, float x, float y, float z);
        void SetUniform(TProgram p, string name, float x, float y, float z, float w);
        void SetUniform(TProgram p, string name, int i);
        void SetUniform(TProgram p, string name, int x, int y);
        void SetUniform(TProgram p, string name, int x, int y, int z);
        void SetUniform(TProgram p, string name, int x, int y, int z, int w);
        void SetUniform(TProgram p, string name, Vector2 v);
        void SetUniform(TProgram p, string name, Vector3 v);
        void SetUniform(TProgram p, string name, Vector4 v);
        void SetUniform(TProgram p, string name, Matrix2 m);
        void SetUniform(TProgram p, string name, Matrix3 m);
        void SetUniform(TProgram p, string name, Matrix4 m);

        void Clear();
        void SetClearColor(float r, float g, float b);

        void SetWindowSize(TWindow window, int width, int height);
        void SetWindowTitle(TWindow window, string title);
        (int Width, int Height) GetWindowSize(TWindow window);
        bool WindowShouldBeOpen(TWindow window);

        void Destroy(params object[] things);
        void Bind(params object[] things);
    }
}