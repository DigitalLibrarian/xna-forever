using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Adam.Screens;
using Forever;
using Forever.Interface;
using Forever.Render;
using Forever.Physics;
using Forever.GameEntities;


namespace Forever.Demos
{
    public class Demo : GameScreen
    {
        protected RenderContext renderContext;
        protected ContentManager content;
        protected QueuedList<IRenderable> renderGroup;
        protected QueuedList<IGameEntity> gameObjects;
        public ForceRegistry ForceRegistry;

        public QueuedList<IRenderable> RenderGroup { get { return renderGroup; } }
        public QueuedList<IGameEntity> GameObjects { get { return gameObjects; } }


        public Random Random { get; set; }

        public List<ICollideable> ICollideables
        {
            get
            {
                List<ICollideable> result = new List<ICollideable>();
                foreach (IGameEntity entity in GameObjects)
                {
                    if (entity is ICollideable)
                    {
                        ICollideable collider = entity as ICollideable;
                        result.Add(collider);
                    }
                }
                return result;
            }
        }

        public RenderContext RenderContext { get { return renderContext; } }
        public ContentManager Content { get { return content; } }

        protected UserControls CamControls { get; set; }


        private ModelEntityFactory modelEntityFactory;
        public ModelEntityFactory MEFactory { get { return modelEntityFactory; } }

        

        public ICamera Camera
        {
            get { return renderContext.Camera; }
        }


        public Demo()
        {
            renderGroup = new QueuedList<IRenderable>();
            gameObjects = new QueuedList<IGameEntity>();

            modelEntityFactory = new ModelEntityFactory();
            Random = new Random();

            ForceRegistry = new ForceRegistry();
        }

        public Demo(
            ContentManager contentManager, 
            RenderContext renderContext, 
            QueuedList<IRenderable> renderGroup, 
            QueuedList<IGameEntity> gameObjects) : base()
        {
            content = contentManager;
            this.renderContext = renderContext;

            this.renderGroup = renderGroup;
            this.gameObjects = gameObjects;

            ForceRegistry = new ForceRegistry();
        }

        public override void UnloadContent()
        {
            renderGroup.Clear();
            gameObjects.Clear();

            base.UnloadContent();
        }

        


        public override void LoadContent()
        {

            if (renderContext == null)
            {
                SetupRenderContextAndCamera();
            }

            if (content == null)
            {
                content = this.ScreenManager.Game.Content;
            }

            if (ForceRegistry == null)
            {
                ForceRegistry = new ForceRegistry();
            }

            MEFactory.LoadContent(Content);
            

        }
        protected void SetupRenderContextAndCamera()
        {
            ICamera cam = new EyeCamera();

            renderContext = new RenderContext(
                cam,
                this.ScreenManager.Game.GraphicsDevice
                );


            float radius = 2f;
            float mass = 1f;

            CamBody = new RigidBody(cam.Position);
            CamBody.Label = "Camera Body";
            CamBody.Awake = true;
            CamBody.LinearDamping = 0.9f;
            CamBody.AngularDamping = 0.7f;
            CamBody.Mass = mass;
            CamBody.InertiaTensor = InertiaTensorFactory.Sphere(mass, radius);
            CamControls = new UserControls(PlayerIndex.One, 0.000015f, 0.25f, 0.0003f, 1f);

        }

  


        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!otherScreenHasFocus && !coveredByOtherScreen)
            {
                UpdateCamera(gameTime);
            }


