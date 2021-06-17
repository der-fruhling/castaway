using System;
using Castaway.OpenGL;
using Castaway.Rendering;

namespace Castaway.Level.OpenGL
{
    public enum BuiltinShader
    {
        Default,
        TexturedDefault,
        NoTransform,
        TexturedNoTransform
    }

    public class ShaderController : EmptyController
    {
        [LevelSerialized("Name")] public BuiltinShader BuiltinShaderName;

        public ShaderObject? Shader;

        private ShaderObject? _previous;

        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            Shader = BuiltinShaderName switch
            {
                BuiltinShader.Default => BuiltinShaders.Default,
                BuiltinShader.TexturedDefault => BuiltinShaders.DefaultTextured,
                BuiltinShader.NoTransform => BuiltinShaders.Direct,
                BuiltinShader.TexturedNoTransform => BuiltinShaders.DirectTextured,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void PreRender(LevelObject camera, LevelObject parent)
        {
            base.PreRender(camera, parent);
            var g = Graphics.Current;
            _previous = g.BoundShader!;
            if (Shader == null) throw new InvalidOperationException($"Unloaded shader {BuiltinShaderName}");
            Shader.Bind();
            g.SetUniform(Shader, UniformType.TransformPerspective, camera.Get<CameraController>()!.PerspectiveTransform);
            g.SetUniform(Shader, UniformType.TransformView, camera.Get<CameraController>()!.ViewTransform);
            g.SetUniform(Shader, UniformType.ViewPosition, camera.Position);
            LightResolver.Push();
        }

        public override void PostRender(LevelObject camera, LevelObject parent)
        {
            base.PostRender(camera, parent);
            _previous?.Bind();
        }
    }
}