using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;

namespace Castaway.Level.Physics;

internal struct NarrowPhase : INarrowPhaseCallbacks
{
	public SpringSettings Springiness { get; set; }
	public float MaximumRecoveryVelocity { get; set; }
	public float FrictionCoefficient { get; set; }

	public NarrowPhase(SpringSettings springiness, float maximumRecoveryVelocity, float frictionCoefficient)
	{
		Springiness = springiness;
		MaximumRecoveryVelocity = maximumRecoveryVelocity;
		FrictionCoefficient = frictionCoefficient;
	}

	public void Initialize(Simulation simulation)
	{
		if (Springiness.AngularFrequency != 0 || Springiness.TwiceDampingRatio != 0) return;
		Springiness = new SpringSettings(30, 1);
		MaximumRecoveryVelocity = 2f;
		FrictionCoefficient = 1f;
	}

	public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b,
		ref float speculativeMargin)
	{
		// TODO? Kinematic?
		return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
	}

	public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold,
		out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
	{
		pairMaterial.FrictionCoefficient = FrictionCoefficient;
		pairMaterial.MaximumRecoveryVelocity = MaximumRecoveryVelocity;
		pairMaterial.SpringSettings = Springiness;
		return true;
	}

	public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
	{
		return true;
	}

	public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB,
		ref ConvexContactManifold manifold)
	{
		return true;
	}

	public void Dispose()
	{
	}
}