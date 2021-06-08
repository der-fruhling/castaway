namespace Castaway.Level
{
    public abstract class CameraController : EmptyController
    {
        [LevelSerialized("ID")] public uint CameraID;
        [LevelSerialized("NearCutoff")] public float NearCutoff = 0.01f;
        [LevelSerialized("FarCutoff")] public float FarCutoff = 100;
        [LevelSerialized("Size")] public float Size = 1;

        public abstract void PreRenderFrame(LevelObject camera);
        public abstract void PostRenderFrame(LevelObject camera);
    }
}