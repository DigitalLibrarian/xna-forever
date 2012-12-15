using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Forever.Physics;

namespace Forever.Interface
{


  public interface IRigidBody : IPhysicsObject
  {
      Matrix InertiaTensor { get; set; }
      Matrix InertiaTensorWorld { get; }
      Matrix InverseInertiaTensor { get; set; }
      Matrix InverseInertiaTensorWorld { get; }
      Matrix World { get; }

      [EntityInspector("Orient(quat)")]
      Quaternion Orientation { get; set; }

      //Matrix OrientationMatrix { get; set; }

      [EntityInspector("Angular Momentum")]
      Vector3 AngularMomentum { get; }

      [EntityInspector("Linear Momentum")]
      Vector3 LinearMomentum { get; }


      [EntityInspector("Mass")]
      float Mass { get; set; }
      float InverseMass { get; set; }
      bool HasFiniteMass { get; }


      [EntityInspector("LineDamp")]
      float LinearDamping { get; set; }

      [EntityInspector("AngDamp")]
      float AngularDamping { get; set; }

      [EntityInspector("Rot")]
      Vector3 Rotation { get; }

      [EntityInspector("Pos.")]
      Vector3 Position { get; set; }
      [EntityInspector("Velo.")]
      Vector3 Velocity { get;  }
      [EntityInspector("Accel.")]
      Vector3 Acceler { get; set; }

      Vector3 LastAccel { get; }

      [EntityInspector("Awake")]
      bool Awake { get; set; }

      bool CanSleep { get; set; }

      void addTorque(Vector3 torque);
      new void addForce(Vector3 force, Vector3 point);
      void clearAccumulators();
      new void integrate(float duration);

      void addVelocity(Vector3 velo);
      void addRotation(Vector3 rot);

      Vector3 Up { get; }
      Vector3 Right { get; }
      Vector3 Forward { get; }
  };
}
