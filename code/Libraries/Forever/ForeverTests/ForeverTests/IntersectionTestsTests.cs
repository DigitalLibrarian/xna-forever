using System;
using System.Collections.Generic;
using NUnit.Framework;

using Microsoft.Xna.Framework;

using Forever;
using Forever.Interface;
using Forever.Physics.Collide;

namespace ForeverTests
{
    [TestFixture]
    public class IntersectionTestsTests
    {


        [Test]
        public void IntersectionTests_SphereVsSphere_NoIntersection()
        {
            IRigidBody sphereOneBody = new NoBody(Matrix.CreateTranslation(Vector3.UnitX * -2f));

            float sphereOneRadius = 1f;
            Matrix sphereOneMatrix = Matrix.Identity;
            Sphere sphereOne = new Sphere(sphereOneBody, sphereOneMatrix, sphereOneRadius);


            IRigidBody sphereTwoBody = new NoBody(Matrix.CreateTranslation(Vector3.UnitX * 2f));

            float sphereTwoRadius = 1f;
            Matrix sphereTwoMatrix = Matrix.Identity;
            Sphere sphereTwo = new Sphere(sphereTwoBody, sphereTwoMatrix, sphereTwoRadius);


            
            CollisionData data = new CollisionData();
            Assert.AreEqual(0, IntersectionTests.sphereAndSphere(sphereOne, sphereTwo, data));

        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void IntersectionTests_SphereVsSphere_BodyPointIntersection()
        {
            IRigidBody sphereOneBody = new NoBody();

            float sphereOneRadius = 1f;
            Matrix sphereOneMatrix = Matrix.Identity;
            Sphere sphereOne = new Sphere(sphereOneBody, sphereOneMatrix, sphereOneRadius);

            IRigidBody sphereTwoBody = new NoBody();

            float sphereTwoRadius = 1f;
            Matrix sphereTwoMatrix = Matrix.Identity;
            Sphere sphereTwo = new Sphere(sphereTwoBody, sphereTwoMatrix, sphereTwoRadius);


            CollisionData data = new CollisionData();
            IntersectionTests.sphereAndSphere(sphereOne, sphereTwo, data);
        }


        [Test]
        public void IntersectionTests_SphereVsSphere_SurfaceCollision()
        {
            Vector3 sphereOnePosition = Vector3.Zero;
            IRigidBody sphereOneBody = new NoBody(Matrix.CreateTranslation(sphereOnePosition));

            float sphereOneRadius = 0.5f;
            Matrix sphereOneMatrix = Matrix.Identity;
            Sphere sphereOne = new Sphere(sphereOneBody, sphereOneMatrix, sphereOneRadius);


            Vector3 sphereTwoPosition = Vector3.UnitX;
            IRigidBody sphereTwoBody = new NoBody(Matrix.CreateTranslation(sphereTwoPosition));

            float sphereTwoRadius = 0.5f;
            Matrix sphereTwoMatrix = Matrix.Identity;
            Sphere sphereTwo = new Sphere(sphereTwoBody, sphereTwoMatrix, sphereTwoRadius);


            CollisionData data = new CollisionData();
            Assert.AreEqual(1, IntersectionTests.sphereAndSphere(sphereOne, sphereTwo, data), "How many contacts?");

            Contact contact;
            Assert.NotNull(data.contacts, "Contacts Container");
            Assert.AreEqual(1, data.contacts.Count, "One Contact Expected");
            contact = data.contacts[0];
            Assert.AreEqual(0, contact.Friction, "0 friction");
            Assert.AreEqual(1f, contact.Restitution, "1 restitution");
            Assert.AreEqual(0, contact.Penetration, "0 penetration (surface collision)");
            Assert.AreEqual(new Vector3(0.5f, 0f, 0f), contact.Point, "expected contact point");
            Assert.AreEqual(-Vector3.UnitX, contact.Normal, "expected contact normal");

            //Assymetric

            data = new CollisionData();

            IntersectionTests.sphereAndSphere(sphereTwo, sphereOne, data);
            Assert.NotNull(data.contacts, "Contacts Container");
            Assert.AreEqual(1, data.contacts.Count, "One Contact Expected");
            contact = data.contacts[0];
            Assert.AreEqual(0, contact.Friction, "0 friction");
            Assert.AreEqual(1, contact.Restitution, "1 restitution");
            Assert.AreEqual(0, contact.Penetration, "0 penetration (surface collision)");
            Assert.AreEqual(new Vector3(0.5f, 0f, 0f), contact.Point, "expected contact point");
            Assert.AreEqual(Vector3.UnitX, contact.Normal, "expected contact normal");
        }


        [Test]
        public void IntersectionTests_SphereVsSphere_Overlapping()
        {
            Vector3 sphereOnePosition = new Vector3(0.5f, 0f, 0f);
            IRigidBody sphereOneBody = new NoBody(Matrix.CreateTranslation(sphereOnePosition));

            float sphereOneRadius = 1f;
            Matrix sphereOneMatrix = Matrix.Identity;
            Sphere sphereOne = new Sphere(sphereOneBody, sphereOneMatrix, sphereOneRadius);


            Vector3 sphereTwoPosition = Vector3.UnitX;
            IRigidBody sphereTwoBody = new NoBody(Matrix.CreateTranslation(sphereTwoPosition));

            float sphereTwoRadius = 1f;
            Matrix sphereTwoMatrix = Matrix.Identity;
            Sphere sphereTwo = new Sphere(sphereTwoBody, sphereTwoMatrix, sphereTwoRadius);



            CollisionDetector detect = new CollisionDetector();


            CollisionData data = new CollisionData();
            Assert.AreEqual(1, IntersectionTests.sphereAndSphere(sphereOne, sphereTwo, data), "How many contacts?");


            Contact contact;

            Assert.NotNull(data.contacts, "Contacts Container");
            Assert.AreEqual(1, data.contacts.Count, "One Contact Expected");
            contact = data.contacts[0];
            Assert.AreEqual(0, contact.Friction, "0 friction");
            Assert.AreEqual(1f, contact.Restitution, "1 restitution");
            Assert.AreEqual(1.5f, contact.Penetration, "0 penetration (surface collision)");
            Assert.AreEqual(new Vector3(0.75f, 0f, 0f), contact.Point, "expected contact point");
            Assert.AreEqual(-Vector3.UnitX, contact.Normal, "expected contact normal");

            //Assymetric

            data = new CollisionData();

            IntersectionTests.sphereAndSphere(sphereTwo, sphereOne, data);
            Assert.NotNull(data.contacts, "Contacts Container");
            Assert.AreEqual(1, data.contacts.Count, "One Contact Expected");
            contact = data.contacts[0];
            Assert.AreEqual(0, contact.Friction, "0 friction");
            Assert.AreEqual(1f, contact.Restitution, "1 restitution");
            Assert.AreEqual(1.5f, contact.Penetration, "0 penetration (surface collision)");
            Assert.AreEqual(new Vector3(0.75f, 0f, 0f), contact.Point, "expected contact point");
            Assert.AreEqual(Vector3.UnitX, contact.Normal, "expected contact normal");

        }


        [Test]
        public void IntersectionTests_SphereVsPlane()
        {
            Matrix sphereMatrix;
            float sphereRadius = 1f;
            Forever.Physics.Collide.Plane plane;
            Forever.Physics.Collide.Sphere sphere;
            CollisionData data;
            int contactsFound;
            Contact contact1;

            // plane is always the x-z plane for this test method
            Vector3 planeNormal = Vector3.Up;
            plane = new Forever.Physics.Collide.Plane(new NoBody(), Vector3.Zero, planeNormal);


            // unit sphere centered at y=1 (one point of surface touching)
            sphereMatrix = Matrix.CreateTranslation(Vector3.UnitY);
            sphere = new Sphere( new NoBody(sphereMatrix), Matrix.Identity, sphereRadius);

                
            data = new CollisionData();
            contactsFound = IntersectionTests.sphereAndHalfSpace(sphere, plane, data);

            Assert.AreEqual(1, contactsFound);

            contact1 = data.contacts[0];

            Assert.AreEqual(Vector3.Zero, contact1.Point);
            Assert.AreEqual(0f, contact1.Penetration);
            Assert.AreEqual(planeNormal, contact1.Normal);



            // unit sphere centered at y=0.9 (.1 penetration)
            sphereMatrix = Matrix.CreateTranslation(Vector3.UnitY  * 0.9f);
            sphere = new Sphere(new NoBody(sphereMatrix), Matrix.Identity, sphereRadius);


            data = new CollisionData();
            contactsFound = IntersectionTests.sphereAndHalfSpace(sphere, plane, data);

            Assert.AreEqual(1, contactsFound);

            contact1 = data.contacts[0];

            Assert.AreEqual(Vector3.Zero, contact1.Point);
            Assert.True(
                TrickyMathHelper.AlmostEquals(0.1f, contact1.Penetration)
                );
            Assert.AreEqual(planeNormal, contact1.Normal);
        }



        [Test]
        public void IntersectionTests_BoxVsSphere()
        {
            float sphereRadius = 1f;
            CollisionData data;
            int contactsFound;
            Contact contact1;

            Forever.Physics.Collide.Sphere sphere;
            Forever.Physics.Collide.Box box;

            // no intersection

            sphere = new Sphere( 
                new NoBody( Matrix.CreateTranslation(new Vector3(-10000f, -10000f, -10000f))),
                Matrix.Identity,
                sphereRadius
                    );


            box = new Box(
                new NoBody(Matrix.CreateTranslation(new Vector3(10000f, 10000f, 10000f))),
                Matrix.Identity,
                new Vector3(1f, 1f, 1f)
                );

            data = new CollisionData();

            contactsFound = IntersectionTests.boxAndSphere(box, sphere, data);

            Assert.AreEqual(0, contactsFound);

            //box face intersection

            sphere = new Sphere(
                new NoBody(Matrix.CreateTranslation(Vector3.Up)),
                Matrix.Identity,
                sphereRadius);

            box = new Box(
                new NoBody(Matrix.CreateTranslation(Vector3.Down)),
                Matrix.Identity,
                new Vector3(1f, 1f, 1f));

            data = new CollisionData();

            contactsFound = IntersectionTests.boxAndSphere(box, sphere, data);

            Assert.AreEqual(1, contactsFound);

            contact1 = data.contacts[0];

            Assert.AreEqual(0f, contact1.Penetration);
            Assert.AreEqual(Vector3.Zero, contact1.Point);
            Assert.AreEqual(Vector3.Up, contact1.Normal);
            

            //box corner intersection

            box = new Box(
                new NoBody(), //box centered at origin
                Matrix.Identity,
                new Vector3(1f, 1f, 1f)
                );


            Vector3 piece = new Vector3(1f, 1f, 1f);
            piece.Normalize();

            sphere = new Sphere(
                new NoBody(
                    Matrix.CreateTranslation(
                        new Vector3(1f, 1f, 1f) //box corner
                        + piece
                    ) 
                ),
                Matrix.Identity,
                sphereRadius
                );

            data = new CollisionData();
            
            contactsFound = IntersectionTests.boxAndSphere(box, sphere, data);


            Assert.AreEqual(1, contactsFound);

            contact1 = data.contacts[0];

            Assert.True(
                TrickyMathHelper.AlmostEquals(0f, contact1.Penetration)
                );
            
            Assert.AreEqual(new Vector3(1f, 1f, 1f), contact1.Point);
            Assert.True(
                TrickyMathHelper.CloseEnoughToBeSame(
                new Vector3(0.5773503f, 0.5773503f, 0.5773503f),
                contact1.Normal)
                );

            



            //box edge intersection


            box = new Box(
                new NoBody(), //box centered at origin
                Matrix.Identity,
                new Vector3(1f, 1f, 1f)
                );

            Vector3 spherePos = new Vector3(0f, 1f, 1f);
            spherePos.Normalize();
            spherePos = new Vector3(0f, 1f, 1f) + spherePos;

            sphere = new Sphere(
                new NoBody(
                    Matrix.CreateTranslation(spherePos)
                    ),
                Matrix.Identity,
                sphereRadius);
            data = new CollisionData();

            contactsFound = IntersectionTests.boxAndSphere(box, sphere, data);


            Assert.AreEqual(1, contactsFound);
            contact1 = data.contacts[0];

            Assert.True(
                TrickyMathHelper.AlmostEquals(0f, contact1.Penetration)
                );
            
            Assert.AreEqual(new Vector3(0f, 1f, 1f), contact1.Point);
            Assert.True(
                TrickyMathHelper.CloseEnoughToBeSame(
                new Vector3(0f, 0.7071068f, 0.7071068f),
                contact1.Normal)
                );
        }



        [Test]
        public void IntersectionTests_BoxVsPlane()
        {
            IRigidBody planeBody = new NoBody();
            Box box;
            Forever.Physics.Collide.Plane plane;
            CollisionData data;

            Vector3 planeNormal = Vector3.Up;

            // Plane is x-z plane for all tests
            
            //////// unit box is sitting at {0,1,0} (surface touching)

            box = new Box(new  NoBody( Matrix.CreateTranslation(new Vector3(0f, 1f, 0f))), Matrix.Identity, new Vector3(1f, 1f, 1f));
            plane = new Forever.Physics.Collide.Plane(planeBody, Vector3.Zero, planeNormal);
            data = new CollisionData();

            int contactsFound = IntersectionTests.boxAndPlane(box, plane, data);

            Assert.AreEqual(4, contactsFound);

            Contact contact1 = data.contacts[0];
            Contact contact2 = data.contacts[1];
            Contact contact3 = data.contacts[2];
            Contact contact4 = data.contacts[3];


            Assert.AreEqual(new Vector3(-1f, 0f, -1f), contact1.Point);
            Assert.AreEqual(planeNormal, contact1.Normal);
            Assert.AreEqual(0, contact1.Penetration);

            Assert.AreEqual(new Vector3(-1f, 0f, 1f), contact2.Point);
            Assert.AreEqual(planeNormal, contact2.Normal);
            Assert.AreEqual(0, contact2.Penetration);

            Assert.AreEqual(new Vector3(1f, 0f, -1f), contact3.Point);
            Assert.AreEqual(planeNormal, contact3.Normal);
            Assert.AreEqual(0, contact3.Penetration);

            Assert.AreEqual(new Vector3(1f, 0f, 1f), contact4.Point);
            Assert.AreEqual(planeNormal, contact4.Normal);
            Assert.AreEqual(0, contact4.Penetration);

            //////// unit box is centered at origin

            box = new Box(new NoBody(Matrix.CreateTranslation(new Vector3(0f, 0f, 0f))), Matrix.Identity, new Vector3(1f, 1f, 1f));
            plane = new Forever.Physics.Collide.Plane(planeBody, Vector3.Zero, planeNormal);
            data = new CollisionData();

            contactsFound = IntersectionTests.boxAndPlane(box, plane, data);

            Assert.AreEqual(4, contactsFound);

            contact1 = data.contacts[0];
            contact2 = data.contacts[1];
            contact3 = data.contacts[2];
            contact4 = data.contacts[3];


            Assert.AreEqual(new Vector3(-1f, 0f, -1f), contact1.Point);
            Assert.AreEqual(planeNormal, contact1.Normal);
            Assert.AreEqual(1f, contact1.Penetration);

            Assert.AreEqual(new Vector3(-1f, 0f, 1f), contact2.Point);
            Assert.AreEqual(planeNormal, contact2.Normal);
            Assert.AreEqual(1f, contact2.Penetration);

            Assert.AreEqual(new Vector3(1f, 0f, -1f), contact3.Point);
            Assert.AreEqual(planeNormal, contact3.Normal);
            Assert.AreEqual(1f, contact3.Penetration);
            
            Assert.AreEqual(new Vector3(1f, 0f, 1f), contact4.Point);
            Assert.AreEqual(planeNormal, contact4.Normal);
            Assert.AreEqual(1f, contact4.Penetration);


            //// unit box centered at y=1 and rotated 45 degrees on each axis

            float angle = (float)Math.PI * 0.25f;
            Matrix boxMatrix = Matrix.CreateFromYawPitchRoll(angle, angle, angle) * Matrix.CreateTranslation(new Vector3(0f, 1f, 0f));


            box = new Box(new NoBody(boxMatrix), Matrix.Identity, new Vector3(1f, 1f, 1f));
            plane = new Forever.Physics.Collide.Plane(planeBody, Vector3.Zero, planeNormal);
            data = new CollisionData();

            contactsFound = IntersectionTests.boxAndPlane(box, plane, data);

            Assert.AreEqual(1, contactsFound);

            contact1 = data.contacts[0];


            Assert.True(
                TrickyMathHelper.AlmostEquals(0.7071067f, contact1.Penetration)
            );

            Assert.True(
                TrickyMathHelper.CloseEnoughToBeSame(
                    new Vector3(-0.2071069f, 0f, -0.2071068f),
                    contact1.Point));
                    

            

            ////unit box centered at y=sqrt(2) and rotated 45 degrees on each axis

            boxMatrix = Matrix.CreateFromYawPitchRoll(angle, angle, angle) * Matrix.CreateTranslation(new Vector3(0f, TrickyMathHelper.Sqrt(2), 0f));

            box = new Box(new NoBody(boxMatrix), Matrix.Identity, new Vector3(1f, 1f, 1f));

            plane = new Forever.Physics.Collide.Plane(planeBody, Vector3.Zero, planeNormal);
            data = new CollisionData();

            contactsFound = IntersectionTests.boxAndPlane(box, plane, data);

            Assert.AreEqual(1, contactsFound);

            contact1 = data.contacts[0];


            Assert.True(
                TrickyMathHelper.AlmostEquals(0.292893142f,contact1.Penetration)
               
            );
            

            Assert.True(
                TrickyMathHelper.CloseEnoughToBeSame(
                    new Vector3(-0.2071069f, 0f, -0.2071068f), 
                    contact1.Point
                )
            );
            Assert.AreEqual(Vector3.UnitY, contact1.Normal);

             
        }


        [Test]
        public void IntersectionTests_BoxVsPoint()
        {
            IRigidBody body = new NoBody();
            Box box;
            Vector3 point;
            CollisionData data;
            Contact contact;

            box = new Box(body, Matrix.Identity, new Vector3(1f, 1f, 1f));
            //////////////////////////  point@+,+,+
            point = new Vector3(0.75f, 1f, 1f);

            data = new CollisionData();
            data.contactsLeft = 1;

            Assert.AreEqual(1, IntersectionTests.boxAndPoint(box, point, data));
            contact = data.contacts[0];

            Assert.AreEqual(Vector3.Left, contact.Normal);
            Assert.AreEqual(0.25f, contact.Penetration);
            Assert.AreEqual(1, data.contacts.Count);
            Assert.AreEqual(point, contact.Point);
            Assert.Null(contact.Bodies[1]);

            ///////////////////////////// point@center when box is translated


            Vector3 translatedPos = Vector3.UnitY * 10f;
            body = new NoBody(Matrix.CreateTranslation(translatedPos));

            box = new Box(body, Matrix.Identity, new Vector3(1f, 1f, 1f));
            point = translatedPos;

            data = new CollisionData();
            data.contactsLeft = 1;

            Assert.AreEqual(1, IntersectionTests.boxAndPoint(box, point, data));
            contact = data.contacts[0];

            Assert.AreEqual(1f, contact.Penetration);
            Assert.AreEqual(1, data.contacts.Count);
            Assert.AreEqual(point, contact.Point);
            Assert.AreEqual(Vector3.Right, contact.Normal);
            Assert.Null(contact.Bodies[1]);



            /////////////////////////////////////point@+,+,+ when box is translated

            point = translatedPos + Vector3.Right * 0.75f;
            data = new CollisionData();
            data.contactsLeft = 1;

            Assert.AreEqual(1, IntersectionTests.boxAndPoint(box, point, data));
            contact = data.contacts[0];

            Assert.AreEqual(0.25f, contact.Penetration);
            Assert.AreEqual(1, data.contacts.Count);
            Assert.AreEqual(point, contact.Point);

            Vector3 normal = Vector3.Left;

            Assert.AreEqual(normal, contact.Normal);

            Assert.Null(contact.Bodies[1]);


            //////////////////////////////////////////point@tip(+,+,+)


            body = new NoBody();

            box = new Box(body, Matrix.Identity, new Vector3(1f, 1f, 1f));

            point = new Vector3(1f, 1f, 1f);
            data = new CollisionData();
            data.contactsLeft = 1;
            Assert.AreEqual(1, IntersectionTests.boxAndPoint(box, point, data));
            contact = data.contacts[0];

            Assert.AreEqual(0f, contact.Penetration);
            Assert.AreEqual(1, data.contacts.Count);
            Assert.AreEqual(point, contact.Point);
            Assert.AreEqual(Vector3.Left, contact.Normal);
            Assert.Null(contact.Bodies[1]);

            /////////////////////////////////////////point@tip(-,-,-)


            point = new Vector3(-1f, -1f, -1f);
            data = new CollisionData();
            data.contactsLeft = 1;
            Assert.AreEqual(1, IntersectionTests.boxAndPoint(box, point, data));
            contact = data.contacts[0];

            Assert.AreEqual(0f, contact.Penetration);
            Assert.AreEqual(1, data.contacts.Count);
            Assert.AreEqual(point, contact.Point);
            Assert.AreEqual(Vector3.Right, contact.Normal);
            Assert.Null(contact.Bodies[1]);



            /////////////////////////////////////////point@middle of nowhere

            point = new Vector3(10000f, 10000f, 10000f);
            data = new CollisionData();
            data.contactsLeft = 1;
            Assert.AreEqual(0, IntersectionTests.boxAndPoint(box, point, data));


            /////////////////////////////////////////point@+,0,0 after box is rotated
            float rotY = (float)Math.PI / 4f; //45 degrees

            
            point = new Vector3(1.001f, 0f, 0f);
            body = new NoBody(Matrix.CreateRotationY(rotY));
            box = new Box(body, Matrix.Identity, new Vector3(1f, 1f, 1f));
            data = new CollisionData();

            Assert.AreEqual(1, IntersectionTests.boxAndPoint(box, point, data));
            Assert.AreEqual(1, data.contacts.Count);
            contact = data.contacts[0];

            Assert.True(
                TrickyMathHelper.CloseEnoughToBeSame(contact.Normal, new Vector3(-0.7071068f, 0, 0.7071068f) )
            );

            /////////////////////////////////////////////////////point@+,0,0 after box is rotated other direction
            
            point = new Vector3(1.001f, 0f, 0f);
            body = new NoBody(Matrix.CreateRotationY(-rotY));
            box = new Box(body, Matrix.Identity, new Vector3(1f, 1f, 1f));
            data = new CollisionData();

            Assert.AreEqual(1, IntersectionTests.boxAndPoint(box, point, data));
            Assert.AreEqual(1, data.contacts.Count);
            contact = data.contacts[0];

            Assert.True(
                TrickyMathHelper.CloseEnoughToBeSame(contact.Normal, new Vector3(-0.7071068f, 0, -0.7071068f))
            );

        }




        [Test]
        public void IntersectionTests_BoxVsBox_ZeroPenetration()
        {
            Vector3 leftPos = new Vector3(-100f, -100f, -100f);
            IRigidBody leftBody = new NoBody(Matrix.CreateTranslation(leftPos));
            Box left = new Box(leftBody, Matrix.Identity, new Vector3(1f, 1f, 1f));

            Vector3 rightPos = new Vector3(100f, 100f, 100f);
            IRigidBody rightBody = new NoBody(Matrix.CreateTranslation(rightPos));
            Box right = new Box(rightBody, Matrix.Identity, new Vector3(1f, 1f, 1f));

            CollisionData data = new CollisionData();
            int contactsFound = IntersectionTests.boxAndBox(left, right, data);


            Assert.AreEqual(0, contactsFound);
            Assert.AreEqual(0, data.contacts.Count);
        }




        //////////////////////////////////////////////////////////////////////////////////////////





        #region iRigidBody player
        private class NoBody : IRigidBody
        {

            public NoBody(Matrix world)
            {
                this.world = world;
            }
            public NoBody()
            {
                this.world = Matrix.Identity;
            }

            public Vector3 Up { get { return Vector3.Up; } }
            public Vector3 Right { get { return Vector3.Right; } }
            public Vector3 Forward { get { return Vector3.Forward; } }

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

            public Vector3 Rotation { get { throw new NotImplementedException(); } }
            public void addVelocity(Vector3 v)
            {
                throw new NotImplementedException();
            }
            public void addRotation(Vector3 v)
            {
                throw new NotImplementedException();
            }

            public Microsoft.Xna.Framework.Matrix InertiaTensorWorld
            {
                get { throw new NotImplementedException(); }
            }


            public Microsoft.Xna.Framework.Matrix InverseInertiaTensorWorld
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

            Matrix world = Matrix.Identity;
            public Matrix World { get { return world; } }

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

            public Microsoft.Xna.Framework.Vector3 LinearMomentum
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
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
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

            public Microsoft.Xna.Framework.Vector3 Position
            {
                get
                {
                    return this.world.Translation;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public Microsoft.Xna.Framework.Vector3 Velocity
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

            public Microsoft.Xna.Framework.Vector3 Acceler
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

            public Microsoft.Xna.Framework.Vector3 LastAccel
            {
                get { throw new NotImplementedException(); }
            }

            public bool Awake
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
