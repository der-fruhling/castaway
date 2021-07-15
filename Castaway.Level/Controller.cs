using Castaway.Base;
using Castaway.Math;
using Serilog;

namespace Castaway.Level
{
    [ControllerBase]
    public class Controller
    {
        private static readonly ILogger Logger = CastawayGlobal.GetLogger();
        private readonly ILogger _inheritorLogger;

        public Controller()
        {
            _inheritorLogger = CastawayGlobal.GetLogger();
        }

        public virtual Vector3 Position
        {
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        public virtual Quaternion Rotation
        {
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        public virtual Vector3 Scale
        {
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        public virtual void OnInit(LevelObject parent)
        {
            Logger.Verbose("Initializing {$This} on {Parent}", this, parent.Name);
        }

        public virtual void PreRender(LevelObject camera, LevelObject parent)
        {
        }

        public virtual void OnRender(LevelObject camera, LevelObject parent)
        {
        }

        public virtual void PostRender(LevelObject camera, LevelObject parent)
        {
        }

        public virtual void OnDestroy(LevelObject parent)
        {
            Logger.Verbose("Destroying {$This} on {Parent}", this, parent.Name);
        }

        public virtual void PreRenderFrame(LevelObject camera, LevelObject? parent)
        {
        }

        public virtual void PostRenderFrame(LevelObject camera, LevelObject? parent)
        {
        }

        public virtual void PreUpdate(LevelObject parent)
        {
        }

        public virtual void OnUpdate(LevelObject parent)
        {
        }

        public virtual void PostUpdate(LevelObject parent)
        {
        }
    }
}