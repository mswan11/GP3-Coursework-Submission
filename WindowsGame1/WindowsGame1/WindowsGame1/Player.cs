using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    class Player
    {

        // Set the position of the model in world space, and set the rotation.
        public Vector3 playerPosition = Vector3.Zero;
        public float playerRotation = 0.0f;
        public Vector3 playerVelocity = Vector3.Zero;

        public void MoveModel(Laser[] laserList, SoundEffect firingSound, int volumeMultiplier)
        {
            KeyboardState lastState;
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            MouseState mouseState = Mouse.GetState();
            // Create some velocity if the right trigger is down.
            Vector3 playerVelocityAdd = Vector3.Zero;
            // Find out what direction we should be thrusting, using rotation

            if (keyboardState.IsKeyDown(Keys.Left) || gamePadState.ThumbSticks.Right.X < 0)
            {
                // Rotate left.
                playerRotation -= -1.0f * 0.10f;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || gamePadState.ThumbSticks.Right.X > 0)
            {
                // Rotate right.
                playerRotation -= 1.0f * 0.10f;
            }
            if (keyboardState.IsKeyDown(Keys.W) || gamePadState.ThumbSticks.Left.Y > 0)
            {
                // Move forwards
                playerVelocityAdd.X = -(float)Math.Sin(playerRotation);
                playerVelocityAdd.Z = -(float)Math.Cos(playerRotation);
                playerVelocityAdd *= 0.5f;
                playerVelocity += playerVelocityAdd;
            }
            if (keyboardState.IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Left.X < 0)
            {
                // Move left
                playerVelocityAdd.X = -(float)Math.Cos(playerRotation);
                playerVelocityAdd.Z = +(float)Math.Sin(playerRotation);
                playerVelocityAdd *= 0.5f;
                playerVelocity += playerVelocityAdd;
            }
            if (keyboardState.IsKeyDown(Keys.S) || gamePadState.ThumbSticks.Left.Y < 0)
            {
                // Move right
                playerVelocityAdd.X = +(float)Math.Sin(playerRotation);
                playerVelocityAdd.Z = +(float)Math.Cos(playerRotation);
                playerVelocityAdd *= 0.5f;
                playerVelocity += playerVelocityAdd;
            }
            if (keyboardState.IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Left.X > 0)
            {
                // Move forwards
                playerVelocityAdd.X = +(float)Math.Cos(playerRotation);
                playerVelocityAdd.Z = -(float)Math.Sin(playerRotation);
                playerVelocityAdd *= 0.5f;
                playerVelocity += playerVelocityAdd;
            }
            if (keyboardState.IsKeyDown(Keys.R) || gamePadState.IsButtonDown(Buttons.Back))
            {
                playerVelocity = Vector3.Zero;
                playerPosition = Vector3.Zero;
                playerRotation = 0.0f;
            }                

            //are we shooting?
            if ((mouseState.LeftButton == ButtonState.Pressed) || gamePadState.Triggers.Right > 0)
            {
                //add another bullet.  Find an inactive bullet slot and use it
                //if all bullets slots are used, ignore the user input
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (!laserList[i].isActive)
                    {
                        Matrix playerTransform = Matrix.CreateRotationY(playerRotation);
                        laserList[i].direction = playerTransform.Forward;
                        laserList[i].speed = GameConstants.LaserSpeedAdjustment;
                        laserList[i].position = playerPosition + laserList[i].direction;
                        laserList[i].isActive = true;
                        firingSound.Play((0.1f * volumeMultiplier), 0.0f, 0.0f);
                        break; //exit the loop     
                    }
                }
            }

            lastState = keyboardState;

        }
    }
}
