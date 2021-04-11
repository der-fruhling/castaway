using System.Collections.Generic;
using Castaway.Core;

namespace Castaway.Level
{
    public class Level
    {
        public List<LevelObject> Objects { get; private set; } = new List<LevelObject>();

        public void StartAll() => Objects.ForEach(o => o.Start());
        public void StopAll() => Objects.ForEach(o => o.Stop());
        public void UpdateAll() => Objects.ForEach(o => o.OnUpdate());
        public void DrawAll() => Objects.ForEach(o => o.OnDraw());

        public void Activate()
        {
            StartAll();
            Events.Draw += DrawAll;
            Events.Update += UpdateAll;
        }

        public void Deactivate()
        {
            StopAll();
            Events.Draw -= DrawAll;
            Events.Update -= UpdateAll;
        }

        public LevelObject Get(int index) => Objects[index];
        public void Set(int index, LevelObject obj) => Objects[index] = obj;
        public T Get<T>(int index) where T : LevelObject => (T) Get(index);

        public ObjectRef<LevelObject> Create()
        {
            var r = new ObjectRef<LevelObject>(Objects.Count, this);
            Objects.Add(new LevelObject());
            return r;
        }

        public ObjectRef<LevelObject> Create(params Controller[] controllers)
        {
            var o = Create();
            var obj = o.Object;
            foreach (var c in controllers) obj.Add(c);
            o.Object = obj;
            return o;
        }

        public ObjectRef<T> Create<T>() where T : LevelObject => Create().To<T>();
        public ObjectRef<T> Create<T>(params Controller[] controllers) where T : LevelObject 
            => Create(controllers).To<T>();
    }
}