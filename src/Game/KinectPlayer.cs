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
        public Hero modelPosition;
        private readonly Model model;

        private float modelGroundLevel;

        private float spaceRequiredToSideJump = 4.0f;
        private float spaceRequiredToJump = 4.0f;

        private float idleShoulderHeight = 8.0f;

        public float idleJumpTime=0;
        public float jumpTime = 0;
        public float verticalVelocity=0;
        public float horizontalVelocity=0;
        public float currentJumpTime = 0;
        public float timeAmount;
        private float jumpDirection;
        public float platformRadius;
        public float idleJumpPlatformRadius;

        public Stopwatch timeLeftToJump;
        public Stopwatch newGameCounter;

        public bool isFirstPlatformBehindPlayer = false;

        private float playerToPlatformDistance;
        public int timeCounter = 0;
        private float idleJumpDistanceRatio;
        private float idleJumpGravityMultiplier;
        private int idleJumpExpectedFunctionCalls;
        private float idleJumpVelocity;
        private float idleJumpGravity;
        private float idleJumpHeightDivider;


        private float jumpDistanceRatio;
        private float jumpGravityMultiplier;
        private float jumpExpectedFunctionCalls;
        private float jumpVelocity;
        private float jumpGravity ;
        private float jumpHeightDivider;
        




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


        public void NewGameDataReset()
        {

            lastStance = currentStance;
            currentStance = GameConstants.PlayerStance.GameStartCountDown;
            newGameCounter.Reset();
            modelPosition.objectArrangement.Position= new Vector3(GameConstants.FirstPlatformPosition + (GameConstants.RowLength/2)*GameConstants.SpaceBetweenPlatforms
                ,modelGroundLevel,
                modelPosition.objectArrangement.Position.Z);
            modelPosition.oldArrangement.Position = modelPosition.objectArrangement.Position;
        }


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
            playerToPlatformDistance = (float)(Math.Sqrt((Math.Pow( modelPosition.objectArrangement.Position.Z - platformList.First().objectArrangement.Position.Z,2))));

            IsBehindFirstPlatform(platformList);

            if ((playerToPlatformDistance >= idleJumpPlatformRadius) && (playerToPlatformDistance < platformRadius))
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
                            GameConstants.zegar.Restart();
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
                else
                {
                    if(GameConstants.zegar.IsRunning)
                    {
                        GameConstants.zegar.Stop();
                    }
                }
            }
        }



        public void SetPlatformToPlatformMoveTime(float platformSpeed, float distanceBetweenRows, float timerUpdate, float distanceBetweenPlatforms)
        {
            idleJumpHeightDivider = 1;
            idleJumpDistanceRatio = (GameConstants.SpaceBetweenRows-2*idleJumpPlatformRadius) / GameConstants.DefaultSpaceBetweenRows;
            idleJumpGravityMultiplier = (GameConstants.SpeedOfPlatformsOneUpdate / GameConstants.DefaultSpeedBetweenPlatforms) * 10;

            idleJumpExpectedFunctionCalls = (int)Math.Round((GameConstants.SpaceBetweenRows - 2 * idleJumpPlatformRadius) / GameConstants.SpeedOfPlatformsOneUpdate);
            idleJumpVelocity = GameConstants.DefaultTimerMultiplier * idleJumpDistanceRatio;
            idleJumpGravity = GameConstants.DefaultJumpGravity * idleJumpGravityMultiplier;

            var maxIdleJumpBallHeight = (((idleJumpGravity * (idleJumpExpectedFunctionCalls / 2) * (idleJumpExpectedFunctionCalls / 2)) / 2) / idleJumpHeightDivider) * 20;
            while (maxIdleJumpBallHeight / idleJumpHeightDivider > GameConstants.MaxiumumJumpHeight)
            {
                idleJumpHeightDivider *= 2;
            }

            jumpHeightDivider = 1;
            jumpDistanceRatio = (2*GameConstants.SpaceBetweenRows - 2 * idleJumpPlatformRadius) / GameConstants.DefaultSpaceBetweenRows;
            jumpGravityMultiplier = (GameConstants.SpeedOfPlatformsOneUpdate / GameConstants.DefaultSpeedBetweenPlatforms) * 10;

            jumpExpectedFunctionCalls = (int)Math.Round((2*GameConstants.SpaceBetweenRows - 2 * idleJumpPlatformRadius) / GameConstants.SpeedOfPlatformsOneUpdate);
            jumpVelocity = GameConstants.DefaultTimerMultiplier * jumpDistanceRatio;
            jumpGravity = GameConstants.DefaultJumpGravity * jumpGravityMultiplier;

            var maxJumpBallHeight = (((jumpGravity * (jumpExpectedFunctionCalls / 2) * (jumpExpectedFunctionCalls / 2)) / 2) / jumpHeightDivider) * 20;
            while (maxJumpBallHeight / jumpHeightDivider > GameConstants.MaxiumumJumpHeight)
            {
                jumpHeightDivider *= 2;
            }


        }
        public void SetPlatformRadius(float singlePlatformRadius)
        {
            platformRadius = singlePlatformRadius;
            idleJumpPlatformRadius = 0.9f * singlePlatformRadius;
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
            if (timeCounter<idleJumpExpectedFunctionCalls)
            {
                timeCounter++;
                currentJumpTime += timeAmount;
                modelPosition.objectArrangement.Position = new Vector3(modelPosition.oldArrangement.Position.X,
                   modelPosition.oldArrangement.Position.Y + (idleJumpVelocity*timeCounter - idleJumpGravity * timeCounter * timeCounter / 2) / idleJumpHeightDivider,
                   modelPosition.oldArrangement.Position.Z);
            }
            else
            {
                timeCounter = 0;
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
            if (timeCounter < jumpExpectedFunctionCalls)
            {
                timeCounter++;
                modelPosition.objectArrangement.Position = new Vector3(modelPosition.oldArrangement.Position.X,
                   modelPosition.oldArrangement.Position.Y + (timeCounter * jumpVelocity - jumpGravity * timeCounter * timeCounter / 2) / jumpHeightDivider,
                   modelPosition.oldArrangement.Position.Z);
            }
            else
            {
                currentJumpTime = 0;
                timeCounter=0;
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


        public void Update(List <Platform> platformList, Camera camera)
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
                    PerformJump();
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
            
            if (xPlatformDistance > platformRadius)
            {
                lastStance = currentStance;
                currentStance = GameConstants.PlayerStance.GameEnded;
            }
            else if (zPlatformDistance > platformRadius)
            {
                lastStance = currentStance;
                currentStance = GameConstants.PlayerStance.GameEnded;
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

        public void Draw(SpriteBatch spriteBatch, SpriteFont font,float aspectRatio,Camera camera)
        {
            modelPosition.Draw(aspectRatio, camera, model);

            switch (currentStance)
            {
                case GameConstants.PlayerStance.GameStartCountDown:
                    spriteBatch.Begin();
                    spriteBatch.DrawString(font, "Stan swobodnie i opusc rece", new Vector2(100, 50), Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 1);
                    spriteBatch.DrawString(font, (GameConstants.NewGameCountdownTime - newGameCounter.Elapsed.Seconds).ToString(), new Vector2(100, 100), Color.Red, 0, Vector2.Zero, 3, SpriteEffects.None, 1);
                    spriteBatch.End();
                    break;
            }
             
        }
    }
}
