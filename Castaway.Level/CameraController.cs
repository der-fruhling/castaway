using Castaway.Math;

namespace Castaway.Level
{
    public abstract class CameraController : EmptyController
    {
        [LevelSerialized("ID")] public uint CameraID;
        [LevelSerialized("NearCutoff")] public float NearCutoff = 0.01f;
        [LevelSerialized("FarCutoff")] public float FarCutoff = 100;
        [LevelSerialized("Size")] public float Size = 1;
        [LevelSerialized("AmbientLight")] public float AmbientLight = .1f;
        [LevelSerialized("AmbientLightColor")] public Vector3 AmbientLightColor = new(1, 1, 1);
    }
}