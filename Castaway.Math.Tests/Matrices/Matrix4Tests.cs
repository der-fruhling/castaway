using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Base.Tests.Matrices;

[TestFixture]
[TestOf(typeof(Matrix4))]
public class Matrix4Tests
{
	[Test]
	public void TestIdent()
	{
		var m = Matrix4.Ident;
		Assert.AreEqual(new Matrix4(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1), m);
	}

	[Test]
	public void TestAdd()
	{
		var a = new Matrix4(
			1, 2, 3, 4,
			2, 3, 4, 5,
			3, 4, 5, 6,
			4, 5, 6, 7);
		var b = new Matrix4(
			2, 3, 4, 5,
			3, 4, 5, 6,
			4, 5, 6, 7,
			5, 6, 7, 8);
		var m = a + b;
		Assert.AreEqual(new Matrix4(
			1 + 2, 2 + 3, 3 + 4, 4 + 5,
			2 + 3, 3 + 4, 4 + 5, 5 + 6,
			3 + 4, 4 + 5, 5 + 6, 6 + 7,
			4 + 5, 5 + 6, 6 + 7, 7 + 8), m);
	}

	[Test]
	public void TestSubtract()
	{
		var a = new Matrix4(
			1, 2, 3, 4,
			2, 3, 4, 5,
			3, 4, 5, 6,
			4, 5, 6, 7);
		var b = new Matrix4(
			2, 3, 4, 5,
			3, 4, 5, 6,
			4, 5, 6, 7,
			5, 6, 7, 8);
		var m = a - b;
		Assert.AreEqual(new Matrix4(
			1 - 2, 2 - 3, 3 - 4, 4 - 5,
			2 - 3, 3 - 4, 4 - 5, 5 - 6,
			3 - 4, 4 - 5, 5 - 6, 6 - 7,
			4 - 5, 5 - 6, 6 - 7, 7 - 8), m);
	}

	[Test]
	public void TestMultiplyVector()
	{
		var a = new Matrix4(
			2, 0, 0, 1,
			0, 2, 0, 2,
			0, 0, 2, 3,
			0, 0, 0, 1);
		var v = a * new Vector4(1, 1, 1, 1);
		Assert.AreEqual(new Vector4(3, 4, 5, 1), v);
	}

	[Test]
	public void TestMultiplyMatrix()
	{
		var a = new Matrix4(
			1, 0, 0, 1,
			0, 1, 0, 2,
			0, 0, 1, 3,
			0, 0, 0, 1);
		var b = new Matrix4(
			2, 0, 0, 0,
			0, 2, 0, 0,
			0, 0, 2, 0,
			0, 0, 0, 1);
		var m = a * b;
		Assert.AreEqual(new Matrix4(
			2, 0, 0, 1,
			0, 2, 0, 2,
			0, 0, 2, 3,
			0, 0, 0, 1), m);
	}

	[Test]
	public void TestScale()
	{
		var m = Matrix4.Scale(2, 2, 2);
		Assert.AreEqual(new Matrix4(
			2, 0, 0, 0,
			0, 2, 0, 0,
			0, 0, 2, 0,
			0, 0, 0, 1), m);
	}

	[Test]
	public void TestTranslate()
	{
		var m = Matrix4.Translate(1, 2, 3);
		Assert.AreEqual(new Matrix4(
			1, 0, 0, 1,
			0, 1, 0, 2,
			0, 0, 1, 3,
			0, 0, 0, 1), m);
	}

	[Test]
	public void TestTranslateScale()
	{
		var a = Matrix4.Translate(1, 2, 3);
		var b = Matrix4.Scale(2, 2, 2);
		var m = a * b;
		Assert.AreEqual(new Matrix4(
			2, 0, 0, 1,
			0, 2, 0, 2,
			0, 0, 2, 3,
			0, 0, 0, 1), m);
	}
}