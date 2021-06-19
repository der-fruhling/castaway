using System.Collections.Generic;

namespace Castaway.Rendering.UI
{
    // ReSharper disable once InconsistentNaming
    public sealed class UIRenderer
    {
        private List<UIElement> _elements = new();

        public void Add(UIElement e) => _elements.Add(e);
        public void Add(params UIElement[] e) => _elements.AddRange(e);
        public void Overwrite() => _elements.Clear();

        public void Draw()
        {
            foreach (var element in _elements)
            {
                
            }
        }
    }
}