using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Forever.Physics;
using Forever.Geometry;
using Forever.Physics.Collide;


namespace Forever.GameEntities
{
    // "Collide Demo Test" Factory
    public class ModelEntityFactory
    {
        Dictionary<CollideType, Model> Models { get; set; }

        public float LinearDamping { get; set; }
        public float AngularDamping { get; set; }

        public ModelEntityFactory()
        {
            LinearDamping = 0.99997f;
            AngularDamping = 0.9999997f;
        }

        public void LoadContent(ContentManager content)
        {
            Models = new Dictionary<CollideType, Model>();
            Models.Add(CollideType.Sphere, content.Load<Model>("basketball"));
            Models.Add(CollideType.Box, content.Load<Model>("sharpCube"));
        }

        public ModelEntity Create(CollideType primType, Vector3 position, float mass, float dimension = 1f)
        {

            switch (primType)
            {
                case CollideType.Sphere:
                    return CreateSphere(position, mass, dimension);
                    
                case CollideType.Box:
                    return CreateCube(position, mass, dimension);

            }

            throw new ArgumentException("Invalid CollideType");
        }

        public ModelEntity CreateSphere(Vector3 position, float mass, float dimension)
        {
            Model model = Models[CollideType.Sphere];
            float radius = dimension;
         
            RigidBody body = new RigidBody(position);

            body.LinearDamping = LinearDamping;
            body.AngularDamping = AngularDamping;
            body.Awake = true;
            body.Mass = mass;
            body.InertiaTensor = InertiaTensorFactory.Sphere(mass, radius);

            Sphere prim = new Sphere(body, Matrix.Identity, radius);

            EntityGeometryData geoData = new EntityGeometryData();

            geoData.Prim = prim;
            geoData.BoundingSphere = new BoundingSphere(Vector3.Zero, prim.Radius);

            return new ModelEntity(model, body, geoData);
        }

        public ModelEntity CreateCube(Vector3 position, float mass, float dimension)
        {
            Model model = Models[CollideType.Box];
            float size = dimension;

            RigidBody body = new RigidBody(position);

            body.LinearDamping = LinearDamping;
            body.AngularDamping = AngularDamping;
            body.Awake = true;
            body.Mass = mass;
            body.InertiaTensor = InertiaTensorFactory.Cubiod(mass, size, size, size);

            float halfSize = size;

            Box prim = new Box(body, Matrix.Identity, new Vector3(halfSize, halfSize, halfSize));
            EntityGeometryData geoData = new EntityGeometryData();
            geoData.Prim = prim;

            geoData.BoundingSphere = new BoundingSphere(Vector3.Zero, halfSize);
            geoData.BoundingSphere = Forever.Physics.Collide.PrimitiveHelper.ModelBoundingSphere(model);
            return new ModelEntity(model, body, geoData);
        }



        public ModelEntity CreateDefaultFromModel(Model model)
        {
            return CreateDefaultFromModel(model, Vector3.Zero);
        }
        public ModelEntity CreateDefaultFromModel(Model model, Vector3 pos)
        {


            AABBTree tree = AABBFactory.AABBTree(model);
            BoundingBox box = tree.Root.BBox;

            float radius = BoundingSphere.CreateFromBoundingBox(box).Radius;
            float mass = 1f;
            RigidBody body = new RigidBody(pos);

            body.LinearDamping = LinearDamping;
            body.AngularDamping = AngularDamping;
            body.Awake = true;
            body.Mass = mass;
            body.InertiaTensor = InertiaTensorFactory.Sphere(mass, radius);

            Sphere prim = new Sphere(body, Matrix.Identity, radius);
            EntityGeometryData geoData = new EntityGeometryData();
            geoData.BoundingSphere = new BoundingSphere(Vector3.Zero, radius);
            geoData.Prim = prim;

            return new ModelEntity(model, body, geoData);
        }
    }

}
