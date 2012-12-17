using System;
using NUnit.Framework;

using Microsoft.Xna.Framework;
using Forever.Physics;
using Forever;

namespace ForeverTests
{


    [TestFixture]
    public class RigidBodyTests
    {

        public class Helpers
        {
            // Just checks that body isn't moving and has default values
            public static void AssertBodyAtRest(RigidBody body, Vector3 expectedPosition = new Vector3())
            {
                Assert.AreEqual(expectedPosition, body.CenterOfMass, "Center of Mass");
                Assert.AreEqual(expectedPosition, body.Position, "Position");
                Assert.AreEqual(Vector3.Zero, body.Velocity, "Velocity");
                Assert.AreEqual(Vector3.Zero, body.Acceler, "Acceleration");
                Assert.AreEqual(Quaternion.Identity, body.Orientation, "Orientation");
                
                Assert.AreEqual(1f, body.LinearDamping, "LinearDamping");
                Assert.AreEqual(1f, body.AngularDamping, "AngularDamping");
                Assert.AreEqual(Matrix.Identity, body.World, "World Matrix");
            }


            public static RigidBody CreateSphereBody(float mass, float radius)
            {

                RigidBody body = new RigidBody(Vector3.Zero);
                body.Awake = true;

                //simulating hollow sphere of mass and length

                body.Mass = mass;

                float sphere_coeff = ((1f / 12f) * mass * radius * radius);
                Matrix inertiaTensor = new Matrix(
                    sphere_coeff, 0f, 0f, 0f,
                    0f, sphere_coeff, 0f, 0f,
                    0f, 0f, sphere_coeff, 0f,
                    0f, 0f, 0f, sphere_coeff
                );

                body.InertiaTensor = inertiaTensor;
                body.LinearDamping = 1f;
                body.AngularDamping = 1f;

                return body;
            }

        }

        public int manyTimes = 10000;
        public float simulationTick = 1f;


        [Test]
        public void RigidBody_Zero_Case()
        {
            RigidBody body = new RigidBody(Vector3.Zero);

            //starts off awake
            Assert.AreEqual(true, body.Awake);

            //default starting state
            Helpers.AssertBodyAtRest(body);
        }

        [Test]
        public void RigidBody_InfiniteMass()
        {
            RigidBody body = new RigidBody(Vector3.Zero);
            body.InverseMass = 0f;


            Assert.AreEqual(float.MaxValue, body.Mass, "Mass");
            Assert.False(body.HasFiniteMass, "HasFiniteMass");

            Vector3 force = Vector3.Up;
            Helpers.AssertBodyAtRest(body, Vector3.Zero);


            //add force at center of mass
            body.addForce(force);
            body.integrate(1);
            Helpers.AssertBodyAtRest(body);

            //Now add force at point other than center of mass
            Vector3 worldCoord = Vector3.Right;
            body.addForce(force, worldCoord);
            body.integrate(1);
            
            Helpers.AssertBodyAtRest(body);


            //Now let's show that infinitely massed objects don't respond to torque application
            Vector3 torque = Vector3.Right;

            body.addTorque(torque);
            body.integrate(1);

            Helpers.AssertBodyAtRest(body);
        }



        [Test]
        public void RigidBody_Sleeping()
        {
            RigidBody body = new RigidBody(Vector3.Zero);
            body.Mass = 1f; // it's allowed to move if it thinks it shoud...

            //starts out awake
            Assert.AreEqual(true, body.Awake);
            Helpers.AssertBodyAtRest(body);


            // Adding a force, wakes it up
            body.addForce(Vector3.Up);
            Assert.AreEqual(true, body.Awake);

            // putting  it to sleep before integration should bypass any movement calcs.

            body.Awake = false;
            body.integrate(1);

            Helpers.AssertBodyAtRest(body);
        }


        [Test]
        public void RigidBody_Translate()
        {
            RigidBody body = new RigidBody(Vector3.Zero);

            Helpers.AssertBodyAtRest(body);
            body.Translate(Vector3.Up);
            Helpers.AssertBodyAtRest(body, Vector3.Up);
            body.Translate(Vector3.Up);
            Helpers.AssertBodyAtRest(body, Vector3.Up*2f);
        }



