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
        private IMesh _mesh;

        public MeshRendererController(IMesh m, Vector4? color = null)
        {
            Vertices = MeshConverter.Vertices(m, color);
            _mesh = m;
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
            if(ShaderManager.ActiveHandle == null) return;
            base.OnDraw();
            if (_mesh is OBJMesh mesh)
            {
                ShaderManager.ActiveHandle.SetMaterial(mesh.Material);
            }
            else
            {
                ShaderManager.ActiveHandle.SetMaterial(new Material
                {
                    Ambient = new Vector3(1, 1, 1),
                    Diffuse = new Vector3(1, 1, 1),
                    Specular = new Vector3(1, 1, 1),
                    SpecularExponent = 32,
                    Dissolve = 1,
                    IndexOfRefraction = 1.45f,
                    Mode = Material.IllumMode.Highlight
                });
            }
            _vbo.Draw();
        }
    }
}