using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Adam.Screens;

using Forever;
using Forever.Interface;
using Forever.Render;

namespace Forever.Demos.Components.Guns
{
    public class EntityInspectorScreenGun : CameraGun
    {
        ScreenManager ScreenManager { get; set; }
        EntityInspectorScreen Screen { get; set; }
        public EntityInspectorScreenGun(ScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }

        public override void Fire(FiringType firingType)
        {
            base.Fire(firingType);

            if (Screen != null)
            {
                Screen.ExitScreen();
            }
            if (Target != null)
            {
                Screen = new EntityInspectorScreen(Target);
                ScreenManager.AddScreen(Screen);
            }

        }

        
    }
}
