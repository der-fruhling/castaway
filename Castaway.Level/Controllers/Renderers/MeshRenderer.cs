#nullable enable
using Castaway.Math;
using Castaway.Mesh;
using Castaway.Render;
using MeshConverter = Castaway.Render.MeshConverter;

namespace Castaway.Level.Controllers.Renderers
{
    public class MeshRenderer : Controller
    {
        public VertexBuffer.Vertex[] Vertices;
        private VBO _vbo = null!;

        public MeshRenderer(IMesh m, Vector4? color = null)
        {
            Vertices = MeshConverter.Vertices(m, color);
        }

        public override void OnBegin()
        {
            base.OnBegin();
            _vbo = new VBO {Vertices = Vertices};
            _vbo.Setup();
        }

        public override void OnDraw()
        {
            base.OnDraw();
            _vbo.Draw();
        }
    }
}