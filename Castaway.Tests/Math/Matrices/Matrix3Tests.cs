using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Tests.Math.Matrices
{
    [TestFixture, TestOf(typeof(Matrix3))]
    public class Matrix3Tests
    {
        [Test]
        public void TestIdent()
        {
            var m = Matrix3.Ident;
            Assert.AreEqual(new Matrix3(
                1, 0, 0,
                0, 1, 0,
                0, 0, 1), m);
        }

        [Test]
        public void TestAdd()
        {
            var a = new Matrix3(
                1, 2, 3,
                2, 3, 4,
                3, 4, 5);
            var b = new Matrix3(
                2, 3, 4,
                3, 4, 5,
                4, 5, 6);
            var m = a + b;
            Assert.AreEqual(new Matrix3(
                1 + 2, 2 + 3, 3 + 4,
                2 + 3, 3 + 4, 4 + 5,
                3 + 4, 4 + 5, 5 + 6), m);
        }

        [Test]
        public void TestSubtract()
        {
            var a = new Matrix3(
                1, 2, 3,
                2, 3, 4,
                3, 4, 5);
            var b = new Matrix3(
                2, 3, 4,
                3, 4, 5,
                4, 5, 6);
            var m = a - b;
            Assert.AreEqual(new Matrix3(
                1 - 2, 2 - 3, 3 - 4,
                2 - 3, 3 - 4, 4 - 5,
                3 - 4, 4 - 5, 5 - 6), m);
        }

        [Test]
        public void TestMultiplyVector()
        {
            var a = new Matrix3(
                2, 0, 0,
                0, 3, 0,
                0, 0, 4);
            var v = a * new Vector3(1, 1, 1);
            Assert.AreEqual(new Vector3(2, 3, 4), v);
        }

        [Test]
        public void TestMultiplyMatrix()
        {
            var a = new Matrix3(
                2, 0, 0,
                0, 2, 0,
                0, 0, 1);
            var b = new Matrix3(
                1, 0, 3,
                0, 1, 4,
                0, 0, 1);
            var m = b * a;
            Assert.AreEqual(new Matrix3(
                2, 0, 3,
                0, 2, 4,
                0, 0, 1), m);
        }

        [Test]
        public void TestScale()
        {
            var a = Matrix3.Scale(2, 3, 4);
            var v = a * new Vector3(1, 1, 1);
            Assert.AreEqual(new Vector3(2, 3, 4), v);
        }

        [Test]
        public void TestTranslate()
        {
            var a = Matrix3.Translate(2, 3);
            var v = a * new Vector3(0, 0, 1);
            Assert.AreEqual(new Vector3(2, 3, 1), v);
        }

        [Test]
        public void TestScaleTranslate()
        {
            var a = Matrix3.Translate(2, 3) * Matrix3.Scale(2, 2);
            var v = a * new Vector3(1, 1, 1);
            Assert.AreEqual(new Vector3(4, 5, 1), v);
        }
    }
}