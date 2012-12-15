/*
 * Created by Alex Urbano Álvarez
 * goefuika@gmail.com
 */


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using Forever.Interface;
using Forever.Render;

namespace Forever.Demos.Components.SkyDome
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SkyDome : IRenderable
    {

        #region Properties
        private Model domeModel;
        private Effect domeEffect ;

        private float fTheta = 0.0f;
        private float fPhi = 0.0f;

        private bool realTime;

        //Camera camera;
        //Game game;

        Texture2D day, sunset, night;

        SkyDomeParameters parameters;
        #endregion

        #region Gets/Sets
        /// <summary>
        /// Gets/Sets Theta value
        /// </summary>
        public float Theta { get { return fTheta; } set { fTheta = value; } }

        /// <summary>
        /// Gets/Sets Phi value
        /// </summary>
        public float Phi { get { return fPhi; } set { fPhi = value; } }

        /// <summary>
        /// Gets/Sets actual time computation
        /// </summary>
        public bool RealTime { get { return realTime; } set { realTime = value; } }

        /// <summary>
        /// Gets/Sets the SkyDome parameters
        /// </summary>
        public SkyDomeParameters Parameters { get { return parameters; } set { parameters = value; } }
        #endregion

        #region Contructor

        public SkyDome()
        {

        }
        #endregion

        public void LoadContent(ContentManager content)
        {
            //TODO - move these asset names out
            this.domeModel = content.Load<Model>("SkyDome/skydome");
            this.domeEffect = content.Load<Effect>("SkyDome/Sky");

            this.day = content.Load<Texture2D>("SkyDome/SkyDay");
            this.sunset = content.Load<Texture2D>("SkyDome/Sunset");
            this.night = content.Load<Texture2D>("SkyDome/SkyNight");

            domeEffect.CurrentTechnique = domeEffect.Techniques["SkyDome"];

            RemapModel(domeModel, domeEffect);

            realTime = true;

            parameters = new SkyDomeParameters();
            parameters.LightColor = Color.Yellow.ToVector4();
            parameters.SunRadiusAttenuation = 100f;
            parameters.LargeSunRadiusAttenuation = 0f;
        
        }


        


        #region Update
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        public  void Update()
        {
           // KeyboardState keyState = Keyboard.GetState();

            if (realTime)
            {
                int minutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

                minutes = DateTime.Now.Second * 60  + (DateTime.Now.Millisecond * 6 / 100 );

                // break the day up into <num minutes in a day> equal sections and map to the correct angle based upon the date information
                this.fTheta = (float)minutes * (float)(Math.PI) / 12.0f / 60.0f;
            }

            parameters.LightDirection = this.GetDirection();
            parameters.LightDirection.Normalize();

           // base.Update(gameTime);
        }
        #endregion

        #region Draw

        //public override void Draw(GameTime gameTime)
        
        public void Render(RenderContext renderContext, GameTime gameTime)
        {
            ICamera camera = renderContext.Camera;

            Matrix[] boneTransforms = new Matrix[domeModel.Bones.Count];
            domeModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Matrix View = camera.View;
            Matrix Projection = camera.Projection;

            //game.GraphicsDevice.RenderState.DepthBufferEnable = false;
            //game.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

            GraphicsDevice graphics = renderContext.GraphicsDevice;

            graphics.BlendState = BlendState.AlphaBlend;
            graphics.DepthStencilState = DepthStencilState.DepthRead;

            foreach (ModelMesh mesh in domeModel.Meshes)
            {
                //TODO - magic number, parameterize this class
                Matrix World = boneTransforms[mesh.ParentBone.Index] *
                    Matrix.CreateTranslation(camera.Position.X, -10.0f, camera.Position.Z);

                Matrix WorldIT = Matrix.Invert(World);
                WorldIT = Matrix.Transpose(WorldIT);


                
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["WorldIT"].SetValue(WorldIT);
                    effect.Parameters["WorldViewProj"].SetValue(World * View * Projection);
                    effect.Parameters["ViewInv"].SetValue(Matrix.Invert(View));
                    effect.Parameters["World"].SetValue(World);

                    effect.Parameters["SkyTextureNight"].SetValue(night);
                    effect.Parameters["SkyTextureSunset"].SetValue(sunset);
                    effect.Parameters["SkyTextureDay"].SetValue(day);

                    effect.Parameters["isSkydome"].SetValue(true);

                    effect.Parameters["LightDirection"].SetValue(parameters.LightDirection);
                    effect.Parameters["LightColor"].SetValue(parameters.LightColor);
                    effect.Parameters["LightColorAmbient"].SetValue(parameters.LightColorAmbient);
                    effect.Parameters["FogColor"].SetValue(parameters.FogColor);
                    effect.Parameters["fDensity"].SetValue(parameters.FogDensity);
                    effect.Parameters["SunLightness"].SetValue(parameters.SunLightness);
                    effect.Parameters["sunRadiusAttenuation"].SetValue(parameters.SunRadiusAttenuation);
                    effect.Parameters["largeSunLightness"].SetValue(parameters.LargeSunLightness);
                    effect.Parameters["largeSunRadiusAttenuation"].SetValue(parameters.LargeSunRadiusAttenuation);
                    effect.Parameters["dayToSunsetSharpness"].SetValue(parameters.DayToSunsetSharpness);
                    effect.Parameters["hazeTopAltitude"].SetValue(parameters.HazeTopAltitude);
                   

                    mesh.Draw();
                }
                 
            }

            //game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            //game.GraphicsDevice.RenderState.DepthBufferEnable = true;
        }

        #endregion

        #region Private Methods

        #region Remap Model
        public static void RemapModel(Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                   part.Effect = effect;
                }
            }
        }
        #endregion

        #region Get Light Direction
        Vector4 GetDirection()
        {

            float y = (float)Math.Cos((double)this.fTheta);
            float x = (float)(Math.Sin((double)this.fTheta) * Math.Cos(this.fPhi));
            float z = (float)(Math.Sin((double)this.fTheta) * Math.Sin(this.fPhi));
            float w = 1.0f;

            return new Vector4(x, y, z, w);
        }
        #endregion

        #endregion
    }
}