        [Test]
        public void RigidBody_WorldMatrix()
        {
            RigidBody body = new RigidBody(Vector3.Zero);
            body.Awake = true;
            body.Mass = 1f;

            float angle = (float)Math.PI;
            Quaternion orientation = Quaternion.CreateFromYawPitchRoll(angle, 0f, 0f);
            body.Orientation = orientation;
            body.Position = Vector3.Up;

            Matrix expectedWorld = new Matrix(
                    -1f, 0f, .00000008742278f, 0f,
                    0f, 1f, 0f, 0f,
                    -.00000008742278f, 0f, -1f, 0f,
                    0f, 1f, 0f, 1f
                );


            //integrate once  for update
            body.integrate(1f);

            Assert.AreEqual(expectedWorld, body.World);

            int total_iterations = 1000000;
            for (int i = 1; i <= total_iterations; i++)
            {
                body.integrate(1); 
                Assert.AreEqual(expectedWorld, body.World, "Iteration: " + i);

            }

            Assert.AreEqual(expectedWorld, body.World);

            

        }



        [Test]
        public void RigidBody_InertiaTensor()
        {
            RigidBody body = new RigidBody(Vector3.Zero);
            body.Awake = true;

            //simulating hollow sphere of mass and length

            float radius = 2f;
            float mass = 1f;
            body.Mass = mass;

            float sphere_coeff = ((1f / 12f) * mass * radius  * radius);
            Matrix inertiaTensor = new Matrix(
                sphere_coeff, 0f, 0f, 0f,
                0f, sphere_coeff, 0f, 0f, 
                0f, 0f, sphere_coeff, 0f,
                0f, 0f, 0f, 1f
            );

            body.InertiaTensor = inertiaTensor;

            

            Assert.AreEqual(Matrix.Identity, body.World, "Initial World");
            Assert.AreEqual(inertiaTensor, body.InertiaTensor, "Initial Intertia Tensor");
            Assert.AreEqual(Matrix.Invert(inertiaTensor), body.InverseInertiaTensor, "Initial InverseInertiaTensor");

            //no bleed off
            body.LinearDamping = 1f;

            //adding a force once should set it moving, never to stop again
            body.addForce(Vector3.UnitZ, body.Position + (new Vector3(-radius, 0f, 0f)));

            //nothing physically happens until integration time
            Assert.AreEqual(Vector3.Zero, body.Position, "Intitial Position");
            Assert.AreEqual(Vector3.Zero, body.AngularMomentum, "Intial Rotation");
            Assert.AreEqual(Quaternion.Identity, body.Orientation, "Intial Orientation");

            body.integrate(1f);

            Assert.AreEqual(Vector3.UnitZ * 1f, body.Position, "Computed Position");
            Assert.AreEqual(new Vector3(0f, 2f, 0f), body.AngularMomentum, "Computed Rotation");
            Quaternion orientation = new Quaternion(0f, 0.707106769f, 0f, 0.707106769f);
            Assert.AreEqual(orientation.X, body.Orientation.X);
            Assert.AreEqual(orientation.Y, body.Orientation.Y);
            Assert.AreEqual(orientation.Z, body.Orientation.Z);
            Assert.AreEqual(orientation.W, body.Orientation.W);
            
            Assert.AreEqual(orientation, body.Orientation, "Computed Orientation");


        }


        [Test]
        public void RigidBody_ForceOnCenterOfMass()
        {
            
            RigidBody body = new RigidBody(Vector3.Zero);
            body.Awake = true;
            body.Mass = 1f;
            Helpers.AssertBodyAtRest(body);


            body.addForce(Vector3.UnitZ);
            body.integrate(1f);
            Assert.AreEqual(Vector3.Zero, body.AngularMomentum);
            Assert.AreEqual(Vector3.UnitZ, body.LinearMomentum);
            Assert.AreEqual(new Vector3(0f, 0f, 1f), body.Position);


            body.addForce(-Vector3.UnitZ * 2f);
            body.integrate(1f);
            Assert.AreEqual(Vector3.Zero, body.AngularMomentum);
            Assert.AreEqual(-Vector3.UnitZ, body.LinearMomentum);
            Assert.AreEqual(new Vector3(0f, 0f, 0f), body.Position);

            body = new RigidBody(Vector3.Zero);
            body.Awake = true;
            body.Mass = 1f;
            Helpers.AssertBodyAtRest(body);
            
            body.addForce(-Vector3.UnitZ);
            body.integrate(10f);
            
            Assert.AreEqual(new Vector3(0f, 0f, -1000f), body.Position);
            Assert.AreEqual(Vector3.Zero, body.AngularMomentum);


        }


