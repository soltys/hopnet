using System;
using Microsoft.Kinect;


namespace Game
{
    class KinectPlayer
    {
        #region Public properties

        public Skeleton playerSkeleton;
        public enum PlayerStances { None, JumpLeft, JumpRight, JumpUp }
        public PlayerStances CurrentStance { get; private set; }
        public double HeadToRightPalmDistance
        {
            get
            {
                return Math.Abs(playerSkeleton.Joints[JointType.Head].Position.Z - playerSkeleton.Joints[JointType.HandRight].Position.Z);
            }
        }

        #endregion

        const double heightRequiredToJump = -0.25;


        public KinectPlayer()
        {
            CurrentStance = PlayerStances.None;
            playerSkeleton = new Skeleton();
        }

        public void SetSkeleton(Skeleton PlayerSkeleton)
        {
            this.playerSkeleton = PlayerSkeleton;
        }

        public void Update()
        {
            CalculateStance();
        }



        private void CalculateStance()
        {
            // Brzydko toto wygląda, ale ciężko zrobić to ładniej
            if (playerSkeleton.Joints[JointType.KneeLeft].Position.Y > heightRequiredToJump)
            {
                // left leg raised. Check right leg.
                if (playerSkeleton.Joints[JointType.KneeRight].Position.Y > heightRequiredToJump)
                {
                    //both leg raised. Player is jumping.
                    this.CurrentStance = PlayerStances.JumpUp;
                    return;
                }
                // only left leg raised. Jump left!
                this.CurrentStance = PlayerStances.JumpLeft;
                return;
            }
            // now check the right leg
            else if (playerSkeleton.Joints[JointType.KneeRight].Position.Y > heightRequiredToJump)
            {
                // right is raised and left is not. let's jump right.
                this.CurrentStance = PlayerStances.JumpRight;
                return;
            }
            else
            {
                // no leg raised. No stance.
                CurrentStance = PlayerStances.None;
                return;
            }
        }

    }
}
