using System;  
using System.Collections.Generic;  
 
using System.Text;  
using Microsoft.Xna.Framework;  
using Microsoft.Xna.Framework.Audio;  
using Microsoft.Xna.Framework.Content;  
using Microsoft.Xna.Framework.GamerServices;  
using Microsoft.Xna.Framework.Graphics;  
using Microsoft.Xna.Framework.Input;  
using Microsoft.Xna.Framework.Net;  
using Microsoft.Xna.Framework.Storage;  
using System.Diagnostics;


namespace Forever.Render
{

  /// <summary>  
  /// Provides a set of methods for the rendering BoundingBoxs.  
  /// </summary>  
  public static class BoundingBoxRenderer
  {
    #region Fields

    static VertexPositionColor[] verts = new VertexPositionColor[8];
    static int[] wireframeIndices = new int[]  
        {  
            0, 1,  
            1, 2,  
            2, 3,  
            3, 0,  
            0, 4,  
            1, 5,  
            2, 6,  
            3, 7,  
            4, 5,  
            5, 6,  
            6, 7,  
            7, 4,  
        };

        static int[] solidIndices = new int[]  
        {  
          0, 1, 3,
          1, 2, 3,
          1, 5, 2, 
          5, 2, 6,
          4, 1, 0, 
          4, 5, 1, 
          4, 7, 6,
          4, 6, 5,
          0, 4, 3,
          4, 3, 7,
          7, 3, 2,
          6, 7, 2,

          
          3, 1, 0,
          3, 2, 1,
          2, 5, 1,
          6, 2, 5,
          0, 1, 4,
          1, 5, 4,
          6, 7, 4,
          5, 6, 4,
          3, 4, 0,
          7, 3, 4,
          2, 3, 7,
          2, 7, 6

        };

    static BasicEffect effect;
    static VertexDeclaration vertDecl;
    #endregion

    /// <summary>  
    /// Renders the bounding box for debugging purposes.  
    /// </summary>  
    /// <param name="box">The box to render.</param>  
    /// <param name="graphicsDevice">The graphics device to use when rendering.</param>  
    /// <param name="view">The current view matrix.</param>  
    /// <param name="projection">The current projection matrix.</param>  
    /// <param name="color">The color to use drawing the lines of the box.</param>  
    public static void Render(
        BoundingBox box,
        GraphicsDevice graphicsDevice,
        Matrix world,
        Matrix view,
        Matrix projection,
        Color color)
    {
      if (effect == null)
      //if (true)  
      {
        effect = new BasicEffect(graphicsDevice);
        effect.VertexColorEnabled = true;
        effect.LightingEnabled = false;

        //effect.EmissiveColor = true; //raj  
        vertDecl = VertexPositionColor.VertexDeclaration;
      }

      Vector3[] corners = box.GetCorners();
      for (int i = 0; i < 8; i++)
      {
        verts[i].Position = corners[i];
        verts[i].Color = color;
      }

     // graphicsDevice.VertexDeclaration = vertDecl;

      effect.World = world;
      effect.View = view;
      effect.Projection = projection;

      //effect.Begin();
      foreach (EffectPass pass in effect.CurrentTechnique.Passes)
      {
        pass.Apply();

        graphicsDevice.DrawUserIndexedPrimitives(
          PrimitiveType.LineList,
           verts,
            0,
            8,
            wireframeIndices,
            0,
            wireframeIndices.Length / 2);

        //pass.End();
      }
      //effect.End();
    }

  
    /// <summary>  
    /// Renders the bounding box for debugging purposes.  
    /// </summary>  
    /// <param name="box">The box to render.</param>  
    /// <param name="graphicsDevice">The graphics device to use when rendering.</param>  
    /// <param name="view">The current view matrix.</param>  
    /// <param name="projection">The current projection matrix.</param>  
    /// <param name="color">The color to use drawing the lines of the box.</param>  
    public static void RenderSolid(
        BoundingBox box,
        GraphicsDevice graphicsDevice,
        Matrix world,
        Matrix view,
        Matrix projection,
        Color color)
    {
      if (effect == null)
      //if (true)  
      {
        effect = new BasicEffect(graphicsDevice);//, null);
        effect.VertexColorEnabled = true;
        effect.LightingEnabled = false;

        vertDecl = VertexPositionColor.VertexDeclaration;
      }

      Vector3[] corners = box.GetCorners();
      for (int i = 0; i < 8; i++)
      {
        verts[i].Position = corners[i];
        verts[i].Color = color;
      }

      //graphicsDevice.VertexDeclaration = vertDecl;

      effect.World = world;
      effect.View = view;
      effect.Projection = projection;

      foreach (EffectPass pass in effect.CurrentTechnique.Passes)
      {
        pass.Apply();

        graphicsDevice.DrawUserIndexedPrimitives(
          PrimitiveType.TriangleList,
           verts,
            0,
            8,
            solidIndices,
            0,
            24);

      }
      
    }
  }
}
 
     