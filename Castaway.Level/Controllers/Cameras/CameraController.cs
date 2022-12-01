using System;
using Castaway.Base;
using Castaway.Math;
using Castaway.Rendering;
using Castaway.Rendering.Objects;
using Castaway.Rendering.Structures;
using Serilog;

namespace Castaway.Level.Controllers;

[ControllerBase]
public abstract class CameraController : Controller
{
	private static readonly ILogger Logger = CastawayGlobal.GetLogger();

	private Drawable? _fullscreenDrawable;
	[LevelSerialized("AmbientLight")] public float AmbientLight = .1f;
	[LevelSerialized("AmbientLightColor")] public Vector3 AmbientLightColor = new(1, 1, 1);
	[LevelSerialized("ID")] public uint CameraId;
	[LevelSerialized("FarCutoff")] public float FarCutoff = 100;

	public FramebufferObject? Framebuffer;
	[LevelSerialized("NearCutoff")] public float NearCutoff = 0.01f;
	public Matrix4 PerspectiveTransform;
	[LevelSerialized("Size")] public float Size = 1;
	public Matrix4 ViewTransform;

	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		var g = Graphics.Current;
		Framebuffer = g.NewFramebuffer();

		_fullscreenDrawable = new Mesh(new Mesh.Vertex[]
		{
			new(new Vector3(-1, -1, 0), texture: new Vector3(0, 0, 0)),
			new(new Vector3(1, -1, 0), texture: new Vector3(1, 0, 0)),
			new(new Vector3(-1, 1, 0), texture: new Vector3(0, 1, 0)),
			new(new Vector3(1, 1, 0), texture: new Vector3(1, 1, 0))
		}, new uint[] { 0, 1, 2, 1, 3, 2 }).ConstructFor(GlobalShader.DirectTextured);

		Logger.Debug("Created new camera {Type} filling {ID}", GetType(), CameraId);
	}

	public override void OnDestroy(LevelObject parent)
	{
		base.OnDestroy(parent);
		Framebuffer?.Dispose();
		_fullscreenDrawable?.Dispose();
		_fullscreenDrawable = null;
		Logger.Debug("Removed camera filling {ID}", CameraId);
	}

	public override void PreRenderFrame(LevelObject camera, LevelObject? parent)
	{
		var g = Graphics.Current;
		Framebuffer?.Bind();
		g.Clear();
	}

	public override void PostRenderFrame(LevelObject camera, LevelObject? parent)
	{
		var g = Graphics.Current;
		g.UnbindFramebuffer();

		if (camera.Level.ActiveCamera != CameraId) return;
		var bp = g.BoundShader;
		if (bp != GlobalShader.DirectTextured)
			GlobalShader.DirectTextured.Bind();
		g.Draw(GlobalShader.DirectTextured,
			_fullscreenDrawable ?? throw new InvalidOperationException("Must initialize before draw."));
		if (bp != null && bp != GlobalShader.DirectTextured) bp.Bind();
	}
}