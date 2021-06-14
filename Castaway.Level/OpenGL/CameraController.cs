using System;
using Castaway.Math;
using Castaway.OpenGL;

namespace Castaway.Level.OpenGL
{
    public abstract class CameraController : Castaway.Level.CameraController
    {
        public Framebuffer Framebuffer;
        public Matrix4 PerspectiveTransform;
        public Matrix4 ViewTransform;

        private IDrawable? _fullscreenDrawable;

        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            Framebuffer = g.CreateFramebuffer(g.BoundWindow!.Value);

            _fullscreenDrawable = new Mesh(new Mesh.Vertex[]
            {
                new(new Vector3(-1, -1, 0), texture: new Vector3(0, 0, 0)),
                new(new Vector3(1, -1, 0),  texture: new Vector3(1, 0, 0)),
                new(new Vector3(-1, 1, 0),  texture: new Vector3(0, 1, 0)),
                new(new Vector3(1, 1, 0),   texture: new Vector3(1, 1, 0)),
            }, new uint[] {0, 1, 2, 3, 1, 2}).ConstructFor(g, BuiltinShaders.DirectTextured);
        }

        public override void OnDestroy(LevelObject parent)
        {
            base.OnDestroy(parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            g.Destroy(Framebuffer);
            if(_fullscreenDrawable != null)
                g.Destroy(_fullscreenDrawable.ElementArray!.Value, _fullscreenDrawable.VertexArray!.Value);
            _fullscreenDrawable = null;
        }

        public override void PreRenderFrame(LevelObject camera, LevelObject? parent)
        {
            var g = Castaway.OpenGL.OpenGL.Get();
            g.Bind(Framebuffer);
            g.Clear();
        }

        public override void PostRenderFrame(LevelObject camera, LevelObject? parent)
        {
            var g = Castaway.OpenGL.OpenGL.Get();
            g.UnbindFramebuffer();

            if (camera.Level.ActiveCamera != CameraID) return;
            var bp = g.BoundProgram;
            if (bp != BuiltinShaders.DirectTextured)
                g.Bind(BuiltinShaders.DirectTextured);
            g.Draw(BuiltinShaders.DirectTextured, _fullscreenDrawable ?? throw new InvalidOperationException("Must initialize before draw."));
            if(bp != null && bp != BuiltinShaders.DirectTextured) g.Bind(bp!.Value);
        }
    }
}