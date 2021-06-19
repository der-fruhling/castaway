using System;

namespace Castaway.Rendering
{
    /// <summary>
    /// Workaround type. Use with all implementations that you will use.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ImportsAttribute : Attribute
    {
        public Type Type;

        public ImportsAttribute(Type type)
        {
            Type = type;
        }
    }
}