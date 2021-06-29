using System;
using Castaway.Base;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.Rendering;
using Castaway.Rendering.Structures;
using Serilog;

namespace Castaway.Level.OpenGL
{
    [ControllerBase, Imports(typeof(OpenGLImpl))]
    public abstract class CameraController : Castaway.Level.CameraController
    {
        private static readonly ILogger Logger = CastawayGlobal.GetLogger();

        public FramebufferObject? Framebuffer;
        public Matrix4 PerspectiveTransform;
        public Matrix4 ViewTransform;

        private Drawable? _fullscreenDrawable;

        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            var g = Graphics.Current;
            Framebuffer = g.NewFramebuffer();

            _fullscreenDrawable = new Mesh(new Mesh.Vertex[]
            {
                new(new Vector3(-1, -1, 0), texture: new Vector3(0, 0, 0)),
                new(new Vector3(1, -1, 0),  texture: new Vector3(1, 0, 0)),
                new(new Vector3(-1, 1, 0),  texture: new Vector3(0, 1, 0)),
                new(new Vector3(1, 1, 0),   texture: new Vector3(1, 1, 0)),
            }, new uint[] {0, 1, 2, 1, 3, 2}).ConstructUnoptimisedFor(BuiltinShaders.DirectTextured);
            // TODO Work around unoptimised fullscreen buffer.
            
            Logger.Debug("Created new camera {Type} filling {ID}", GetType(), CameraID);
        }

        public override void OnDestroy(LevelObject parent)
        {
            base.OnDestroy(parent);
            Framebuffer?.Dispose();
            _fullscreenDrawable?.Dispose();
            _fullscreenDrawable = null;
            Logger.Debug("Removed camera filling {ID}", CameraID);
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

            if (camera.Level.ActiveCamera != CameraID) return;
            var bp = g.BoundShader;
            if (bp != BuiltinShaders.DirectTextured)
                BuiltinShaders.DirectTextured!.Bind();
            g.Draw(BuiltinShaders.DirectTextured!, _fullscreenDrawable ?? throw new InvalidOperationException("Must initialize before draw."));
            if(bp != null && bp != BuiltinShaders.DirectTextured) bp!.Bind();
        }
    }
}