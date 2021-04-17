#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Math;
using Castaway.Render;

namespace Castaway.Levels
{
    /// <summary>
    /// An object in a <see cref="Level"/>.
    /// </summary>
    public class LevelObject
    {
        /// <summary>
        /// Position of this object. Z is up.
        /// </summary>
        public Vector3 Position = Vector3.Zero;

        /// <summary>
        /// Rotation of this object, in euler angles.
        /// </summary>
        public Vector3 Rotation = Vector3.Zero;

        /// <summary>
        /// Scale multiplier of this object.
        /// </summary>
        public Vector3 Scale = new Vector3(1, 1, 1);

        public Vector3 ForwardVector => Matrix4.RotateDeg(Rotation) * new Vector3(0, 0, 1);
        public Vector3 UpVector => Matrix4.RotateDeg(Rotation) * new Vector3(0, 1, 0);
        public Vector3 RightVector => Matrix4.RotateDeg(Rotation) * new Vector3(1, 0, 0);
        public Vector3 BackwardVector => -ForwardVector;
        public Vector3 DownVector => -UpVector;
        public Vector3 LeftVector => -RightVector;

        public string? Name = null;

        public ObjectRef<LevelObject> Ref { get; internal set; }
        public Level Level { get; internal set; } = null!;

        private readonly List<Controller> _controllers = new List<Controller>();
        private bool _running;

        /// <summary>
        /// Adds a controller of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the controller to add.</typeparam>
        public void Add<T>() where T : Controller, new() => Add(new T());

        /// <summary>
        /// Adds a controller by an instance.
        /// </summary>
        /// <param name="controller">Controller to add.</param>
        public void Add(Controller controller)
        {
            if (_controllers.Any(c => c.GetType() == controller.GetType()))
                throw new ApplicationException("Cannot have more than one controller of the same type.");
            var index = _controllers.Count;
            controller.SetParent(Ref);
            _controllers.Add(controller);
            if (_running) _controllers[index].OnBegin();
        }

        /// <summary>
        /// Removes a controller of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the controller to remove.</typeparam>
        public void Remove<T>() where T : Controller, new()
        {
            var index = _controllers.FindIndex(c => c.GetType() == typeof(T));
            if (_running) _controllers[index].OnEnd();
            _controllers.RemoveAt(index);
        }

        /// <summary>
        /// Starts executing this object.
        /// </summary>
        public void Start()
        {
            if (_running) return;
            foreach (var c in _controllers) c.OnBegin();

            _running = true;
        }

        /// <summary>
        /// Stops executing this object.
        /// </summary>
        public void Stop()
        {
            if (!_running) return;
            _running = false;
            foreach (var c in _controllers) c.OnEnd();
        }

        internal void OnUpdate()
        {
            if (!_running) return;
            _controllers.ForEach(c => c.PreOnUpdate());
            _controllers.ForEach(c => c.OnUpdate());
            _controllers.ForEach(c => c.PostOnUpdate());
        }

        internal void OnDraw()
        {
            if (!_running) return;
            var s = ShaderManager.ActiveHandle;
            _controllers.ForEach(c => c.PreOnDraw());
            _controllers.ForEach(c => c.OnDraw());
            _controllers.ForEach(c => c.PostOnDraw());
        }

        public T? Get<T>() where T : Controller
        {
            return (T?) _controllers.FirstOrDefault(c => c is T);
        }

        public void Set(Controller value)
        {
            var i = _controllers.FindIndex(c => c.GetType() == value.GetType());
            _controllers[i] = value;
        }
    }
}