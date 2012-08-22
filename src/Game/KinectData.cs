using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Game
{
    public class KinectData
    {
        private KinectSensor kinectSensor;
        private Skeleton[] skeletonData;
        private Skeleton skeleton;
        private readonly bool isKinectConnected = true;

        #region public properties and accessors
        public bool IsKinectConnected
        {
            get { return isKinectConnected; }
        }
        public Skeleton Skeleton
        {
            get { return skeleton; }
        }
        public Skeleton[] SkeletonData
        {
            get { return skeletonData; }
        }
        public KinectSensor KinectSensor
        {
            get { return kinectSensor; }
            set { kinectSensor = value; }
        }
        #endregion

        #region constructor

        public KinectData()
        {
            try
            {
                kinectSensor = KinectSensor.KinectSensors[0];
                kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(KinectSkeletonFrameReady);
                kinectSensor.SkeletonStream.Enable();
                kinectSensor.Start();
            }
            catch (Exception e)
            {
                isKinectConnected = false;
            }
        }

        #endregion

        private void KinectSkeletonFrameReady(object sender, AllFramesReadyEventArgs imageFrames)
        {
            using (SkeletonFrame skeletonFrame = imageFrames.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                }
            }

            if (skeletonData != null)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skeleton = skel;
                    }
                }
            }

        }

        
    }
}
