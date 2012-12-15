using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Forever.Geometry;

namespace Forever.Physics.Collide
{
    public interface IGeometryData
    {
        BoundingSphere BoundingSphere { get; set; }
        Primitive Prim { get; set; }
        

    }
}
