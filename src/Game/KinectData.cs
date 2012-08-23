using System;
using Microsoft.Kinect;
using System.Diagnostics;

namespace Game
{
    public class KinectData
    {
        private KinectSensor kinectSensor;
        private Skeleton[] skeletonData;
        private Skeleton skeleton;
        private readonly bool isKinectConnected = true;
        private Stopwatch heightChangeStopWatch;
        private float currentPersonHeight;
        private float lastPersonHeight = 100.0f;
        private float personIdleHeight = 0.0f;

        #region public properties and accessors
        public bool IsKinectConnected
        {
            get { return isKinectConnected; }
        }
        public Skeleton Skeleton
        {
            get { return skeleton; }
            set { skeleton = value; }
        }
        public Skeleton[] SkeletonData
        {
            get { return skeletonData; }
            set { skeletonData = value; }
        }
        public KinectSensor KinectSensor
        {
            get { return kinectSensor; }
            set { kinectSensor = value; }
        }
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

            if (KinectSensor.KinectSensors.Count>0)
            {
                kinectSensor = KinectSensor.KinectSensors[0];
            }
            else
            {
                isKinectConnected = false;
            }
        }

        #endregion

        public void CalculatePersonShoulderHeight()
        {
            if (skeleton != null)
            {
                if (!heightChangeStopWatch.IsRunning)
                {
                    lastPersonHeight = currentPersonHeight;
                }
                currentPersonHeight = skeleton.Joints[JointType.ShoulderCenter].Position.Y;

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
}
