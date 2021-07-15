using System;
using BepuPhysics;
using BepuPhysics.Collidables;
using Castaway.Math;

namespace Castaway.Level.Controllers.Colliders
{
    [ControllerName("Collider.Box")]
    public class ColliderBoxController : Controller
    {
        private bool _running;

        private Simulation? _simulation;

        public BodyHandle Body;
        [LevelSerialized("Mass")] public float Mass = 1f;
        [LevelSerialized("Mode")] public PhysicsMode PhysicsMode = PhysicsMode.Dynamic;
        public TypedIndex Shape;
        [LevelSerialized("Size")] public Vector3 Size = new(1, 1, 1);
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
                            new System.Numerics.Vector3((float) value.X, (float) value.Y, (float) value.Z);
                        _simulation.Bodies.ApplyDescription(Body, desc);
                        break;
                    }
                    case PhysicsMode.Static:
                    {
                        _simulation.Statics.GetDescription(Static, out var desc);
                        desc.Pose.Position =
                            new System.Numerics.Vector3((float) value.X, (float) value.Y, (float) value.Z);
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
                            (float) value.X, (float) value.Y, (float) value.Z, (float) value.W);
                        _simulation.Bodies.ApplyDescription(Body, desc);
                        break;
                    }
                    case PhysicsMode.Static:
                    {
                        _simulation.Statics.GetDescription(Static, out var desc);
                        desc.Pose.Orientation = new System.Numerics.Quaternion(
                            (float) value.X, (float) value.Y, (float) value.Z, (float) value.W);
                        _simulation.Statics.ApplyDescription(Static, desc);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override Vector3 Scale
        {
            set => base.Scale = value;
        }

        public override void OnInit(LevelObject parent)
        {
            base.OnInit(parent);
            var sim = parent.Level.PhysicsSimulation;
            var shape = new Box((float) Size.X, (float) Size.Y, (float) Size.Z);
            shape.ComputeInertia(Mass, out var inertia);
            Shape = sim.Shapes.Add(shape);
            var pos = parent.Position;
            switch (PhysicsMode)
            {
                case PhysicsMode.Dynamic:
                    Body = sim.Bodies.Add(BodyDescription.CreateDynamic(
                        new System.Numerics.Vector3((float) pos.X, (float) pos.Y, (float) pos.Z),
                        inertia,
                        new CollidableDescription(Shape, 0.1f),
                        new BodyActivityDescription(0.01f)));
                    break;
                case PhysicsMode.Static:
                    Static = sim.Statics.Add(new StaticDescription(
                        new System.Numerics.Vector3((float) pos.X, (float) pos.Y, (float) pos.Z),
                        new CollidableDescription(Shape, 0.1f)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _simulation = sim;
            _running = true;
        }

        public override void OnDestroy(LevelObject parent)
        {
            _running = false;
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

            _simulation?.Shapes?.Remove(Shape);
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
}