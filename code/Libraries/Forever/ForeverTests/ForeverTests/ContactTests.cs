using System;
using NUnit.Framework;

using Microsoft.Xna.Framework;
using Forever.Interface;
using Forever.Physics.Collide;

namespace ForeverTests
{
    [TestFixture]
    public class ContactTests
    {
        [Test]
        public void Contact_XAxis()
        {
            NoBody bodyZero = new NoBody(Vector3.Right);

            bodyZero.Velocity = Vector3.Left;
            bodyZero.Mass = 1f;
            bodyZero.calculateDerivedData();

            NoBody bodyOne = new NoBody(Vector3.Left * .9f);

            bodyOne.Velocity = Vector3.Right;
            bodyOne.Mass = 1f;
            bodyOne.calculateDerivedData();

            Contact contact = new Contact();
            contact.Bodies[0] = bodyZero;
            contact.Bodies[1] = bodyOne;

            contact.Point = Vector3.Zero;
            contact.Penetration = 0.1f;
            contact.Restitution = 1f;
            contact.Friction = 0f;
            contact.Normal = Vector3.Right;


            contact.ReCalc(1f);

            Assert.AreEqual(Vector3.Left, contact.CalcLocalVelocity(0, 1f));
            Assert.AreEqual(Vector3.Right, contact.CalcLocalVelocity(1, 1f));

            Assert.AreEqual(Vector3.Left, contact.RelativeContactPositions[0]);
            Assert.AreEqual(Vector3.Right * 0.9f, contact.RelativeContactPositions[1]);
            Assert.AreEqual(new Vector3(-2f, 0f, 0f), contact.ContactVelocity);


            // Position Change

            Vector3[] linearChange = new Vector3[2];
            Vector3[] angularChange = new Vector3[2];
            contact.ApplyPositionChange(
                ref linearChange, ref angularChange, contact.Penetration);

            Assert.AreEqual(Vector3.Zero, angularChange[0], "Zero angular change for object 0");
            Assert.AreEqual(Vector3.Zero, angularChange[1], "Zero angular change for object 1");

            Assert.AreEqual(new Vector3(0.05f, 0f, 0f), linearChange[0],
                "Body 0 is pushed right by half penetration");
            Assert.AreEqual(new Vector3(-0.05f, 0f, 0f), linearChange[1],
                "Body 1 is pushed left by half penetration");

            //Velocity Change


            Vector3[] velocityChange = new Vector3[2];
            Vector3[] rotationChange = new Vector3[2];


            contact.ApplyVelocityChange(ref velocityChange, ref rotationChange);


            Assert.AreEqual(
                Vector3.Zero, 
                rotationChange[0], 
                "Zero rotation change for object 0");
            Assert.AreEqual(
                Vector3.Zero, 
                rotationChange[1], 
                "Zero rotation change for object 1");

            Assert.AreEqual(
                Vector3.Right * 2f,
                velocityChange[0],
                "Counter velocity applied to body 0");

            Assert.AreEqual(
                Vector3.Left * 2f,
                velocityChange[1],
                "Counter velocity applied to body 1");


            Assert.AreEqual(
                Vector3.Right,
                bodyZero.Velocity);
            Assert.AreEqual(Vector3.Zero, bodyZero.Rotation);

            Assert.AreEqual(
                Vector3.Left,
                bodyOne.Velocity);
            Assert.AreEqual(Vector3.Zero, bodyOne.Rotation);
        }

        [Test]
        public void Contact_BoxSpinningOnYAxisFallsOnToXZPlane()
        {
            // X-Z plane
            NoBody planeBody = new NoBody(Vector3.Zero);
            planeBody.InverseMass = 0f; //infinite mass
            Forever.Physics.Collide.Plane plane = new Forever.Physics.Collide.Plane(planeBody, Vector3.Zero, Vector3.Up);

            planeBody.calculateDerivedData();

            // Spinning box colliding with the plane
            NoBody boxBody = new NoBody(Vector3.Up * 0.95f);

            boxBody.Mass = 1f;
            boxBody.Rotation = new Vector3(0f, 0.01f, 0f);
            boxBody.Velocity = new Vector3(0f, -1f, 0f);
            boxBody.calculateDerivedData();
            Box box = new Box(boxBody, Matrix.Identity, new Vector3(1f, 1f, 1f));


            Contact contact = new Contact();
            contact.Bodies[0] = planeBody;
            contact.Bodies[1] = boxBody;

            contact.Point = Vector3.Zero;
            contact.Penetration = 0.05f;
            contact.Restitution = 1f;
            contact.Friction = 0f;
            contact.Normal = Vector3.Up;


            contact.ReCalc(1f);

            Assert.AreEqual(Vector3.Zero, contact.CalcLocalVelocity(0, 1f));
            Assert.AreEqual(Vector3.Left, contact.CalcLocalVelocity(1, 1f));

            Assert.AreEqual(Vector3.Zero, contact.RelativeContactPositions[0]);
            Assert.AreEqual(Vector3.Down * 0.95f, contact.RelativeContactPositions[1]);
            Assert.AreEqual(Vector3.Right, contact.ContactVelocity);


            // Position Change

            Vector3[] linearChange = new Vector3[2];
            Vector3[] angularChange = new Vector3[2];
            contact.ApplyPositionChange(
                ref linearChange, ref angularChange, contact.Penetration);

            Assert.AreEqual(Vector3.Zero, angularChange[0], "Zero angular change for object 0");
            Assert.AreEqual(Vector3.Zero, angularChange[1], "Zero angular change for object 1");

            Assert.AreEqual(new Vector3(0.0f, 0f, 0f), linearChange[0],
                "Body 0 is not pushed at all because it has infinite mass");
            Assert.AreEqual(new Vector3(0f, -0.05f, 0f), linearChange[1],
                "Body 1 is pushed up (forward in contact direction) by half penetration");

            //Velocity Change


            Vector3[] velocityChange = new Vector3[2];
            Vector3[] rotationChange = new Vector3[2];


            contact.ApplyVelocityChange(ref velocityChange, ref rotationChange);


            Assert.AreEqual(
                Vector3.Zero,
                rotationChange[0],
                "Zero rotation change for object 0");

            Assert.AreEqual(
                Vector3.Zero,
                rotationChange[1],
                "Zero rotation change for object 1");

            Assert.AreEqual(
                Vector3.Zero,
                velocityChange[0],
                "Zero velocity applied to body 0");

            Assert.AreEqual(
                Vector3.Up * 2f,
                velocityChange[1],
                "Counter velocity applied to body 1");
        }

        #region IRigidBody player
        private class NoBody : Forever.Physics.RigidBody
        {
            public NoBody(Vector3 pos)
                : base(pos)
            {

            }

        }
        #endregion
    }
}
