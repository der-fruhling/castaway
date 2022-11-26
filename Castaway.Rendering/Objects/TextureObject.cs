namespace Castaway.Rendering.Objects;

public abstract class TextureObject : RenderObject, IValidatable
{
	public abstract bool Valid { get; }
	public abstract void Bind(int slot);
	public abstract void Unbind(int slot);
}