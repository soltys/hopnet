using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;
using System.Diagnostics;

namespace Game
{
    class KinectPlayer
    {
        #region Public properties

        public GameConstants.PlayerStance currentStance { get; set; }
        public GameConstants.PlayerStance lastStance { get; set; }
        private bool isMotionCheckEnabled;
        private readonly Hero modelPosition;
        private readonly Model model;

        private float modelGroundLevel;

        private float spaceRequiredToSideJump = 4.0f;
        private float spaceRequiredToJump = 4.0f;

        private float idleShoulderHeight = 2.0f;

        public float rowToRowIdleMoveTime=0;
        public float rowToRowMoveTime = 0;
        public float verticalVelocity=0;
        public float horizontalVelocity=0;
        public float currentJumpTime = 0;
        public float idleJumpGravity = 0;
        public float jumpGravity = 0;
        public float timeAmount;
        private float jumpDirection;
        public float platformRadius;
        public float idleJumpPlatformRadius;

        public Stopwatch timeLeftToJump;
        public Stopwatch newGameCounter;

        public float distance = 0;
        public float tdistance = 0;
        public bool isFirstPlatformBehindPlayer = false;
        public bool isPlayerOnPlatform = true;

        public KinectPlayer(ContentManager content, Vector3 platformData)
        {
            timeLeftToJump = new Stopwatch();
            timeLeftToJump.Reset();

            
            newGameCounter = new Stopwatch();
            newGameCounter.Reset();
            
            modelPosition= new Hero(new ObjectData3D
                                      {
                                          Position = new Vector3(0.0f, 0, 0.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      });
            model = content.Load<Model>(@"Models\hero");
            currentStance = GameConstants.PlayerStance.GameStartCountDown;
            lastStance = GameConstants.PlayerStance.GameStartCountDown;
            isMotionCheckEnabled = true;
            modelGroundLevel = platformData.Y+GameConstants.PlayerModelHeight;
            modelPosition.objectArrangement.Position = new Vector3(platformData.X,modelGroundLevel,platformData.Z);
            modelPosition.oldArrangement = modelPosition.objectArrangement;
        }
        #endregion





        private void IsBehindFirstPlatform(List<Platform> platformList)
        {
            if (modelPosition.objectArrangement.Position.Z > platformList.First().objectArrangement.Position.Z)
            {
                isFirstPlatformBehindPlayer = false;
            }
            else
            {
                isFirstPlatformBehindPlayer = true;
            }
        }
        public void WaitForPlatformEnd(List<Platform> platformList)
        {
            distance = (float)(Math.Sqrt((Math.Pow( modelPosition.objectArrangement.Position.Z - platformList.First().objectArrangement.Position.Z,2))));

            IsBehindFirstPlatform(platformList);

            if ((distance >= idleJumpPlatformRadius) && (distance < platformRadius))
            {
                if (isFirstPlatformBehindPlayer)
                {
                    switch (currentStance)
                    {
                            case GameConstants.PlayerStance.GameStartCountDown:
                            lastStance = currentStance;
                            currentStance = GameConstants.PlayerStance.IdleJump;
                            break;

                            case GameConstants.PlayerStance.Idle:
                            lastStance = currentStance;
                            currentStance = GameConstants.PlayerStance.IdleJump;
                            break;

                            case GameConstants.PlayerStance.JumpReady:
                            lastStance = currentStance;
                            currentStance = GameConstants.PlayerStance.Jump;
                            break;

                            case  GameConstants.PlayerStance.SideJumpReady:
                            lastStance = currentStance;
                            currentStance=GameConstants.PlayerStance.SideJump;
                            break;
                    }
                }
            }
        }

        public void SetPlatformToPlatformMoveTime(float platformSpeed, float distanceBetweenRows, float timerUpdate, float distanceBetweenPlatforms)
        {
            rowToRowIdleMoveTime =  (((distanceBetweenRows-2*idleJumpPlatformRadius) / platformSpeed)/60)*1000;
            rowToRowMoveTime = (((2*distanceBetweenRows -2*idleJumpPlatformRadius) / platformSpeed) / 60) * 1000;
            verticalVelocity = platformSpeed;
            horizontalVelocity = (distanceBetweenRows/distanceBetweenPlatforms) * platformSpeed;
            idleJumpGravity = ((((2 * verticalVelocity) / (rowToRowIdleMoveTime)))/60)*1000;
            jumpGravity = ((((2 * verticalVelocity) / (rowToRowMoveTime))) / 60) * 1000;
            timeAmount = timerUpdate/10;
        }
        public void SetPlatformRadius(float singlePlatformRadius)
        {
            platformRadius = singlePlatformRadius;
            idleJumpPlatformRadius = 0.6f * singlePlatformRadius;
        }

        private void CheckJumpForward(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.ShoulderCenter].Position.Y > spaceRequiredToJump)
            {
                lastStance = currentStance;
                currentStance = GameConstants.PlayerStance.JumpReady;
            }
        }
        private void CheckLeftHandUp(Skeleton skeleton)
        {
            if (isMotionCheckEnabled)
            {
                if (skeleton.Joints[JointType.HandLeft].Position.Y > spaceRequiredToSideJump)
                {
                    isMotionCheckEnabled = false;
                    jumpDirection = -1;
                    lastStance = currentStance;
                    currentStance = GameConstants.PlayerStance.SideJumpReady;
                }
            }
        }
        private void CheckRightHandUp(Skeleton skeleton)
        {
            if (isMotionCheckEnabled)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y > spaceRequiredToSideJump)
                {
                    isMotionCheckEnabled = false;
                    jumpDirection = 1;
                    lastStance = currentStance;
                    currentStance = GameConstants.PlayerStance.SideJumpReady;
                }
            }
        }
        private void CheckPlayerStance(Skeleton skeleton)
        {
            switch (currentStance)
            {
                case GameConstants.PlayerStance.Idle:
                    isMotionCheckEnabled = true;
                    CheckJumpForward(skeleton);
                    if (currentStance == GameConstants.PlayerStance.Idle)
                    {
                        CheckLeftHandUp(skeleton);
                        CheckRightHandUp(skeleton);
                    }
                    break;
            }
        }

        private void PerformIdleJump()
        {
            if ((currentJumpTime < rowToRowIdleMoveTime) & modelPosition.objectArrangement.Position.Y >= modelGroundLevel)
            {
                currentJumpTime += timeAmount;
                modelPosition.objectArrangement.Position = new Vector3(modelPosition.oldArrangement.Position.X,
                   modelPosition.oldArrangement.Position.Y + (verticalVelocity * currentJumpTime - idleJumpGravity * currentJumpTime * currentJumpTime /2) / GameConstants.JumpHeightDivider,
                   modelPosition.oldArrangement.Position.Z);
            }
            else
            {
                currentJumpTime = 0;

                modelPosition.objectArrangement.Position = new Vector3(
                    modelPosition.objectArrangement.Position.X,
                    modelGroundLevel,
                    modelPosition.objectArrangement.Position.Z);

                modelPosition.oldArrangement.Position = new Vector3(
                    modelPosition.objectArrangement.Position.X,
                    modelPosition.objectArrangement.Position.Y,
                    modelPosition.objectArrangement.Position.Z);

                lastStance = currentStance;
                currentStance = GameConstants.PlayerStance.Idle;
            }
        }
        private void PerformJump()
        {
            if ((currentJumpTime < rowToRowMoveTime) & modelPosition.objectArrangement.Position.Y >= modelGroundLevel)
            {
                currentJumpTime += timeAmount;
                modelPosition.objectArrangement.Position = new Vector3(modelPosition.oldArrangement.Position.X,
                   modelPosition.oldArrangement.Position.Y + (verticalVelocity * currentJumpTime - jumpGravity * currentJumpTime * currentJumpTime / 2) / GameConstants.JumpHeightDivider,
                   modelPosition.oldArrangement.Position.Z);
            }
            else
            {
                currentJumpTime = 0;

                modelPosition.objectArrangement.Position = new Vector3(
                    modelPosition.objectArrangement.Position.X,
                    modelGroundLevel,
                    modelPosition.objectArrangement.Position.Z);

                modelPosition.oldArrangement.Position = new Vector3(
                    modelPosition.objectArrangement.Position.X,
                    modelPosition.objectArrangement.Position.Y,
                    modelPosition.objectArrangement.Position.Z);

                lastStance = currentStance;
                currentStance = GameConstants.PlayerStance.Idle;
            }
        }
        private void PerformHorizontalJump()
        {
            modelPosition.objectArrangement.Position =
                new Vector3(modelPosition.oldArrangement.Position.X + jumpDirection*horizontalVelocity,
                            modelPosition.objectArrangement.Position.Y,
                            modelPosition.objectArrangement.Position.Z);
        }


        public void Update(List <Platform> platformList, float distanceBetweenRows)
        {
            WaitForPlatformEnd(platformList);

            switch (currentStance)
            {
                    
                case GameConstants.PlayerStance.GameStartCountDown:
                    if (!newGameCounter.IsRunning)
                    {
                        newGameCounter.Start();
                    }
                    else
                    {

                        if (GameConstants.NewGameCountdownTime - newGameCounter.Elapsed.Seconds <= 0)
                        {
                            lastStance = GameConstants.PlayerStance.Idle;
                            currentStance = GameConstants.PlayerStance.Idle;
                        }
                         
                    }
                    break;
                    
                case GameConstants.PlayerStance.Idle:
                    CheckPlayerOnPlatformPosition(platformList);
                    break;
                case GameConstants.PlayerStance.IdleJump:
                    PerformIdleJump();
                    break;
                case GameConstants.PlayerStance.Jump:
                    PerformJump();
                    break;
                case GameConstants.PlayerStance.SideJump:
                    PerformIdleJump();
                    PerformHorizontalJump();
                    break;
            }
        }

        
        public void CheckPlayerOnPlatformPosition(List<Platform> platformList)
        {
            var xPlatformDistance = Math.Sqrt( (modelPosition.objectArrangement.Position.X-platformList.First().objectArrangement.Position.X)
                *(modelPosition.objectArrangement.Position.X-platformList.First().objectArrangement.Position.X));

            var zPlatformDistance = Math.Sqrt((modelPosition.objectArrangement.Position.Z - platformList.First().objectArrangement.Position.Z)
                * (modelPosition.objectArrangement.Position.Z - platformList.First().objectArrangement.Position.Z));

            isPlayerOnPlatform = true;
            if (xPlatformDistance > platformRadius)
            {
                 isPlayerOnPlatform=false;
            }
            else if (zPlatformDistance > platformRadius)
            {
                isPlayerOnPlatform = false;
            }
        }
        

        public void KinectUpdate(KinectData kinectData)
        {
            idleShoulderHeight = kinectData.PersonIdleHeight;
            spaceRequiredToJump = idleShoulderHeight + GameConstants.PlayerForwardJumpModifier;
            spaceRequiredToSideJump = idleShoulderHeight + GameConstants.PlayerSideJumpModifier;

            if (kinectData.Skeleton != null)
            {
                CheckPlayerStance(kinectData.Skeleton);
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font,float aspectRatio, Vector3 cameraPosition)
        {
            modelPosition.Draw(aspectRatio, cameraPosition, model);

            
            switch (currentStance)
            {
                case GameConstants.PlayerStance.GameStartCountDown:
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, (GameConstants.NewGameCountdownTime - newGameCounter.Elapsed.Seconds).ToString(), new Vector2(100, 100), Color.Red, 0, Vector2.Zero, 3, SpriteEffects.None, 1);
                    spriteBatch.End();
                    break;
                case  GameConstants.PlayerStance.GameEnded:
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "Przegrales!", new Vector2(0.5f*GameConstants.HorizontalGameResolution, 0.1f*GameConstants.VerticalGameResolution), Color.Orange, 0, Vector2.Zero, 3, SpriteEffects.None, 1);
                    spriteBatch.End();
                    break;
            }
             
        }
    }
}
