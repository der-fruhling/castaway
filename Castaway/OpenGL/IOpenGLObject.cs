namespace Castaway.OpenGL
{
    public enum OpenGLType
    {
        Buffer,
        Shader,
        ShaderProgram,
    }
    
    public interface IOpenGLObject
    {
        uint Number { get; set; }
        OpenGLType Type { get; }

        bool Validate();
        void MarkDirty();
    }
}