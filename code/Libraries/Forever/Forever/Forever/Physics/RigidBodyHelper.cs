using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Forever.Physics.MadMatrix;

//List of inertia tensors: http://en.wikipedia.org/wiki/List_of_moment_of_inertia_tensors


namespace Forever.Physics
{


  public class RigidBodyHelper
  {
    public static Quaternion AddScaledVector(Quaternion quat, Vector3 vector, float scale)
    {
      Quaternion q = new Quaternion(
        vector.X * scale,
        vector.Y * scale,
        vector.Z * scale, 
        0f
      );
      q *= quat;

      float w = quat.W;
      float i = quat.X;
      float j = quat.Y;
      float k = quat.Z;
      
      w += q.W * (0.5f);
      i += q.X * (0.5f);
      j += q.Y * (0.5f);
      k += q.Z * (0.5f);
      
      return new Quaternion(i, j, k, w);
    }

    public static void TransformInertiaTensor(ref Matrix iitWorld4x4,
                                       ref Quaternion q,
                                       ref Matrix iitBody4x4,
                                       ref Matrix rotmat)
    {

        //iitWorld4x4 = iitWorld4x4 * rotmat;

        
      Matrix3 iitWorld = MatrixtoMatrix3(iitWorld4x4);
      Matrix3 iitBody = MatrixtoMatrix3(iitBody4x4);
      Matrix4 rotmat4x3 = MatrixtoMatrix4(rotmat);

      _transformInertiaTensor(ref iitWorld, ref q, ref iitBody, ref rotmat4x3);

      iitWorld4x4 = Matrix3toMatrix(iitWorld);
      Debug.Sanity(iitWorld4x4);

    }


    #region Ugliness

    protected static void _transformInertiaTensor(ref Matrix3 inverseInertiaTensorWorld,
                                       ref Quaternion quat,
                                       ref Matrix3 inverseInertiaTensorBody,
                                       ref Matrix rotmat)
    {

      float t4 = rotmat.M11 * inverseInertiaTensorBody.data[0] +
          rotmat.M12 * inverseInertiaTensorBody.data[3] +
          rotmat.M13 * inverseInertiaTensorBody.data[6];
      float t9 = rotmat.M11 * inverseInertiaTensorBody.data[1] +
          rotmat.M12 * inverseInertiaTensorBody.data[4] +
          rotmat.M13 * inverseInertiaTensorBody.data[7];
      float t14 = rotmat.M11 * inverseInertiaTensorBody.data[2] +
          rotmat.M12 * inverseInertiaTensorBody.data[5] +
          rotmat.M13 * inverseInertiaTensorBody.data[8];
      float t28 = rotmat.M21 * inverseInertiaTensorBody.data[0] +
          rotmat.M22 * inverseInertiaTensorBody.data[3] +
          rotmat.M23 * inverseInertiaTensorBody.data[6];
      float t33 = rotmat.M21 * inverseInertiaTensorBody.data[1] +
          rotmat.M22 * inverseInertiaTensorBody.data[4] +
          rotmat.M23 * inverseInertiaTensorBody.data[7];
      float t38 = rotmat.M21 * inverseInertiaTensorBody.data[2] +
          rotmat.M22 * inverseInertiaTensorBody.data[5] +
          rotmat.M23 * inverseInertiaTensorBody.data[8];
      float t52 = rotmat.M31 * inverseInertiaTensorBody.data[0] +
          rotmat.M32 * inverseInertiaTensorBody.data[3] +
          rotmat.M33 * inverseInertiaTensorBody.data[6];
      float t57 = rotmat.M31 * inverseInertiaTensorBody.data[1] +
          rotmat.M32 * inverseInertiaTensorBody.data[4] +
          rotmat.M33 * inverseInertiaTensorBody.data[7];
      float t62 = rotmat.M31 * inverseInertiaTensorBody.data[2] +
          rotmat.M32 * inverseInertiaTensorBody.data[5] +
          rotmat.M33 * inverseInertiaTensorBody.data[8];

      inverseInertiaTensorWorld.data[0] = t4 * rotmat.M11 +
          t9 * rotmat.M12 +
          t14 * rotmat.M13;
      inverseInertiaTensorWorld.data[1] = t4 * rotmat.M21 +
          t9 * rotmat.M22 +
          t14 * rotmat.M23;
      inverseInertiaTensorWorld.data[2] = t4 * rotmat.M31 +
          t9 * rotmat.M32 +
          t14 * rotmat.M33;
      inverseInertiaTensorWorld.data[3] = t28 * rotmat.M11 +
          t33 * rotmat.M12 +
          t38 * rotmat.M13;
      inverseInertiaTensorWorld.data[4] = t28 * rotmat.M21 +
          t33 * rotmat.M22 +
          t38 * rotmat.M23;
      inverseInertiaTensorWorld.data[5] = t28 * rotmat.M31 +
          t33 * rotmat.M32 +
          t38 * rotmat.M33;
      inverseInertiaTensorWorld.data[6] = t52 * rotmat.M11 +
          t57 * rotmat.M12 +
          t62 * rotmat.M13;
      inverseInertiaTensorWorld.data[7] = t52 * rotmat.M21 +
          t57 * rotmat.M22 +
          t62 * rotmat.M23;
      inverseInertiaTensorWorld.data[8] = t52 * rotmat.M31 +
          t57 * rotmat.M32 +
          t62 * rotmat.M33;
    }


