using Castaway.Math;
using static Castaway.Levels.Lighting;

namespace Castaway.Levels.Controllers.Lights
{
    [ControllerInfo(Name = "Point Light")]
    public class PointLightController : Controller
    {
        public Vector3 Tint = new Vector3(1, 1, 1);
        public float AttenLinear = 0.07f;
        public float AttenQuadratic = 0.017f;
        
        public override void OnUpdate()
        {
            base.OnUpdate();
            Add(new PointLightConfig(Tint, Tint, Tint, 
                parent.Position, 1, AttenLinear, AttenQuadratic));
        }
    }
}