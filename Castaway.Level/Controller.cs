using System.Threading.Tasks;
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
            _inheritorLogger = CastawayGlobal.GetLogger(GetType());
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
#pragma warning disable 1998
        public virtual async Task PreUpdate(LevelObject parent)
        {
        }

        public virtual async Task OnUpdate(LevelObject parent)
        {
        }

        public virtual async Task PostUpdate(LevelObject parent)
        {
        }
#pragma warning restore 1998
    }
}