using System.Collections.Generic;
using System.Linq;
using Castaway.Math;

namespace Castaway.Level
{
    public class LevelObject
    {
        public List<Controller> Controllers = new();
        public Level Level;
        public string? Name;
        public LevelObject? Parent;
        public Vector3 Position = new(0, 0, 0), Scale = new(1, 1, 1);
        public Quaternion Rotation = Quaternion.Rotation(0, 0, 0).Normalize();
        public List<LevelObject> Subobjects = new();

        internal LevelObject(Level level)
        {
            Level = level;
        }

        public Vector3 RealPosition => Position + (Parent?.RealPosition ?? new Vector3(0, 0, 0));

        public LevelObject this[string i] => Get(i);

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

        public void Add(LevelObject obj)
        {
            Subobjects.Add(obj);
        }

        public LevelObject Get(string name)
        {
            return Subobjects.Single(o => o.Name == name);
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