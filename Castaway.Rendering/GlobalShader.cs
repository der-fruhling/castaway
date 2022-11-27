using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Castaway.Base;
using Castaway.Rendering.Objects;

namespace Castaway.Rendering;

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
		UIScaledTextured
	}

	static GlobalShader()
	{
		var logger = CastawayGlobal.GetLogger();
		logger.Debug("Starting initializing global shader set");
		var g = Graphics.Current;

		var p = Provider();
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

	private static IShaderProvider Provider()
	{
		return Activator.CreateInstance(AppDomain.CurrentDomain
				       .GetAssemblies()
				       .SelectMany(a => a.GetTypes())
				       .Single(m => m.GetCustomAttributes<ProvidesShadersForAttribute>().Any() &&
				                    m.GetCustomAttributes<ProvidesShadersForAttribute>()
					                    .Select(a => a.When)
					                    .Contains(Graphics.Current.GetType())))
			       as IShaderProvider ??
		       throw new InvalidOperationException($"Shader provider does not extend {nameof(IShaderProvider)}");
	}

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