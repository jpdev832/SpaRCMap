using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Research.Kinect.Nui;
using JoelParrish.NUI.FaceRecognition;

namespace JoelParrish.NUI.Kinect
{
    public class UserManager
    {
        private Dictionary<int, User> users;
        private ImageFrame depthFrame;
        private ImageFrame colorFrame;
        private int thres_skele_timeout;

        private IFaceRecognition recogEngine;

        public UserManager(IFaceRecognition recogEngine)
        {
            this.recogEngine = recogEngine;

            users = new Dictionary<int, User>();
        }

        public void updateDepthFrame(ImageFrame iFrame)
        {
            depthFrame = iFrame;
        }

        public void updateColorFrame(ImageFrame iFrame)
        {
            colorFrame = iFrame;
        }

        public void updateSkeleton(SkeletonData data, int frameNumber)
        {
            if (!users.ContainsKey(data.TrackingID))
            {
                User user = new User(data.TrackingID, recogEngine);
                users.Add(data.TrackingID, user);
            }

            /**
             * Check for all users that have passed the threshold
             */

            if (analyzeFrame(frameNumber))
            {
                User u;

                if (!users.TryGetValue(data.TrackingID, out u))
                    throw new Exception("user data not found");

                //update user with data
                //users only need update depth, color, and skeleton data when it pertains to them
            }
        }

        public bool analyzeFrame(int frameNumber)
        {
            if (frameNumber == depthFrame.FrameNumber && frameNumber == colorFrame.FrameNumber)
                return true;

            return false;
        }
    }
}
