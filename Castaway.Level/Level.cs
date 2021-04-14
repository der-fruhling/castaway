using System.Collections.Generic;
using Castaway.Core;

namespace Castaway.Levels
{
    /// <summary>
    /// A collection of <see cref="LevelObject"/>s.
    /// </summary>
    public class Level
    {
        public uint CurrentCamera = 0;
        
        /// <summary>
        /// Gets a list of every object in this level.
        /// </summary>
        public List<LevelObject> Objects { get; } = new List<LevelObject>();

        public void StartAll() => Objects.ForEach(o => o.Start());
        public void StopAll() => Objects.ForEach(o => o.Stop());
        public void UpdateAll() => Objects.ForEach(o => o.OnUpdate());
        public void DrawAll() => Objects.ForEach(o => o.OnDraw());

        /// <summary>
        /// Initializes this level. Should be put somewhere in an event, such
        /// as <see cref="Events.Init"/>.
        /// </summary>
        public void Activate()
        {
            StartAll();
            Events.Draw += DrawAll;
            Events.Update += UpdateAll;
        }
        
        /// <summary>
        /// Initializes this level. Should be put somewhere in an event, such
        /// as <see cref="Events.CloseNormally"/>.
        /// </summary>
        public void Deactivate()
        {
            StopAll();
            Events.Draw -= DrawAll;
            Events.Update -= UpdateAll;
        }

        /// <summary>
        /// Gets an object by index.
        /// </summary>
        /// <param name="index">Index to get the object for.</param>
        /// <returns>The object at <paramref name="index"/>.</returns>
        public LevelObject Get(int index) => Objects[index];
        
        /// <summary>
        /// Sets the object at index to a new value.
        /// </summary>
        /// <param name="index">Index of the object to set.</param>
        /// <param name="obj">New value.</param>
        public void Set(int index, LevelObject obj) => Objects[index] = obj;
        
        /// <summary>
        /// Gets a specially typed object by index.
        /// </summary>
        /// <inheritdoc cref="Get"/>
        /// <typeparam name="T">Type to cast to.</typeparam>
        public T Get<T>(int index) where T : LevelObject => (T) Get(index);

        /// <summary>
        /// Creates a new object, returning an <see cref="ObjectRef{T}"/> to
        /// it.
        /// </summary>
        /// <returns>Reference to the new object.</returns>
        public ObjectRef<LevelObject> Create()
        {
            var r = new ObjectRef<LevelObject>(Objects.Count, this);
            Objects.Add(new LevelObject
            {
                Ref = r,
                Level = this
            });
            return r;
        }

        /// <summary>
        /// Creates a new object, returning an <see cref="ObjectRef{T}"/> to
        /// it. This method also adds the specified controllers to the newly
        /// created object.
        /// </summary>
        /// <inheritdoc cref="Create()"/>
        /// <param name="controllers">Controllers to add.</param>
        public ObjectRef<LevelObject> Create(params Controller[] controllers)
        {
            var o = Create();
            var obj = o.Object;
            foreach (var c in controllers) obj.Add(c);
            o.Object = obj;
            return o;
        }

        /// <inheritdoc cref="Create()"/>
        /// <typeparam name="T">Type of the new object.</typeparam>
        public ObjectRef<T> Create<T>() where T : LevelObject => Create().To<T>();
        
        /// <inheritdoc cref="Create(Controller[])"/>
        /// <typeparam name="T">Type of the new object.</typeparam>
        public ObjectRef<T> Create<T>(params Controller[] controllers) where T : LevelObject 
            => Create(controllers).To<T>();
    }
}