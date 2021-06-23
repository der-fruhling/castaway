using System;

namespace Castaway.Rendering
{
    public interface IValidatable
    {
        public string Name => "";
        public bool Valid { get; }

        public void CheckValid()
        {
            if (!Valid) throw new RenderObjectInvalidException(GetType(), Name);
        }
    }

    public interface IBindable
    {
        public void Bind();
        public void Unbind();

        public void Bound(Action a)
        {
            Bind();
            a();
            Unbind();
        }
    }
}