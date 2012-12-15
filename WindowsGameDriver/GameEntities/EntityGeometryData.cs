using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Forever.Physics.Collide;



namespace Forever.GameEntities
{
    public class EntityGeometryData : IGeometryData
    {

        public BoundingSphere BoundingSphere { get; set; }
        public Primitive Prim { get; set; }

    }


}
