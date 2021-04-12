using System;

namespace Castaway.Core
{
    /// <summary>
    /// Contains some methods for interacting with
    /// <see cref="Module"/>s.
    /// </summary>
    public static class Modules
    {
        /// <summary>
        /// Enables a module by instance. Use <see cref="Use{T}"/> instead.
        /// </summary>
        /// <param name="module">Module to enable.</param>
        [Obsolete] public static void Use(Module module) => module.Activate();

        /// <summary>
        /// Enables a module by type.
        /// </summary>
        /// <typeparam name="T">Type of the module to enable.</typeparam>
        public static void Use<T>() where T : Module, new() => new T().Activate();
    }
}
