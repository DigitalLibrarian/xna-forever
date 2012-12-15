using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Forever.Geometry;

namespace Forever.Render
{
  public class AABBTreeRenderer
  {
    public static void Render(AABBTree tree, Matrix world, Matrix view, Matrix proj, RenderContext render_context, Color color)
    {
      RenderNode(tree.Root, world, view, proj, render_context, color);
    }
    public static void RenderNode(AABBNode node, Matrix world, Matrix view, Matrix proj, RenderContext render_context, Color leaf_color)
    {
      RenderNode(node, world, view, proj, render_context, leaf_color, leaf_color);
    }
    public static void RenderNode(AABBNode node,  Matrix world, Matrix view, Matrix proj, RenderContext render_context, Color leaf_color, Color marked_color){

      if (node.IsLeaf)
      {
        Color color = leaf_color;
        if (node.Marked)
        {
          color = marked_color;
        }
        BoundingBoxRenderer.Render(node.BBox, render_context.GraphicsDevice, world, view, proj, color);

      }else{
        RenderNode(node.Left, world, view, proj, render_context, leaf_color, marked_color);
        RenderNode(node.Right, world, view, proj, render_context, leaf_color, marked_color);
      }
    }
  }
}