    private static void _transformInertiaTensor(ref Matrix3 iitWorld,
                                       ref Quaternion q,
                                       ref Matrix3 iitBody,
                                       ref Matrix4 rotmat)
    {
      float t4 = rotmat.data[0] * iitBody.data[0] +
          rotmat.data[1] * iitBody.data[3] +
          rotmat.data[2] * iitBody.data[6];
      float t9 = rotmat.data[0] * iitBody.data[1] +
          rotmat.data[1] * iitBody.data[4] +
          rotmat.data[2] * iitBody.data[7];
      float t14 = rotmat.data[0] * iitBody.data[2] +
          rotmat.data[1] * iitBody.data[5] +
          rotmat.data[2] * iitBody.data[8];
      float t28 = rotmat.data[4] * iitBody.data[0] +
          rotmat.data[5] * iitBody.data[3] +
          rotmat.data[6] * iitBody.data[6];
      float t33 = rotmat.data[4] * iitBody.data[1] +
          rotmat.data[5] * iitBody.data[4] +
          rotmat.data[6] * iitBody.data[7];
      float t38 = rotmat.data[4] * iitBody.data[2] +
          rotmat.data[5] * iitBody.data[5] +
          rotmat.data[6] * iitBody.data[8];
      float t52 = rotmat.data[8] * iitBody.data[0] +
          rotmat.data[9] * iitBody.data[3] +
          rotmat.data[10] * iitBody.data[6];
      float t57 = rotmat.data[8] * iitBody.data[1] +
          rotmat.data[9] * iitBody.data[4] +
          rotmat.data[10] * iitBody.data[7];
      float t62 = rotmat.data[8] * iitBody.data[2] +
          rotmat.data[9] * iitBody.data[5] +
          rotmat.data[10] * iitBody.data[8];

      iitWorld.data[0] = t4 * rotmat.data[0] +
          t9 * rotmat.data[1] +
          t14 * rotmat.data[2];
      iitWorld.data[1] = t4 * rotmat.data[4] +
          t9 * rotmat.data[5] +
          t14 * rotmat.data[6];
      iitWorld.data[2] = t4 * rotmat.data[8] +
          t9 * rotmat.data[9] +
          t14 * rotmat.data[10];
      iitWorld.data[3] = t28 * rotmat.data[0] +
          t33 * rotmat.data[1] +
          t38 * rotmat.data[2];
      iitWorld.data[4] = t28 * rotmat.data[4] +
          t33 * rotmat.data[5] +
          t38 * rotmat.data[6];
      iitWorld.data[5] = t28 * rotmat.data[8] +
          t33 * rotmat.data[9] +
          t38 * rotmat.data[10];
      iitWorld.data[6] = t52 * rotmat.data[0] +
          t57 * rotmat.data[1] +
          t62 * rotmat.data[2];
      iitWorld.data[7] = t52 * rotmat.data[4] +
          t57 * rotmat.data[5] +
          t62 * rotmat.data[6];
      iitWorld.data[8] = t52 * rotmat.data[8] +
          t57 * rotmat.data[9] +
          t62 * rotmat.data[10];
    }

    #endregion

