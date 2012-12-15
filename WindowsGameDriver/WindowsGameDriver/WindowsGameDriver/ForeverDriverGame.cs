using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

// http://www.protohacks.net/xna_debug_terminal/HowTo.php5
using DebugTerminal;
using Adam.Screens;
using Forever.Interface;
using Forever.Demos;

using Forever.GameEntities;
namespace Forever.Demos
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ForeverDriverGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ScreenManager screenManager;
        ScreenManager ScreenManager { get { return screenManager; } }

        #region Debugging Hot Coding Context
        /* These things allow the debugging console to have some stuff handy */

        // Make a special property Top.  When assigned, it will clear the screenmanager stack and load new value.
        GameScreen topDemo;
        GameScreen Top {
            get { return topDemo; }
            set {
                GameScreen nextDemo = value;
                if (topDemo != null)
                {
                    ScreenManager.RemoveScreen(topDemo);
                }

                topDemo = nextDemo;
                if (topDemo != null)
                {
                    ScreenManager.AddScreen(topDemo);
                }
            } 
        }
        GameScreen top { get { return Top; } set { Top = value; } }

        // Some assignable names for arbitrary assignnment
        [EntityInspector("X:")]
        string X { get; set; }
        
        [EntityInspector("Bag")]
        object[] Bag;

        protected Vector3 RandVector(float magnitude)
        {
            Random rand = new Random();
            Vector3 v = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
            v -= new Vector3(1f, 1f, 1f);
            v.Normalize();
            v *= magnitude;
            return v;
        }


        //meaningful way to load the BallDemo with arbitrary model filename
        void load(string assetFile)
        {
            Top = new BallDemo(assetFile);
        }

        void inspect(object entity)
        {
            closeInspector();
            EntityInspectorScreen eiScreen = new EntityInspectorScreen(entity);
            ScreenManager.AddScreen(eiScreen);
            
        }

        void closeInspector()
        {
            EntityInspectorScreen eiScreen = null;
            GameScreen[] screens = ScreenManager.GetScreens();
            foreach (GameScreen screen in screens)
            {
                if (screen is EntityInspectorScreen)
                {
                    eiScreen = screen as EntityInspectorScreen;
                }
            }

            if (eiScreen != null)
            {
                ScreenManager.RemoveScreen(eiScreen);
            }
        }

        // Camera manipulation
        bool ResetCam()
        {
            if (Top is Demo)
            {
                Tele();
                Demo demo = Top as Demo;
                demo.CamBody.Orientation = Quaternion.Identity;
                return true;
            }   
            return false;
        }
        bool Tele() { return Tele(Zero); }
        bool Tele(Vector3 position) { return Teleport(position); }
        bool Teleport(Vector3 position)
        {
            if (Top is Demo)
            {
                Demo demo = Top as Demo;
                Teleport(demo.CamBody, position);
                return true;
            }

            return false;
        }
        void Tele(IRigidBody body, Vector3 position) { Teleport(body, position); }
        void Teleport(IRigidBody body,Vector3 position)
        {
            body.Translate(position - body.Position);
        }

        // Some Vector shortcuts
        public Vector3 Zero { get { return Vector3.Zero; } }
        public Vector3 UnitX { get { return Vector3.UnitX; } }
        public Vector3 UnitY { get { return Vector3.UnitY; } }
        public Vector3 UnitZ { get { return Vector3.UnitZ; } }


        private Vector3 SpawnLocation { get; set; }
        private ModelEntity LastSpawn { get; set; }
        bool Spawn(string assetName)
        {
            Demo demo = Top as Demo;
            return Spawn(assetName, 1, demo.Camera.Position, demo.Camera.Forward);
        }
        bool Spawn(int numSpawn)
        {
            return Spawn("basketball", numSpawn);
        }
        bool Spawn(string assetName, int numSpawn)
        {
            Demo demo = Top as Demo;
            return Spawn(assetName, numSpawn, demo.Camera.Position, demo.Camera.Forward * 10f);
        }

        protected virtual bool Spawn(string assetName, int numSpawn, Vector3 position, Vector3 ray)
        {
            Demo demo = Top as Demo;
            List<ModelEntity> spawnList =  demo.Spawn(assetName, numSpawn, position, ray);
            LastSpawn = spawnList.Last();


            return true;

            
        }

        

        #endregion



        #region Content and Setup

        public ForeverDriverGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            screenManager = new ScreenManager(this);
            this.IsMouseVisible = true;
           
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            ScreenManager.Initialize();

          
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
                        
            SpriteFont spriteFont = Content.Load<SpriteFont>("TerminalFont");
            TerminalSkin skin = new TerminalSkin(TerminalThemeType.HALLOWEEN_TWO);

            Terminal.Init(this, spriteBatch, spriteFont, GraphicsDevice);
            Terminal.SetSkin(skin);
            
            Components.Add(ScreenManager);

            //Watch out "trickey" assignment
            Top = CollideDemo.BoxOnBox();
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all conten
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (!Terminal.CheckOpen(Keys.OemTilde, Keyboard.GetState(PlayerIndex.One)))
            {
                base.Update(gameTime);
            }

        }

        #region Renderin'

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Set3DRenderStates();
            base.Draw(gameTime);

            //Sprites drawn as overlay
            Set2DRenderStates();
            Terminal.CheckDraw(true);
        }

        void Set2DRenderStates()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }
        void Set3DRenderStates()
        {
            // Set suitable renderstates for drawing a 3D model
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }

        #endregion
    }
}
