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
            NoBody bodyZero = new NoBody();
            bodyZero.Position = Vector3.Right;
            bodyZero.Velocity = Vector3.Left;
            NoBody bodyOne = new NoBody();
            bodyOne.Position = Vector3.Left * .9f;
            bodyOne.Velocity = Vector3.Right;

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

            Assert.AreEqual(Vector3.Right, contact.RelativeContactPositions[0]);
            Assert.AreEqual(Vector3.Left * 0.9f, contact.RelativeContactPositions[1]);
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

            Assert.AreEqual(
                Vector3.Left,
                bodyOne.Velocity);

            int brak = 3;
        }


        #region IRigidBody player
        private class NoBody : IRigidBody
        {

            public NoBody()
            {
                Position = Vector3.Zero;
                Velocity = Vector3.Zero;
                LastAccel = Vector3.Zero;
                Acceler = Vector3.Zero;
                Rotation = Vector3.Zero;
                Orientation = Quaternion.Identity;

                Awake = true;
                Mass = 1;
            }


            public Microsoft.Xna.Framework.Vector3 Position
            {
                get;
                set;
            }

            public Microsoft.Xna.Framework.Vector3 Velocity
            {
                get;
                set;
            }

            public Microsoft.Xna.Framework.Vector3 LastAccel
            {
                get;
                set;
            }


            public Microsoft.Xna.Framework.Vector3 Acceler
            {
                get;
                set;
            }


            public Microsoft.Xna.Framework.Vector3 Rotation
            {
                get;
                set;
            }



            public bool Awake
            {
                get;
                set;
            }


            public Microsoft.Xna.Framework.Matrix InertiaTensor
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public Microsoft.Xna.Framework.Matrix InertiaTensorWorld
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.Xna.Framework.Matrix InverseInertiaTensor
            {
                get
                {
                    return Matrix.Identity;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public Microsoft.Xna.Framework.Matrix InverseInertiaTensorWorld
            {
                get { return Matrix.Identity; }
            }

            public Microsoft.Xna.Framework.Matrix World
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.Xna.Framework.Quaternion Orientation
            {
                get;
                set;
            }

            public Microsoft.Xna.Framework.Vector3 AngularMomentum
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.Xna.Framework.Vector3 LinearMomentum
            {
                get { throw new NotImplementedException(); }
            }

            public float Mass
            {
                get;
                set;
            }

            public float InverseMass
            {
                get
                {
                    return 1f / Mass;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public bool HasFiniteMass
            {
                get { throw new NotImplementedException(); }
            }

            public float LinearDamping
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public float AngularDamping
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

           



          


            public void addTorque(Microsoft.Xna.Framework.Vector3 torque)
            {
                throw new NotImplementedException();
            }

            public void addForce(Microsoft.Xna.Framework.Vector3 force, Microsoft.Xna.Framework.Vector3 point)
            {
                throw new NotImplementedException();
            }

            public void clearAccumulators()
            {
                throw new NotImplementedException();
            }

            public void integrate(float duration)
            {
                throw new NotImplementedException();
            }

            public void addVelocity(Microsoft.Xna.Framework.Vector3 velo)
            {
                Velocity += velo;
            }

            public void addRotation(Microsoft.Xna.Framework.Vector3 rot)
            {
                Rotation += rot;
            }

            public Microsoft.Xna.Framework.Vector3 Up
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.Xna.Framework.Vector3 Right
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.Xna.Framework.Vector3 Forward
            {
                get { throw new NotImplementedException(); }
            }

            public void addForce(Microsoft.Xna.Framework.Vector3 force)
            {
                throw new NotImplementedException();
            }

            public Microsoft.Xna.Framework.Vector3 CenterOfMass
            {
                get { throw new NotImplementedException(); }
            }

            public void Translate(Microsoft.Xna.Framework.Vector3 translation)
            {
                throw new NotImplementedException();
            }


            public bool CanSleep
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }
        #endregion
    }
}
