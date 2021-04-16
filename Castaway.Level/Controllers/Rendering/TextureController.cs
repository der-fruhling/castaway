using Castaway.Render;

namespace Castaway.Levels.Controllers.Rendering
{
    [ControllerInfo(Name = "Texture")]
    public class TextureController : Controller
    {
        public Texture Texture;
        private BoundTexture _boundTexture;

        public bool FilterNearest = false;
        public bool UseAlpha = true;
        
        public override void OnBegin()
        {
            base.OnBegin();
            if(!Texture.IsSetUp) Texture.Setup(FilterNearest, UseAlpha);
        }

        public override void PreOnDraw()
        {
            base.PreOnDraw();
            _boundTexture = Texture.Use();
        }

        public override void PostOnDraw()
        {
            base.PostOnDraw();
            _boundTexture.Dispose();
        }
    }
}