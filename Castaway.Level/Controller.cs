namespace Castaway.Level
{
    public abstract class Controller
    {
        // ReSharper disable InconsistentNaming
        protected ObjectRef<LevelObject> parent;

        protected LevelObject parentObject
        {
            get => parent.Object;
            set => parent.Object = value;
        }
        // ReSharper restore InconsistentNaming

        public virtual void SetParent(ObjectRef<LevelObject> p)
        {
            parent = p;
        }
        
        public virtual void OnBegin() {}
        public virtual void OnEnd() {}
        public virtual void OnDraw() {}
        public virtual void OnUpdate() {}
    }
}