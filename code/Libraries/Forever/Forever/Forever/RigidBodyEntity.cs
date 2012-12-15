using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


using Forever.Interface;
using Forever.Geometry;
using Forever.Render;
using Forever.Physics;
using Forever.Physics.Collide;

namespace Forever
{
  public class RigidBodyEntity : IGameEntity, IRenderable, IPhysicsObject,  IHasRigidBody, iHasPrimitive
  {
    Model model;
    AABBTree tree;
    RigidBody body;
    Primitive prim;

    protected Matrix world;
    public Model Model
    {
      get { return model; }
      set { model = value; }
    }

    public Matrix World { get { return world; } }
    public AABBTree AABB { get { return tree; } set { tree = value; } }

    public IRigidBody Body { get { return body; } }

    public Vector3 CenterOfMass { get { return body.Position; } }


    public Primitive Prim { get { return prim; } set { prim = value; } }

    DebugCompass debug_compass;
    bool debug = false;
    public bool Debug { get { return debug; } set { debug = value; } }

    public bool Awake { get { return body.Awake; } }

    public RigidBodyEntity(Vector3 pos)
    {
      body = new RigidBody(pos);

      debug_compass = new DebugCompass(this.CenterOfMass);

      debug_compass.LoadContent(null);
    }
    
    public void LoadContent(ContentManager content)
    {

    }

    public void UnloadContent()
    {
      prim = null;
    }


    public void Render(RenderContext render_context, GameTime gameTime)
    {
      Renderer.RenderModel(model, world, render_context);
      

      if (debug)
      {
        debug_compass.Render(render_context, gameTime);
        if (tree != null)
        {
            AABBTreeRenderer.RenderNode(
                tree.Root,
                world,
                render_context.Camera.View,
                render_context.Camera.Projection,
                render_context,
                Color.Red,
                Color.SkyBlue
            );
        }
      }
    }

    public void Update(GameTime gameTime)
    {
      world = this.body.World;
      debug_compass.Position = this.CenterOfMass;
    }

    public void addForce(Vector3 force)
    {
      this.body.addForce(force);
    }

    public void addForce(Vector3 force, Vector3 point)
    {
      this.body.addForce(force, point);
    }
    public void integrate(float duration)
    {
      this.body.integrate(duration);
    }
    public void Translate(Vector3 translation)
    {
      body.Translate(translation);
    }

  }

  
}
