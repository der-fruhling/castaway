using Castaway.Base;
using Castaway.Level;
using Castaway.Math;
using Castaway.Rendering;

namespace Castaway.OpenGL.Controllers;

[ControllerName("OrthoCamera")]
[Imports(typeof(OpenGLImpl))]
public class OrthographicCameraController : CameraController
{
	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		var win = Graphics.Current.Window!;
		win.WindowResize += OnWindowResize;
		win.GetSize(out var w, out var h);
		OnWindowResize(w, h);
	}

	public override void OnDestroy(LevelObject parent)
	{
		base.OnDestroy(parent);
		Graphics.Current.Window!.WindowResize -= OnWindowResize;
	}

	private void OnWindowResize(int w, int h)
	{
		var win = Graphics.Current.Window!;
		PerspectiveTransform = CameraMath.Ortho(win, FarCutoff, NearCutoff, Size);
	}

	public override void PreRenderFrame(LevelObject camera, LevelObject? parent)
	{
		base.PreRenderFrame(camera, parent);
		ViewTransform = Matrix4.Translate(-camera.Position);
	}
}