using BepuPhysics;
using BepuPhysics.Collidables;

namespace Castaway.Level.Controllers.Colliders;

[ControllerName("Collider.Sphere")]
public class ColliderSphereController : Controller, ICollider
{
	[LevelSerialized("Mass")] public float Mass = 1f;
	[LevelSerialized("Radius")] public float Radius = 0.5f;

	private Sphere Sphere => new(Radius);
	public TypedIndex Shape { get; private set; }
	public BodyInertia Inertia => Sphere.ComputeInertia(Mass);

	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		Shape = parent.Level.PhysicsSimulation.Shapes!.Add(Sphere);
	}

	public override void OnDestroy(LevelObject parent)
	{
		base.OnDestroy(parent);
		parent.Level.PhysicsSimulation.Shapes!.Remove(Shape);
	}
}