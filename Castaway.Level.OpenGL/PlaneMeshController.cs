using Castaway.Base;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.Rendering.Structures;

namespace Castaway.Level.OpenGL
{
    [ControllerName("PlaneMesh")]
    [Imports(typeof(OpenGLImpl))]
    public class PlaneMeshController : MeshController
    {
        [LevelSerialized("Color")] public Vector4 Color = new(1, 1, 1, 1);
        [LevelSerialized("Size")] public Vector2 Size = new(1, 1);

        public override void OnInit(LevelObject parent)
        {
            var right = Size.X / 2f;
            var forward = Size.Y / 2f;

            Mesh = new Mesh(new Mesh.Vertex[]
            {
                new(new Vector3(-right, 0, -forward), Color, new Vector3(0, 1, 0), new Vector3(0, 0, 0)),
                new(new Vector3(right, 0, -forward), Color, new Vector3(0, 1, 0), new Vector3(1, 0, 0)),
                new(new Vector3(-right, 0, forward), Color, new Vector3(0, 1, 0), new Vector3(0, 1, 0)),
                new(new Vector3(right, 0, forward), Color, new Vector3(0, 1, 0), new Vector3(1, 1, 0))
            }, new uint[] {0, 1, 2, 3, 1, 2});
        }
    }
}