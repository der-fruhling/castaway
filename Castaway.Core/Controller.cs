namespace Castaway.Core
{
    public abstract class Controller
    {
        public virtual void OnBegin() {}
        public virtual void OnEnd() {}
        public virtual void OnDraw() {}
        public virtual void OnUpdate() {}
    }
}