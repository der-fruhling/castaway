using System;
using BepuPhysics;
using BepuPhysics.Collidables;
using Castaway.Level.Controllers.Colliders;
using Castaway.Math;

namespace Castaway.Level.Controllers;

[ControllerName("Physics")]
public class PhysicsController : Controller
{
	private Simulation? _simulation;

	public BodyHandle Body;
	[LevelSerialized("Mode")] public PhysicsMode PhysicsMode = PhysicsMode.Dynamic;
	public StaticHandle Static;

	public override Vector3 Position
	{
		set
		{
			if (_simulation == null) return;
			switch (PhysicsMode)
			{
				case PhysicsMode.Dynamic:
				{
					_simulation.Bodies.GetDescription(Body, out var desc);
					desc.Pose.Position =
						new System.Numerics.Vector3((float)value.X, (float)value.Y, (float)value.Z);
					_simulation.Bodies.ApplyDescription(Body, desc);
					break;
				}
				case PhysicsMode.Static:
				{
					_simulation.Statics.GetDescription(Static, out var desc);
					desc.Pose.Position =
						new System.Numerics.Vector3((float)value.X, (float)value.Y, (float)value.Z);
					_simulation.Statics.ApplyDescription(Static, desc);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public override Quaternion Rotation
	{
		set
		{
			if (_simulation == null) return;
			switch (PhysicsMode)
			{
				case PhysicsMode.Dynamic:
				{
					_simulation.Bodies.GetDescription(Body, out var desc);
					desc.Pose.Orientation = new System.Numerics.Quaternion(
						(float)value.X, (float)value.Y, (float)value.Z, (float)value.W);
					_simulation.Bodies.ApplyDescription(Body, desc);
					break;
				}
				case PhysicsMode.Static:
				{
					_simulation.Statics.GetDescription(Static, out var desc);
					desc.Pose.Orientation = new System.Numerics.Quaternion(
						(float)value.X, (float)value.Y, (float)value.Z, (float)value.W);
					_simulation.Statics.ApplyDescription(Static, desc);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		var sim = parent.Level.PhysicsSimulation;
		var collider = parent.Controllers.Find(controller => controller is ICollider) as ICollider
		               ?? throw new InvalidOperationException("No collider found");
		var inertia = collider.Inertia;
		var shape = collider.Shape;
		var pos = parent.Position;
		var rot = parent.Rotation;
		switch (PhysicsMode)
		{
			case PhysicsMode.Dynamic:
			{
				var desc = BodyDescription.CreateDynamic(
					new RigidPose(
						new System.Numerics.Vector3(
							(float)pos.X,
							(float)pos.Y,
							(float)pos.Z),
						new System.Numerics.Quaternion(
							(float)rot.X,
							(float)rot.Y,
							(float)rot.Z,
							(float)rot.W)),
					inertia,
					new CollidableDescription(shape, 0.1f),
					new BodyActivityDescription(0.01f));
				Body = sim.Bodies.Add(desc);
				break;
			}
			case PhysicsMode.Static:
			{
				var desc = new StaticDescription(
					new System.Numerics.Vector3(
						(float)pos.X,
						(float)pos.Y,
						(float)pos.Z),
					new System.Numerics.Quaternion(
						(float)rot.X,
						(float)rot.Y,
						(float)rot.Z,
						(float)rot.W),
					shape,
					ContinuousDetection.Passive);
				Static = sim.Statics.Add(desc);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
		}

		_simulation = sim;
	}

	public override void OnDestroy(LevelObject parent)
	{
		base.OnDestroy(parent);
		switch (PhysicsMode)
		{
			case PhysicsMode.Dynamic:
				_simulation?.Bodies?.Remove(Body);
				break;
			case PhysicsMode.Static:
				_simulation?.Statics?.Remove(Static);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public override void PostUpdate(LevelObject parent)
	{
		base.PostUpdate(parent);
		System.Numerics.Vector3 p;
		var sim = parent.Level.PhysicsSimulation;
		switch (PhysicsMode)
		{
			case PhysicsMode.Dynamic:
			{
				sim.Bodies.GetDescription(Body, out var desc);
				p = desc.Pose.Position;
				parent.Position = new Vector3(p.X, p.Y, p.Z);
				var r = desc.Pose.Orientation;
				parent.Rotation = new Quaternion(r.W, r.X, r.Y, r.Z);
				break;
			}
			case PhysicsMode.Static:
			{
				sim.Statics.GetDescription(Static, out var desc);
				p = desc.Pose.Position;
				parent.Position = new Vector3(p.X, p.Y, p.Z);
				var r = desc.Pose.Orientation;
				parent.Rotation = new Quaternion(r.W, r.X, r.Y, r.Z);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}