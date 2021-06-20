using Castaway.Assets;
using Castaway.Base;
using Castaway.Level;
using Castaway.OpenGL;
using Castaway.Rendering;
using Castaway.Rendering.Input;
using GLFW;
using Serilog.Events;
using Window = Castaway.Rendering.Window;

namespace Test
{
    [Imports(typeof(OpenGLImpl))]
    internal class Program : IApplication
    {
        private static int Main()
        {
            CastawayGlobal.LevelSwitch.MinimumLevel = LogEventLevel.Information;
            return CastawayGlobal.Run<Program>();
        }

        private Window _window;
        private Level _level;
#pragma warning disable 649
        private Graphics g;
#pragma warning restore 649

        public bool ShouldStop => _window.ShouldClose;

        public Program()
        {
            // Perform global initialization
            AssetLoader.Init();
            
            _window = new Window(800, 600, "name", false);
            _window.Bind();

            g = _window.GL;
            g.ExpectedFrameTime = 1f / 144f;

            _level = new Level(AssetLoader.Loader!.GetAssetByName("/test_level.xml"));

            _level.Start();
            _window.Visible = true;
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
    }
}