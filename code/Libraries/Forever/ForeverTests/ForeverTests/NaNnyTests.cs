using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

using Forever;

namespace ForeverTests
{

    [TestFixture]
    public class NaNnyTests
    {
        [Test]
        public void Quat_IsNaN()
        {
            Quaternion quat = new Quaternion(1f, 1f, 1f, 2f);

            Assert.AreEqual(false, NaNny.IsNaN(quat));

            quat = new Quaternion((float)Math.Sqrt(-1D), 1f, 1f, 1f);
            Assert.AreEqual(true, NaNny.IsNaN(quat));
        }

       
    }
}
