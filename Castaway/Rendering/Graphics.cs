using System;

#nullable enable
namespace Castaway.Rendering
{
    public static class Graphics
    {
        private static IGraphics? _impl;
        
        public static void SetImpl<T>() where T : class, IGraphics, new()
        {
            if (_impl != null) throw new GraphicsException("Cannot switch graphics library at runtime.");
            _impl = new T();
        }
        
        public static IGraphics GetImpl()
        {
            return _impl ?? throw new GraphicsException("No graphics library set.");
        }

        static Graphics()
        {
            AppDomain.CurrentDomain.ProcessExit += (_, _) => _impl?.Dispose();
        }
    }
}