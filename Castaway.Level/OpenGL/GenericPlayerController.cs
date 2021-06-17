using System;
using Castaway.Math;
using Castaway.OpenGL.Input;
using Castaway.Rendering;
using GLFW;

namespace Castaway.Level.OpenGL
{
    public class GenericPlayerController : EmptyController
    {
        private float _rx, _ry;
        
        [LevelSerialized("MovementSpeed")] public float MovementSpeed = 0.125f;
        [LevelSerialized("RotationSpeed")] public float RotationSpeed = 2f;
        [LevelSerialized("Lock.Depth")] public bool DepthLocked = false;
        [LevelSerialized("Lock.Rotation")] public bool RotationLocked = false;
        [LevelSerialized("Lock.Movement")] public bool MovementLocked = false;
        [LevelSerialized("Lock.X")] public bool MovementXLocked = false;
        [LevelSerialized("Lock.Y")] public bool MovementYLocked = false;
        [LevelSerialized("Lock.Z")] public bool MovementZLocked = false;
        
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
                    if(!MovementXLocked) move.X += moveGamepad.X;
                    if(DepthLocked && !MovementYLocked) move.Y += moveGamepad.Y;
                    else if(!MovementZLocked) move.Z += moveGamepad.Y;
                }

                if (!RotationLocked)
                {
                    var rotateGamepad = -InputSystem.Gamepad.RightStick * rotateSpeed * g.FrameChange;
                    _rx += rotateGamepad.X;
                    _ry += rotateGamepad.Y;
                }
            }

            // Keyboard
            if (!MovementLocked)
            {
                if (InputSystem.Keyboard.IsDown(Keys.A)) move.X -= MovementSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.D)) move.X += MovementSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.W)) move.Z -= MovementSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.S)) move.Z += MovementSpeed * g.FrameChange;
            }

            if (!RotationLocked)
            {
                if (InputSystem.Keyboard.IsDown(Keys.Up)) _ry += rotateSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.Down)) _ry -= rotateSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.Left)) _rx += rotateSpeed * g.FrameChange;
                if (InputSystem.Keyboard.IsDown(Keys.Right)) _rx -= rotateSpeed * g.FrameChange;
            }

            _ry = MathEx.Clamp(_ry, MathF.PI / -2, MathF.PI / 2);

            var rotate = new Quaternion(1, 0, 0, 0);
            rotate *= Quaternion.Rotation(Vector3.Up, _rx);
            rotate *= Quaternion.Rotation(Vector3.Right, _ry);
            parent.Position += rotate * move;
            parent.Rotation = rotate;
        }
    }
}