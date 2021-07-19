using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Castaway.Components
{
    public class ComponentManager
    {
        public readonly string Path;

        private Dictionary<string, Assembly> _assemblies;
        private Dictionary<Type, object> _containers;
        private Dictionary<string, EventHook> _eventHooks;
        private Dictionary<string, Func<object>> _getHooks;

        public ComponentManager(string path)
        {
            Path = path;
            _assemblies = Directory.EnumerateFiles(path)
                .Where(f => f.EndsWith(".dll"))
                .Select(n => AppDomain.CurrentDomain.Load(Assembly.LoadFile(n).GetName()))
                .Concat(new[] {Assembly.GetCallingAssembly()})
                .Concat(AppDomain.CurrentDomain.GetAssemblies())
                .Where(a => a.GetCustomAttribute<IncludedComponentAttribute>() != null ||
                            a.GetCustomAttribute<BaseAttribute>() != null).Distinct()
                .ToDictionary(a => a.GetCustomAttribute<IncludedComponentAttribute>()?.Id ?? "base");
            _containers = _assemblies.Values
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Contains(typeof(IContainer)))
                .ToDictionary(t => t, Activator.CreateInstance);
            _eventHooks = _containers
                .SelectMany(t => t.Key.GetEvents().Select(e => (t.Value, e)))
                .Where(t => t.e.GetCustomAttribute<PublishAttribute>() != null)
                .Select(t => new EventHook(t.Value, t.e))
                .ToDictionary(t => t.Event.GetCustomAttribute<PublishAttribute>()!.Id);
            var eventHandlers = _containers
                .SelectMany(t => t.Key.GetMethods())
                .Where(m => m.GetCustomAttribute<EventHandlerAttribute>() != null);
            _getHooks = _containers
                .SelectMany(t => t.Key.GetFields()
                    .Where(f => f.GetCustomAttribute<PublishAttribute>() != null)
                    .Select<FieldInfo, (string, Func<object>)>(f => (f.GetCustomAttribute<PublishAttribute>()!.Id,
                        () => f.GetValue(_containers[f.DeclaringType!]))))
                .Concat(_containers.SelectMany(t => t.Key.GetProperties()
                    .Where(p => p.GetCustomAttribute<PublishAttribute>() != null && p.GetMethod != null)
                    .Select<PropertyInfo, (string, Func<object>)>(p => (p.GetCustomAttribute<PublishAttribute>()!.Id,
                        () => p.GetValue(_containers[p.DeclaringType!])))))
                .ToDictionary(d => d.Item1, d => d.Item2);
            var setTo = _containers
                .SelectMany(t => t.Key.GetFields()
                    .Where(f => f.GetCustomAttribute<SetToAttribute>() != null)
                    .Select<FieldInfo, (string, Action<object, object>, Type)>(
                        f => (f.GetCustomAttribute<SetToAttribute>()!.Id, f.SetValue, f.DeclaringType)))
                .Concat(_containers.SelectMany(t => t.Key.GetProperties()
                    .Where(p => p.GetCustomAttribute<SetToAttribute>() != null && p.SetMethod != null)
                    .Select<PropertyInfo, (string, Action<object, object>, Type)>(
                        f => (f.GetCustomAttribute<SetToAttribute>()!.Id, f.SetValue, f.DeclaringType))));
            foreach (var m in eventHandlers)
            {
                var a = m.GetCustomAttribute<EventHandlerAttribute>();
                Debug.Assert(a != null, nameof(a) + " != null");
                var e = _eventHooks[a.Hook];
                e.Event.AddEventHandler(_containers[e.Event.DeclaringType!],
                    m.CreateDelegate(e.Event.EventHandlerType!));
            }

            foreach (var (hook, func, type) in setTo) func(_containers[type], _getHooks[hook]());
        }

        public ComponentManager() : this(System.IO.Path.GetDirectoryName(Assembly.GetCallingAssembly().Location))
        {
        }

        public Assembly this[string id] => _assemblies[id];
        public object this[Type type] => _containers[type];

        public bool IsPresent(string id) => _assemblies.ContainsKey(id);
        public EventHook GetEventHook(string id) => _eventHooks[id];
    }
}