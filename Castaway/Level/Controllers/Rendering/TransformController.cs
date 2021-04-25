using static Castaway.Math.Matrix4;
using static Castaway.Render.ShaderManager;

namespace Castaway.Levels.Controllers.Rendering
{
    [ControllerInfo(Name = "Transform [2D]")]
    public class TransformController : Controller
    {
        public override void PreOnDraw()
        {
            base.PreOnDraw();
            
            var m = Translate(parent.Position) * Scale(parent.Scale) * RotateDeg(parent.Rotation);
            if(ActiveHandle == null) return;
            ActiveHandle.SetTModel(m);
        }

        public override void PostOnDraw()
        {
            base.PostOnDraw();
            
            if(ActiveHandle == null) return;
            ActiveHandle.SetTModel(Identity);
        }
    }
}