using System;
using Microsoft.Kinect;
using System.Diagnostics;

namespace Game
{
    public class KinectData
    {
        private readonly bool isKinectConnected = true;
        private readonly Stopwatch heightChangeStopWatch;
        private float currentPersonHeight;
        private float lastPersonHeight = 100.0f;
        private float personIdleHeight;

        #region public properties and accessors
        public bool IsKinectConnected
        {
            get { return isKinectConnected; }
        }

        public Skeleton Skeleton { get; set; }
        public Skeleton[] SkeletonData { get; set; }
        public KinectSensor KinectSensor { get; set; }

        public float PersonIdleHeight
        {
            get { return personIdleHeight; }   
        }

        #endregion

        #region constructor

        public KinectData()
        {
            heightChangeStopWatch = new Stopwatch();
            heightChangeStopWatch.Reset();

            try
            {
                KinectSensor = KinectSensor.KinectSensors[0];
            }
            catch (Exception)
            {
                isKinectConnected = false;
            }
        }

        #endregion

        public void CalculatePersonShoulderHeight()
        {
            if (Skeleton == null){return;}

            if (!heightChangeStopWatch.IsRunning)
            {
                lastPersonHeight = currentPersonHeight;
            }
            currentPersonHeight = Skeleton.Joints[JointType.ShoulderCenter].Position.Y;

            if (Math.Abs(lastPersonHeight - currentPersonHeight) > GameConstants.HeightChangeThreshold)
            {
                if (!heightChangeStopWatch.IsRunning)
                {
                    heightChangeStopWatch.Start();
                }
                else
                {
                    if (heightChangeStopWatch.Elapsed.TotalMilliseconds > GameConstants.HeightChangeTimeMiliseconds)
                    {
                        personIdleHeight = currentPersonHeight;
                        heightChangeStopWatch.Reset();
                    }
                }
            }
        }
    }
}
