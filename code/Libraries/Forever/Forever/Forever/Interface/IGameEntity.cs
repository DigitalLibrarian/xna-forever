using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Forever.Render;
using Forever.Physics;

namespace Forever.Interface
{
  public interface IGameEntity : IPhysicsObject, IRenderable
  {

    Matrix World { get; }

    new void LoadContent(ContentManager content);
    void UnloadContent();
    void Update(GameTime gameTime);
  }

}
