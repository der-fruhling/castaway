using BepuPhysics;
using BepuPhysics.Collidables;
using Castaway.Math;

namespace Castaway.Level.Controllers.Colliders;

[ControllerName("Collider.Box")]
public class ColliderBoxController : Controller, ICollider
{
	[LevelSerialized("Mass")] public float Mass = 1f;
	[LevelSerialized("Size")] public Vector3 Size = new(1, 1, 1);

	private Box Box => new((float)Size.X, (float)Size.Y, (float)Size.Z);
	public TypedIndex Shape { get; private set; }
	public BodyInertia Inertia => Box.ComputeInertia(Mass);

	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		Shape = parent.Level.PhysicsSimulation.Shapes.Add(Box);
	}

	public override void OnDestroy(LevelObject parent)
	{
		base.OnDestroy(parent);
		parent.Level.PhysicsSimulation.Shapes.Remove(Shape);
	}
}