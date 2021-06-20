using System;
using System.Linq;
using Castaway.Assets;
using Castaway.Base;
using Castaway.Level;
using Castaway.Rendering;
using Serilog;

namespace Castaway.OpenGL.Controllers
{
    public enum BuiltinShader
    {
        Default,
        TexturedDefault,
        NoTransform,
        TexturedNoTransform
    }

    [ControllerName("Shader")]
    public class ShaderController : EmptyController
    {
        [LevelSerialized("Builtin")] public BuiltinShader BuiltinShaderName;

        [LevelSerialized("Asset")] public string AssetName = string.Empty;
        public ShaderObject? Shader;

        private ShaderObject? _previous;

        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            if (AssetName.Any())
            {
                Shader = AssetLoader.Loader!.GetAssetByName(AssetName).To<ShaderObject>();
            }
            else
            {
                Shader = BuiltinShaderName switch
                {
                    BuiltinShader.Default => BuiltinShaders.Default,
                    BuiltinShader.TexturedDefault => BuiltinShaders.DefaultTextured,
                    BuiltinShader.NoTransform => BuiltinShaders.Direct,
                    BuiltinShader.TexturedNoTransform => BuiltinShaders.DirectTextured,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public override void PreRender(LevelObject camera, LevelObject parent)
        {
            base.PreRender(camera, parent);
            var g = Graphics.Current;
            _previous = g.BoundShader!;
            if (Shader == null) throw new InvalidOperationException($"Unloaded shader {BuiltinShaderName}");
            Shader.Bind();
            g.SetFloatUniform(Shader, UniformType.TransformPerspective, camera.Get<CameraController>()!.PerspectiveTransform);
            g.SetFloatUniform(Shader, UniformType.TransformView, camera.Get<CameraController>()!.ViewTransform);
            g.SetFloatUniform(Shader, UniformType.ViewPosition, camera.Position);
            LightResolver.Push();
        }

        public override void PostRender(LevelObject camera, LevelObject parent)
        {
            base.PostRender(camera, parent);
            _previous?.Bind();
        }
    }
}