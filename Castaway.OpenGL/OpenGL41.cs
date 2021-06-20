using Castaway.Rendering;

namespace Castaway.OpenGL
{
    [Implements("OpenGL-4.1")]
    public class OpenGL41 : OpenGL40
    {
        public override string Name => "OpenGL-4.1";
        
        // TODO Program Pipelines?
    }
}