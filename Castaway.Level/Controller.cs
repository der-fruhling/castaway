namespace Castaway.Level
{
    /// <summary>
    /// A controller that can be attached to a <see cref="LevelObject"/>.
    /// </summary>
    public abstract class Controller
    {
        protected ObjectRef<LevelObject> Parent;

        protected LevelObject ParentObject
        {
            get => Parent.Object;
            set => Parent.Object = value;
        }

        /// <summary>
        /// Sets the parent of this controller.
        /// </summary>
        /// <param name="p">A reference to the new parent object.</param>
        public virtual void SetParent(ObjectRef<LevelObject> p)
        {
            Parent = p;
        }
        
        public virtual void OnBegin() {}
        public virtual void OnEnd() {}
        public virtual void OnDraw() {}
        public virtual void OnUpdate() {}
    }
}