using static Castaway.Math.Matrix4;
using static Castaway.Render.ShaderManager;

namespace Castaway.Level.Controllers.Rendering2D
{
    [ControllerInfo(Name = "Transform [2D]")]
    public class Transform2DController : Controller
    {
        public override void PreOnDraw()
        {
            base.PreOnDraw();
            
            var m = Translate(parent.Position) * Scale(parent.Scale) * Rotate(parent.Rotation);
            ActiveHandle.SetTModel(m);
        }

        public override void PostOnDraw()
        {
            base.PostOnDraw();
            
            ActiveHandle.SetTModel(Identity);
        }
    }
}