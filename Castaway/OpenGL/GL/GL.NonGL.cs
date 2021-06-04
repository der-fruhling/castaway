using System;
using System.Linq;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace Castaway.OpenGL
{
    public static unsafe partial class GL
    {
        [AttributeUsage(AttributeTargets.Field)]
        private class ConstValueAttribute : Attribute
        {
            public readonly uint Value;

            public ConstValueAttribute(uint value)
            {
                Value = value;
            }
        }

        public static readonly (int Major, int Minor) MaxSupportedVersion = (2, 1);

        public static void Init()
        {
        }

        public static T ValueEnum<T>(uint c) where T : Enum
        {
            foreach (var f in typeof(T).GetFields()
                .Where(f => f.GetCustomAttribute<ConstValueAttribute>() != null))
            {
                var a = f.GetCustomAttribute<ConstValueAttribute>();
                if (a!.Value == c) return ((T?) f.GetValue(null))!;
            }

            throw new ArgumentOutOfRangeException(nameof(c), c, $"No value of {typeof(T).Name} matches {c}");
        }

        public static uint EnumValue<T>(T e) where T : struct, Enum
        {
            var f = typeof(T).GetField(Enum.GetName(e)!);
            if (f != null && f.GetCustomAttribute<ConstValueAttribute>() != null)
                return f.GetCustomAttribute<ConstValueAttribute>()!.Value;
            throw new ArgumentOutOfRangeException(nameof(e), e, $"No value of {typeof(T).Name} matches {e}");
        }

        [Obsolete("Use CreateBuffer instead")]
        public static uint GenBuffer()
        {
            GenBuffers(1, out var u);
            return u[0];
        }

        public static uint CreateBuffer()
        {
            CreateBuffers(1, out var u);
            return u[0];
        }
    }
}