using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Forever.Interface;
using Forever.Geometry;
using Forever.GameEntities;

namespace Forever.Demos
{
    public class BallDemo : Forever.Demos.Demo
    {
        class BallDemoContent
        {
            public string ModelAsset;
            public Model BallModel { get; set; }


            public void LoadContent(ContentManager content)
            {
                if (ModelAsset == null)
                {
                    throw new NullReferenceException("No model asset path defined.");
                }
                BallModel = content.Load<Model>(ModelAsset);

            }
        }


        float BallRadius = 1f;
        float BallMass = 1f;

        RigidBodyEntity ballEntity;
        BallDemoContent myContent = new BallDemoContent();

        public IGameEntity Ball { get { return ballEntity; } }
        public bool Debug { get { return ballEntity.Debug; } set { ballEntity.Debug = true; } }

        #region Constructors
        public BallDemo(string modelName) : base()
        {
             myContent.ModelAsset = modelName;
        }

        public BallDemo() : this("basketball")
        {
        }
        #endregion

        
        public void Nudge()
        {

            IRigidBody body = ballEntity.Body;

            Random rand = new Random();
            Vector3 direction = TrickyMathHelper.RandVector(rand);
            direction.Normalize();
            float forceMag = 0.0005f;
            Vector3 force = direction * forceMag;

            Vector3 bodyPoint = TrickyMathHelper.RandVector(rand);
            bodyPoint.Normalize();
            bodyPoint *= BallRadius;
            
            Ball.addForce(force, Ball.CenterOfMass + bodyPoint);


        }

 


        public override void LoadContent()
        {
            base.LoadContent();

            myContent.LoadContent(Content);


            
            ballEntity = new RigidBodyEntity(new Vector3(0f, 0f, -5f));
            ballEntity.Body.AngularDamping = 0.9997f;
            ballEntity.Body.LinearDamping = 0.9997f;
            ballEntity.Body.Awake = true;
            ballEntity.Body.Mass = BallMass;
            ballEntity.Body.InertiaTensor = InertiaTensorFactory.Sphere(BallMass, BallRadius);
            ballEntity.Body.Orientation = Quaternion.CreateFromYawPitchRoll(-4f, 0f, 0f);
            ballEntity.Model = myContent.BallModel;
            ballEntity.AABB = AABBFactory.AABBTree(myContent.BallModel);
            float radius = BoundingSphere.CreateFromBoundingBox(ballEntity.AABB.Root.BBox).Radius;
            this.AddEntity(ballEntity);


            this.Camera.Position = new Vector3(0f, 0f, 0f);
            this.Camera.Rotation = Quaternion.Identity;


        }


    }
}
