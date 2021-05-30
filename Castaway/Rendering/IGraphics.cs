#nullable enable
using System;

namespace Castaway.Rendering
{
    public interface IGraphics : IDisposable
    {
        IWindow CreateWindow(string title, int width, int height);
        void FinishFrame(IWindow w);
        void StartFrame(IWindow w);

        IBuffer CreateBuffer(BufferTarget target);
        void Destroy(IBuffer buf);
        void Use(IBuffer buf);
        DrawBuffer CreateDrawBuffer(IBuffer buf, int vertexCount);
        void Draw(DrawBuffer drawBuffer);
        
        IShader CreateShader(ShaderStage stage, string source);
        void Destroy(IShader shader);
        IProgram CreateProgram(params IShader[] shaders);
        void Destroy(IProgram program);
        void Use(IProgram program);
        
        void CreateInput(IProgram program, VertexInputType inputType, string name);
        void CreateOutput(IProgram program, uint color, string name);
        void CreateUniform(IProgram program, string name, UniformType type);
        void RemoveInput(IProgram program, string name);
        void RemoveOutput(IProgram program, string name);
        void RemoveUniform(IProgram program, string name);
        void FinishProgram(IProgram program);

        void SetUniform(string name, object @object);
        void SetUniform(string name, object[] objects);
        void SetUniform(UniformType name, object @object);
        void SetUniform(UniformType name, object[] objects);
        
        void Clear();
        void SetClearColor(float r, float g, float b);
    }
}
