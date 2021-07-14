using System.Collections.Generic;
using System.Linq;
using Castaway.Level.Controllers;
using Castaway.Math;

namespace Castaway.Level
{
    public class LevelObject
    {
        public List<Controller> Controllers = new();
        public Level Level;

        internal LevelObject(Level level)
        {
            Level = level;
        }

        public virtual string? Name { get; set; }
        public virtual Vector3 Position { get; set; } = new(0, 0, 0);
        public virtual Vector3 Scale { get; set; } = new(1, 1, 1);
        public virtual Quaternion Rotation { get; set; } = Quaternion.Rotation(0, 0, 0).Normalize();

        public void OnInit()
        {
            foreach (var c in Controllers) c.OnInit(this);
        }

        public void OnRender(LevelObject cam)
        {
            if (!Controllers.Any()) return;
            foreach (var c in Controllers) c.PreRender(cam, this);
            foreach (var c in Controllers) c.OnRender(cam, this);
            foreach (var c in Controllers) c.PostRender(cam, this);
        }

        public void OnUpdate()
        {
            if (!Controllers.Any()) return;
            foreach (var c in Controllers) c.PreUpdate(this);
            foreach (var c in Controllers) c.OnUpdate(this);
            foreach (var c in Controllers) c.PostUpdate(this);
        }

        public void OnDestroy()
        {
            foreach (var c in Controllers) c.OnDestroy(this);
        }

        public T? Get<T>() where T : Controller
        {
            return Controllers.SingleOrDefault(c => c is T) as T;
        }

        public T[] GetAll<T>() where T : Controller
        {
            return Controllers.Where(c => c is T).Cast<T>().ToArray();
        }

        public void Add(Controller controller)
        {
            Controllers.Add(controller);
        }

        public void OnPreRender(LevelObject cam)
        {
            if (!Controllers.Any()) return;
            var conts = Controllers.Except(GetAll<CameraController>()).ToArray();
            if (!conts.Any()) return;
            foreach (var c in conts) c.PreRenderFrame(cam, this);
        }

        public void OnPostRender(LevelObject cam)
        {
            if (!Controllers.Any()) return;
            var conts = Controllers.Except(GetAll<CameraController>()).ToArray();
            if (!conts.Any()) return;
            foreach (var c in conts) c.PostRenderFrame(cam, this);
        }
    }
}