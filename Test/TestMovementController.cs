using Castaway.Input;
using Castaway.Levels;
using Castaway.Math;

public class TestMovementController : Controller
{
    public override void OnUpdate()
    {
        base.OnUpdate();
        var speed = InputSystem.Keyboard.IsPressed(Keys.LeftControl) ? 0.15f : .075f;
        const float lookSpeed = 3f;

        var movement = Vector3.Zero;
        if (InputSystem.Keyboard.IsPressed(Keys.W)) movement.Z += speed;
        if (InputSystem.Keyboard.IsPressed(Keys.S)) movement.Z -= speed;
        if (InputSystem.Keyboard.IsPressed(Keys.D)) movement.X += speed;
        if (InputSystem.Keyboard.IsPressed(Keys.A)) movement.X -= speed;
        if (InputSystem.Keyboard.IsPressed(Keys.Spacebar)) movement.Y += speed;
        if (InputSystem.Keyboard.IsPressed(Keys.LeftShift)) movement.Y -= speed;

        if (InputSystem.Keyboard.IsPressed(Keys.Up) && parent.Rotation.X < 87.5f)    parent.Rotation.X += lookSpeed;
        if (InputSystem.Keyboard.IsPressed(Keys.Down) && parent.Rotation.X > -87.5f)  parent.Rotation.X -= lookSpeed;
        if (InputSystem.Keyboard.IsPressed(Keys.Left))  parent.Rotation.Y += lookSpeed;
        if (InputSystem.Keyboard.IsPressed(Keys.Right)) parent.Rotation.Y -= lookSpeed;

        parent.Position -= Matrix4.RotateYDeg(-parent.Rotation.Y) * movement;
    }
}
