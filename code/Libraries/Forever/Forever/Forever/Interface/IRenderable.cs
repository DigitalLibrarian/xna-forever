using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Forever.Render;

namespace Forever.Interface
{
  public interface IRenderable
  {
    void LoadContent(ContentManager content);
    void Render(RenderContext render_context, GameTime gameTime);
  }
}
