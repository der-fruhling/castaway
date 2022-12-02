using System;
using System.Linq;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;

namespace Castaway.Level.Controllers.Colliders;

[ControllerName("Collider.Mesh")]
public class ColliderMeshController : Controller, ICollider
{
	public enum MeshOpenness
	{
		Closed,
		Open
	}

	[LevelSerialized("Mass")] public float Mass { get; set; } = 1f;
	[LevelSerialized("Openness")] public MeshOpenness Openness { get; set; } = MeshOpenness.Closed;
	[LevelSerialized("Size")] public Vector3 Size { get; set; } = new(1, 1, 1);

	private Mesh Mesh { get; set; }
	public TypedIndex Shape { get; private set; }

	public BodyInertia Inertia => Openness switch
	{
		MeshOpenness.Closed => Mesh.ComputeClosedInertia(Mass),
		MeshOpenness.Open => Mesh.ComputeOpenInertia(Mass),
		_ => throw new ArgumentOutOfRangeException()
	};

	private static float Ccw(Vector3 a, Vector3 b, Vector3 c)
	{
		return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
	}

	public override void OnInit(LevelObject parent)
	{
		base.OnInit(parent);
		var cont = parent.Controllers.Find(c => c is MeshController) as MeshController
		           ?? throw new InvalidOperationException("Collider.Mesh requires Mesh");
		var mesh = cont.Mesh!.Value;
		var tris = mesh.Elements.Chunk(3).Select(idxs =>
		{
			var vtx = idxs.Reverse().Select(idx => (Vector3)mesh.Vertices[idx].Position).ToArray();
			if (Ccw(vtx[0], vtx[1], vtx[2]) < 0) vtx = vtx.Reverse().ToArray();
			Ccw(vtx[0], vtx[1], vtx[2]);
			return new Triangle(vtx[0], vtx[1], vtx[2]);
		}).ToArray();

		var sim = parent.Level.PhysicsSimulation;
		sim.BufferPool!.Take(tris.Length, out Buffer<Triangle> buf);
		buf.CopyFrom(tris, 0, 0, tris.Length);
		Mesh = new Mesh(buf, Vector3.One, sim.BufferPool);
		Shape = parent.Level.PhysicsSimulation.Shapes!.Add(Mesh);
	}

	public override void OnDestroy(LevelObject parent)
	{
		base.OnDestroy(parent);
		parent.Level.PhysicsSimulation.Shapes!.Remove(Shape);
	}
}