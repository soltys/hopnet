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
        private bool moveEnabled;
        public bool isOnPlatform;
        private Hero modelPosition;
        private Model model;

        private float modelGroundLevel=0.8f;
        private float hPlatformSpace;
        private float dPlatformSpace;

        private float spaceRequiredToSideJump = 4.0f;
        private float spaceRequiredToJump = 4.0f;
        private float spaceRequiredToResetHand = -4f;

        private float shoulderMinToChange = 0.005f;
        private float shoulderHeight = 0.3f;
        private float lastShoulderHeight = 0.3f;
        private float shoulderChangeTime = 3000.0f;
        private float idleShoulderHeight = 2.0f;

        private Stopwatch heightChangeTimer;
        private Stopwatch movementTimer;

        private float platformToPlatformMoveTime;
        private float playerVelocity;
        private float playerToPlatformThreshold;
        private float timer = 0.0f;
        private float gravity = 0.0f;



        public KinectPlayer(ContentManager content, Vector3 platformData, float playerToPlatformSafeArea)
        {
            isOnPlatform = true;
            heightChangeTimer = new Stopwatch();
            heightChangeTimer.Reset();
            movementTimer = new Stopwatch();
            movementTimer.Reset();

            playerToPlatformThreshold = playerToPlatformSafeArea;

            modelPosition= new Hero(new ObjectData3D
                                      {
                                          Position = new Vector3(0.0f, modelGroundLevel, 9.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      });
            model = content.Load<Model>(@"Models\hero");
            currentStance = PlayerStances.Idle;
            lastStance = PlayerStances.Idle;
            moveEnabled = true;
            hPlatformSpace = platformData.X;
            dPlatformSpace = platformData.Z;
        }
        #endregion

        private float distance;
        public void IsPlayerOnPlatform(List<Platform> platformList)
        {
            distance = (float)(Math.Sqrt((Math.Pow( modelPosition.objectArrangement.Position.Z - platformList.First().objectArrangement.Position.Z,2))));

            if (distance < playerToPlatformThreshold)
            {
                if (distance < 0.01f)
                {
                    lastStance = currentStance;
                    currentStance = PlayerStances.JumpForwardUp;
                }
                isOnPlatform = true;
            }
            else
            {
                isOnPlatform = false;
            }

        }


        public void GetPlatformToPlatformMoveTime(float platformSpeed, float distanceBetweenRows)
        {
            platformToPlatformMoveTime =  ((distanceBetweenRows / platformSpeed)/60)*1000;
            playerVelocity = platformSpeed;
            gravity = ((((2 * playerVelocity) / (platformToPlatformMoveTime)))/60)*1000;
        }

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
        private void CalcualteHeightPosition(Skeleton skeleton)
        {
            if (!heightChangeTimer.IsRunning)
            {
                lastShoulderHeight = shoulderHeight;
            }
            shoulderHeight = skeleton.Joints[JointType.ShoulderCenter].Position.Y;

            if (Math.Abs(lastShoulderHeight - shoulderHeight) > shoulderMinToChange)
            {
                if (!heightChangeTimer.IsRunning)
                {
                    heightChangeTimer.Start();
                }
                else
                {
                    if (heightChangeTimer.Elapsed.TotalMilliseconds > shoulderChangeTime)
                    {
                        idleShoulderHeight = shoulderHeight;
                        spaceRequiredToJump = idleShoulderHeight + 0.15f;
                        spaceRequiredToResetHand = idleShoulderHeight - 0.3f;
                        spaceRequiredToSideJump = idleShoulderHeight + 0.25f;
                        heightChangeTimer.Reset();
                    }
                }
            }
        }

        
        public void Update(float spaceBetweenPlatforms, float spaceBetweenRows, float movementSpeed)
        {
            switch (currentStance)
            {
                case PlayerStances.JumpForwardUp:
                    
                    if ((timer < platformToPlatformMoveTime) & modelPosition.objectArrangement.Position.Y>=modelGroundLevel)
                    {
                        timer += 1.6f;
                        modelPosition.objectArrangement.Position = new Vector3(modelPosition.oldArrangement.Position.X,
                           modelPosition.oldArrangement.Position.Y + (playerVelocity * timer - gravity * timer * timer / 2),
                           modelPosition.oldArrangement.Position.Z);
                    }
                        
                    else
                    {
                        timer = 0;
                        
                        modelPosition.objectArrangement.Position = new Vector3(
                            modelPosition.objectArrangement.Position.X, 
                            modelGroundLevel, 
                            modelPosition.objectArrangement.Position.Z);
                        

                        modelPosition.oldArrangement.Position = new Vector3(
                            modelPosition.objectArrangement.Position.X,
                            modelPosition.objectArrangement.Position.Y,
                            modelPosition.objectArrangement.Position.Z);

                        lastStance = currentStance;
                        currentStance = PlayerStances.Idle;
                    }
                         
                    
                    break;
            }

        }

        public void KinectUpdate(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                CalcualteHeightPosition(skeleton);
                CheckPlayerStance(skeleton);
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font,float aspectRatio, Vector3 cameraPosition)
        {
            modelPosition.Draw(aspectRatio, cameraPosition, model);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, distance.ToString(), new Vector2(100, 200), Color.Red);
            spriteBatch.DrawString(font, currentStance.ToString(), new Vector2(400, 320), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, idleShoulderHeight.ToString(), new Vector2(400, 400), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, timer.ToString(), new Vector2(200, 300), Color.Red);
            spriteBatch.DrawString(font, platformToPlatformMoveTime.ToString(), new Vector2(200, 450), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "gravity :" + gravity.ToString(), new Vector2(200, 550), Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.End();
        }
    }
}
