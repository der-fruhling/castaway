using System;
using System.Linq;
using System.Reflection;

namespace Castaway.Components
{
    public static class Ex
    {
        public static Type[] AllExtending(this Type type, Type @base) =>
            type.GetNestedTypes().Where(t => t.BaseType == @base).ToArray();

        public static Type[] AllExtending<T>(this Type type) => AllExtending(type, typeof(T));

        public static Type[] AllExtending(this Assembly asm, Type @base) =>
            asm.GetTypes().Where(t => t.BaseType == @base).ToArray();

        public static Type[] AllExtending<T>(this Assembly asm) => AllExtending(asm, typeof(T));
    }
}