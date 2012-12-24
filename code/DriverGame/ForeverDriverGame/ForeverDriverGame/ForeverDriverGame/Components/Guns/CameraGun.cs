using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Forever;
using Forever.Interface;
using Forever.Render;

namespace Forever.Demos.Components.Guns
{

    public enum FiringType { Primary, Secondary };

    public abstract class CameraGun : IRenderable 
    {
        public SpriteBatch SpriteBatch { get; set; }

        public ICollideable Target { get; set; }
        public Ray Ray { get; set; }


        /// <summary>
        /// Expected to be centered on center pixel
        /// </summary>
        public Texture2D ReticuleTexture { get; protected set; }
        protected Color ReticuleColor { get; set; }


        public virtual void Update(GameTime gameTime, List<ICollideable> gameObjects, ICamera camera)
        {
            Ray  = new Ray(camera.Position, camera.Forward);
            Target = null;

            foreach (ICollideable gameObject in gameObjects)
            {
                float? dist = this.DistanceToIntersect(Ray, gameObject);
                if (dist != null && dist > 0)
                {
                    Target = gameObject;
                    break;
                }
            }
        }

        protected float? DistanceToIntersect(Ray ray, ICollideable gameObject)
        {

            BoundingSphere sphere = gameObject.GeometryData.BoundingSphere;
            sphere = sphere.Transform(gameObject.GeometryData.Prim.Transform);

            return  Ray.Intersects(sphere);
        }

        public virtual void LoadContent(ContentManager content)
        {
            string textureName = "crosshair";
            ReticuleTexture = content.Load<Texture2D>(textureName);
            

        }


        public virtual void Render(RenderContext renderContext, GameTime gameTime)
        {
            if (SpriteBatch == null)
            {
                return;
            }

            if (Target == null)
            {
                ReticuleColor = Color.White;
            }
            else
            {
                ReticuleColor = Color.Red;
            }


            Viewport viewport = renderContext.GraphicsDevice.Viewport;
            int middlePixelX = viewport.Width / 2;
            int middlePixelY = viewport.Height / 2;
            int halfSize = 15;

            SpriteBatch.Begin();
            SpriteBatch.Draw(ReticuleTexture,
                new Rectangle(middlePixelX - halfSize, middlePixelY - halfSize, halfSize * 2, halfSize * 2),
                ReticuleColor );
            SpriteBatch.End();
        }


        public virtual void Fire(FiringType firingType){}

    }
}
