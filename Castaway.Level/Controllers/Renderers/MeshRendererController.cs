#nullable enable
using System;
using Castaway.Levels.Controllers.Storage;
using Castaway.Math;
using Castaway.Mesh;
using Castaway.Render;
using MeshConverter = Castaway.Render.MeshConverter;

namespace Castaway.Levels.Controllers.Renderers
{
    public class MeshRendererController : Controller
    {
        public VertexBuffer.Vertex[]? Vertices;
        private VBO _vbo = null!;

        public MeshRendererController(IMesh m, Vector4? color = null)
        {
            Vertices = MeshConverter.Vertices(m, color);
        }
        
        public MeshRendererController() {}

        public override void OnBegin()
        {
            base.OnBegin();
            
            var s = parent.Get<MeshStorageController>();
            if (s != null && Vertices == null) Vertices = MeshConverter.Vertices(s.Mesh);

            if (Vertices == null) throw new ApplicationException("No vertices for MeshRenderer");
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