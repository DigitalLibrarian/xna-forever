    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Forever.Physics.Collide;
using Forever.Physics.ForceGen;
using Forever.Interface;
using Forever.GameEntities;

using Forever.Demos.Components.SkyDome;

namespace Forever.Demos
{
    public class CollideDemo : Forever.Demos.Demo
    {
        # region Properties
        public Vector3 DefaultSpawnPosOne { get; set; }
        public Vector3 DefaultSpawnPosTwo { get; set; }
        public float DefaultSphereRadius { get; set; }
        public float DefaultSphereMass { get; set; }
        public float DefaultRestitution { get; set; }


        public ModelEntity EntityOne { get; set; }
        public ModelEntity EntityTwo { get; set; }


        public SkyDome SkyDome { get; set; }


        public ContactResolver ContactResolver { get; set; }
        #endregion

        CollideType primOneCollideType;
        CollideType primTwoCollideType;

        #region Constructors
        public CollideDemo()
            : this(CollideType.Sphere, CollideType.Sphere) { }

        public CollideDemo(CollideType primOne, CollideType primTwo) : base()
        {
            ContactResolver = new Physics.Collide.ContactResolver();

            primOneCollideType = primOne;
            primTwoCollideType = primTwo;

            DefaultSphereMass = 1f;
            DefaultSphereRadius = 1f;

            DefaultSpawnPosOne = new Vector3(-DefaultSphereRadius * 3.51f, 10.001f, 0.001f);
            DefaultSpawnPosTwo = new Vector3(DefaultSphereRadius * 3.51f, 10.001f, 0.001f);

           
            DefaultRestitution = 1f;
            LastContact = new Contact();

        }

        public static CollideDemo SphereOnSphere()
        {
            return new CollideDemo(CollideType.Sphere, CollideType.Sphere);
        }
        public static CollideDemo BoxOnSphere()
        {
            return new CollideDemo(CollideType.Box, CollideType.Sphere);
        }

        public static CollideDemo BoxOnBox()
        {
            return new CollideDemo(CollideType.Box, CollideType.Box);
        }
        
        #endregion

        public override void AddEntity(IGameEntity ent)
        {
            //this.ForceRegistry.Add(ent, new WhenAwakeFG(Vector3.Down * 0.0000004f));
            //this.ForceRegistry.Add(ent, new TowardsBodyFG(CamBody, 0.000004f));
            base.AddEntity(ent);
        }


        public override void LoadContent()
        {
            base.LoadContent();

            
            EntityOne = MEFactory.Create(primOneCollideType, DefaultSpawnPosOne, DefaultSphereMass, DefaultSphereRadius);
            EntityTwo = MEFactory.Create(primTwoCollideType, DefaultSpawnPosTwo, DefaultSphereMass, DefaultSphereRadius);
            
            EntityOne.Body.AngularDamping = 0.999999999f;
            EntityOne.Body.LinearDamping = 0.999999999f;
            EntityTwo.Body.AngularDamping = 0.999999999f;
            EntityTwo.Body.LinearDamping = 0.999999999f;
            
            
            //EntityOne.Body.addTorque(new Vector3(0.0f, 0.001f, 0.0f));
            //EntityTwo.Body.addTorque(new Vector3(0.001f, 0f, 0.001f));
            
            /*
            EntityOne.addForce(new Vector3(
                0.00001f, 
                0.0000005f, 
                0f
                ));
            

            EntityTwo.addForce(new Vector3(
                -0.00001f,
                0.00000025f,
                0f
                ));
             */
            

            
            // Thump Entity One towards Entity Two by applying a force on it's surface
            float radius = EntityOne.GeometryData.BoundingSphere.Radius;
            EntityOne.addForce(
                new Vector3(0.00001f, 0f, 0f),
                EntityOne.CenterOfMass -
                    new Vector3(0f, radius, 0f)
                    );


            // Thump Entity Two towards Entity One by applying a force on it's surface
            radius = EntityTwo.GeometryData.BoundingSphere.Radius;
            EntityTwo.addForce(
                new Vector3(-0.00001f, 0f, 0f),
                EntityTwo.CenterOfMass +
                    new Vector3(0f, radius, 0f)
                    );

            

            this.AddEntity(EntityOne);
            this.AddEntity(EntityTwo);

            LoadBackground();

            CamBody.Position = new Vector3(0f, 10f, 20f);
           
        }

        private void LoadBackground()
        {
            SkyDome = new SkyDome();
            SkyDome.LoadContent(this.ScreenManager.Game.Content);
            this.RenderGroup.Add(SkyDome);


            IRigidBody body = new Forever.Physics.RigidBody(Vector3.Zero);
            Forever.Physics.Collide.Plane plane = new Physics.Collide.Plane(body, Vector3.Zero, Vector3.Up);
            IGeometryData geoData = new Forever.GameEntities.EntityGeometryData();
            geoData.Prim = plane;

            TilingPlaneEntity floorEntity = new TilingPlaneEntity(body, geoData);
            floorEntity.LoadContent(this.ScreenManager.Game.Content);
            this.AddEntity(floorEntity);
            SpawnGroupOfBlocks();
        }

