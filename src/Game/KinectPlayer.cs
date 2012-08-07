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
using Microsoft.Kinect;
using System.Diagnostics;



namespace Game
{
    class KinectPlayer
    {
        #region Public properties

        public enum PlayerStances { Idle = 1, JumpForwardUp = 2, JumpLeftDown = 3, JumpRightDown = 4, JumpLeftUp = 5, JumpRightUp = 6,JumpForwardDown = 7, TooNear = 8, TooFar = 9 }
        public PlayerStances currentStance { get; set; }
        private PlayerStances lastStance { get; set; }
        public bool moveEnabled;
        private Hero modelPosition;
        private Model model;

        private float modelGroundLevel;
        private float hPlatformSpace;
        private float dPlatformSpace;

        private float spaceRequiredToSideJump = 0.2f;
        private float spaceRequiredToJump = 0.1f;
        private float spaceRequiredToResetHand = -0.55f;

        private float shoulderMinToChange = 0.005f;
        private float shoulderHeight = 0.8f;
        private float lastShoulderHeight = 0.1f;
        private float shoulderChangeTime = 3000.0f;
        private float idleShoulderHeight = 0.7f;

        private Stopwatch jumpTimer;


        public KinectPlayer(ContentManager content, Vector3 platformData)
        {
            jumpTimer = new Stopwatch();
            jumpTimer.Reset();
            modelPosition= new Hero(new ObjectData3D
                                      {
                                          Position = new Vector3(0.0f, 0.5f, 9.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      });
            model = content.Load<Model>(@"Models\hero");
            currentStance = PlayerStances.Idle;
            lastStance = PlayerStances.Idle;
            moveEnabled = true;
            hPlatformSpace = platformData.X;
            modelGroundLevel = platformData.Y;
            dPlatformSpace = platformData.Z;
        }
        #endregion

        private void CheckJumpForward(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.ShoulderCenter].Position.Y > spaceRequiredToJump)
            {
                lastStance = currentStance;
                currentStance = PlayerStances.JumpForwardUp;
            }
        }
        private void CheckJumpLeftUp(Skeleton skeleton)
        {
            if (moveEnabled)
            {
                if (skeleton.Joints[JointType.HandLeft].Position.Y > spaceRequiredToSideJump)
                {
                    moveEnabled = false;
                    modelPosition.MoveLeft();
                    lastStance = currentStance;
                    currentStance = PlayerStances.JumpLeftUp;
                }
            }
        }
        private void CheckJumpRightUp(Skeleton skeleton)
        {
            if (moveEnabled)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y > spaceRequiredToSideJump)
                {
                    moveEnabled = false;
                    modelPosition.MoveRight();
                    lastStance = currentStance;
                    currentStance = PlayerStances.JumpRightUp;
                }
            }
        }
        private void CheckJumpLeftDown(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.Y < spaceRequiredToResetHand)
                {
                    lastStance = currentStance;
                    currentStance = PlayerStances.Idle;
                }
        }
        private void CheckJumpRightDown(Skeleton skeleton)
        {
                if(skeleton.Joints[JointType.HandRight].Position.Y < spaceRequiredToResetHand)
                {
                    lastStance = currentStance;
                    currentStance = PlayerStances.Idle;
                }
        }
        private void CheckPlayerStance(Skeleton skeleton)
        {
            switch (currentStance)
            {
                case PlayerStances.Idle:
                    moveEnabled = true;
                    CheckJumpForward(skeleton);
                    if (currentStance == PlayerStances.Idle)
                    {
                        CheckJumpLeftUp(skeleton);
                        CheckJumpRightUp(skeleton);
                    }
                    
                    break;
                case PlayerStances.JumpLeftUp:
                    CheckJumpForward(skeleton);
                    CheckJumpLeftDown(skeleton);
                    break;
                case PlayerStances.JumpRightUp:
                    CheckJumpForward(skeleton);
                    CheckJumpRightDown(skeleton);
                    break;
                case PlayerStances.JumpForwardUp:
                    CheckJumpLeftUp(skeleton);
                    CheckJumpRightUp(skeleton);
                    break;
            }
        }

        public void Update(double dTimeBetweenPlatforms, double platformsSpeed, double gravity)
        {
        }

        public void Update(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                    if(!jumpTimer.IsRunning)
                    {
                        lastShoulderHeight = shoulderHeight;
                    }
                    shoulderHeight = skeleton.Joints[JointType.ShoulderCenter].Position.Y;

                    if(Math.Abs(lastShoulderHeight-shoulderHeight) > shoulderMinToChange)
                    {
                        if (!jumpTimer.IsRunning)
                        {
                            jumpTimer.Start();
                        }
                        else
                        {
                            if (jumpTimer.Elapsed.TotalMilliseconds > shoulderChangeTime)
                            {
                                idleShoulderHeight = shoulderHeight;
                                spaceRequiredToJump = idleShoulderHeight + 0.15f;
                                spaceRequiredToResetHand = idleShoulderHeight - 0.3f;
                                spaceRequiredToSideJump = idleShoulderHeight + 0.25f;
                                jumpTimer.Reset();
                            }
                        }
                    }
                    CheckPlayerStance(skeleton);
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font,float aspectRatio, Vector3 cameraPosition)
        {
            modelPosition.Draw(aspectRatio, cameraPosition, model);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, currentStance.ToString(), new Vector2(400, 320), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, idleShoulderHeight.ToString(), new Vector2(400, 400), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.End();
        }
    }
}
