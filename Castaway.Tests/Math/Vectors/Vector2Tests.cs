using System;
using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Tests.Math.Vectors
{
    [TestFixture, TestOf(typeof(Vector2))]
    public class Vector2Tests
    {
        [Test]
        public void TestEmptyNew()
        {
            var v = new Vector2();
            Assert.AreEqual(0f, v.X);
            Assert.AreEqual(0f, v.Y);
        }

        [Test]
        public void TestAdd()
        {
            var a = new Vector2(1, 2);
            var b = new Vector2(3, 4);
            var c = a + b;
            Assert.AreEqual(4F, c.X);
            Assert.AreEqual(6F, c.Y);
        }

        [Test]
        public void TestSubtract()
        {
            var a = new Vector2(1, 2);
            var b = new Vector2(3, 4);
            var c = a - b;
            Assert.AreEqual(-2F, c.X);
            Assert.AreEqual(-2F, c.Y);
        }

        [Test]
        public void TestMultiply()
        {
            var a = new Vector2(1, 2);
            var b = new Vector2(3, 4);
            var c = a * b;
            Assert.AreEqual(3F, c.X);
            Assert.AreEqual(8F, c.Y);
        }

        [Test]
        public void TestDivide()
        {
            var a = new Vector2(1, 2);
            var b = new Vector2(3, 4);
            var c = a / b;
            Assert.AreEqual(0.33333334F, c.X);
            Assert.AreEqual(0.5F, c.Y);
        }
        
        [Test]
        public void TestAddScalar()
        {
            var a = new Vector2(1, 2);
            var c = a + 3;
            Assert.AreEqual(4F, c.X);
            Assert.AreEqual(5F, c.Y);
        }
        
        [Test]
        public void TestSubtractScalar()
        {
            var a = new Vector2(1, 2);
            var c = a - 3;
            Assert.AreEqual(-2F, c.X);
            Assert.AreEqual(-1F, c.Y);
        }
        
        [Test]
        public void TestMultiplyScalar()
        {
            var a = new Vector2(1, 2);
            var c = a * 3;
            Assert.AreEqual(3F, c.X);
            Assert.AreEqual(6F, c.Y);
        }
        
        [Test]
        public void TestDivideScalar()
        {
            var a = new Vector2(1, 2);
            var c = a / 3;
            Assert.AreEqual(0.33333334F, c.X);
            Assert.AreEqual(0.6666667F, c.Y);
        }
        
        [Test]
        public void TestToSpan()
        {
            ReadOnlySpan<float> a = new Vector2(1, 2);
            Assert.AreEqual(1f, a[0]);
            Assert.AreEqual(2f, a[1]);
        }
    }
}