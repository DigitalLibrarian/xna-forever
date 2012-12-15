using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Forever.Interface;


namespace Forever.Physics.ForceGen
{
    public class TowardsBodyFG :  IForceGenerator
    {
        public IRigidBody BodyToFollow { get; set; }
        public float ForceMag { get; set; }
        public TowardsBodyFG(IRigidBody body, float forceMag)
        {
            BodyToFollow = body;
            ForceMag = forceMag;
        }

        public void updateForce(IPhysicsObject forceTarget, GameTime gameTime)
        {
            Vector3 dir = BodyToFollow.Position - forceTarget.CenterOfMass;

            dir.Normalize();

            dir *= ForceMag;
            forceTarget.addForce(dir);
        }
    }
}
