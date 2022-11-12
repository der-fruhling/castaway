using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Base.Tests.Vectors;

[TestFixture]
[TestOf(typeof(Vector2))]
public class Vector2Tests
{
    [Test]
    public void TestEmptyNew()
    {
        var v = new Vector2();
        Assert.AreEqual(0.0, v.X);
        Assert.AreEqual(0.0, v.Y);
    }

    [Test]
    public void TestAdd()
    {
        var a = new Vector2(1, 2);
        var b = new Vector2(3, 4);
        var c = a + b;
        Assert.AreEqual(4.0, c.X);
        Assert.AreEqual(6.0, c.Y);
    }

    [Test]
    public void TestSubtract()
    {
        var a = new Vector2(1, 2);
        var b = new Vector2(3, 4);
        var c = a - b;
        Assert.AreEqual(-2.0, c.X);
        Assert.AreEqual(-2.0, c.Y);
    }

    [Test]
    public void TestMultiply()
    {
        var a = new Vector2(1, 2);
        var b = new Vector2(3, 4);
        var c = a * b;
        Assert.AreEqual(3.0, c.X);
        Assert.AreEqual(8.0, c.Y);
    }

    [Test]
    public void TestDivide()
    {
        var a = new Vector2(1, 2);
        var b = new Vector2(3, 4);
        var c = a / b;
        Assert.AreEqual(1.0 / 3.0, c.X);
        Assert.AreEqual(0.5, c.Y);
    }

    [Test]
    public void TestAddScalar()
    {
        var a = new Vector2(1, 2);
        var c = a + 3;
        Assert.AreEqual(4.0, c.X);
        Assert.AreEqual(5.0, c.Y);
    }

    [Test]
    public void TestSubtractScalar()
    {
        var a = new Vector2(1, 2);
        var c = a - 3;
        Assert.AreEqual(-2.0, c.X);
        Assert.AreEqual(-1.0, c.Y);
    }

    [Test]
    public void TestMultiplyScalar()
    {
        var a = new Vector2(1, 2);
        var c = a * 3;
        Assert.AreEqual(3.0, c.X);
        Assert.AreEqual(6.0, c.Y);
    }

    [Test]
    public void TestDivideScalar()
    {
        var a = new Vector2(1, 2);
        var c = a / 3;
        Assert.AreEqual(1.0 / 3.0, c.X);
        Assert.AreEqual(2.0 / 3.0, c.Y);
    }

    [Test]
    public void TestToSpan()
    {
        var a = (double[]) new Vector2(1, 2);
        Assert.AreEqual(1.0, a[0]);
        Assert.AreEqual(2.0, a[1]);
    }
}