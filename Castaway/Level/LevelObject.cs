using System.Collections.Generic;
using System.Linq;
using Castaway.Math;

namespace Castaway.Level
{
    public class LevelObject
    {
        public string? Name;
        public Vector3 Position = new(0, 0, 0), Scale = new(1, 1, 1);
        public Vector3 RealPosition => Position + (Parent?.RealPosition ?? new Vector3(0, 0, 0));
        public List<EmptyController> Controllers = new();
        public List<LevelObject> Subobjects = new();
        public LevelObject? Parent;

        public void OnInit()
        {
            foreach(var c in Controllers) c.OnInit(this);
        }

        public void OnRender()
        {
            foreach(var c in Controllers) c.PreRender(this);
            foreach(var c in Controllers) c.OnRender(this);
            foreach(var c in Controllers) c.PostRender(this);
        }

        public void OnUpdate()
        {
            foreach(var c in Controllers) c.PreUpdate(this);
            foreach(var c in Controllers) c.OnUpdate(this);
            foreach(var c in Controllers) c.PostUpdate(this);
        }

        public void OnDestroy()
        {
            foreach(var c in Controllers) c.OnDestroy(this);
        }

        public void Add(LevelObject obj) => Subobjects.Add(obj);
        public LevelObject Get(string name) => Subobjects.Single(o => o.Name == name);
        public LevelObject this[string i] => Get(i);
    }
}