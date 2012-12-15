﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Forever.Physics;
using Forever.Physics.MadMatrix;

namespace Forever
{
    public  static class TrickyMathHelper
    {

        public static Vector3 Project(Vector3 Source, Vector3 Target)
        {
            float dotProduct = 0.0f;

            Vector3.Dot(ref Source, ref Target, out dotProduct);
            return (dotProduct / Target.LengthSquared()) * Target;
        }


        private static float Randfloat(Random rand)
        {
            return -5000f + ((float)rand.NextDouble() * 10000f);
        }


        public static Vector3 RandVector(Random rand)
        {
            return new Vector3(
                Randfloat(rand),
                Randfloat(rand),
                Randfloat(rand)
                );
        }


        public static Vector3 RandVector(Random rand, float magnitude)
        {
            Vector3 v = new Vector3(
                Randfloat(rand),
                Randfloat(rand),
                Randfloat(rand)
                );

            v.Normalize();
            v *= magnitude;
            return v;
        }

        public static float Min(float f1, float f2)
        {
            return (float) Math.Min( (float) f1, (float) f2);
        }

        public static float Max(float f1, float f2)
        {
            return (float)Math.Max((float)f1, (float)f2);
        }
        public static float Abs(float f)
        {
            return (float)Math.Abs(f);
        }


        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt(f);
        }

        public static float ScalarProduct(Vector3 v1, Vector3 v2)
        {
            return Vector3.Dot(v1, v2);
        }

        public static Vector3 VectorProduct(Vector3 v1, Vector3 v2)
        {
            return Vector3.Cross(v1, v2);

        }


        public static Quaternion AddVector(Quaternion addee, Vector3 vector)
        {

            Quaternion q = Quaternion.Concatenate(
                addee,
                new Quaternion(
                    vector.X,
                    vector.Y,
                    vector.Z, 0f)
            );

            float r = q.W * q.W * (0.5f);
            float x = q.X * q.X * (0.5f);
            float y = q.Y * q.Y * (0.5f);
            float z = q.Z * q.Z * (0.5f);
            return new Quaternion(x, y, z, r);
        }

        public static bool AlmostEquals(float f1, float f2, float precision = 0.025f)
        {
            return TrickyMathHelper.Abs(f1 - f2) <= precision;
        }


        public static bool CloseEnoughToBeSame(Vector3 v1, Vector3 v2)
        {
            return
                AlmostEquals(v1.X, v2.X) &&
                AlmostEquals(v1.Y, v2.Y) &&
                AlmostEquals(v1.Z, v2.Z);
        }
        public static bool CloseEnoughToBeSame(Vector3 v1, Vector3 v2, float precision)
        {
            return
                AlmostEquals(v1.X, v2.X, precision) &&
                AlmostEquals(v1.Y, v2.Y, precision) &&
                AlmostEquals(v1.Z, v2.Z, precision);
        }
       


        public static Vector3[] MatrixColumnVectors(Matrix m)
        {
            Matrix4 mat4 = RigidBodyHelper.MatrixtoMatrix4(m);

            return new Vector3[]{
                new Vector3( mat4.data[0], mat4.data[4], mat4.data[8]),
                new Vector3( mat4.data[1], mat4.data[5], mat4.data[9]),
                new Vector3( mat4.data[2], mat4.data[6], mat4.data[10])
            };
        }


        public static bool MakeOrthonormalBasis(Vector3 x, Vector3 suggestedY, out Vector3 y, out Vector3 z)
        {
            
            z = TrickyMathHelper.VectorProduct(x, suggestedY);

            if (z.LengthSquared() == 0f)
            {
                y = Vector3.Zero;
                return false;
            }

            y = TrickyMathHelper.VectorProduct(x, z);

            y.Normalize();
            z.Normalize();
            
            return true;
        }

        public static void MakeOrthonormalBasis(Vector3 Normal, out Vector3 tangent1, out Vector3 tangent2)
        {
            Vector3[] tangent = new Vector3[2];
            float sWas;
            bool top = false;
            if (TrickyMathHelper.Abs(Normal.X) > TrickyMathHelper.Abs(Normal.Y))
            {
                float s = 1f / TrickyMathHelper.Sqrt(Normal.Z * Normal.Z + Normal.X * Normal.X);
                sWas = s;
                top = true;
                tangent[0] = new Vector3(
                    Normal.Z * s,
                    0f,
                    -Normal.X * s

                    );

                tangent[1] = new Vector3(
                    Normal.Y * tangent[0].X,
                    Normal.Z * tangent[0].X - Normal.X * tangent[0].Z,
                    -Normal.Y * tangent[0].X
                    );
            }
            else
            {
                float s = 1f / TrickyMathHelper.Sqrt(Normal.Z * Normal.Z + Normal.Y * Normal.Y);
                sWas = s;
                tangent[0] = new Vector3(
                    0f,
                    -Normal.Z * s,
                    Normal.Y * s
                    );

                tangent[1] = new Vector3(
                    Normal.Y * tangent[0].Z - Normal.Z * tangent[1].Y,
                    -Normal.X * tangent[0].Z,
                    Normal.X * tangent[0].Y
                    );
            }

            tangent1 = tangent[0];
            tangent2 = tangent[1];
     
        }

        public static Matrix CreateOrthonormalBasisFromColumnVectors(Vector3 localX, Vector3 localY, Vector3 localZ)
        {
            return new Matrix( 
                localX.X, localY.X, localZ.X, 0f,
                localX.Y, localY.Y, localZ.Y, 0f,
                localX.Z, localY.Z, localZ.Z, 0f,
                0f, 0f, 0f, 1f 
                    );
        }

        public static float[] Vector3ToFloatArray(Vector3 vec)
        {
            return new float[] { vec.X, vec.Y, vec.Z };
        }
        public static Vector3 FloatArrayToVector3(float[] array)
        {
            return new Vector3(array[0], array[1], array[2]);
        }
    }
}
