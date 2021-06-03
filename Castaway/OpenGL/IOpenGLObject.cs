using Castaway.Rendering;

namespace Castaway.OpenGL
{
    public interface IOpenGLObject : IGraphicsObject
    {
        uint Number { get; set; }
    }
}