using Castaway.Math;

namespace Castaway.Level.Controllers
{
    [ControllerBase]
    public abstract class CameraController : Controller
    {
        [LevelSerialized("AmbientLight")] public float AmbientLight = .1f;
        [LevelSerialized("AmbientLightColor")] public Vector3 AmbientLightColor = new(1, 1, 1);
        [LevelSerialized("ID")] public uint CameraID;
        [LevelSerialized("FarCutoff")] public float FarCutoff = 100;
        [LevelSerialized("NearCutoff")] public float NearCutoff = 0.01f;
        [LevelSerialized("Size")] public float Size = 1;
    }
}