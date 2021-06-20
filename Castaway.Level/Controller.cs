using Castaway.Base;
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
            _inheritorLogger = CastawayGlobal.GetLogger(this.GetType());
        }

        public virtual void OnInit(LevelObject parent) => Logger.Verbose("Initializing {@This}", this);
        public virtual void PreRender(LevelObject camera, LevelObject parent) {}
        public virtual void OnRender(LevelObject camera, LevelObject parent) {}
        public virtual void PostRender(LevelObject camera, LevelObject parent) {}
        public virtual void PreUpdate(LevelObject parent) {}
        public virtual void OnUpdate(LevelObject parent) {}
        public virtual void PostUpdate(LevelObject parent) {}
        public virtual void OnDestroy(LevelObject parent) => Logger.Verbose("Destroying {@This}", this);
        public virtual void PreRenderFrame(LevelObject camera, LevelObject? parent) {}
        public virtual void PostRenderFrame(LevelObject camera, LevelObject? parent) {}
    }
}