using Castaway.Math;
using Castaway.Rendering.Structures;

namespace Castaway.Level.Controllers
{
    [ControllerName("PyramidMesh")]
    public class PyramidMeshController : MeshController
    {
        [LevelSerialized("Color")] public Vector4 Color;
        [LevelSerialized("Size")] public Vector3 Size;

        public override void OnInit(LevelObject parent)
        {
            var right = Size.X / 2f;
            var up = Size.Y / 2f;
            var forward = Size.Z / 2f;

            Mesh = new Mesh(new Mesh.Vertex[]
            {
                new(new Vector3(-right, -up, -forward), Color),
                new(new Vector3(right, -up, -forward), Color),
                new(new Vector3(-right, -up, forward), Color),
                new(new Vector3(right, -up, forward), Color),
                new(new Vector3(0, up, 0), Color)
            }, new uint[]
            {
                0, 1, 2, 3, 1, 2,
                0, 1, 4,
                1, 2, 4,
                2, 3, 4,
                3, 0, 4
            });
        }
    }
}