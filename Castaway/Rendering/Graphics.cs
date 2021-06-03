namespace Castaway.Rendering
{
    public class Graphics
    {
        private static dynamic _global;

        public static T Setup<T>() where T : class, new() => _global = new T();
        public static T Get<T>() where T : class => _global as T;
    }
}