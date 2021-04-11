using System;
using System.Collections.Generic;
using System.Linq;
using Castaway.Math;

namespace Castaway.Level
{
    internal class ControllerException : ApplicationException
    {
        public ControllerException() { }
        public ControllerException(string message) : base(message) { }
        public ControllerException(string message, Exception innerException) : base(message, innerException) { }
    }
    
    public class LevelObject
    {
        public Vector3 Position, Rotation, Scale;
        
        private readonly List<Controller> _controllers = new List<Controller>();
        private bool _running;

        public void Add<T>() where T : Controller, new() => Add(new T());
        
        public void Add(Controller controller)
        {
            if (_controllers.Any(c => c.GetType() == controller.GetType()))
                throw new ControllerException("Cannot have more than one controller of the same type.");
            var index = _controllers.Count;
            _controllers.Add(controller);
            if (_running) _controllers[index].OnBegin();
        }

        public void Remove<T>() where T : Controller, new()
        {
            var index = _controllers.FindIndex(c => c.GetType() == typeof(T));
            if (_running) _controllers[index].OnEnd();
            _controllers.RemoveAt(index);
        }

        public void Start()
        {
            if(_running) return;
            foreach (var c in _controllers) c.OnBegin();
            
            _running = true;
        }

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