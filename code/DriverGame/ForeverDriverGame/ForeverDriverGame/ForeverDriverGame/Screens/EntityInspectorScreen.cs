using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Forever.Interface;
using Adam.Screens;

namespace Forever.Demos
{
    public class EntityInspectorScreen : GameScreen
    {
        object Entity { get; set; }
        Texture2D gradientTexture;
        SpriteFont spriteFont;

        public EntityInspectorScreen(object entity)
        {
            Entity = entity;


            ScreenState = ScreenState.NoTransitions;
            PropagateDraw = true;
            

        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gradientTexture = content.Load<Texture2D>("gradient");
            spriteFont = content.Load<SpriteFont>("Arial9");
        }

        
        public override void Draw(GameTime gameTime)
        {
            List<string[]> table = ExtractEntityTableInfo(Entity);


            StringBuilder sBuild = new StringBuilder();
            if (table.Count == 0)
            {
                sBuild.Append("Instance of Type '" + Entity.GetType() + "' yeilds no inspector information.");

            }
            else
            {

                foreach (string[] pair in table)
                {
                    sBuild.AppendLine(pair[0] + ": " + pair[1]);
                }
            }


            string message = sBuild.ToString();

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = spriteFont;



            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = Vector2.Zero;//(viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            Color color = new Color(255, 255, 255);

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            // Draw the message box text.
            spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        protected List<string[]> ExtractEntityTableInfo(object entity)
        {
            return EntityInspectorAttribute.GetDisplayData(entity);
        }

    }
}
