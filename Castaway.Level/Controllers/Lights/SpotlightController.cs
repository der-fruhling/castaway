using System;
using Castaway.Math;
using static Castaway.Levels.Lighting;

namespace Castaway.Levels.Controllers.Lights
{
    [ControllerInfo(Name = "Spotlight")]
    public class SpotlightController : Controller
    {
        public Vector3 Tint = new Vector3(1, 1, 1);
        public float CutOff = 22.5f;
        public float OuterCutOff = 30f;
        
        public override void OnUpdate()
        {
            base.OnUpdate();
            Add(new SpotlightConfig(Tint, Tint, Tint,
                parent.Position,
                Matrix4.RotateXDeg(parent.Rotation.X) * 
                Matrix4.RotateYDeg(parent.Rotation.Y) * 
                new Vector3(0, 0, 1), 
                MathF.Cos(CMath.Radians(CutOff)), 
                MathF.Cos(CMath.Radians(OuterCutOff))));
        }
    }
}