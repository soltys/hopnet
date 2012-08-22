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
        private float personHeight;
        private float lastPersonHeight = 100.0f;

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
        public float PlayerHeight
        {
            get { return personHeight; }   
        }

        #endregion

        #region constructor

        public KinectData()
        {
            heightChangeStopWatch = new Stopwatch();
            heightChangeStopWatch.Reset();
            try
            {
                kinectSensor = KinectSensor.KinectSensors[0];
            }
            catch (Exception e)
            {
                isKinectConnected = false;
            }
        }

        #endregion



        private void CalculatePersonHeight()
        {
            
        }

    }
}
