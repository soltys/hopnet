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

        public enum PlayerStances { Idle = 1, IdleJump = 2, LeftHandUp = 3, RightHandUp = 4, JumpReady=7, Jump=8, SideJump=9, SideJumpReady=10}
        public PlayerStances currentStance { get; set; }
        private PlayerStances lastStance { get; set; }
        private bool isMotionCheckEnabled;
        private Hero modelPosition;
        private Model model;

        private float modelGroundLevel=0.8f;

        private float spaceRequiredToSideJump = 4.0f;
        private float spaceRequiredToJump = 4.0f;

        private float shoulderMinToChange = 0.005f;
        private float shoulderHeight = 0.3f;
        private float lastShoulderHeight = 0.3f;
        private float shoulderChangeTime = 3000.0f;
        public float idleShoulderHeight = 2.0f;

        private Stopwatch heightChangeStopwatch;
        private Stopwatch movementStopwatch;

        public float rowToRowIdleMoveTime=0;
        public float rowToRowMoveTime = 0;
        public float verticalVelocity=0;
        public float horizontalVelocity=0;
        public float timer = 0;
        public float idleJumpGravity = 0;
        public float jumpGravity = 0;
        private float jumpHeightDivider = 20f;
        public float timeAmount;
        private float jumpDirection;
        private float modelHeight = 1.0f;
        public float platformRadius;
        public float radiusToIdleJump;

        public Stopwatch timeLeftToJump;


        public KinectPlayer(ContentManager content, Vector3 platformData)
        {
            timeLeftToJump = new Stopwatch();
            timeLeftToJump.Reset();


            heightChangeStopwatch = new Stopwatch();
            heightChangeStopwatch.Reset();
            movementStopwatch = new Stopwatch();
            movementStopwatch.Reset();


            modelPosition= new Hero(new ObjectData3D
                                      {
                                          Position = new Vector3(0.0f, 0, 0.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      });
            model = content.Load<Model>(@"Models\hero");
            currentStance = PlayerStances.Idle;
            lastStance = PlayerStances.Idle;
            isMotionCheckEnabled = true;
            modelGroundLevel = platformData.Y+modelHeight;
            modelPosition.objectArrangement.Position = new Vector3(platformData.X,modelGroundLevel,platformData.Z);
            modelPosition.oldArrangement = modelPosition.objectArrangement;
        }
        #endregion

        public float distance=0;
        public float tdistance=0;
        public bool isFirstPlatformBehindPlayer=false;



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

            if ((distance >= radiusToIdleJump) && (distance < platformRadius))
            {
                if (isFirstPlatformBehindPlayer)
                {
                    switch (currentStance)
                    {
                            case PlayerStances.Idle:
                            lastStance = currentStance;
                            currentStance = PlayerStances.IdleJump;
                            break;

                            case PlayerStances.JumpReady:
                            lastStance = currentStance;
                            currentStance = PlayerStances.Jump;
                            break;

                            case  PlayerStances.SideJumpReady:
                            lastStance = currentStance;
                            currentStance=PlayerStances.SideJump;
                            break;
                    }
                }
            }
        }

        public void SetPlatformToPlatformMoveTime(float platformSpeed, float distanceBetweenRows, float timerUpdate, float distanceBetweenPlatforms)
        {
            rowToRowIdleMoveTime =  (((distanceBetweenRows-2*radiusToIdleJump) / platformSpeed)/60)*1000;
            rowToRowMoveTime = (((2*distanceBetweenRows -2*radiusToIdleJump) / platformSpeed) / 60) * 1000;
            verticalVelocity = platformSpeed;
            horizontalVelocity = (distanceBetweenRows/distanceBetweenPlatforms) * platformSpeed;
            idleJumpGravity = ((((2 * verticalVelocity) / (rowToRowIdleMoveTime)))/60)*1000;
            jumpGravity = ((((2 * verticalVelocity) / (rowToRowMoveTime))) / 60) * 1000;
            timeAmount = timerUpdate/10;
        }
        public void SetPlatformRadius(float singlePlatformRadius)
        {
            platformRadius = singlePlatformRadius;
            radiusToIdleJump = 0.6f * singlePlatformRadius;
        }

        private void CheckJumpForward(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.ShoulderCenter].Position.Y > spaceRequiredToJump)
            {
                lastStance = currentStance;
                currentStance = PlayerStances.JumpReady;
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
                    currentStance = PlayerStances.SideJumpReady;
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
                    currentStance = PlayerStances.SideJumpReady;
                }
            }
        }
        private void CheckPlayerStance(Skeleton skeleton)
        {
            switch (currentStance)
            {
                case PlayerStances.Idle:
                    isMotionCheckEnabled = true;
                    CheckJumpForward(skeleton);
                    if (currentStance == PlayerStances.Idle)
                    {
                        CheckLeftHandUp(skeleton);
                        CheckRightHandUp(skeleton);
                    }
                    break;
            }
        }

        private void CalculateShoulderPosition(Skeleton skeleton)
        {
            if (!heightChangeStopwatch.IsRunning)
            {
                lastShoulderHeight = shoulderHeight;
            }
            shoulderHeight = skeleton.Joints[JointType.ShoulderCenter].Position.Y;

            if (Math.Abs(lastShoulderHeight - shoulderHeight) > shoulderMinToChange)
            {
                if (!heightChangeStopwatch.IsRunning)
                {
                    heightChangeStopwatch.Start();
                }
                else
                {
                    if (heightChangeStopwatch.Elapsed.TotalMilliseconds > shoulderChangeTime)
                    {
                        idleShoulderHeight = shoulderHeight;
                        spaceRequiredToJump = idleShoulderHeight + 0.15f;
                        spaceRequiredToSideJump = idleShoulderHeight + 0.25f;
                        heightChangeStopwatch.Reset();
                    }
                }
            }
        }

        
        private void PerformIdleJump()
        {
            if ((timer < rowToRowIdleMoveTime) & modelPosition.objectArrangement.Position.Y >= modelGroundLevel)
            {
                timer += timeAmount;
                modelPosition.objectArrangement.Position = new Vector3(modelPosition.oldArrangement.Position.X,
                   modelPosition.oldArrangement.Position.Y + (verticalVelocity * timer - idleJumpGravity * timer * timer /2) / jumpHeightDivider,
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
        }

        private void PerformJump()
        {
            if ((timer < rowToRowMoveTime) & modelPosition.objectArrangement.Position.Y >= modelGroundLevel)
            {
                timer += timeAmount;
                modelPosition.objectArrangement.Position = new Vector3(modelPosition.oldArrangement.Position.X,
                   modelPosition.oldArrangement.Position.Y + (verticalVelocity * timer - jumpGravity * timer * timer / 2) / jumpHeightDivider,
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
                case PlayerStances.IdleJump:
                    PerformIdleJump();
                    break;
                case PlayerStances.Jump:
                    PerformJump();
                    break;
                case PlayerStances.SideJump:
                    PerformIdleJump();
                    PerformHorizontalJump();
                    break;
            }
        }

        public void KinectUpdate(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                CalculateShoulderPosition(skeleton);
                CheckPlayerStance(skeleton);
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font,float aspectRatio, Vector3 cameraPosition)
        {
            modelPosition.Draw(aspectRatio, cameraPosition, model);
        }
    }
}
