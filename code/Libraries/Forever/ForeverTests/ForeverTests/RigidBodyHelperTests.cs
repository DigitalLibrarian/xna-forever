using System;
using NUnit.Framework;

using Microsoft.Xna.Framework;
using Forever.Physics;
using Forever.Physics.MadMatrix;

namespace ForeverTests
{
    [TestFixture]
    public class RigidBodyHelperTests
    {

        [Test]
        public void RigidbodyHelper_Matrix3_Conversions()
        {
            Matrix matrix = new Matrix(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f);
            Assert.AreEqual(
                new Matrix(1f, 2f, 3f, 0f, 5f, 6f, 7f, 0f, 9f, 10f, 11f, 0f, 0f, 0f, 0f, 1f),
                RigidBodyHelper.Matrix3toMatrix(
                    RigidBodyHelper.MatrixtoMatrix3(matrix)
                )
            );
        }

        [Test]
        public void RigidBodyHelper_Matrix4_Conversions()
        {
            Matrix matrix = new Matrix(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f);

            Assert.AreEqual(
                new Matrix(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 0f, 0f, 0f, 1f),
                RigidBodyHelper.Matrix4toMatrix(
                    RigidBodyHelper.MatrixtoMatrix4(matrix)
                )
            );

        }





        [Test]
        public void RigidBodyHelper_Matrix4IsomorphicTransforms()
        {

            Matrix originalMatrix = new Matrix(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f);

            Matrix3 originalConverted = RigidBodyHelper.MatrixtoMatrix3(originalMatrix);




        }
    }
}
