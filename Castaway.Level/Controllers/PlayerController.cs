using System;
using Castaway.Input;
using Castaway.Math;
using Castaway.Rendering;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Castaway.Level.Controllers;

[ControllerName("Player")]
public class PlayerController : Controller
{
	protected double CurrentRotationX, CurrentRotationY;
	[LevelSerialized("Lock.Depth")] public bool DepthLocked { get; set; } = false;
	[LevelSerialized("MouseSensitivity")] public double MouseSensitivity { get; set; } = 0.075;
	[LevelSerialized("Lock.Movement")] public bool MovementLocked { get; set; } = false;

	[LevelSerialized("MovementSpeed")] public double MovementSpeed { get; set; } = 0.35;
	[LevelSerialized("Lock.X")] public bool MovementXLocked { get; set; } = false;
	[LevelSerialized("Lock.Y")] public bool MovementYLocked { get; set; } = false;
	[LevelSerialized("Lock.Z")] public bool MovementZLocked { get; set; } = false;
	[LevelSerialized("Lock.Rotation")] public bool RotationLocked { get; set; } = false;
	[LevelSerialized("RotationSpeed")] public double RotationSpeed { get; set; } = 5;

	public override void OnUpdate(LevelObject parent)
	{
		base.OnUpdate(parent);
		var move = new Vector3(0, 0, 0);

		// Keyboard
		if (!MovementLocked) move = GetMovementVector();
		if (!RotationLocked) UpdateRotation();

		CurrentRotationY = MathEx.Clamp(CurrentRotationY, MathF.PI / -2, MathF.PI / 2);

		var rotate = new Quaternion(1, 0, 0, 0);
		rotate *= Quaternion.Rotation(Vector3.Up, CurrentRotationX);
		rotate *= Quaternion.Rotation(Vector3.Right, CurrentRotationY);
		var parentPos = parent.Position;
		parentPos += rotate * move;
		parent.Position = parentPos;
		parent.Rotation = rotate;
	}

	public virtual Vector3 GetMovementVector()
	{
		var g = Graphics.Current;
		Vector3 vec = new(0, 0, 0);

		if (InputSystem.Gamepad.Valid)
		{
			var moveGamepad = InputSystem.Gamepad.LeftStick * MovementSpeed * g.FrameChange;
			if (!MovementXLocked) vec.X += moveGamepad.X;
			if (DepthLocked && !MovementYLocked) vec.Y += moveGamepad.Y;
			else if (!MovementZLocked) vec.Z += moveGamepad.Y;
		}

		if (InputSystem.Keyboard.IsDown(Keys.A)) vec.X -= MovementSpeed * g.FrameChange;
		if (InputSystem.Keyboard.IsDown(Keys.D)) vec.X += MovementSpeed * g.FrameChange;
		if (InputSystem.Keyboard.IsDown(Keys.W)) vec.Z -= MovementSpeed * g.FrameChange;
		if (InputSystem.Keyboard.IsDown(Keys.S)) vec.Z += MovementSpeed * g.FrameChange;
		if (InputSystem.Keyboard.IsDown(Keys.LeftShift)) vec.Y -= MovementSpeed * g.FrameChange;
		if (InputSystem.Keyboard.IsDown(Keys.Space)) vec.Y += MovementSpeed * g.FrameChange;

		return vec;
	}

	public virtual void UpdateRotation()
	{
		var g = Graphics.Current;
		var rotateSpeed = MathEx.ToRadians(RotationSpeed);

		if (InputSystem.Gamepad.Valid)
		{
			var rotateGamepad = -InputSystem.Gamepad.RightStick * rotateSpeed * g.FrameChange;
			CurrentRotationX += (float)rotateGamepad.X;
			CurrentRotationY += (float)rotateGamepad.Y;
		}

		if (InputSystem.Keyboard.IsDown(Keys.Up)) CurrentRotationY += rotateSpeed * g.FrameChange;
		if (InputSystem.Keyboard.IsDown(Keys.Down)) CurrentRotationY -= rotateSpeed * g.FrameChange;
		if (InputSystem.Keyboard.IsDown(Keys.Left)) CurrentRotationX += rotateSpeed * g.FrameChange;
		if (InputSystem.Keyboard.IsDown(Keys.Right)) CurrentRotationX -= rotateSpeed * g.FrameChange;

		if (!InputSystem.Mouse.RawInput) return;
		var pos = InputSystem.Mouse.CursorMovement;
		var x = MathEx.ToRadians(pos.X * MouseSensitivity * g.FrameChange);
		var y = MathEx.ToRadians(pos.Y * MouseSensitivity * g.FrameChange);
		CurrentRotationX -= x;
		CurrentRotationY -= y;
	}
}