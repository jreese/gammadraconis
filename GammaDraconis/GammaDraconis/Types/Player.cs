using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GammaDraconis.Core.Input;

namespace GammaDraconis.Types
{
    /// <summary>
    /// A Player is a special racer that is controlled by a human.
    /// </summary>
    class Player : Racer
    {
        public static Player[] players = new Player[4];

        public PlayerIndex index;
        public PlayerInput input;

        public Coords camera;

        public Player(PlayerIndex index)
            : base()
        {
            this.index = index;
            input = new PlayerInput(index);

            camera = new Coords();

            Player.players[(int)index] = this;
        }

        public override void think(GameTime gameTime)
        {
            input.update();

            float rate = 1f / gameTime.ElapsedGameTime.Milliseconds;

            Matrix translation = Matrix.Identity;
            Quaternion rotation = Quaternion.Identity;
            Quaternion cameraR = Quaternion.Identity;

            #region Keyboard input handling
            if (input.inputDown("Up"))
            {
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Right, -rate);
            }
            if (input.inputDown("Down"))
            {
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Right, rate);
            }
            if (input.inputDown("Left"))
            {
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Backward, rate);
            }
            if (input.inputDown("Right"))
            {
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Backward, -rate);
            }
            if (input.inputDown("YawLeft"))
            {
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, rate);
            }
            if (input.inputDown("YawRight"))
            {
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, -rate);
            }

            if (input.inputDown("ThrottleUp"))
            {
                translation *= Matrix.CreateTranslation(0f, 0f, -rate * 50);
            }
            if (input.inputDown("ThrottleDown"))
            {
                translation *= Matrix.CreateTranslation(0f, 0f, rate * 50);
            }
            #endregion

            #region Gamepad input handling
            {
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Right, -rate * input.axis("Pitch"));
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Backward, -rate * input.axis("Roll"));
                rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, -rate * input.axis("Yaw"));

                cameraR *= Quaternion.CreateFromYawPitchRoll(0.5f * input.axis("CameraY"), 0.5f * input.axis("CameraX"), 0f);
            }
            #endregion

            position.R *= rotation;
            position.T *= translation;

            camera.R = position.R * cameraR;
            camera.T = Matrix.CreateTranslation(0f, 100f, 1200f) * Matrix.CreateFromQuaternion(position.R) * position.T;
        }
    }
}
 