        private void SpawnGroupOfBlocks()
        {
            int size = 3;

            float spacing = 5f;
            Vector3 origin = Vector3.Zero + (new Vector3(-(size*spacing)/2f, 0f, (-size*spacing)/2f) );
            int numRows = size;
            int numCols = size;
            float ceilingY = 15f;
            float floorY = 5f;
            for (float i = 0; i < numCols; i++)
            {
                for (float j = 0; j < numCols; j++)
                {
                    Vector3 spawnPos = origin + new Vector3(i * spacing, ceilingY, j * spacing);
                    ModelEntity ceiling = MEFactory.Create(CollideType.Box, spawnPos, 1f, 1f);
                    spawnPos = origin + new Vector3(i * spacing, floorY, j *spacing);
                    ModelEntity floor = MEFactory.Create(CollideType.Box, spawnPos, 1f, 1f);

                    this.AddEntity(ceiling);
                    this.AddEntity(floor);

                }
            }
        }

        

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            PumpCollisions((float)gameTime.ElapsedGameTime.Milliseconds);
            SkyDome.Update();
        }

        private void PumpCollisions(float duration)
        {

            List<ICollideable> colliders = this.ICollideables;

            IEnumerable<ICollideable> gameObjectsLeft = colliders;
            IEnumerable<ICollideable> gameObjectsRight = colliders;

            CollisionDetector detect = new CollisionDetector();
            List<Contact> contacts = new List<Contact>();
            foreach (ICollideable left in gameObjectsLeft)
            {
                foreach (ICollideable right in gameObjectsRight)
                {
                    if (left != right)
                    {
                        CollisionData data = ManageCollisions(detect, left, right, duration);
                        contacts.AddRange(data.contacts);
                    }
                }
            }

            if (contacts.Count > 0)
            {
                int brak = 2344;
            }

            ContactResolver.FullContactResolution(contacts, duration);

            if (contacts.Count > 0)
            {
                saveLastContact(contacts[0]);
            }


        }

        private CollisionData ManageCollisions(CollisionDetector detect, ICollideable left, ICollideable right, float duration)
        {
 
            CollisionData data = new CollisionData();
            data.restitution = this.DefaultRestitution;
            detect.FindContacts(left.GeometryData.Prim, right.GeometryData.Prim, data);
            return data;
        }



        #region LastContact for debugging
        public Contact LastContact { get; set; }
        void clearLastContact()
        {
            LastContact.Bodies = new IRigidBody[2];
            LastContact.Normal = Vector3.Zero;
            LastContact.Point = Vector3.Zero;
            LastContact.Friction = 0f;
            LastContact.Penetration = 0f;
            LastContact.Restitution = 0f;
            LastContact.DiscoveryHint = ContactDiscoveryHint.None;

        }
        void saveLastContact(Contact contact)
        {
            LastContact.DiscoveryHint = contact.DiscoveryHint;
            LastContact.Bodies = contact.Bodies;
            LastContact.Normal = contact.Normal;
            LastContact.Point = contact.Point;
            LastContact.Friction = contact.Friction;
            LastContact.Penetration = contact.Penetration;
            LastContact.Restitution = contact.Restitution;
            LastContact.DesiredDeltaVelocity = contact.DesiredDeltaVelocity;
            LastContact.ContactVelocity = contact.ContactVelocity;
        }
        #endregion 



        #region Console Commands
        public void Together()
        {
            PushTogether(this.EntityOne, this.EntityTwo, 0.0000025f);
        }

        public void Apart()
        {
            PushApart(this.EntityOne, this.EntityTwo, 0.00001f);
        }

        public void Tug()
        {
            Tug(EntityOne, CamBody.Position);
        }

        public void ConstantPull()
        {
            ConstantPull(EntityOne, EntityTwo, 0.000001f);
        }
        public void ConstantPull(ModelEntity entityOne, ModelEntity entityTwo, float forceMag)
        {
            this.ForceRegistry.Add( entityOne, new TowardsBodyFG( entityTwo.Body, forceMag));
            this.ForceRegistry.Add( entityTwo, new TowardsBodyFG( entityOne.Body, forceMag));
        }
        /// <summary>
        /// Produce a physical box and spawn it at the camera position
        /// </summary>
        /// <returns></returns>
        private ModelEntity TestCube()
        {
            return MEFactory.Create(CollideType.Box, Camera.Position, DefaultSphereMass, DefaultSphereRadius);

        }
        
        #endregion

    }
}

