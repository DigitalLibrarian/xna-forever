using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Forever.Interface;

namespace Forever.Physics.ForceGen
{
    public class WhenAwakeFG : IForceGenerator
    {
        public Vector3 Force { get; set; }
        public WhenAwakeFG(Vector3 f)
        {
            Force = f;
        }

        public void updateForce(IPhysicsObject forceTarget, GameTime gameTime)
        {
            // Allow target to go to sleep
            if (forceTarget.Awake)
            {
                Vector3 appliedForce = Force * forceTarget.Mass;
                Forever.Interface.ICollideable bodyHaver = forceTarget as ICollideable;
                Forever.Physics.Collide.Primitive prim = bodyHaver.GeometryData.Prim;
                if (prim is Forever.Physics.Collide.Box)
                {
                    Forever.Physics.Collide.Box box = prim as Forever.Physics.Collide.Box;
                    Vector3 totalForce = appliedForce;
                    float oldMag = totalForce.Length();
                    totalForce.Normalize();
                    float newMag = (oldMag/8f);
                    totalForce *= newMag;

                    foreach (Vector3 corner in box.WorldVerts())
                    {
                        forceTarget.addForce(totalForce, corner);
                    }

                }
                else
                {
                    forceTarget.addForce(appliedForce);
                }
            }
        }
    }
}
