using Castaway.Rendering;
using Castaway.Rendering.Shaders;

namespace Castaway.UI;

// ReSharper disable once InconsistentNaming
public static class UI
{
	public static int Scale = 1;

	public static void ApplyUniforms()
	{
		var g = Graphics.Current;
		g.Window!.GetFramebufferSize(out var w, out var h);
		g.SetIntUniform(g.BoundShader!, UniformType.FramebufferSize, w, h);
		g.SetIntUniform(g.BoundShader!, UniformType.UIScale, Scale);
	}
}