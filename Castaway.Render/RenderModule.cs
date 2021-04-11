using Castaway.Assets;
using Castaway.Core;

namespace Castaway.Render
{
    public class RenderModule : Module
    {
        protected override void Start()
        {
            base.Start();
            AssetManager.CreateAssetLoader(new GLSLShaderAssetLoader());
        }
    }
}