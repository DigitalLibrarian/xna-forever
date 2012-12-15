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
            if (forceTarget.Awake)
            {
                forceTarget.addForce(Force);
            }
        }
    }
}
