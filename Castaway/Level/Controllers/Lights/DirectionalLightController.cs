using Castaway.Math;
using static Castaway.Levels.Lighting;

namespace Castaway.Levels.Controllers.Lights
{
    [ControllerInfo(Name = "Directional Light")]
    public class DirectionalLightController : Controller
    {
        public Vector3 Tint = new Vector3(1, 1, 1);
        
        public override void OnUpdate()
        {
            base.OnUpdate();
            Add(new DirectionalLightConfig(Tint, Tint, Tint, parent.ForwardVector));
        }
    }
}