        [Test]
        public void RigidBody_NoDamping()
        {
            RigidBody body = Helpers.CreateSphereBody(1f, 1f);

            

            // Get it spinning

            float totalTorqueMag = 1f;
            Vector3 totalTorque = new Vector3(totalTorqueMag, 0f, 0f);
            body.addTorque(totalTorque);
            
            body.integrate(simulationTick);
            Assert.AreEqual(totalTorque, body.AngularMomentum);


            IntegrateBody(body, simulationTick, manyTimes);

            Assert.AreEqual(totalTorque, body.AngularMomentum);

            Assert.AreEqual(0f, body.LinearMomentum.Length());
            Assert.AreEqual(0f, body.Velocity.Length());
            

            // Now thump it with diembodied force

            

            float totalForceMag = 1f;
            Vector3 totalForce = new Vector3(totalForceMag, 0f, 0f);
            body.addForce(totalForce);


            body.integrate(simulationTick);
            Assert.AreEqual(totalForce, body.LinearMomentum);
            Assert.AreEqual(totalTorque, body.AngularMomentum);

            body.integrate(manyTimes);
            Assert.AreEqual(totalForce, body.LinearMomentum);
            Assert.AreEqual(totalTorque, body.AngularMomentum);


            // Now thump it a specific point of sphere radius
            Vector3 sphereThump = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 thumpPoint = new Vector3(1f, 0f, 0f);
            Vector3 preThumpBodyPosition = body.Position;
            totalForce += sphereThump;
            body.addForce(sphereThump, thumpPoint);
            body.integrate(simulationTick);
            Assert.AreEqual(totalForce, body.LinearMomentum);
            Assert.AreNotEqual(totalTorque, body.AngularMomentum);
            

            Vector3 pt = thumpPoint;
            pt -= preThumpBodyPosition ;
            Vector3 thumpResultantTorque = Vector3.Cross(thumpPoint - preThumpBodyPosition, sphereThump);
            thumpResultantTorque = Vector3.Transform(thumpResultantTorque, body.InverseInertiaTensorWorld);
            totalTorque += thumpResultantTorque;
            //Assert.AreEqual(totalTorque, body.AngularMomentum);
            bool areSame = TrickyMath.CloseEnoughToBeSame(totalTorque, body.AngularMomentum, 0.05f);
            Assert.True( areSame );

            //Now if we turn on Damping, it'll  bleed back to zero

            body.LinearDamping = 0.99f;
            body.AngularDamping = 0.99f;

            IntegrateBody(body, simulationTick, manyTimes);

            Assert.LessOrEqual(float.MinValue, body.LinearMomentum.Length());
            Assert.LessOrEqual(float.MinValue, body.AngularMomentum.Length());

        }

        [Test]
        /// <summary>
        /// An object at rest stays at rest.  
        /// An object in uniform motion, stayas in uniform motion
        /// </summary>
        public void RigidBody_FirstLaw()
        {

            RigidBody body = Helpers.CreateSphereBody(1f, 1f);

            Helpers.AssertBodyAtRest(body, Vector3.Zero);

            IntegrateBody(body, simulationTick, manyTimes);
            Helpers.AssertBodyAtRest(body, Vector3.Zero);


            Vector3 totalForce = Vector3.Zero;
            Vector3 thump = new Vector3(1f, 1f, 1f);

            body.addForce(thump);
            totalForce += thump;

            body.integrate(1f);

            Assert.AreEqual(totalForce, body.LinearMomentum);
            Assert.AreEqual(totalForce, body.Velocity);

            IntegrateBody(body, simulationTick, manyTimes);

            Assert.AreEqual(totalForce, body.LinearMomentum);
            Assert.AreEqual(totalForce, body.Velocity);


        }

