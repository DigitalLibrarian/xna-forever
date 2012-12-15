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

using Forever.Physics.Collide;

namespace Forever.Render
{
  public static class PrimitiveRenderer
  {


    static VertexPositionColor[] planeVerts = new VertexPositionColor[4];
    static int[] planeIndices = new int[]  
    {  
      0, 1, 2,
      2, 3, 0,
      2, 1, 0,
      0, 3, 2
    };


    static BasicEffect effect;

    static VertexDeclaration vertDecl;

    public static void Render(Primitive prim, RenderContext context, Color color)
    {
      if (prim.PrimType == Forever.Physics.Collide.CollideType.Sphere)
      {
        Sphere sphere = (Sphere)prim;
        BoundingSphere bs = new BoundingSphere(prim.Body.Position, sphere.Radius);
        BoundingSphereRenderer.Render(bs, context, color);
      }
      else if (prim.PrimType == Forever.Physics.Collide.CollideType.Box)
      {
        Box b = (Box)prim;
        Matrix world = prim.Body.World;
        BoundingBoxRenderer.Render(
          BoundingBox.CreateFromPoints(b.LocalVerts()),
          context.GraphicsDevice,
          world * b.OffsetMatrix,
          context.Camera.View,
          context.Camera.Projection,
          color
        );
      }else if(prim.PrimType == Forever.Physics.Collide.CollideType.Plane){

          Render((Forever.Physics.Collide.Plane)prim, context, color);
      }
      else
      {
        throw new Exception("I don't know how to draw that!");
      }
    }


    public static void Render(Sphere sphere, RenderContext context, Color color)
    {
      BoundingSphere bs = new BoundingSphere(sphere.Body.Position, sphere.Radius);
      BoundingSphereRenderer.Render(bs, context, color);
    }

    private static void InitializeGraphics(GraphicsDevice graphicsDevice)
    {

      effect = new BasicEffect(graphicsDevice);
      effect.VertexColorEnabled = true;
      effect.LightingEnabled = false;

      vertDecl = VertexPositionColor.VertexDeclaration;
    }

      

    public static void Render(Forever.Physics.Collide.Plane plane, RenderContext context, Color color)
    {
      if (effect == null)
      {
        InitializeGraphics(context.GraphicsDevice);
      }
      Matrix view = context.Camera.View;
      Matrix projection = context.Camera.Projection;
      GraphicsDevice graphicsDevice = context.GraphicsDevice;

      Vector3 w_point = context.Camera.Position;
      Vector3 point = plane.ClosestPoint(w_point);

      Vector3 away =  point - plane.PrinciplePoint;

      Vector3 a = Vector3.Cross(away,  plane.Normal);
      Vector3 b = Vector3.Cross(a, plane.Normal);
      if (a.Length() != 0)
      {
        a.Normalize();
      }

      if (b.Length() != 0)
      {
        b.Normalize();
      }

      float check = Vector3.Dot(plane.Normal, b);
      
      Vector3 planeOrigin = plane.PrinciplePoint;
      planeVerts[3].Position = planeOrigin + (a * -10000f) + (b * 10000f);
      planeVerts[2].Position = planeOrigin + (a * 10000f)  + (b * 10000f);
      planeVerts[1].Position = planeOrigin + (a * 10000f)  + (b * -10000f);
      planeVerts[0].Position = planeOrigin + (a * -10000f) + (b * -10000f);
      
      for (int i = 0; i < 4; i++)
      {
        planeVerts[i].Color = color;
      }

      //graphicsDevice.VertexDeclaration = vertDecl;

      effect.World = Matrix.Identity;
      effect.View = view;
      effect.Projection = projection;

      //effect.Begin();
      foreach (EffectPass pass in effect.CurrentTechnique.Passes)
      {
        pass.Apply();

        graphicsDevice.DrawUserIndexedPrimitives(
          Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList,
          planeVerts,
          0,
          4,
          planeIndices,
          0,
          4
         );

        //pass.End();
      }
      //effect.End();
    }


  }
}
