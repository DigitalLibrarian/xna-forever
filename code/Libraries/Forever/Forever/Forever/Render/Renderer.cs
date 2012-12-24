using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Forever.Render
{
  public class Renderer
  {

    public static void RenderModel(Model model, 
      Matrix world, RenderContext renderContext)
    {

      Renderer.RenderModel(model, 
        world, renderContext.Camera.View, renderContext.Camera.Projection);
    }

    public static void RenderModel(Model model, 
      Matrix world, Matrix view, Matrix proj)
    {
      Matrix[] transforms = new Matrix[model.Bones.Count];
      model.CopyAbsoluteBoneTransformsTo(transforms);

      foreach (ModelMesh mesh in model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          effect.EnableDefaultLighting();
          
          effect.View = view;
          effect.Projection = proj;
          effect.World = transforms[mesh.ParentBone.Index] * world;
        }
        mesh.Draw();
        
      }
    }


    public static void RenderVertexPositionColorList(RenderContext render_context, 
      Matrix world,
      VertexPositionColor[] vertices, VertexDeclaration vertexDeclaration, 
      VertexBuffer vertex_buffer  )
    {
      GraphicsDevice gd = render_context.GraphicsDevice;
      BasicEffect effect = render_context.BasicEffect;
      Matrix view = render_context.Camera.View;
      Matrix proj = render_context.Camera.Projection;
      /* Would the real RenderVertexPositionColorList please stand up? */
      RenderVertexPositionColorList(gd, effect, world, view, proj, vertices, vertexDeclaration, vertex_buffer);
    }

    public static void RenderVertexPositionColorList(GraphicsDevice gd, 
      BasicEffect effect, Matrix world, Matrix view, Matrix proj, 
      VertexPositionColor[] vertices, VertexDeclaration vertexDeclaration, 
      VertexBuffer vertex_buffer  )
    {

     // gd.VertexDeclaration = vertexDeclaration;
       

      effect.World = world;
      effect.View = view;
      effect.Projection = proj;
      effect.VertexColorEnabled = true;

      if (vertex_buffer == null)
      {
        vertex_buffer = new VertexBuffer(gd, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        vertex_buffer.SetData<VertexPositionColor>(vertices);
      }

      foreach (EffectPass pass in effect.CurrentTechnique.Passes)
      {
          pass.Apply();
        gd.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, vertices.Length / 3);

      }

    }

  }
}
