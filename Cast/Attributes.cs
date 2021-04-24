using System;
using System.Collections.Generic;
using Castaway.Assets;
using Castaway.Core;
using Castaway.Input;
using Castaway.Levels;
using Castaway.Mesh;
using Castaway.Render;
using Castaway.Serializable;

namespace Cast
{
    /// <summary>
    /// Used in <see cref="RequiresModulesAttribute"/> to identity modules.
    /// </summary>
    public enum CModule
    {
        Render,
        Assets,
        Mesh,
        Serializable,
        Input,
        Level
    }

    /// <summary>
    /// Used in <see cref="EventHandlerAttribute"/> to identify events.
    /// </summary>
    public enum EventType
    {
        UseMethodName,
        
        PreInit,
        Init,
        PostInit,
        
        PreDraw,
        Draw,
        PostDraw,
        
        PreUpdate,
        Update,
        PostUpdate,
        
        Finish
    }

    internal static class CModules
    {
        private static readonly List<CModule> Loaded = new List<CModule>();
        
        internal static void Load(CModule lm)
        {
            if(Loaded.Contains(lm)) return;
            
            switch (lm)
            {
                case CModule.Assets:
                    Modules.Use<AssetsModule>();
                    break;
                case CModule.Render:
                    Modules.Use<RenderModule>();
                    break;
                case CModule.Mesh:
                    Modules.Use<MeshModule>();
                    break;
                case CModule.Serializable:
                    Modules.Use<SerializableModule>();
                    break;
                case CModule.Input:
                    Modules.Use<InputSystemModule>();
                    break;
                case CModule.Level:
                    Modules.Use<LevelModule>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lm), lm, null);
            }
            
            Loaded.Add(lm);
        }
    }
    
    /// <summary>
    /// Tells <see cref="Cast"/> that a class requires a certain module to be
    /// loaded and used, which will then automatically call
    /// <see cref="Castaway.Core.Modules.Use{T}"/> to set it up.
    /// </summary>
    /// <seealso cref="Cast"/>
    /// <seealso cref="CModule"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresModulesAttribute : Attribute
    {
        public readonly CModule[] Modules;

        public RequiresModulesAttribute(params CModule[] modules)
        {
            Modules = modules;
        }
    }

    /// <summary>
    /// If used on a class, <see cref="Cast"/> will search for the following
    /// other attributes on various objects:
    /// <list type="bullet">
    /// <item><see cref="EntrypointAttribute"/></item>
    /// <item><see cref="EventHandlerAttribute"/></item>
    /// </list>
    ///
    /// If used on a method, <see cref="Cast"/> will run that method while
    /// starting. (requires <see cref="EntrypointAttribute"/> on parent class)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class EntrypointAttribute : Attribute
    {
        public EntrypointAttribute() { }
    }

    /// <summary>
    /// Specifies that a method should react to an event. (requires
    /// <see cref="EntrypointAttribute"/> on parent class)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class EventHandlerAttribute : Attribute
    {
        public readonly EventType EventType;

        public EventHandlerAttribute(EventType eventType = EventType.UseMethodName)
        {
            EventType = eventType;
        }
    }
}