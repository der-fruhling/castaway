using System;
using System.Collections.Generic;
using Castaway.Assets;
using Castaway.Core;
using Castaway.Render;

namespace Castaway.Exec
{
    public enum CModule
    {
        Render,
        Assets
    }

    public enum EventType
    {
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(lm), lm, null);
            }
            
            Loaded.Add(lm);
        }
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresModulesAttribute : Attribute
    {
        public readonly CModule[] Modules;

        public RequiresModulesAttribute(params CModule[] modules)
        {
            Modules = modules;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class EntrypointAttribute : Attribute
    {
        public EntrypointAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class EventHandlerAttribute : Attribute
    {
        public readonly EventType EventType;

        public EventHandlerAttribute(EventType eventType)
        {
            EventType = eventType;
        }
    }
}