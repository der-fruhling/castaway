using System;
using Castaway;
using Castaway.Level;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.OpenGL.Input;
using GLFW;
using static Castaway.Assets.AssetLoader;

namespace Test
{
    internal static class Program
    {
        private static void Main()
        {
            // Load assets from config.json
            CastawayEngine.Init();

            // Graphics setup.
            using var g = OpenGL.Setup();
            
            // Window setup
            var window = g.CreateWindowWindowed("name", 800, 600, false);
            g.Bind(window);

            // Level setup
            var level = new Level(Loader!.GetAssetByName("/test_level.xml"));

            // Start level.
            level.Start();
            
            // Show window.
            g.ShowWindow(window);
            
            // Rendering loop!
            while (g.WindowShouldBeOpen(window))
            {
                g.StartFrame();
                level.Render();
                g.FinishFrame(window);
                level.Update();
                if(InputSystem.Gamepad.Valid)
                    Console.WriteLine(InputSystem.Gamepad);
                else
                    Console.WriteLine("(No valid Gamepad)");
                var obj = level.Get("Camera");
                const float speed = 0.125f;
                var move = new Vector3(0, 0, 0);
                
                // Gamepad
                if (InputSystem.Gamepad.Valid)
                {
                    var moveGamepad = InputSystem.Gamepad.LeftStick * speed;
                    move.X += moveGamepad.X;
                    move.Y -= moveGamepad.Y;
                    move.Z += (InputSystem.Gamepad.RightTrigger - InputSystem.Gamepad.LeftTrigger) * speed;
                }
                
                // Keyboard
                if (InputSystem.Keyboard.IsDown(Keys.A)) move.X -= speed;
                if (InputSystem.Keyboard.IsDown(Keys.D)) move.X += speed;
                if (InputSystem.Keyboard.IsDown(Keys.S)) move.Y -= speed;
                if (InputSystem.Keyboard.IsDown(Keys.W)) move.Y += speed;
                if (InputSystem.Keyboard.IsDown(Keys.Q)) move.Z -= speed;
                if (InputSystem.Keyboard.IsDown(Keys.E)) move.Z += speed;

                obj.Position += move;
            }
            
            level.End();

            g.Destroy(window); // Absolutely ensure that the window is
                               // destroyed last. If it isn't all destroy
                               // operations after it will fail.
        }
    }
}