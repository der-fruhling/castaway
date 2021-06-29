using System.Reflection;

namespace Castaway.Base
{
    public static class Ex
    {
        public static bool HasAttribute<T>(MemberInfo type)
        {
            return type.GetCustomAttribute(typeof(T)) != null;
        }

        public static bool HasNoAttribute<T>(MemberInfo type)
        {
            return type.GetCustomAttribute(typeof(T)) == null;
        }
    }
}