using BepuPhysics;
using BepuPhysics.Collidables;

namespace Castaway.Level.Controllers.Colliders;

public interface ICollider
{
	TypedIndex Shape { get; }
	BodyInertia Inertia { get; }
}