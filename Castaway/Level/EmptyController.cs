using System.Diagnostics;

namespace Castaway.Level
{
    public class EmptyController
    {
        public virtual void OnInit(LevelObject parent) {}
        public virtual void PreRender(LevelObject parent) {}
        public virtual void OnRender(LevelObject parent) {}
        public virtual void PostRender(LevelObject parent) {}
        public virtual void PreUpdate(LevelObject parent) {}
        public virtual void OnUpdate(LevelObject parent) {}
        public virtual void PostUpdate(LevelObject parent) {}
        public virtual void OnDestroy(LevelObject parent) {}
    }
}