using Castaway.Rendering.Objects;

namespace Castaway.Rendering;

public interface IValidatable
{
    public string Name => "";
    public bool Valid { get; }

    public void CheckValid()
    {
        if (!Valid) throw new RenderObjectInvalidException(GetType(), Name);
    }
}