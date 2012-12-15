using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Forever.Physics;
using Forever.Physics.Collide;

namespace Forever.Interface
{
    public interface ICollideable 
    {

        IGeometryData GeometryData { get; }
    }
}