        [Test]
        /// <summary>
        /// F = mA
        /// </summary>
        public void RigidBody_SecondLaw()
        {

            RigidBody body = Helpers.CreateSphereBody(1f, 1f);
            Helpers.AssertBodyAtRest(body, Vector3.Zero);

            Vector3 totalForce = new Vector3(1f, 1f, 1f);

            body.addForce(totalForce);
            body.integrate(1f);

            Vector3 resultantAccel = totalForce * body.InverseMass  * simulationTick;

            Assert.AreEqual(Vector3.Zero, body.LastAccel);
            Assert.AreEqual(resultantAccel, body.Acceler);

            //Can expect this since we  know that was the only force acting  on object
            Assert.AreEqual(resultantAccel, body.Velocity);

            body.integrate(1f);

            Assert.AreEqual(Vector3.Zero, body.Acceler);
            Assert.AreEqual(resultantAccel, body.LastAccel);
            //Can expect this since we  know that was the only force acting  on object
            Assert.AreEqual(resultantAccel, body.Velocity);

            IntegrateBody(body, simulationTick, manyTimes);

            Assert.AreEqual(Vector3.Zero, body.Acceler);
            Assert.AreEqual(Vector3.Zero, body.LastAccel);
            //Can expect this since we  know that was the only force acting  on object
            Assert.AreEqual(resultantAccel, body.Velocity);




            body = Helpers.CreateSphereBody(1f, 1f);
            Helpers.AssertBodyAtRest(body);

            Vector3 thump = new Vector3(1f, 1f, 0f);
            Vector3 thumpPoint = new Vector3(1f, 1f, 1f);

            Vector3 resultantForce = thump;
            Vector3 resultantTorque = Vector3.Cross(thumpPoint - body.Position, thump);
            body.addForce(thump, thumpPoint);

            body.integrate(1f);

            Assert.AreEqual(resultantForce, body.LinearMomentum);
            Assert.AreEqual(resultantTorque, body.AngularMomentum);

            IntegrateBody(body, simulationTick, manyTimes);
            Assert.AreEqual(resultantForce, body.LinearMomentum);
            Assert.AreEqual(resultantTorque, body.AngularMomentum);

        }


        [Test]
        public void RigidBody_SpinTest()
        {

            RigidBody body = Helpers.CreateSphereBody(1f, 1f);

            

            // Get it spinning

            float totalTorqueMag = 1f;
            Vector3 spin = new Vector3(0f, 0f, totalTorqueMag);
            Vector3 totalTorque = spin;
            Vector3 totalForce = Vector3.Zero;

            body.addTorque(spin);
            IntegrateBody(body, simulationTick, 1);

            Assert.AreEqual(totalTorque, body.AngularMomentum);

            Assert.AreEqual(totalForce, body.LinearMomentum);
            Assert.AreEqual(0f, body.Velocity.Length());

            for (int i = 0; i < 100; i++)
            {
                //body.addTorque(spin);
                //totalTorque += spin;

                Quaternion orientBefore = body.Orientation;
                IntegrateBody(body, simulationTick, 1);
                Assert.AreNotEqual(orientBefore, body.Orientation, "Spin Test #1 - iteration " + i);
            }

            Assert.AreEqual(totalTorque * body.Mass, body.AngularMomentum);
            Assert.AreEqual(totalForce, body.LinearMomentum);
            Assert.AreEqual(0f, body.Velocity.Length());

            IntegrateBody(body, simulationTick, manyTimes);
            Assert.AreEqual(totalTorque, body.AngularMomentum);
            Assert.AreEqual(totalForce, body.LinearMomentum);
            Assert.AreEqual(0f, body.Velocity.Length());


        }


        [Test]
        public void RigidBody_ContactResolutionResponse()
        {
            RigidBody body = Helpers.CreateSphereBody(1f, 1f);

            Vector3 totalVelo = Vector3.Zero;
            Vector3 totalRot = Vector3.Zero;

            IntegrateBody(body, simulationTick, manyTimes);
            Helpers.AssertBodyAtRest(body);



            Vector3 initialThump = new Vector3(1f, 0f, 0f);
            body.addForce(initialThump);
            totalVelo += initialThump;
            

            Vector3 initialRot = new Vector3(1f, 0f, 0f);
            body.addRotation(initialRot);
            totalRot += initialRot;
            
            IntegrateBody(body, simulationTick, manyTimes);

            Assert.AreEqual(totalVelo, body.Velocity);


            Vector3 veloBump = new Vector3(0f, 1f, 0f);
            body.addVelocity(veloBump);
            totalVelo += veloBump;

            Vector3 rotBump = new Vector3(0f, 1f, 0f);
            body.addRotation(rotBump);
            totalRot += rotBump;

            IntegrateBody(body, simulationTick, manyTimes);

            Assert.AreEqual(totalVelo, body.Velocity);
            Assert.AreEqual(totalRot, body.Rotation);

        }



        void IntegrateBody(RigidBody rigidBody, float duration, int numIterations)
        {
            for (int i = 0; i < numIterations; i++)
            {
                rigidBody.integrate(duration);
            }
        }


      
    }
}
