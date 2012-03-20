using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Research.Kinect.Nui;
using JoelParrish.NUI.FaceRecognition;

namespace JoelParrish.NUI.Kinect
{
    public class UserManager
    {
        public bool hasUsers { get; private set; }

        private Dictionary<int, User> users;
        private ImageFrame depthFrame;
        private ImageFrame colorFrame;
        private IFaceRecognition recogEngine;
        private TimeSpan timeoutThres;

        public UserManager(IFaceRecognition recogEngine, TimeSpan timeoutThres)
        {
            this.recogEngine = recogEngine;
            this.timeoutThres = timeoutThres;

            hasUsers = false;

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

        public void updateSkeleton(SkeletonFrame frame)
        {
            foreach (SkeletonData data in frame.Skeletons)
            {
                if (users.ContainsKey(data.TrackingID))
                {
                    User u;
                    if (!users.TryGetValue(data.TrackingID, out u))
                        throw new Exception("user data not found");

                    u.updateColorFrame(colorFrame);
                    u.updateDepthFrame(depthFrame);
                    u.updateSkeleton(data, frame.FrameNumber);
                }
                else
                {
                    User user = new User(data.TrackingID, recogEngine);
                    users.Add(data.TrackingID, user);

                    user.updateColorFrame(colorFrame);
                    user.updateDepthFrame(depthFrame);
                    user.updateSkeleton(data, frame.FrameNumber);
                }
            }

            List<int> rKeys = new List<int>();
            foreach (KeyValuePair<int, User> user in users)
            {
                if(DateTime.Now.Subtract(user.Value.lastUpdate) > timeoutThres){
                    rKeys.Add(user.Key);
                }
            }

            foreach (int key in rKeys)
            {
                users.Remove(key);
            }
        }
    }
}
