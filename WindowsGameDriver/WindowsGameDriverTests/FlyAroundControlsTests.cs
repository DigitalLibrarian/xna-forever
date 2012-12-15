using System;
using NUnit.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Adam.Screens;
using Forever.Demos;

namespace WindowsGameDriverTests
{

    [TestFixture]
    public class FlyAroundControlsTests
    {
        [Test]
        public void FlyAroundKeyboardControls()
        {
            UserControls flyControls = new UserControls(PlayerIndex.One, 1f, 1f, 2f, 3f);

            AssertOutput(flyControls, new Keys[] { Keys.W }, Vector3.Forward, Vector3.Zero);
            AssertOutput(flyControls, new Keys[] { Keys.S }, Vector3.Backward, Vector3.Zero);
            AssertOutput(flyControls, new Keys[] { Keys.A }, Vector3.Left, Vector3.Zero);
            AssertOutput(flyControls, new Keys[] { Keys.D }, Vector3.Right, Vector3.Zero);

            AssertOutput(flyControls, new Keys[] { Keys.Q }, Vector3.Zero, Vector3.Up);
            AssertOutput(flyControls, new Keys[] { Keys.E }, Vector3.Zero, Vector3.Down);

            
            AssertOutput(flyControls, new Keys[] { Keys.X }, Vector3.Up, Vector3.Zero);
            AssertOutput(flyControls, new Keys[] { Keys.Z }, Vector3.Down, Vector3.Zero);

            /* conficting keys cancel each other out */
            AssertOutput(flyControls, new Keys[] { Keys.W, Keys.S }, Vector3.Zero, Vector3.Zero);
            AssertOutput(flyControls, new Keys[] { Keys.A, Keys.D }, Vector3.Zero, Vector3.Zero);
            AssertOutput(flyControls, new Keys[] { Keys.Q, Keys.E }, Vector3.Zero, Vector3.Zero);


            /* combinations are additive */
            Vector3 direction = Vector3.Forward + Vector3.Right;
            direction.Normalize();
            AssertOutput(flyControls, new Keys[] { Keys.W, Keys.D }, direction, Vector3.Zero);
            AssertOutput(flyControls, new Keys[] { Keys.S, Keys.A }, -direction, Vector3.Zero);
            AssertOutput(flyControls, new Keys[] { Keys.W, Keys.E }, Vector3.Forward, Vector3.Down);

           
        }

        public void AssertOutput(UserControls flyControls, Keys[] keysDown, Vector3 expectedTrans, Vector3 expectedRot)
        {
            flyControls.HandleInput(new SpoofedInputState(PlayerIndex.One, keysDown));
            Assert.AreEqual(expectedTrans, flyControls.LocalForce);
            Assert.AreEqual(expectedRot, flyControls.LocalTorque);
        }


        /* Dedicated subclass for testing */

        class SpoofedInputState : InputState
        {
            PlayerIndex spoofedPlayerIndex;
            Keys[] spoofedKeysDown;
            public SpoofedInputState(PlayerIndex playerIndex, Keys[] keysDown)
            {
                this.spoofedPlayerIndex = playerIndex;
                this.spoofedKeysDown = keysDown;
            }

            public override bool IsKeyDown(Keys key, PlayerIndex playerIndex)
            {
                if (playerIndex == spoofedPlayerIndex)
                {
                    foreach (Keys spoofedKeyDown in spoofedKeysDown)
                    {
                        if (spoofedKeyDown == key)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }


    }



}
