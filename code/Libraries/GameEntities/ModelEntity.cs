using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Forever.Physics;
using Forever.Physics.Collide;
using Forever.Interface;
using Forever.Render;

namespace Forever.GameEntities
{
    public class ModelEntity : IHasRigidBody, IGameEntity, ICollideable
    {


        
        public ModelEntity(Model model, IRigidBody body, EntityGeometryData geoData)
        {
            this.body = body;
            this.Model = model;
            //this.Prim = prim;

            this.GeometryData = geoData;
            DebugCompass = new DebugCompass(this.CenterOfMass);
            DebugCompass.LoadContent(null);

        }

        public Model Model { get; set; }

        public bool Awake { get { return Body.Awake; } }

        IRigidBody body;
        public IRigidBody Body
        {
            get { return body; }
        }

        public float Mass { get { return body.Mass; } }

        //public Primitive Prim { get; set; }


        public IGeometryData GeometryData { get; set; }

        public Matrix World
        {
            get { return body.World; }
        }


        public bool Debug { get; set; }
        public DebugCompass DebugCompass { get; set; }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content){ }

        public void UnloadContent(){}

        public void Update(GameTime gameTime)
        {
            DebugCompass.Position = Body.Position;
        }

        public void addForce(Vector3 force)
        {
            body.addForce(force);
        }

        public void addForce(Vector3 force, Vector3 point)
        {
            body.addForce(force, point);
        }

        public void integrate(float duration)
        {
            body.integrate(duration);
        }

        public Vector3 CenterOfMass
        {
            get { return body.Position; }
        }

        public void Translate(Vector3 translation)
        {
            body.Translate(translation);           
        }


        public void Render(Render.RenderContext renderContext, GameTime gameTime)
        {

            Renderer.RenderModel(Model, World, renderContext);


            if (Debug)
            {
                DebugCompass.Render(renderContext, gameTime);
                BoundingSphere worldSphere = new BoundingSphere( Body.Position, GeometryData.BoundingSphere.Radius);
                BoundingSphereRenderer.Render(worldSphere, renderContext, Color.OrangeRed);
            }
        }



        

    }
}
