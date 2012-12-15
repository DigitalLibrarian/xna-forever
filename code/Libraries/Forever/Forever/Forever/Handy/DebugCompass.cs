using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Forever.Render;

namespace Forever.Interface
{
  public class DebugCompass : IRenderable
  {
    Matrix world;
    public Matrix World { get { return world; } }

    VertexPositionColor[] vertices;

    VertexBuffer vertex_buffer = null;

    Vector3 origin;

    public Vector3 Position { get { return origin; } set { origin = value; } }

    public DebugCompass(Vector3 origin_position)
    {
      origin = origin_position;
    }

    public void LoadContent(ContentManager content)
    {
      Color x_color = Color.Blue;
      Color y_color = Color.Green;
      Color z_color = Color.Red;

      float start = -1f;
      float end = 1f;
      float cell_size = 5000f;

      int total_verts =  (6);
      vertices = new VertexPositionColor[total_verts];

      int next_vert = 0;

      Vector3 x_line_start = new Vector3(start, 0f, 0f) * cell_size;
      Vector3 x_line_end = new Vector3(end, 0f, 0f) * cell_size;

      vertices[next_vert++] = new VertexPositionColor(x_line_start, Color.White);
      vertices[next_vert++] = new VertexPositionColor(x_line_end, x_color);


      Vector3 y_line_start = new Vector3(0f, start, 0f) * cell_size;
      Vector3 y_line_end = new Vector3(0f, end, 0f) * cell_size;

      vertices[next_vert++] = new VertexPositionColor(y_line_start, Color.White);
      vertices[next_vert++] = new VertexPositionColor(y_line_end, y_color);


      Vector3 z_line_start = new Vector3(0f, 0f, start) * cell_size;
      Vector3 z_line_end = new Vector3(0f, 0f, end) * cell_size;

      vertices[next_vert++] = new VertexPositionColor(z_line_start, Color.White);
      vertices[next_vert++] = new VertexPositionColor(z_line_end, z_color);
      
    }
    public void Render(RenderContext render_context, GameTime gameTime)
    {
      /// since we are only using one vertex declaration, we can just set the
      /// GraphicsDevice.VertexDeclaration once in the Initialize() method call.

      GraphicsDevice gd = render_context.GraphicsDevice;
      BasicEffect effect = render_context.BasicEffect;

      VertexDeclaration vertexDeclaration = VertexPositionColor.VertexDeclaration;
      //gd.VertexDeclaration = vertexDeclaration;


      effect.World = Matrix.CreateTranslation(origin);
      effect.View = render_context.Camera.View;
      effect.Projection = render_context.Camera.Projection;
      effect.VertexColorEnabled = true;

      if (vertex_buffer == null)
      {
        vertex_buffer = new VertexBuffer(gd, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        vertex_buffer.SetData<VertexPositionColor>(vertices);
      }



      foreach (EffectPass pass in effect.CurrentTechnique.Passes)
      {
          pass.Apply();
        gd.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, vertices.Length / 2);
      }

    }
  }
}
