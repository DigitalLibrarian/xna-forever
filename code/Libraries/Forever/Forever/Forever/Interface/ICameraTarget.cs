using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

// TODO - Make a class that can control and iCamera into a third-person like camera to follow one of these
namespace Forever.Interface
{
  public interface ICameraTarget
  {
    Vector3 Position { get; }
    Matrix Rotation { get; }

    BoundingSphere BoundingSphere { get; }
  }
}
