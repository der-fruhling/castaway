using System.Collections.Generic;
using System.Threading.Tasks;
using Castaway.Assets;
using Castaway.Core;
using Castaway.Input;
using Castaway.Native;
using Castaway.Render;
using Castaway.Window;
using static Castaway.Assets.Properties<Castaway.Exec.Cast.CastProperty>;
using static Castaway.Exec.Cast.CastProperty;

namespace Castaway.Exec
{
    public static class Cast
    {
        public enum CastProperty
        {
            DefaultWindowWidth,
            DefaultWindowHeight,
            WindowTitle,
            Fullscreen,
        }

        public static Properties<CastProperty> Properties;
        public static GLFWWindow Window;
        public static bool DebugMode { get; set; }

        private static void SetupProperties()
        {
            var settings = new Dictionary<CastProperty, Settings>
            {
                [DefaultWindowWidth] = new Settings(PropertyReaders.Int32),
                [DefaultWindowHeight] = new Settings(PropertyReaders.Int32),
                [WindowTitle] = new Settings(PropertyReaders.String)
            };
            Properties = new Properties<CastProperty>(settings);
            Properties.Load(AssetManager.Get<string>("/Cast.properties.txt")!.Split('\n'));
        }
        
        private static void SetupWindow()
        {
            GLFWWindow.Init();
            Window = GLFWWindow.Windowed(
                Properties.Get<int>(DefaultWindowWidth),
                Properties.Get<int>(DefaultWindowHeight),
                Properties.Get<string>(WindowTitle));
            Events.Finish += DrawDebugMode;
            Events.Finish += Window.Finish;
            Events.ShouldClose = () => Window.ShouldClose;
            unsafe { Window.KeyCallback = InputSystem.Keyboard.Handler; }
        }

        private static void DrawDebugMode()
        {
            if(!DebugMode) return;
        }
        
        public static async Task Start()
        {
            await Task.Run(() =>
            {
                Modules.Use<AssetsModule>();
                Modules.Use<RenderModule>();
                Events.PreInit += SetupProperties;
                Events.PreInit += SetupWindow;
                Events.CloseNormally += GLFW.glfwTerminate;
                Events.Loop();
            });
        }
    }
}
