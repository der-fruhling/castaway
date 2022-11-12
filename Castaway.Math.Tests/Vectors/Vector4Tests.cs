using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Base.Tests.Vectors;

[TestFixture]
[TestOf(typeof(Vector4))]
public class Vector4Tests
{
    [Test]
    public void TestEmptyNew()
    {
        var v = new Vector4();
        Assert.AreEqual(0d, v.X);
        Assert.AreEqual(0d, v.Y);
        Assert.AreEqual(0d, v.Z);
        Assert.AreEqual(0d, v.W);
    }

    [Test]
    public void TestAdd()
    {
        var a = new Vector4(1, 2, 3, 4);
        var b = new Vector4(5, 6, 7, 8);
        var v = a + b;
        Assert.AreEqual(1d + 5d, v.X);
        Assert.AreEqual(2d + 6d, v.Y);
        Assert.AreEqual(3d + 7d, v.Z);
        Assert.AreEqual(4d + 8d, v.W);
    }

    [Test]
    public void TestSubtract()
    {
        var a = new Vector4(1, 2, 3, 4);
        var b = new Vector4(5, 6, 7, 8);
        var v = a - b;
        Assert.AreEqual(1d - 5d, v.X);
        Assert.AreEqual(2d - 6d, v.Y);
        Assert.AreEqual(3d - 7d, v.Z);
        Assert.AreEqual(4d - 8d, v.W);
    }

    [Test]
    public void TestMultiply()
    {
        var a = new Vector4(1, 2, 3, 4);
        var b = new Vector4(5, 6, 7, 8);
        var v = a * b;
        Assert.AreEqual(1d * 5d, v.X);
        Assert.AreEqual(2d * 6d, v.Y);
        Assert.AreEqual(3d * 7d, v.Z);
        Assert.AreEqual(4d * 8d, v.W);
    }

    [Test]
    public void TestDivide()
    {
        var a = new Vector4(1, 2, 3, 4);
        var b = new Vector4(5, 6, 7, 8);
        var v = a / b;
        Assert.AreEqual(1d / 5d, v.X);
        Assert.AreEqual(2d / 6d, v.Y);
        Assert.AreEqual(3d / 7d, v.Z);
        Assert.AreEqual(4d / 8d, v.W);
    }

    [Test]
    public void TestAddScalar()
    {
        var a = new Vector4(1, 2, 3, 4);
        var v = a + 5;
        Assert.AreEqual(1d + 5d, v.X);
        Assert.AreEqual(2d + 5d, v.Y);
        Assert.AreEqual(3d + 5d, v.Z);
        Assert.AreEqual(4d + 5d, v.W);
    }

    [Test]
    public void TestSubtractScalar()
    {
        var a = new Vector4(1, 2, 3, 4);
        var v = a - 5;
        Assert.AreEqual(1d - 5d, v.X);
        Assert.AreEqual(2d - 5d, v.Y);
        Assert.AreEqual(3d - 5d, v.Z);
        Assert.AreEqual(4d - 5d, v.W);
    }

    [Test]
    public void TestMultiplyScalar()
    {
        var a = new Vector4(1, 2, 3, 4);
        var v = a * 5;
        Assert.AreEqual(1d * 5d, v.X);
        Assert.AreEqual(2d * 5d, v.Y);
        Assert.AreEqual(3d * 5d, v.Z);
        Assert.AreEqual(4d * 5d, v.W);
    }

    [Test]
    public void TestDivideScalar()
    {
        var a = new Vector4(1, 2, 3, 4);
        var v = a / 5;
        Assert.AreEqual(1d / 5d, v.X);
        Assert.AreEqual(2d / 5d, v.Y);
        Assert.AreEqual(3d / 5d, v.Z);
        Assert.AreEqual(4d / 5d, v.W);
    }

    [Test]
    public void TestToSpan()
    {
        var v = (double[]) new Vector4(1, 2, 3, 4);
        Assert.AreEqual(1d, v[0]);
        Assert.AreEqual(2d, v[1]);
        Assert.AreEqual(3d, v[2]);
        Assert.AreEqual(4d, v[3]);
    }
}