            if (!otherScreenHasFocus)
            {

                gameObjects.Update();

                /* 2. Update existing forces */
                foreach (IGameEntity entity in gameObjects)
                {
                    ForceRegistry.UpdateForceGenerators(entity, gameTime);
                    entity.Update(gameTime);

                    /* Step physics simulation forward */
                    entity.integrate((float)gameTime.ElapsedGameTime.Milliseconds);
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        RigidBody camBody;

        public  RigidBody CamBody
        {
            get { return camBody; }
            set { camBody = value; }
        }
        protected void UpdateCamera(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Vector3 actuatorTrans = CamControls.LocalForce;
            Vector3 actuatorRot = CamControls.LocalTorque;


            float forwardForceMag = -actuatorTrans.Z;
            float rightForceMag = actuatorTrans.X;
            float upForceMag = actuatorTrans.Y;

            Vector3 force =
                (Camera.Forward * forwardForceMag) +
                (Camera.Right * rightForceMag) +
                (Camera.Up * upForceMag);

           

            if (force.Length() != 0)
            {
                camBody.addForce(force);
            }



            Vector3 worldTorque = Vector3.Transform(CamControls.LocalTorque, CamBody.Orientation);

            if (worldTorque.Length() != 0)
            {
                camBody.addTorque(worldTorque);
            }

            CamBody.integrate((float)gameTime.ElapsedGameTime.Milliseconds);
            Camera.Position = CamBody.Position;
            Camera.Rotation = CamBody.Orientation;

        }        


        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            renderGroup.Update();
            base.Draw(gameTime);

            foreach (IRenderable renderable in renderGroup)
            {
                renderable.Render(renderContext, gameTime);
            }

        }



        public virtual void RemoveEntity(IGameEntity ent)
        {
            this.GameObjects.Remove(ent);
            this.RenderGroup.Remove(ent);
        }


        public virtual void AddEntity(IGameEntity ent)
        {
            this.GameObjects.Add(ent);
            this.RenderGroup.Add(ent);
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
            CamControls.HandleInput(input);
        }



        public void PushTogether(ModelEntity left, ModelEntity right, float forceMag)
        {
            Vector3 midLine = left.CenterOfMass - right.CenterOfMass;
            midLine.Normalize();
            midLine *= forceMag;
            left.addForce(-midLine);
            right.addForce(midLine);
            left.Body.Awake = true;
            right.Body.Awake = true;
        }
        public void PushApart(ModelEntity left, ModelEntity right, float forceMag)
        {
            Vector3 midLine = left.CenterOfMass - right.CenterOfMass;
            midLine.Normalize();
            midLine *= forceMag;

            left.addForce(midLine);
            right.addForce(-midLine);
        }

        public bool Tug(IGameEntity o)
        {
            if (o is ModelEntity)
            {
                Tug((ModelEntity)o);
                return true;
            }
            return false;
        }
        public void Tug(ModelEntity target)
        {
            Tug(target, Camera.Position);
        }
        public void Tug(ModelEntity target, float forceMag)
        {
            Tug(target, CamBody.Position, forceMag);
        }
        public void Tug(ModelEntity target, Vector3 position)
        {
            float mass = target.Body.Mass;

            float time = 100000;

            Vector3 diff = position - target.CenterOfMass;

            float forceMag = mass * (diff.Length() / time);

            Tug(target, position, forceMag);
        }
        public void Tug(ModelEntity target, Vector3 position, float forceMag)
        {
            Vector3 diff = position - target.CenterOfMass;

            diff.Normalize();
            diff *= forceMag;
            target.addForce(diff);
        }
        public void Tug(List<IGameEntity> entities) { Tug(entities, Camera.Position); }
        public void Tug(List<IGameEntity> entities, Vector3 position)
        {
            foreach (object o in entities)
            {
                if (o is ModelEntity)
                {
                    ModelEntity entity = o as ModelEntity;
                    Tug(entity, position);
                }
            }

        }

        public void Smack(List<IGameEntity> entities)
        {
            foreach (IGameEntity entity in GameObjects)
            {
                if (entity is ModelEntity)
                {
                    ModelEntity modelEntity = entity as ModelEntity;
                    Smack(modelEntity);
                }
            }
        }

        public virtual void Smack(ModelEntity modelEntity){
            Smack(modelEntity, 1f);
        }
        public void Smack(ModelEntity entity, float radius)
        {
            Smack(entity, 0.001f, radius);
        }
        public void Smack(ModelEntity left, float forceMag, float radius)
        {
            left.addForce(TrickyMath.RandVector(Random, forceMag), TrickyMath.RandVector(Random, radius));
        }



        public virtual List<ModelEntity> Spawn(string assetName, int numSpawn, Vector3 position, Vector3 ray)
        {

            Model model = null;
            try
            {
                model = Content.Load<Model>(assetName);
            }
            catch (ContentLoadException)
            {
            }

            if (model == null)
            {
                return null;
            }

            List<ModelEntity> spawnList = new List<ModelEntity>();
            ModelEntity spawn;
            Vector3 offset = ray;
            Vector3 nextSpawnPos;
            for (int i = numSpawn; i > 0; i--)
            {
                nextSpawnPos = position + (offset * i);
                spawn = MEFactory.CreateDefaultFromModel(model, nextSpawnPos);

                AddEntity(spawn);
                spawnList.Add(spawn);
                    

            }
            return spawnList;
        }



    }
}
