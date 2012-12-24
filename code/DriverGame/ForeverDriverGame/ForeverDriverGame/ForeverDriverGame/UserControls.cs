using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Adam.Screens;
using Forever.Physics;
using Forever.Interface;

namespace Forever.Demos
{
    public enum UserControlKeys
    {
        MoveFoward,
        MoveBackward,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        TurnLeft, 
        TurnRight,
        LookUp,
        LookDown,
        RollRight,
        RollLeft,

        Shifter,

        Firing
    };

    public class UserControls 
    {
        // Computed deltas 
        Vector3 force = Vector3.Zero;
        [EntityInspector("CamControls Force")]
        public Vector3 LocalForce { get { return force; } }
        Vector3 torque;
        [EntityInspector("CamControls Torque")]
        public Vector3 LocalTorque { get { return torque; } }

        [EntityInspector("CamControls ForceMag")]
        public float ForceMag { get; set; }

        [EntityInspector("CamControls TorqueMag")]
        public float TorqueMag { get; set; }

        [EntityInspector("ForceShiftMag - needs unit test!!")]
        public float ForceShiftMag { get; set; }
        [EntityInspector("TorqueShiftMag - needs unit test!!")]
        public float TorqueShiftMag { get; set; }

        public bool Firing { get; set; }

        public PlayerIndex playerIndex;
        public Dictionary<Keys, UserControlKeys> KeyMappings = new Dictionary<Keys, UserControlKeys>();

        public UserControls(PlayerIndex playerIndex, float forceMag, float torqueMag, float forceShiftMag, float torqueShiftMag)
        {
            this.ForceMag = forceMag;
            this.TorqueMag = torqueMag;
            this.ForceShiftMag = forceShiftMag;
            this.TorqueShiftMag = torqueShiftMag;
            this.playerIndex = playerIndex;

            //default mappings
            KeyMappings[Keys.W] = UserControlKeys.MoveFoward;
            KeyMappings[Keys.S] = UserControlKeys.MoveBackward;
            KeyMappings[Keys.A] = UserControlKeys.MoveLeft;
            KeyMappings[Keys.D] = UserControlKeys.MoveRight;
            KeyMappings[Keys.Q] = UserControlKeys.TurnLeft;
            KeyMappings[Keys.Left] = UserControlKeys.TurnLeft;
            KeyMappings[Keys.E] = UserControlKeys.TurnRight;
            KeyMappings[Keys.Right] = UserControlKeys.TurnRight;
            KeyMappings[Keys.X] = UserControlKeys.MoveUp;
            KeyMappings[Keys.Z] = UserControlKeys.MoveDown;

            KeyMappings[Keys.Down] = UserControlKeys.LookUp;
            KeyMappings[Keys.Up] = UserControlKeys.LookDown;

            KeyMappings[Keys.T] = UserControlKeys.RollRight;
            KeyMappings[Keys.G] = UserControlKeys.RollLeft;

            KeyMappings[Keys.LeftShift] = UserControlKeys.Shifter;
            KeyMappings[Keys.RightShift] = UserControlKeys.Shifter;

            KeyMappings[Keys.Space] = UserControlKeys.Firing;
        }


        public void HandleInput(InputState inputState)
        {
            Vector3 newTrans = Vector3.Zero;
            Vector3 newRot = Vector3.Zero;
            bool shift = false;
            bool firing = false;
            foreach (KeyValuePair<Keys, UserControlKeys> pair in KeyMappings)
            {
                Keys key = pair.Key;
                if (inputState.IsKeyDown(key, playerIndex))
                {
                    UserControlKeys flyKey = pair.Value;
                    switch (flyKey)
                    {
                        case UserControlKeys.MoveFoward:
                            newTrans += Vector3.Forward;
                            break;
                        case UserControlKeys.MoveBackward:
                            newTrans += Vector3.Backward;
                            break;
                        case UserControlKeys.MoveRight:
                            newTrans += Vector3.Right;
                            break;
                        case UserControlKeys.MoveLeft:
                            newTrans += Vector3.Left;
                            break;
                        case UserControlKeys.MoveUp:
                            newTrans += Vector3.Up;
                            break;
                        case UserControlKeys.MoveDown:
                            newTrans += Vector3.Down;
                            break;
                        case UserControlKeys.TurnLeft:
                            newRot += Vector3.Up;
                            break;
                        case UserControlKeys.TurnRight:
                            newRot += Vector3.Down;
                            break;
                        case UserControlKeys.LookUp:
                            newRot += Vector3.Right;
                            break;
                        case UserControlKeys.LookDown:
                            newRot += Vector3.Left;
                            break;
                        case UserControlKeys.RollRight:
                            newRot += Vector3.Forward;
                            break;
                        case UserControlKeys.RollLeft:
                            newRot += Vector3.Backward;
                            break;
                        case UserControlKeys.Shifter:
                            shift = true;
                            break;
                        case UserControlKeys.Firing:
                            firing = true;
                            break;
                    }
                }
            }
            force = newTrans;
            if (force.Length() > 0)
            {
                force.Normalize();
                force *= shift ? ForceShiftMag : ForceMag;
            }

            torque = newRot;
            if (torque.Length() > 0)
            {
                torque.Normalize();
                torque *= shift ? TorqueShiftMag : TorqueMag;
            }

            Firing = firing;
        }


    }
}
