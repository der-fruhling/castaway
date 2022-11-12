namespace Castaway.Rendering.Objects;

public abstract class FramebufferObject : RenderObject, IValidatable, IBindable
{
    public TextureObject? Color;
    public TextureObject? Depth;
    public TextureObject? Stencil;
    public abstract void Bind();
    public abstract void Unbind();
    public abstract bool Valid { get; }
}