using System;
using System.Threading.Tasks;
using Castaway.Base;
using Castaway.Input;
using Castaway.Math;
using Castaway.OpenGL;
using Castaway.Rendering;
using GLFW;

namespace Castaway.Level.OpenGL
{
    [ControllerName("GenericPlayer"), Imports(typeof(OpenGLImpl))]
    public class GenericPlayerController : Controller
    {
        private float _rx, _ry;
        
        [LevelSerialized("MovementSpeed")] public float MovementSpeed = 0.125f;
        [LevelSerialized("RotationSpeed")] public float RotationSpeed = 2f;
        [LevelSerialized("MouseSensitivity")] public float MouseSensitivity = 0.15f;
        [LevelSerialized("Lock.Depth")] public bool DepthLocked = false;
        [LevelSerialized("Lock.Rotation")] public bool RotationLocked = false;
        [LevelSerialized("Lock.Movement")] public bool MovementLocked = false;
        [LevelSerialized("Lock.X")] public bool MovementXLocked = false;
        [LevelSerialized("Lock.Y")] public bool MovementYLocked = false;
        [LevelSerialized("Lock.Z")] public bool MovementZLocked = false;
        
        public override async Task OnUpdate(LevelObject parent)
        {
            await base.OnUpdate(parent);
            var g = Graphics.Current;
            var rotateSpeed = MathEx.ToRadians(RotationSpeed);
            var move = new Vector3(0, 0, 0);

            var gamepadTask = Task.Run(delegate
            {
                // Gamepad
                if (!InputSystem.Gamepad.Valid) return;
                if (!MovementLocked)
                {
                    var moveGamepad = InputSystem.Gamepad.LeftStick * MovementSpeed * g.FrameChange;
                    if (!MovementXLocked) move.X += moveGamepad.X;
                    if (DepthLocked && !MovementYLocked) move.Y += moveGamepad.Y;
                    else if (!MovementZLocked) move.Z += moveGamepad.Y;
                }

                if (RotationLocked) return;
                var rotateGamepad = -InputSystem.Gamepad.RightStick * rotateSpeed * g.FrameChange;
                _rx += (float) rotateGamepad.X;
                _ry += (float) rotateGamepad.Y;
            });

            var keyboardTask = Task.Run(delegate
            {
                // Keyboard
                if (!MovementLocked)
                {
                    if (InputSystem.Keyboard.IsDown(Keys.A)) move.X -= MovementSpeed * g.FrameChange;
                    if (InputSystem.Keyboard.IsDown(Keys.D)) move.X += MovementSpeed * g.FrameChange;
                    if (InputSystem.Keyboard.IsDown(Keys.W)) move.Z -= MovementSpeed * g.FrameChange;
                    if (InputSystem.Keyboard.IsDown(Keys.S)) move.Z += MovementSpeed * g.FrameChange;
                    if (InputSystem.Keyboard.IsDown(Keys.LeftShift)) move.Y -= MovementSpeed * g.FrameChange;
                    if (InputSystem.Keyboard.IsDown(Keys.Space)) move.Y += MovementSpeed * g.FrameChange;
                }

                if (RotationLocked) return;
                if (InputSystem.Keyboard.IsDown(Keys.Up)) _ry += rotateSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.Down)) _ry -= rotateSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.Left)) _rx += rotateSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.Right)) _rx -= rotateSpeed * g.FrameChange;
            });

            var mouseTask = Task.Run(delegate
            {
                if(!InputSystem.Mouse.RawInput) return;
                var pos = InputSystem.Mouse.CursorMovement;
                var x = MathEx.ToRadians((float) pos.X * MouseSensitivity * g.FrameChange);
                var y = MathEx.ToRadians((float) pos.Y * MouseSensitivity * g.FrameChange);
                _rx -= x;
                _ry -= y;
            });

            await gamepadTask;
            await keyboardTask;
            await mouseTask;
            
            _ry = MathEx.Clamp(_ry, MathF.PI / -2, MathF.PI / 2);

            await Task.Run(() =>
            {
                var rotate = new Quaternion(1, 0, 0, 0);
                rotate *= Quaternion.Rotation(Vector3.Up, _rx);
                rotate *= Quaternion.Rotation(Vector3.Right, _ry);
                parent.Position += rotate * new Vector3(move.X, 0, move.Z);
                parent.Position.Y += move.Y;
                parent.Rotation = rotate;
            });
        }
    }
}