using System;

namespace Castaway.Core
{
    /// <summary>
    /// Base class for Castaway modules that can be activated with
    /// <see cref="Modules.Use{T}"/>.
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// Enables this module.
        /// </summary>
        internal void Activate()
        {
            Start();
            Events.PreInit += PreInit;
            Events.PreDraw += PreDraw;
            Events.PreUpdate += PreUpdate;
            Events.Init += Init;
            Events.Draw += Draw;
            Events.Update += Update;
            Events.PostInit += PostInit;
            Events.PostDraw += PostDraw;
            Events.PostUpdate += PostUpdate;
        }

        protected virtual void Start() { }
        
        protected virtual void PreInit() {}
        protected virtual void Init() {}
        protected virtual void PostInit() {}
        protected virtual void PreDraw() {}
        protected virtual void Draw() {}
        protected virtual void PostDraw() {}
        protected virtual void PreUpdate() {}
        protected virtual void Update() {}
        protected virtual void PostUpdate() {}
    }
}
