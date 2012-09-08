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
        #region Properties

        public GameConstants.PlayerStance currentStance { get; set; }
        public GameConstants.PlayerStance lastStance { get; set; }
        private bool isMotionCheckEnabled;
        public Hero modelPosition;
        private readonly Model model;

        private float modelGroundLevel;

        private Texture2D []progressBarTextures;
        private int progressBarTextureType;
        private Rectangle progressBarRectangle;
        private int expectedFunctionCallsOnPlatform;
        private int functionCallOnPlatformCounter;
        private int progressBarStep;

        private Sprite progressBarFrame;
        private Sprite progressBarBackground;



        private float spaceRequiredToSideJump = 100.0f;
        private float spaceRequiredToJump = 100.0f;

        private float idleShoulderHeight = 80.0f;

        private float jumpDirection;
        public float platformRadius;
        public float idleJumpPlatformRadius;

        public Stopwatch newGameCounter;

        public bool isFirstPlatformBehindPlayer = false;

        private float playerToPlatformDistance;
        private int timeCounter;

        private int idleJumpExpectedFunctionCalls;
        private float idleJumpVelocity;
        private float idleJumpGravity;
        private float idleJumpHeightDivider;

        private float jumpExpectedFunctionCalls;
        private float jumpVelocity;
        private float jumpGravity ;
        private float jumpHeightDivider;

        private float horizontalJumpMovementStep;


        public int ScoreInCurrentGame;


        public void LoadContent(ContentManager content)
        {
            progressBarFrame.LoadSprite(content, @"Sprites\player_progressbar_frame");
            progressBarTextures[0] = content.Load<Texture2D>(@"Sprites\player_progressbar_high");
            progressBarTextures[1] = content.Load<Texture2D>(@"Sprites\player_progressbar_medium");
            progressBarTextures[2] = content.Load<Texture2D>(@"Sprites\player_progressbar_low");
            progressBarBackground.LoadSprite(content,@"Sprites\player_progressbar_background");
        }

        public KinectPlayer(ContentManager content, Vector3 platformData)
        {
            progressBarBackground = new Sprite();
            progressBarBackground.Rectangle = new Rectangle(0,0,GameConstants.HorizontalGameResolution,GameConstants.VerticalGameResolution/80);
            progressBarFrame = new Sprite();
            progressBarFrame.Rectangle = new Rectangle(0,0,GameConstants.HorizontalGameResolution,GameConstants.VerticalGameResolution/80);
            newGameCounter = new Stopwatch();
            newGameCounter.Reset();
            
            progressBarRectangle=new Rectangle(0,0,GameConstants.HorizontalGameResolution,GameConstants.VerticalGameResolution/80);
            progressBarTextures = new Texture2D[3];
            modelPosition = new Hero(new ObjectData3D
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
            this.ScoreInCurrentGame = 0;
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
                    progressBarRectangle.Width = 0;
                    functionCallOnPlatformCounter = 0;
                }
            }
        }



        public void SetPlatformToPlatformMoveTime(float platformSpeed, float distanceBetweenRows, float timerUpdate, float distanceBetweenPlatforms)
        {
            idleJumpHeightDivider = 1;
            var idleJumpDistanceRatio = (GameConstants.SpaceBetweenRows-2*idleJumpPlatformRadius) / GameConstants.DefaultSpaceBetweenRows;
            var idleJumpGravityMultiplier = (GameConstants.SpeedOfPlatformsOneUpdate / GameConstants.DefaultSpeedBetweenPlatforms) * 10;

            idleJumpExpectedFunctionCalls = (int)Math.Round((GameConstants.SpaceBetweenRows - 2 * idleJumpPlatformRadius) / GameConstants.SpeedOfPlatformsOneUpdate);
            idleJumpVelocity = GameConstants.DefaultTimerMultiplier * idleJumpDistanceRatio;
            idleJumpGravity = GameConstants.DefaultJumpGravity * idleJumpGravityMultiplier;

            var maxIdleJumpBallHeight = (((idleJumpGravity * (idleJumpExpectedFunctionCalls / 2) * (idleJumpExpectedFunctionCalls / 2)) / 2) / idleJumpHeightDivider) * 20;
            while (maxIdleJumpBallHeight / idleJumpHeightDivider > GameConstants.MaxiumumJumpHeight)
            {
                idleJumpHeightDivider *= 2;
            }

            jumpHeightDivider = 1;
            var jumpDistanceRatio = (2*GameConstants.SpaceBetweenRows - 2 * idleJumpPlatformRadius) / GameConstants.DefaultSpaceBetweenRows;
            var jumpGravityMultiplier = (GameConstants.SpeedOfPlatformsOneUpdate / GameConstants.DefaultSpeedBetweenPlatforms) * 10;

            jumpExpectedFunctionCalls = (int)Math.Round((2*GameConstants.SpaceBetweenRows - 2 * idleJumpPlatformRadius) / GameConstants.SpeedOfPlatformsOneUpdate);
            jumpVelocity = GameConstants.DefaultTimerMultiplier * jumpDistanceRatio;
            jumpGravity = GameConstants.DefaultJumpGravity * jumpGravityMultiplier;

            var maxJumpBallHeight = (((jumpGravity * (jumpExpectedFunctionCalls / 2) * (jumpExpectedFunctionCalls / 2)) / 2) / jumpHeightDivider) * 20;
            while (maxJumpBallHeight / jumpHeightDivider > GameConstants.MaxiumumJumpHeight)
            {
                jumpHeightDivider *= 2;
            }

            horizontalJumpMovementStep = GameConstants.SpaceBetweenPlatforms/idleJumpExpectedFunctionCalls;


        }
        public void SetPlatformRadius(float singlePlatformRadius)
        {
            platformRadius = singlePlatformRadius;
            idleJumpPlatformRadius = 0.9f * singlePlatformRadius;

            expectedFunctionCallsOnPlatform = (int)Math.Round((platformRadius + idleJumpPlatformRadius) / GameConstants.SpeedOfPlatformsOneUpdate);
            progressBarStep = (int)Math.Round((double)GameConstants.HorizontalGameResolution / expectedFunctionCallsOnPlatform);
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
                new Vector3(modelPosition.objectArrangement.Position.X + jumpDirection*horizontalJumpMovementStep,
                            modelPosition.objectArrangement.Position.Y,
                            modelPosition.objectArrangement.Position.Z);
        }

        private void UpdateProgressBarWidth()
        {
            progressBarRectangle.Width = expectedFunctionCallsOnPlatform * progressBarStep - functionCallOnPlatformCounter * progressBarStep;
            functionCallOnPlatformCounter++;
            UpdateProgressBarColor();
            
        }

        private void UpdateProgressBarColor()
        {
            if (progressBarRectangle.Width >= 2 * GameConstants.HorizontalGameResolution / 3)
            {
                progressBarTextureType = 0;
            }
            if (progressBarRectangle.Width <= 2 * GameConstants.HorizontalGameResolution / 3)
            {
                progressBarTextureType = 1;
            }
            if (progressBarRectangle.Width <= GameConstants.HorizontalGameResolution / 3)
            {
                progressBarTextureType = 2;
            }
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
                    UpdateProgressBarWidth();
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

                case GameConstants.PlayerStance.JumpReady:
                    UpdateProgressBarWidth();
                    break;

                case GameConstants.PlayerStance.SideJumpReady:
                    UpdateProgressBarWidth();
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

        public void Draw(SpriteBatch spriteBatch, SpriteFont font,Camera camera)
        {
            progressBarBackground.DrawByRectangle(spriteBatch);

            modelPosition.Draw(camera, model);
            spriteBatch.Begin();
            spriteBatch.Draw(progressBarTextures[progressBarTextureType],progressBarRectangle,Color.White);
            spriteBatch.End();

            progressBarFrame.DrawByRectangle(spriteBatch);


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
