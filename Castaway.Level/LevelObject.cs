using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Math;

namespace Castaway.Level
{
    /// <summary>
    /// An object in a <see cref="Level"/>.
    /// </summary>
    public class LevelObject
    {
        /// <summary>
        /// Position of this object. Z is up.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Rotation of this object, in euler angles.
        /// </summary>
        public Vector3 Rotation;

        /// <summary>
        /// Scale multiplier of this object.
        /// </summary>
        public Vector3 Scale;

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
            if(_running) return;
            foreach (var c in _controllers) c.OnBegin();
            
            _running = true;
        }

        /// <summary>
        /// Stops executing this object.
        /// </summary>
        public void Stop()
        {
            if(!_running) return;
            _running = false;
            foreach (var c in _controllers) c.OnEnd();
        }

        internal void OnUpdate()
        {
            if(!_running) return;
            _controllers.ForEach(c => c.OnUpdate());
        }

        internal void OnDraw()
        {
            if(!_running) return;
            _controllers.ForEach(c => c.OnDraw());
        }
    }
}