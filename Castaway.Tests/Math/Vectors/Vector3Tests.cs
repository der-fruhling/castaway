using System;
using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Tests.Math.Vectors
{
    [TestFixture, TestOf(typeof(Vector3))]
    public class Vector3Tests
    {
        [Test]
        public void TestEmptyNew()
        {
            var v = new Vector3();
            Assert.AreEqual(0f, v.X);
            Assert.AreEqual(0f, v.Y);
            Assert.AreEqual(0f, v.Z);
        }
        
        [Test]
        public void TestAdd()
        {
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);
            var v = a + b;
            Assert.AreEqual(1f + 4f, v.X);
            Assert.AreEqual(2f + 5f, v.Y);
            Assert.AreEqual(3f + 6f, v.Z);
        }
        
        [Test]
        public void TestSubtract()
        {
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);
            var v = a - b;
            Assert.AreEqual(1f - 4f, v.X);
            Assert.AreEqual(2f - 5f, v.Y);
            Assert.AreEqual(3f - 6f, v.Z);
        }
        
        [Test]
        public void TestMultiply()
        {
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);
            var v = a * b;
            Assert.AreEqual(1f * 4f, v.X);
            Assert.AreEqual(2f * 5f, v.Y);
            Assert.AreEqual(3f * 6f, v.Z);
        }
        
        [Test]
        public void TestDivide()
        {
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);
            var v = a / b;
            Assert.AreEqual(1f / 4f, v.X);
            Assert.AreEqual(2f / 5f, v.Y);
            Assert.AreEqual(3f / 6f, v.Z);
        }
        
        [Test]
        public void TestAddScalar()
        {
            var a = new Vector3(1, 2, 3);
            var v = a + 4f;
            Assert.AreEqual(1f + 4f, v.X);
            Assert.AreEqual(2f + 4f, v.Y);
            Assert.AreEqual(3f + 4f, v.Z);
        }
        
        [Test]
        public void TestSubtractScalar()
        {
            var a = new Vector3(1, 2, 3);
            var v = a - 4f;
            Assert.AreEqual(1f - 4f, v.X);
            Assert.AreEqual(2f - 4f, v.Y);
            Assert.AreEqual(3f - 4f, v.Z);
        }
        
        [Test]
        public void TestMultiplyScalar()
        {
            var a = new Vector3(1, 2, 3);
            var v = a * 4f;
            Assert.AreEqual(1f * 4f, v.X);
            Assert.AreEqual(2f * 4f, v.Y);
            Assert.AreEqual(3f * 4f, v.Z);
        }
        
        [Test]
        public void TestDivideScalar()
        {
            var a = new Vector3(1, 2, 3);
            var v = a / 4f;
            Assert.AreEqual(1f / 4f, v.X);
            Assert.AreEqual(2f / 4f, v.Y);
            Assert.AreEqual(3f / 4f, v.Z);
        }

        [Test]
        public void TestToSpan()
        {
            ReadOnlySpan<float> a = new Vector3(1, 2, 3);
            Assert.AreEqual(1f, a[0]);
            Assert.AreEqual(2f, a[1]);
            Assert.AreEqual(3f, a[2]);
        }
    }
}