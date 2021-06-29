using Castaway.Base;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.Rendering.Structures;

namespace Castaway.Level.OpenGL
{
    [ControllerName("CubeMesh")]
    [Imports(typeof(OpenGLImpl))]
    public class CubeMeshController : MeshController
    {
        [LevelSerialized("Color")] public Vector4 Color = new(1, 1, 1, 1);
        [LevelSerialized("Size")] public Vector3 Size = new(.5f, .5f, .5f);

        public override void OnInit(LevelObject parent)
        {
            var right = Size.X / 2f;
            var up = Size.Y / 2f;
            var forward = Size.Z / 2f;

            Mesh = new Mesh(new Mesh.Vertex[]
            {
                new(new Vector3(-right, -up, -forward), Color), // Left  Down Back
                new(new Vector3(right, -up, -forward), Color), // Right Down Back
                new(new Vector3(-right, up, -forward), Color), // Left  Up   Back
                new(new Vector3(right, up, -forward), Color), // Right Up   Back
                new(new Vector3(-right, -up, forward), Color), // Left  Down Front
                new(new Vector3(right, -up, forward), Color), // Right Down Front
                new(new Vector3(-right, up, forward), Color), // Left  Up   Front
                new(new Vector3(right, up, forward), Color) // Right Up   Front
            }, new uint[]
            {
                0, 1, 2, 3, 1, 2,
                4, 5, 6, 7, 5, 6,
                0, 2, 6, 0, 4, 6,
                1, 3, 7, 1, 5, 7,
                1, 0, 4, 1, 5, 4,
                3, 2, 6, 3, 7, 6
            });
        }
    }
}