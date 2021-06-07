namespace Castaway.Rendering
{
    public interface IGraphicsObject
    {
        public enum ObjectType
        {
            Window,
            Shader,
            ShaderProgram,
            Texture,
            Framebuffer,
            Buffer
        }

        ObjectType Type { get; }
        bool Destroyed { get; set; }
    }
}