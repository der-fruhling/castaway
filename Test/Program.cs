using Castaway.Assets;
using Castaway.Base;
using Castaway.Input;
using Castaway.Level;
using Castaway.Level.OpenGL;
using Castaway.OpenGL;
using Castaway.Rendering;
using GLFW;
using Serilog;
using Window = Castaway.Rendering.Window;

namespace Test
{
    [Imports(typeof(OpenGLImpl), typeof(ShaderController))]
    internal class Program : IApplication
    {
        private Level _level;

        private Window _window;
#pragma warning disable 649
        private Graphics g;
#pragma warning restore 649

        public bool ShouldStop => _window.ShouldClose;

        public void Init()
        {
            // Perform global initialization
            AssetLoader.Init();

            _window = new Window(800, 600, "name", false);
            _window.Bind();

            Log.Information("{@Shader}", BuiltinShaders.DirectTextured);

            g = _window.GL;
            g.ExpectedFrameTime = 1f / 144f;

            _level = new Level(AssetLoader.Loader!.GetAssetByName("/test_level.xml"));

            _level.Start();
            _window.Visible = true;

            InputSystem.Mouse.RawInput = true;
        }

        public void StartFrame()
        {
            g.StartFrame();
        }

        public void Render()
        {
            _level.Render();
        }

        public void Update()
        {
            _level.Update();
        }

        public void EndFrame()
        {
            g.FinishFrame(_window);
            if (InputSystem.Gamepad.Valid && InputSystem.Gamepad.Start || InputSystem.Keyboard.IsDown(Keys.Escape))
                _window.ShouldClose = true;
        }

        public void Recover(RecoverableException e)
        {
            g.Clear();
            g.FinishFrame(_window);
        }

        public void Dispose()
        {
            _window.Visible = false;
            _level.End();
            _window.Dispose();
        }

        private static int Main()
        {
            return CastawayGlobal.Run<Program>();
        }
    }
}