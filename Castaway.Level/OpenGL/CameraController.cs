using Castaway.Math;
using Castaway.OpenGL;

namespace Castaway.Level.OpenGL
{
    public abstract class CameraController : Castaway.Level.CameraController
    {
        public Framebuffer Framebuffer;
        public Matrix4 PerspectiveTransform;
        public Matrix4 ViewTransform;

        private IDrawable _fullscreenDrawable;

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
            }, new uint[] {0, 1, 2, 3, 1, 2}).ConstructFor(g, BuiltinShaders.NoTransformTextured);
        }

        public override void OnDestroy(LevelObject parent)
        {
            base.OnDestroy(parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            g.Destroy(Framebuffer);
        }

        public override void PreRenderFrame(LevelObject parent)
        {
            var g = Castaway.OpenGL.OpenGL.Get();
            g.Bind(Framebuffer);
            g.Clear();
        }

        public override void PostRenderFrame(LevelObject parent)
        {
            var g = Castaway.OpenGL.OpenGL.Get();
            g.UnbindFramebuffer();

            if (parent.Level.ActiveCamera != CameraID) return;
            var bp = g.BoundProgram;
            if (bp != BuiltinShaders.NoTransformTextured)
                g.Bind(BuiltinShaders.NoTransformTextured);
            g.Draw(BuiltinShaders.NoTransformTextured, _fullscreenDrawable);
            if(bp != null && bp != BuiltinShaders.NoTransformTextured) g.Bind(bp);
        }
    }
}