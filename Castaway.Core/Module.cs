using System;

namespace Castaway.Core
{
    public abstract class Module
    {
        protected enum Event
        {
            PreInit,
            Init,
            PostInit,
            PreDraw,
            Draw,
            PostDraw,
            PreUpdate,
            Update,
            PostUpdate
        }

        public void Activate()
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

        protected void Subscribe(Event e, Events.GenericHandler d)
        {
            switch (e)
            {
                case Event.PreInit:
                    Events.PreInit += d;
                    break;
                case Event.Init:
                    Events.Init += d;
                    break;
                case Event.PostInit:
                    Events.PostInit += d;
                    break;
                case Event.PreDraw:
                    Events.PreDraw += d;
                    break;
                case Event.Draw:
                    Events.Draw += d;
                    break;
                case Event.PostDraw:
                    Events.PostDraw += d;
                    break;
                case Event.PreUpdate:
                    Events.PreUpdate += d;
                    break;
                case Event.Update:
                    Events.Update += d;
                    break;
                case Event.PostUpdate:
                    Events.PostUpdate += d;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }
        }
    }
}
