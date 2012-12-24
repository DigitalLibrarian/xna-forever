using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Forever.Demos.Components.Guns
{
    public class ForceGun : CameraGun
    {
        public Vector3 Force { get; set; }
        public ForceGun(Vector3 force)
        {
            Force = force;
        }
        public override void Fire(FiringType firingType)
        {
            if (Target != null)
            {
                Vector3 appliedForce = Force;
                float dist = (float)this.DistanceToIntersect(Ray, Target);
                Vector3 point = Ray.Position + (Ray.Direction * dist);

                if (firingType == FiringType.Secondary)
                {
                    appliedForce *= -1f;
                }

                Target.GeometryData.Prim.Body.addForce(appliedForce, point);

            }
        }
    }
}
