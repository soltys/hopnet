using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Game
{
    public class KinectData
    {
        public KinectSensor kinectSensor = null;
        private Skeleton[] skeletonData = null;
        private Skeleton skeleton = null;

        public Skeleton Skeleton
        {
            get { return skeleton; }
        }

        public KinectData()
        {
            try
            {
                kinectSensor = KinectSensor.KinectSensors[0];
                kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinectSkeletonFrameReady);
                kinectSensor.SkeletonStream.Enable();
                kinectSensor.Start();
            }
            catch (Exception e)
            {
                throw new ArgumentException();
            }
        }

        private void kinectSkeletonFrameReady(object sender, AllFramesReadyEventArgs imageFrames)
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
