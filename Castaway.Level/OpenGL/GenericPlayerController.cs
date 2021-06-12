using Castaway.Math;
using Castaway.OpenGL.Input;
using GLFW;

namespace Castaway.Level.OpenGL
{
    public class GenericPlayerController : EmptyController
    {
        private float _rx, _ry;
        
        public override void OnUpdate(LevelObject parent)
        {
            base.OnUpdate(parent);
            const float speed = 0.125f;
            var rotateSpeed = MathEx.ToRadians(2);
            var move = new Vector3(0, 0, 0);
            
            // Gamepad
            if (InputSystem.Gamepad.Valid)
            {
                var moveGamepad = InputSystem.Gamepad.LeftStick * speed;
                move.X += moveGamepad.X;
                move.Z += moveGamepad.Y;

                var rotateGamepad = -InputSystem.Gamepad.RightStick * rotateSpeed;
                _rx += rotateGamepad.X;
                _ry += rotateGamepad.Y;
            }
            
            // Keyboard
            if (InputSystem.Keyboard.IsDown(Keys.A)) move.X -= speed;
            if (InputSystem.Keyboard.IsDown(Keys.D)) move.X += speed;
            if (InputSystem.Keyboard.IsDown(Keys.W)) move.Z -= speed;
            if (InputSystem.Keyboard.IsDown(Keys.S)) move.Z += speed;

            var rotate = new Quaternion(1, 0, 0, 0);
            rotate *= Quaternion.Rotation(Vector3.Up, _rx);
            rotate *= Quaternion.Rotation(Vector3.Right, _ry);
            parent.Position += rotate * move;
            parent.Rotation = rotate;
        }
    }
}