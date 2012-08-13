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

        public enum PlayerStances { Idle = 1, IdleJump = 2, JumpLeftDown = 3, JumpRightDown = 4, JumpLeftUp = 5, JumpRightUp = 6, JumpForward=7}
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
        public float idleShoulderHeight = 2.0f;

        private Stopwatch heightChangeTimer;
        private Stopwatch movementTimer;

        public float rowToRowMoveTime=0;
        public float verticalVelocity=0;
        public float horizontalVelocity=0;
        private float playerToPlatformThreshold=0;
        public float timer = 0;
        public float gravity = 0;
        private float jumpHeightDivider = 20f;
        public float timeAmount;
        private float jumpDirection = 0;
        private float modelHeight = 1.0f;
        public float platformRadius;
        public float radiusToIdleJump;

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
                                          Position = new Vector3(0.0f, 0, 0.0f),
                                          Scale = new Vector3(0.5f, 0.5f, 0.5f),
                                          Rotation = new Vector3(0.0f)
                                      });
            model = content.Load<Model>(@"Models\hero");
            currentStance = PlayerStances.Idle;
            lastStance = PlayerStances.Idle;
            moveEnabled = true;
            modelGroundLevel = platformData.Y+modelHeight;
            modelPosition.objectArrangement.Position = new Vector3(platformData.X,modelGroundLevel,platformData.Z);
            modelPosition.oldArrangement = modelPosition.objectArrangement;
        }
        #endregion

        public float distance=0;
        public float tdistance=0;
        public void CheckPlayerOnPlatformStatus(List<Platform> platformList)
        {
            distance = (float)(Math.Sqrt((Math.Pow( modelPosition.objectArrangement.Position.Z - platformList.First().objectArrangement.Position.Z,2))));

            if ((distance >= radiusToIdleJump) && (modelPosition.objectArrangement.Position.Z > platformList.First().objectArrangement.Position.Z))
            {
                tdistance = distance;
                lastStance = currentStance;
                currentStance= PlayerStances.IdleJump;
                isOnPlatform = true;
            }
            else
            {
                isOnPlatform = false;
            }
        }

        public void GetPlatformToPlatformMoveTime(float platformSpeed, float distanceBetweenRows, float timerUpdate, float distanceBetweenPlatforms)
        {
            rowToRowMoveTime =  ((distanceBetweenRows / platformSpeed)/60)*1000;
            verticalVelocity = platformSpeed;
            horizontalVelocity = (distanceBetweenPlatforms / rowToRowMoveTime);
            gravity = ((((2 * verticalVelocity) / (rowToRowMoveTime)))/60)*1000;
            timeAmount = timerUpdate/10;
        }
        public void GetPlatformRadius(float singlePlatformRadius)
        {
            platformRadius = singlePlatformRadius;
            radiusToIdleJump = 0.8f * singlePlatformRadius;
        }


        private void CheckIdleJump()
        {

        }
        private void CheckJumpForward(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.ShoulderCenter].Position.Y > spaceRequiredToJump)
            {
                lastStance = currentStance;
                currentStance = PlayerStances.JumpForward;
            }
        }
        private void CheckJumpLeftUp(Skeleton skeleton)
        {
            if (moveEnabled)
            {
                if (skeleton.Joints[JointType.HandLeft].Position.Y > spaceRequiredToSideJump)
                {
                    moveEnabled = false;
                    jumpDirection = -1;
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
                    jumpDirection = 1;
                    lastStance = currentStance;
                    currentStance = PlayerStances.JumpRightUp;
                }
            }
        }
        private void CheckJumpLeftDown(Skeleton skeleton)
        {
            if(skeleton.Joints[JointType.HandLeft].Position.Y < spaceRequiredToResetHand)
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
                case PlayerStances.JumpForward:
                    CheckJumpLeftUp(skeleton);
                    CheckJumpRightUp(skeleton);
                    break;
            }
        }
        private void CalculateShoulderPosition(Skeleton skeleton)
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

        
        private void VerticalJump(float gravityDivider)
        {
            if ((timer < rowToRowMoveTime) & modelPosition.objectArrangement.Position.Y >= modelGroundLevel)
            {
                timer += timeAmount;
                modelPosition.objectArrangement.Position = new Vector3(modelPosition.oldArrangement.Position.X,
                   modelPosition.oldArrangement.Position.Y + (verticalVelocity * timer - gravity * timer * timer / gravityDivider) / jumpHeightDivider,
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
        private void HorizontalJump()
        {

        }



        public void Update(List <Platform> platformList)
        {
        CheckPlayerOnPlatformStatus(platformList);
                switch (currentStance)
                {
                    case PlayerStances.IdleJump:
                        VerticalJump(2);
                        break;
                    case PlayerStances.JumpForward:
                        VerticalJump(4);
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
