using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Castaway.Base;

namespace Castaway.Rendering
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ProvidesShadersForAttribute : Attribute
    {
        public Type When;

        public ProvidesShadersForAttribute(Type when)
        {
            When = when;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IShaderProvider
    {
        ShaderObject CreateDefault(Graphics g);
        ShaderObject CreateDefaultTextured(Graphics g);
        ShaderObject CreateDirect(Graphics g);
        ShaderObject CreateDirectTextured(Graphics g);
        ShaderObject CreateUIScaled(Graphics g);
        ShaderObject CreateUIScaledTextured(Graphics g);
        ShaderObject CreateUIUnscaled(Graphics g);
        ShaderObject CreateUIUnscaledTextured(Graphics g);
    }

    public static class GlobalShader
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum Of
        {
            Default,
            DefaultTextured,
            Direct,
            DirectTextured,
            UIUnscaled,
            UIScaled,
            UIUnscaledTextured,
            UIScaledTextured,
        }

        static GlobalShader()
        {
            var logger = CastawayGlobal.GetLogger();
            logger.Debug("Starting initializing global shader set");
            var g = Graphics.Current;

            // (The code looks cleaner this way)
            // ReSharper disable once InlineOutVariableDeclaration
            Func<Graphics, ShaderObject>? m;
            var p = Provider(g);
            Default = p.CreateDefault(g);
            DefaultTextured = p.CreateDefaultTextured(g);
            Direct = p.CreateDirect(g);
            DirectTextured = p.CreateDirectTextured(g);
            UIUnscaled = p.CreateUIUnscaled(g);
            UIScaled = p.CreateUIScaled(g);
            UIUnscaledTextured = p.CreateUIUnscaledTextured(g);
            UIScaledTextured = p.CreateUIScaledTextured(g);
            logger.Debug("Finished initializing global shader set");
        }

        private static IShaderProvider Provider(Graphics g) =>
            Activator.CreateInstance(AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Single(m => m.GetCustomAttributes<ProvidesShadersForAttribute>().Any() &&
                                 m.GetCustomAttributes<ProvidesShadersForAttribute>()
                                     .Select(a => a.When)
                                     .Contains(Graphics.Current.GetType())))
                as IShaderProvider ??
            throw new InvalidOperationException($"Shader provider does not extend {nameof(IShaderProvider)}");

        // ReSharper disable InconsistentNaming
        public static ShaderObject Default;
        public static ShaderObject DefaultTextured;
        public static ShaderObject Direct;
        public static ShaderObject DirectTextured;
        public static ShaderObject UIUnscaled;
        public static ShaderObject UIScaled;
        public static ShaderObject UIUnscaledTextured;

        public static ShaderObject UIScaledTextured;
        // ReSharper restore InconsistentNaming
    }
}