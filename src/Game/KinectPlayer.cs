using Microsoft.Kinect;


namespace Game
{
    class KinectPlayer
    {
        #region Public properties

        public Skeleton Skeleton { get; set; }
        public enum PlayerStances { None, JumpLeft, JumpRight, JumpUp }
        public PlayerStances CurrentStance { get; private set; }

        #endregion

        const double heightRequiredToJump = -0.25;


        public KinectPlayer()
        {
            CurrentStance = PlayerStances.None;
            Skeleton = new Skeleton();
        }

        public void Update()
        {
            CalculateStance();
        }



        private void CalculateStance()
        {
            // Brzydko toto wygląda, ale ciężko zrobić to ładniej
            if (Skeleton.Joints[JointType.KneeLeft].Position.Y > heightRequiredToJump)
            {
                // left leg raised. Check right leg.
                if (Skeleton.Joints[JointType.KneeRight].Position.Y > heightRequiredToJump)
                {
                    //both leg raised. Player is jumping.
                    CurrentStance = PlayerStances.JumpUp;
                    return;
                }
                // only left leg raised. Jump left!
                CurrentStance = PlayerStances.JumpLeft;
                return;
            }
            // now check the right leg
            if (Skeleton.Joints[JointType.KneeRight].Position.Y > heightRequiredToJump)
            {
                // right is raised and left is not. let's jump right.
                CurrentStance = PlayerStances.JumpRight;
                return;
            }
            // no leg raised. No stance.
            CurrentStance = PlayerStances.None;
        }
    }
}
