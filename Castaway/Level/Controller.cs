using Castaway.Levels.Controllers.Rendering;

namespace Castaway.Levels
{
    /// <summary>
    /// A controller that can be attached to a <see cref="LevelObject"/>.
    /// </summary>
    public abstract class Controller
    {
        private ObjectRef<LevelObject> _parent;

        // ReSharper disable InconsistentNaming
        protected Level level => parent.Level;
        protected LevelObject parent => _parent.Object;
        protected TransformController transform2d => parent.Get<TransformController>();
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Sets the parent of this controller.
        /// </summary>
        /// <param name="p">A reference to the new parent object.</param>
        public virtual void SetParent(ObjectRef<LevelObject> p)
        {
            _parent = p;
        }
        
        public virtual void OnBegin() {}
        public virtual void OnEnd() {}
        public virtual void PreOnDraw() {}
        public virtual void OnDraw() {}
        public virtual void PostOnDraw() {}
        public virtual void PreOnUpdate() {}
        public virtual void OnUpdate() {}
        public virtual void PostOnUpdate() {}
    }
}