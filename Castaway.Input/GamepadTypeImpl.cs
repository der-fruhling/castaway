using Castaway.Math;

namespace Castaway.Input;

public abstract class GamepadTypeImpl
{
	protected readonly int[] Joysticks;

	protected GamepadTypeImpl(params int[] joysticks)
	{
		Joysticks = joysticks;
	}

	protected abstract float DeadZone { get; }

	public abstract Vector2 LeftStick { get; }
	public abstract Vector2 RightStick { get; }
	public abstract float LeftTrigger { get; }
	public abstract float RightTrigger { get; }
	public abstract bool LeftBumper { get; }
	public abstract bool RightBumper { get; }
	public abstract bool LeftStickPress { get; }
	public abstract bool RightStickPress { get; }
	public abstract bool A { get; }
	public abstract bool B { get; }
	public abstract bool X { get; }
	public abstract bool Y { get; }
	public abstract bool Up { get; }
	public abstract bool Down { get; }
	public abstract bool Left { get; }
	public abstract bool Right { get; }
	public abstract bool Select { get; }
	public abstract bool Start { get; }

	internal abstract void Read();

	protected virtual float ApplyDeadZone(float x)
	{
		return x >= DeadZone || x <= -DeadZone ? x : 0;
	}
}