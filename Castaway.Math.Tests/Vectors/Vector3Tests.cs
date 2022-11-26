using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Base.Tests.Vectors;

[TestFixture]
[TestOf(typeof(Vector3))]
public class Vector3Tests
{
	[Test]
	public void TestEmptyNew()
	{
		var v = new Vector3();
		Assert.AreEqual(0d, v.X);
		Assert.AreEqual(0d, v.Y);
		Assert.AreEqual(0d, v.Z);
	}

	[Test]
	public void TestAdd()
	{
		var a = new Vector3(1, 2, 3);
		var b = new Vector3(4, 5, 6);
		var v = a + b;
		Assert.AreEqual(1d + 4d, v.X);
		Assert.AreEqual(2d + 5d, v.Y);
		Assert.AreEqual(3d + 6d, v.Z);
	}

	[Test]
	public void TestSubtract()
	{
		var a = new Vector3(1, 2, 3);
		var b = new Vector3(4, 5, 6);
		var v = a - b;
		Assert.AreEqual(1d - 4d, v.X);
		Assert.AreEqual(2d - 5d, v.Y);
		Assert.AreEqual(3d - 6d, v.Z);
	}

	[Test]
	public void TestMultiply()
	{
		var a = new Vector3(1, 2, 3);
		var b = new Vector3(4, 5, 6);
		var v = a * b;
		Assert.AreEqual(1d * 4d, v.X);
		Assert.AreEqual(2d * 5d, v.Y);
		Assert.AreEqual(3d * 6d, v.Z);
	}

	[Test]
	public void TestDivide()
	{
		var a = new Vector3(1, 2, 3);
		var b = new Vector3(4, 5, 6);
		var v = a / b;
		Assert.AreEqual(1d / 4d, v.X);
		Assert.AreEqual(2d / 5d, v.Y);
		Assert.AreEqual(3d / 6d, v.Z);
	}

	[Test]
	public void TestAddScalar()
	{
		var a = new Vector3(1, 2, 3);
		var v = a + 4f;
		Assert.AreEqual(1d + 4d, v.X);
		Assert.AreEqual(2d + 4d, v.Y);
		Assert.AreEqual(3d + 4d, v.Z);
	}

	[Test]
	public void TestSubtractScalar()
	{
		var a = new Vector3(1, 2, 3);
		var v = a - 4f;
		Assert.AreEqual(1d - 4d, v.X);
		Assert.AreEqual(2d - 4d, v.Y);
		Assert.AreEqual(3d - 4d, v.Z);
	}

	[Test]
	public void TestMultiplyScalar()
	{
		var a = new Vector3(1, 2, 3);
		var v = a * 4f;
		Assert.AreEqual(1d * 4d, v.X);
		Assert.AreEqual(2d * 4d, v.Y);
		Assert.AreEqual(3d * 4d, v.Z);
	}

	[Test]
	public void TestDivideScalar()
	{
		var a = new Vector3(1, 2, 3);
		var v = a / 4f;
		Assert.AreEqual(1d / 4d, v.X);
		Assert.AreEqual(2d / 4d, v.Y);
		Assert.AreEqual(3d / 4d, v.Z);
	}

	[Test]
	public void TestToSpan()
	{
		var a = (double[])new Vector3(1, 2, 3);
		Assert.AreEqual(1d, a[0]);
		Assert.AreEqual(2d, a[1]);
		Assert.AreEqual(3d, a[2]);
	}
}