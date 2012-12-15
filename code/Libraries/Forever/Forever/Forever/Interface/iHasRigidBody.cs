using System;
using System.Collections.Generic;
using System.Text;

using Forever.Physics;

namespace Forever.Interface
{
  public interface IHasRigidBody
  {
    IRigidBody Body { get; } 
  }
}
