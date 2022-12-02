using BepuPhysics;
using BepuPhysics.Collidables;

namespace Castaway.Level.Controllers.Colliders;

[ControllerName("Collider.Cylinder")]
public class ColliderCylinderController : Controller, ICollider
{
	[LevelSerialized("Length")] public float Length { get; set; }
	[LevelSerialized("Mass")] public float Mass { get; set; }
	[LevelSerialized("Radius")] public float Radius { get; set; }

	private Cylinder Cylinder => new(Radius, Length);
	public TypedIndex Shape { get; private set; }
	public BodyInertia Inertia => Cylinder.ComputeInertia(Mass);

	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		Shape = parent.Level.PhysicsSimulation.Shapes!.Add(Cylinder);
	}

	public override void OnDestroy(LevelObject parent)
	{
		base.OnDestroy(parent);
		parent.Level.PhysicsSimulation.Shapes!.Remove(Shape);
	}
}