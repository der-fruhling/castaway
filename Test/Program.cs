using System;
using System.Threading.Tasks;
using Castaway;
using Castaway.Assets;
using Castaway.Math;
using Castaway.Render;

namespace Test
{
    internal static class Program
    {
        private static Shader _shader;
        private static AssetManager _assets = new DirectoryAssetManager("Assets");
        
        private static async Task Main(string[] args)
        {
            var vertLoad = _assets.LoadAsync("default.v.glsl");
            var fragLoad = _assets.LoadAsync("default.f.glsl");
            
            Cast.Init();
            Console.WriteLine($"Castaway Version {Cast.GetVersion()}");

            Window w;
            using (var c = new Window.Config(800, 600, "cat").AnyGLVersion())
                w = new Window(c);

            var vert = _assets.Get<StringAsset>(await vertLoad);
            var frag = _assets.Get<StringAsset>(await fragLoad);
            
            _shader = new Shader(vert, frag)
            {
                [ShaderAttr.Position2] = "pos", 
                [ShaderAttr.Color3] = "inCol", 
                [0] = "outCol"
            };
            
            _shader.Link();
            _shader.Use();

            _shader["transform"] = new Matrix(4) {[0, 0] = 600f / 800f};
            
            const float a = -.5f, b = .5f;
            var o = new DrawObject()
                .Vertex(a, a, r: 1, g: 0, b: 0)
                .Vertex(a, b, r: 0, g: 1, b: 0)
                .Vertex(b, a, r: 0, g: 0, b: 1)
                .Vertex(b, b, r: 1, g: 1, b: 1)
                .Quad();

            using (w) w.Render = draw =>
            {
                draw.Draw(o);
            };
        }
    }
}