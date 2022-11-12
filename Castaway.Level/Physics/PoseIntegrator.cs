using System;
using System.Numerics;
using BepuPhysics;
using BepuUtilities;

namespace Castaway.Level.Physics;

public struct PoseIntegrator : IPoseIntegratorCallbacks
{
    public Vector3 Gravity;
    public float LinearDamping, AngularDamping;
    public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

    private Vector3 _gravityDt;
    private float _linearDampingDt, _angularDampingDt;

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
        _gravityDt = Gravity * dt;
        _linearDampingDt = MathF.Pow(MathHelper.Clamp(1 - LinearDamping, 0, 1), dt);
        _angularDampingDt = MathF.Pow(MathHelper.Clamp(1 - AngularDamping, 0, 1), dt);
    }

    public void IntegrateVelocity(int bodyIndex, in RigidPose pose, in BodyInertia localInertia, int workerIndex,
        ref BodyVelocity velocity)
    {
        if (localInertia.InverseMass > 0)
        {
            velocity.Linear = (velocity.Linear + _gravityDt) * _linearDampingDt;
            velocity.Angular = velocity.Angular * _angularDampingDt;
        }
    }
}