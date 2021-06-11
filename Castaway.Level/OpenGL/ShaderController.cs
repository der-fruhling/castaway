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

        public ShaderProgram Shader;

        private ShaderProgram? _previous;

        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            Shader = BuiltinShaderName switch
            {
                BuiltinShader.Default => BuiltinShaders.Default,
                BuiltinShader.TexturedDefault => BuiltinShaders.DefaultTextured,
                BuiltinShader.NoTransform => BuiltinShaders.NoTransform,
                BuiltinShader.TexturedNoTransform => BuiltinShaders.NoTransformTextured,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void PreRender(LevelObject camera, LevelObject parent)
        {
            base.PreRender(camera, parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            _previous = g.BoundProgram;
            g.Bind(Shader);
            g.SetUniform(Shader, UniformType.TransformPerspective, camera.Get<CameraController>()!.PerspectiveTransform);
            g.SetUniform(Shader, UniformType.TransformView, camera.Get<CameraController>()!.ViewTransform);
            g.SetUniform(Shader, UniformType.ViewPosition, camera.Position);
            LightResolver.Push();
        }

        public override void PostRender(LevelObject camera, LevelObject parent)
        {
            base.PostRender(camera, parent);
            var g = Castaway.OpenGL.OpenGL.Get();
            if(_previous != null) g.Bind(_previous.Value);
        }
    }
}