    public static Matrix Matrix3toMatrix(Matrix3 m)
    {
      Matrix mat = new Matrix();
      
      mat.M11 = m.data[0];
      mat.M12 = m.data[1];
      mat.M13 = m.data[2];
      mat.M14 = 0;

      mat.M21 = m.data[3];
      mat.M22 = m.data[4];
      mat.M23 = m.data[5];
      mat.M24 = 0;

      mat.M31 = m.data[6];
      mat.M32 = m.data[7];
      mat.M33 = m.data[8];
      mat.M34 = 0;


      mat.M41 = 0;
      mat.M42 = 0;
      mat.M43 = 0;
      mat.M44 = 1;
      return mat;
    }



    public static Matrix3 MatrixtoMatrix3(Matrix mat)
    {
      Matrix3 m = new Matrix3();

      m.data[0]= mat.M11;
      m.data[1]= mat.M12;
      m.data[2]= mat.M13;

      m.data[3] = mat.M21;
      m.data[4] = mat.M22;
      m.data[5] = mat.M23;

      m.data[6] = mat.M31;
      m.data[7] = mat.M32;
      m.data[8] = mat.M33;

      return m;
    }

    public static Matrix Matrix4toMatrix(Matrix4 m)
    {
      Matrix mat = new Matrix();

      mat.M11 = m.data[0];
      mat.M12 = m.data[1];
      mat.M13 = m.data[2];
      mat.M14 = m.data[3];

      mat.M21 = m.data[4];
      mat.M22 = m.data[5];
      mat.M23 = m.data[6];
      mat.M24 = m.data[7];

      mat.M31 = m.data[8];
      mat.M32 = m.data[9];
      mat.M33 = m.data[10];
      mat.M34 = m.data[11];


      mat.M41 = 0;
      mat.M42 = 0;
      mat.M43 = 0;
      mat.M44 = 1;



      //mat.M11 = m.data[0];
      //mat.M12 = m.data[1];
      //mat.M13 = m.data[2];
      //mat.M14 = 0;

      //mat.M21 = m.data[3];
      //mat.M22 = m.data[4];
      //mat.M23 = m.data[5];
      //mat.M24 = 0;

      //mat.M31 = m.data[6];
      //mat.M32 = m.data[7];
      //mat.M33 = m.data[8];
      //mat.M34 = 0;


      //mat.M41 = 0;
      //mat.M42 = 0;
      //mat.M43 = 0;
      //mat.M44 = 1;

      return mat;
    }

    public static Matrix4 MatrixtoMatrix4(Matrix mat)
    {
      Matrix4 m = new Matrix4();
      m.data[0] = mat.M11;
      m.data[1] = mat.M12;
      m.data[2] = mat.M13;
      m.data[3] = mat.M14;

      m.data[4] = mat.M21;
      m.data[5] = mat.M22;
      m.data[6] = mat.M23;
      m.data[7] = mat.M24;

      m.data[8] = mat.M31;
      m.data[9] = mat.M32;
      m.data[10] = mat.M33;
      m.data[11] = mat.M34;
      return m;
    }

    public static Matrix FromITCoefficients(float ix, float iy, float iz,
            float ixy, float ixz, float iyz)
    {

      Matrix3 m = new Matrix3();
      m.data[0] = ix;
      m.data[1] = m.data[3] = -ixy;
      m.data[2] = m.data[6] = -ixz;
      m.data[4] = iy;
      m.data[5] = m.data[7] = -iyz;
      m.data[8] = iz;

      return Matrix3toMatrix(m);
    }

    public static Matrix GetBlockInertiaTensor(Vector3 halfSizes, float mass)
    {
      Vector3 squares = Vector3.Multiply(halfSizes, halfSizes);
      return FromITCoefficients(
          0.3f*mass*(squares.Y + squares.Z),
          0.3f*mass*(squares.X + squares.Z),
          0.3f*mass*(squares.X + squares.Y), 
          0f, 0f, 0f);
    }


    public static Matrix GetSphereInertiaTensor(float radius, float mass)
    {

      return FromITCoefficients(
          (2f / 5f) * mass * (radius * radius),
          (2f / 5f) * mass * (radius * radius),
          (2f / 5f) * mass * (radius * radius),
          0f, 0f, 0f);
    }
  }
}
