using System;
using NUnit.Framework;
using Microsoft.Xna.Framework;

using Forever.Physics;

namespace ForeverTests
{
    [TestFixture]
    public class ParticleTests
    {
        [Test]
        public void Particle_Zero_Case()
        {
            Particle part = new Particle(Vector3.Zero);

            Assert.AreEqual(Vector3.Zero, part.Position);
            Assert.AreEqual(Vector3.Zero, part.Velocity);
            Assert.AreEqual(Vector3.Zero, part.Acceleration);
            Assert.AreEqual(1f, part.LinearDamping);
            Assert.AreEqual(0f, part.InverseMass);
            Assert.AreEqual(0f, part.Mass);

            // Check that N times is cool
            part.integrate(1000); //integrate particle for 1 second
            part.integrate(1000); //integrate particle for 1 second
            part.integrate(1000); //integrate particle for 1 second
            part.integrate(1000); //integrate particle for 1 second

            // it should not have changed state
            Assert.AreEqual(Vector3.Zero, part.Position);
            Assert.AreEqual(Vector3.Zero, part.Velocity);
            Assert.AreEqual(Vector3.Zero, part.Acceleration);
            Assert.AreEqual(1f, part.LinearDamping);
            Assert.AreEqual(0f, part.InverseMass);
            Assert.AreEqual(0f, part.Mass);
        }

        [Test]
        public void Particle_Ideal_Nudge()
        {
            Particle part = new Particle(Vector3.Zero);
            part.Mass = 1f;

            Vector3 force = Vector3.Up;

            part.addForce(force);
            part.integrate(1); // first one only applies last frame's accumulators
            part.integrate(1);

            Assert.AreEqual(Vector3.Up, part.Position, "Position");
            Assert.AreEqual(Vector3.Up, part.Velocity, "Velocity");
            // This maybe be bug....
            Assert.AreEqual(Vector3.Zero, part.Acceleration, "Acceleration");

        }

        [Test]
        public void Particle_InfiniteMass()
        {
            Particle part = new Particle(Vector3.Zero);
            part.InverseMass = 0f; //infinite mass


            Vector3 force = Vector3.Up;
            part.addForce(force);

            part.integrate(1); // first one only applies last frame's accumulators
            part.integrate(1);

            Assert.AreEqual(Vector3.Zero, part.Position);


        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Particle_NoBackInTime()
        {

            Particle part = new Particle(Vector3.Zero);
            part.Mass = 1f;

            Vector3 force = Vector3.Up;

            part.integrate(-1f);
        }


        [Test]
        public void Particle_Linear_Damping()
        {
            Particle part = new Particle(Vector3.Zero);
            part.Mass = 1f;
            part.LinearDamping = 0.5f;  //major damping factor

            part.setVelocity(Vector3.Up * 2f);
            part.integrate(0.5f);

            Assert.AreEqual(Vector3.Up * (float)Math.Sqrt(2D), part.Velocity);
        }

        [Test]
        public void Particle_SetPosition()
        {
            Particle part = new Particle(Vector3.Zero);
            part.setPosition( Vector3.Up );
            Assert.AreEqual(Vector3.Up, part.Position);
        }
    }
}
