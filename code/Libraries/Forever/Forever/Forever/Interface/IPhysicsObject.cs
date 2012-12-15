using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Forever.Interface
{
    public interface IPhysicsObject
    {
        bool Awake { get; }
        void addForce(Vector3 force);
        void addForce(Vector3 force, Vector3 point);
        void integrate(float duration);

        [EntityInspector("CoM")]
        Vector3 CenterOfMass { get; }

        void Translate(Vector3 translation);
    }
}
