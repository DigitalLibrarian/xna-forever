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
        public void Contact_RelativeBodyCenters()
        {
            NoBody bodyOne = new NoBody();
            bodyOne.Position = Vector3.Right;
            bodyOne.Velocity = Vector3.Left;
            NoBody bodyTwo = new NoBody();
            bodyTwo.Position = Vector3.Left * .9f;
            bodyTwo.Velocity = Vector3.Right;

            Contact contact = new Contact();
            contact.Bodies[0] = bodyOne;
            contact.Bodies[1] = bodyTwo;

            contact.Point = Vector3.Zero;
            contact.Penetration = 0.1f;
            contact.Restitution = 1f;
            contact.Friction = 0f;
            contact.Normal = Vector3.Right;

            contact.ReCalc(1f);

            Assert.AreEqual(Vector3.Right, contact.BodyCenters[0]);
            Assert.AreEqual(Vector3.Left * 0.9f, contact.BodyCenters[1]);

            
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

                Awake = true;

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
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public Microsoft.Xna.Framework.Matrix InverseInertiaTensorWorld
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.Xna.Framework.Matrix World
            {
                get { throw new NotImplementedException(); }
            }

            public Microsoft.Xna.Framework.Quaternion Orientation
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
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public float InverseMass
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
                throw new NotImplementedException();
            }

            public void addRotation(Microsoft.Xna.Framework.Vector3 rot)
            {
                throw new NotImplementedException();
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
