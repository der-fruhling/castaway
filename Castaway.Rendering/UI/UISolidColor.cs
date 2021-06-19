using Castaway.Math;

namespace Castaway.Rendering.UI
{
    // ReSharper disable once InconsistentNaming
    public class UISolidColor : UIElement
    {
        private readonly Vector4 _color;
        private Drawable? _drawable;

        public UISolidColor(Vector4 color, int x, int y, int width, int height, Corner relative = Corner.BottomLeft) : base(x, y, width, height, relative)
        {
            _color = color;
        }

        protected override void Initialize()
        {
            _drawable = Graphics.Current.NewDrawable(ConstructMesh(_color));
        }

        protected override void Render()
        {
            Graphics.Current.Draw(Graphics.Current.BoundShader!, _drawable!);
        }

        protected override void Update()
        {
            
        }
    }
}