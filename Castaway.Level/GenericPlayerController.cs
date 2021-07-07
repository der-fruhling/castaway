using System;
using Castaway.Input;
using Castaway.Math;
using Castaway.Rendering;
using GLFW;

namespace Castaway.Level.OpenGL
{
    [ControllerName("GenericPlayer")]
    public class GenericPlayerController : Controller
    {
        private double _rx, _ry;
        [LevelSerialized("Lock.Depth")] public bool DepthLocked = false;
        [LevelSerialized("MouseSensitivity")] public double MouseSensitivity = 0.075;
        [LevelSerialized("Lock.Movement")] public bool MovementLocked = false;

        [LevelSerialized("MovementSpeed")] public double MovementSpeed = 0.35;
        [LevelSerialized("Lock.X")] public bool MovementXLocked = false;
        [LevelSerialized("Lock.Y")] public bool MovementYLocked = false;
        [LevelSerialized("Lock.Z")] public bool MovementZLocked = false;
        [LevelSerialized("Lock.Rotation")] public bool RotationLocked = false;
        [LevelSerialized("RotationSpeed")] public double RotationSpeed = 5;

        public override void OnUpdate(LevelObject parent)
        {
            base.OnUpdate(parent);
            var g = Graphics.Current;
            var rotateSpeed = MathEx.ToRadians(RotationSpeed);
            var move = new Vector3(0, 0, 0);

            // Gamepad
            if (InputSystem.Gamepad.Valid)
            {
                if (!MovementLocked)
                {
                    var moveGamepad = InputSystem.Gamepad.LeftStick * MovementSpeed * g.FrameChange;
                    if (!MovementXLocked) move.X += moveGamepad.X;
                    if (DepthLocked && !MovementYLocked) move.Y += moveGamepad.Y;
                    else if (!MovementZLocked) move.Z += moveGamepad.Y;
                }

                var rotateGamepad = -InputSystem.Gamepad.RightStick * rotateSpeed * g.FrameChange;
                _rx += (float) rotateGamepad.X;
                _ry += (float) rotateGamepad.Y;
            }


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

            if (InputSystem.Keyboard.IsDown(Keys.Up)) _ry += rotateSpeed * g.FrameChange;
            if (InputSystem.Keyboard.IsDown(Keys.Down)) _ry -= rotateSpeed * g.FrameChange;
            if (InputSystem.Keyboard.IsDown(Keys.Left)) _rx += rotateSpeed * g.FrameChange;
            if (InputSystem.Keyboard.IsDown(Keys.Right)) _rx -= rotateSpeed * g.FrameChange;

            if (InputSystem.Mouse.RawInput)
            {
                var pos = InputSystem.Mouse.CursorMovement;
                var x = MathEx.ToRadians(pos.X * MouseSensitivity * g.FrameChange);
                var y = MathEx.ToRadians(pos.Y * MouseSensitivity * g.FrameChange);
                _rx -= x;
                _ry -= y;
            }

            _ry = MathEx.Clamp(_ry, MathF.PI / -2, MathF.PI / 2);

            var rotate = new Quaternion(1, 0, 0, 0);
            rotate *= Quaternion.Rotation(Vector3.Up, _rx);
            rotate *= Quaternion.Rotation(Vector3.Right, _ry);
            parent.Position += rotate * new Vector3(move.X, 0, move.Z);
            parent.Position.Y += move.Y;
            parent.Rotation = rotate;
        }
    }
}