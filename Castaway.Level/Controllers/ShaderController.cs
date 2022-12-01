using System;
using System.Linq;
using Castaway.Assets;
using Castaway.Base;
using Castaway.Rendering;
using Castaway.Rendering.Lighting;
using Castaway.Rendering.Objects;
using Castaway.Rendering.Shaders;
using Serilog;

namespace Castaway.Level.Controllers;

[ControllerName("Shader")]
public class ShaderController : Controller
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	private ShaderObject? _previous;

	[LevelSerialized("Asset")] public string AssetName = string.Empty;
	[LevelSerialized("Builtin")] public string BuiltinShaderName = string.Empty;
	[LevelSerialized("DisableCache")] public bool CacheDisabled = false;
	public ShaderObject? Shader;

	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		if (AssetName.Any())
		{
			if (CacheDisabled) ResolveNormally();
			else if (AssetLoader.Loader!.Cache.IsCached<ShaderObject>(AssetName)) ResolveFromCache();
			else ResolveNormallyAndCache();
		}
		else
		{
			Shader = typeof(GlobalShader).GetField(BuiltinShaderName)?.GetValue(null) as ShaderObject;
			if (Shader == null)
				throw new InvalidOperationException(
					$"Invalid builtin shader name encountered: {BuiltinShaderName}");
			Logger.Debug("Initialized shader controller with builtin shader {Name}", BuiltinShaderName);
		}
	}

	public override void PreRender(LevelObject camera, LevelObject parent)
	{
		base.PreRender(camera, parent);
		var g = Graphics.Current;
		_previous = g.BoundShader!;
		if (Shader == null) throw new InvalidOperationException($"Unloaded shader {BuiltinShaderName}");
		Shader.Bind();
		g.SetFloatUniform(Shader, UniformType.TransformPerspective,
			camera.Get<CameraController>()!.PerspectiveTransform);
		g.SetFloatUniform(Shader, UniformType.TransformView, camera.Get<CameraController>()!.ViewTransform);
		g.SetFloatUniform(Shader, UniformType.ViewPosition, camera.Position);
		LightResolver.Push();
	}

	public override void PostRender(LevelObject camera, LevelObject parent)
	{
		base.PostRender(camera, parent);
		_previous?.Bind();
	}

	private void ResolveNormally()
	{
		Shader = AssetLoader.Loader!.GetAssetByName(AssetName).Read<ShaderObject>();
		Logger.Debug("Resolved shader with asset at {Path}", AssetName);
	}

	private void ResolveNormallyAndCache()
	{
		ResolveNormally();
		AssetLoader.Loader!.Cache.Cache(AssetName, Shader!);
	}

	private void ResolveFromCache()
	{
		Shader = AssetLoader.Loader!.Cache.Get<ShaderObject>(AssetName);
		Logger.Debug("Resolved shader with cache from {ID} of {Type}", AssetName, typeof(ShaderObject));
	}
}