#nullable enable
using System;
using System.Reflection;

namespace Castaway.Core
{
    public static class Modules
    {
        public static void Use(Module module) => module.Activate();
        public static void Use<T>() where T : Module, new() => Use(new T());
    }
}
