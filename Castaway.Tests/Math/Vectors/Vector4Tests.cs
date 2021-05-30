using System;
using Castaway.Math;
using NUnit.Framework;

namespace Castaway.Tests.Math.Vectors
{
    [TestFixture, TestOf(typeof(Vector4))]
    public class Vector4Tests
    {
        [Test]
        public void TestEmptyNew()
        {
            var v = new Vector4();
            Assert.AreEqual(0f, v.X);
            Assert.AreEqual(0f, v.Y);
            Assert.AreEqual(0f, v.Z);
            Assert.AreEqual(0f, v.W);
        }
        
        [Test]
        public void TestAdd()
        {
            var a = new Vector4(1, 2, 3, 4);
            var b = new Vector4(5, 6, 7, 8);
            var v = a + b;
            Assert.AreEqual(1f + 5f, v.X);
            Assert.AreEqual(2f + 6f, v.Y);
            Assert.AreEqual(3f + 7f, v.Z);
            Assert.AreEqual(4f + 8f, v.W);
        }
        
        [Test]
        public void TestSubtract()
        {
            var a = new Vector4(1, 2, 3, 4);
            var b = new Vector4(5, 6, 7, 8);
            var v = a - b;
            Assert.AreEqual(1f - 5f, v.X);
            Assert.AreEqual(2f - 6f, v.Y);
            Assert.AreEqual(3f - 7f, v.Z);
            Assert.AreEqual(4f - 8f, v.W);
        }
        
        [Test]
        public void TestMultiply()
        {
            var a = new Vector4(1, 2, 3, 4);
            var b = new Vector4(5, 6, 7, 8);
            var v = a * b;
            Assert.AreEqual(1f * 5f, v.X);
            Assert.AreEqual(2f * 6f, v.Y);
            Assert.AreEqual(3f * 7f, v.Z);
            Assert.AreEqual(4f * 8f, v.W);
        }
        
        [Test]
        public void TestDivide()
        {
            var a = new Vector4(1, 2, 3, 4);
            var b = new Vector4(5, 6, 7, 8);
            var v = a / b;
            Assert.AreEqual(1f / 5f, v.X);
            Assert.AreEqual(2f / 6f, v.Y);
            Assert.AreEqual(3f / 7f, v.Z);
            Assert.AreEqual(4f / 8f, v.W);
        }
        
        [Test]
        public void TestAddScalar()
        {
            var a = new Vector4(1, 2, 3, 4);
            var v = a + 5;
            Assert.AreEqual(1f + 5f, v.X);
            Assert.AreEqual(2f + 5f, v.Y);
            Assert.AreEqual(3f + 5f, v.Z);
            Assert.AreEqual(4f + 5f, v.W);
        }
        
        [Test]
        public void TestSubtractScalar()
        {
            var a = new Vector4(1, 2, 3, 4);
            var v = a - 5;
            Assert.AreEqual(1f - 5f, v.X);
            Assert.AreEqual(2f - 5f, v.Y);
            Assert.AreEqual(3f - 5f, v.Z);
            Assert.AreEqual(4f - 5f, v.W);
        }
        
        [Test]
        public void TestMultiplyScalar()
        {
            var a = new Vector4(1, 2, 3, 4);
            var v = a * 5;
            Assert.AreEqual(1f * 5f, v.X);
            Assert.AreEqual(2f * 5f, v.Y);
            Assert.AreEqual(3f * 5f, v.Z);
            Assert.AreEqual(4f * 5f, v.W);
        }
        
        [Test]
        public void TestDivideScalar()
        {
            var a = new Vector4(1, 2, 3, 4);
            var v = a / 5;
            Assert.AreEqual(1f / 5f, v.X);
            Assert.AreEqual(2f / 5f, v.Y);
            Assert.AreEqual(3f / 5f, v.Z);
            Assert.AreEqual(4f / 5f, v.W);
        }

        [Test]
        public void TestToSpan()
        {
            ReadOnlySpan<float> v = new Vector4(1, 2, 3, 4);
            Assert.AreEqual(1f, v[0]);
            Assert.AreEqual(2f, v[1]);
            Assert.AreEqual(3f, v[2]);
            Assert.AreEqual(4f, v[3]);
        }
    }
}