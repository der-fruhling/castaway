using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Base.Tests.Matrices;

[TestFixture]
[TestOf(typeof(Matrix2))]
public class Matrix2Tests
{
    [Test]
    public void TestIdent()
    {
        var m = Matrix2.Ident;
        Assert.AreEqual(new Matrix2(
            1, 0,
            0, 1), m);
    }

    [Test]
    public void TestAdd()
    {
        var a = new Matrix2(
            1, 2,
            3, 4);
        var b = new Matrix2(
            2, 3,
            4, 5);
        var m = a + b;
        Assert.AreEqual(new Matrix2(
            1 + 2, 2 + 3,
            3 + 4, 4 + 5), m);
    }

    [Test]
    public void TestSubtract()
    {
        var a = new Matrix2(
            1, 2,
            3, 4);
        var b = new Matrix2(
            2, 3,
            4, 5);
        var m = a - b;
        Assert.AreEqual(new Matrix2(
            1 - 2, 2 - 3,
            3 - 4, 4 - 5), m);
    }

    [Test]
    public void TestMultiplyVector()
    {
        var a = new Matrix2(
            2, 0,
            0, 3);
        var b = new Vector2(1, 1);
        var v = a * b;
        Assert.AreEqual(new Vector2(2, 3), v);
    }

    [Test]
    public void TestMultiplyMatrix()
    {
        var a = new Matrix2(
            2, 0,
            0, 2);
        var b = new Matrix2(
            2, 0,
            0, 2);
        var m = a * b;
        Assert.AreEqual(new Matrix2(
            4, 0,
            0, 4), m);
    }

    [Test]
    public void TestScale()
    {
        var a = Matrix2.Scale(2, 3);
        var b = new Vector2(1, 1);
        var v = a * b;
        Assert.AreEqual(new Vector2(2, 3), v);
    }
}