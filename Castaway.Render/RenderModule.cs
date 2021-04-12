using Castaway.Assets;
using Castaway.Core;

namespace Castaway.Render
{
    /// <summary>
    /// <see cref="Module"/> subclass for <c>Castaway.Render</c>.
    /// </summary>
    /// <seealso cref="GLSLShaderAssetLoader"/>
    public class RenderModule : Module
    {
        protected override void Start()
        {
            base.Start();
            AssetManager.CreateAssetLoader(new GLSLShaderAssetLoader());
        }
    }
}