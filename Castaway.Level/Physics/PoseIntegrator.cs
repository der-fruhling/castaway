using System;
using System.Numerics;
using BepuPhysics;
using BepuUtilities;

namespace Castaway.Level.Physics;

public struct PoseIntegrator : IPoseIntegratorCallbacks
{
	public Vector3 Gravity;
	public float LinearDamping, AngularDamping;

	public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.ConserveMomentum;
	public bool AllowSubstepsForUnconstrainedBodies => false;
	public bool IntegrateVelocityForKinematics => false;

	private Vector3Wide _gravityDt;
	private Vector<float> _linearDampingDt, _angularDampingDt;

	public PoseIntegrator(Vector3 gravity, float linearDamping = .03f, float angularDamping = .03f) : this()
	{
		Gravity = gravity;
		LinearDamping = linearDamping;
		AngularDamping = angularDamping;
	}

	public void Initialize(Simulation simulation)
	{
	}

	public void PrepareForIntegration(float dt)
	{
		_gravityDt = Vector3Wide.Broadcast(Gravity * dt);
		_linearDampingDt = new Vector<float>(MathF.Pow(MathHelper.Clamp(1 - LinearDamping, 0, 1), dt));
		_angularDampingDt = new Vector<float>(MathF.Pow(MathHelper.Clamp(1 - AngularDamping, 0, 1), dt));
	}

	public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation,
		BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt,
		ref BodyVelocityWide velocity)
	{
		velocity.Linear = (velocity.Linear + _gravityDt) * _linearDampingDt;
		velocity.Angular *= _angularDampingDt;
	}
}