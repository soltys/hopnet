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




namespace Game
{
    class KinectPlayer
    {
        #region Public properties

        public enum PlayerStances { Idle = 1, JumpForwardUp = 2, JumpLeftDown = 3, JumpRightDown = 4, JumpLeftUp = 5, JumpRightUp = 6,JumpForwardDown = 7 }
        public PlayerStances currentStance { get; set; }
        private PlayerStances lastStance { get; set; }
        private Vector3 pos;
        public bool moveEnabled;
        private Hero modelPosition;
        private Model model;

        private float modelGroundLevel;
        private float hPlatformSpace;
        private float dPlatformSpace;


        private float spaceRequiredToSideJump = 0.2f;
        private float spaceRequiredToJump = 0.1f;
        private float spaceRequiredToResetHand = -0.55f;

        private float defaultShoulderHeight = 0.7f;
        private float heightModifier = 0f;
        private float defaultFootHeight = -0.8f;

        public KinectPlayer(ContentManager content, Vector3 platformData)
        {
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
                if (moveEnabled)
                {
                    if ((skeleton.Joints[JointType.FootLeft].Position.Y > defaultFootHeight + heightModifier + spaceRequiredToJump) & (skeleton.Joints[JointType.FootRight].Position.Y > defaultFootHeight + heightModifier + spaceRequiredToJump))
                    {
                            lastStance = currentStance;
                            currentStance = PlayerStances.JumpForwardUp;
                    }
                }
        }

        private void CheckJumpLeftUp(Skeleton skeleton)
        {
            if (moveEnabled)
            {
                double vDistance = skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ShoulderCenter].Position.Y;
                if (vDistance > (spaceRequiredToSideJump + heightModifier))
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
                double vDistance = skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderCenter].Position.Y;


                if (vDistance > (spaceRequiredToSideJump + heightModifier))
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
                double vDistance = skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.ShoulderCenter].Position.Y;
                if (vDistance < (spaceRequiredToResetHand + heightModifier))
                {
                    lastStance = currentStance;
                    currentStance = PlayerStances.Idle;
                }
        }
        private void CheckJumpRightDown(Skeleton skeleton)
        {
                double vDistance = skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.ShoulderCenter].Position.Y;
                if (vDistance < (spaceRequiredToResetHand + heightModifier))
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
                    CheckJumpLeftDown(skeleton);
                    break;
                case PlayerStances.JumpRightUp:
                    CheckJumpRightDown(skeleton);
                    break;
                case PlayerStances.JumpForwardUp:
                    break;
            }
        }


        private double jumpTime = 0;

        public void Update(double dTimeBetweenPlatforms, double platformsSpeed, double gravity)
        {
            switch (currentStance)
            {
                case PlayerStances.JumpForwardUp:
                    if (modelPosition.jumpReady)
                    {
                        modelPosition.MoveUp(dTimeBetweenPlatforms, modelGroundLevel, platformsSpeed, gravity);
                    }
                    else
                    {
                        lastStance = currentStance;
                        currentStance = PlayerStances.Idle;
                    }


                    break;
            }
        }





        public void Update(Skeleton skeleton)
        {
            if (skeleton != null)
            {

                if (skeleton.Joints[JointType.ShoulderCenter].Position.Z < 2.1f)
                {
                    spaceRequiredToSideJump = -0.1f;
                    spaceRequiredToResetHand = -0.4f;
                }
                else
                {
                    spaceRequiredToSideJump = -0.15f;
                    spaceRequiredToResetHand = -0.55f;
                }


                heightModifier = defaultShoulderHeight - skeleton.Joints[JointType.ShoulderCenter].Position.Y;
                pos.Y = skeleton.Joints[JointType.FootLeft].Position.Y;
                pos.Z = skeleton.Joints[JointType.FootRight].Position.Y;
                CheckPlayerStance(skeleton);
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font,float aspectRatio, Vector3 cameraPosition)
        {
            modelPosition.Draw(aspectRatio, cameraPosition, model);
            spriteBatch.Begin();
            
            /*
            spriteBatch.DrawString(font, (pos.X * shoulderDepthMultiplier).ToString(), new Vector2(400, 80), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, (pos.Y * lHandDepthMultiplier).ToString(), new Vector2(400, 160), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, (pos.Z * rHandDepthMultiplier).ToString(), new Vector2(400, 240), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            */

            spriteBatch.DrawString(font, pos.X.ToString(), new Vector2(400, 80), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, pos.Y.ToString(), new Vector2(400, 160), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, pos.Z.ToString(), new Vector2(400, 240), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, currentStance.ToString(), new Vector2(400, 320), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, (spaceRequiredToJump + heightModifier + defaultFootHeight).ToString(), new Vector2(400, 400), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            //spriteBatch.DrawString(font, tmp.ToString(), new Vector2(400, 480), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            //spriteBatch.DrawString(font, spaceRequiredToSideJump.ToString(), new Vector2(400, 560), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            /*
            spriteBatch.DrawString(font, lHandDepthMultiplier.ToString(), new Vector2(400, 320), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, rHandDepthMultiplier.ToString(), new Vector2(400, 400), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, shoulderDepthMultiplier.ToString(), new Vector2(400, 480), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
             */
            spriteBatch.End();
             
        }



    }
}
