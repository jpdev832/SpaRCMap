using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Research.Kinect.Nui;
using JoelParrish.NUI.FaceRecognition;
using System.Diagnostics;

namespace JoelParrish.NUI.Kinect
{
    /// <summary>
    /// The user class will be used to identify user, track location space, extract features, 
    /// build a contextual relationships based on space and identity.
    /// </summary>
    public class User
    {
        protected SkeletonData skeletonData;
        protected ImageFrame colorFrame;
        protected ImageFrame depthFrame;

        public int uid;
        public BitmapSource faceImage;

        protected int skeletonFrame;
        public string id;
        public string name;
        public BitmapSource face;

        public bool recognized = false;
        public DateTime lastUpdate { get; private set; }

        private static IFaceRecognition faceRecog;

        public User(int uid, IFaceRecognition faceRecognizer)
        {
            this.uid = uid;
            this.id = "tmp_jhhgfd"; //should be initialized with temp id

            if (faceRecog == null)
                throw new Exception("Could not initialize face recognition! No recognizer supplied");

            faceRecog = faceRecognizer.getNewInstance();
            faceRecog.detected += new EventHandler(faceRecog_detected);
            faceRecog.recognized += new EventHandler(faceRecog_recognized);

            lastUpdate = DateTime.Now;
        }

        void faceRecog_detected(object sender, EventArgs e)
        {
            FaceRecognitionEventArgs fe = (FaceRecognitionEventArgs)e;

            if (fe.faceAPI.photos.Count > 0)
            {
                if (fe.faceAPI.photos[0].tags.Count > 0)
                {
                    string tempId = fe.faceAPI.photos[0].tags[0].tid;

                    //save image location and tempId to database for later verification
                    //also change eventargs to include tempImage location
                }
            }
        }

        void faceRecog_recognized(object sender, EventArgs e)
        {
            FaceRecognitionEventArgs fe = (FaceRecognitionEventArgs)e;

            if (fe.faceAPI.photos.Count > 0)
            {
                if (fe.faceAPI.photos[0].tags.Count > 0)
                {
                    if (fe.faceAPI.photos[0].tags[0].confirmed)
                    {
                        //label should be used to stored id
                        id = fe.faceAPI.photos[0].tags[0].label;
                        Debug.WriteLine("recognized: label => " + id);

                        recognized = true;
                    }
                }
            }

            if(!recognized)
                faceRecog.detect(faceImage, null);
        }

        public void updateColorFrame(ImageFrame frame)
        {
            colorFrame = frame;
        }

        public void updateDepthFrame(ImageFrame frame)
        {
            depthFrame = frame;
        }

        public void updateSkeleton(SkeletonData data, int frameNumber)
        {
            skeletonData = data;

            if (!recognized)
            {
                recognize();
            }

            contextAnalysis();
            lastUpdate = DateTime.Now;
        }

        protected void recognize()
        {
            //check skeleton position to see if user is facing camera. if position is
            //close then extract face and recognize

            //if(closePosition)
            faceRecog.recognize(faceImage, null);
        }

        protected void contextAnalysis()
        {
            //need to have base location data integrate into system that can be accessed globally
            //Construct a context entity
            //create thresholds to determine if an update should be performed

            //compare spatial, identification, and database data
            //create a contextual interface
        }

        protected void extractFace()
        {
            float x, y;

            KinectController.getInstance().nui.SkeletonEngine.SkeletonToDepthImage(
                skeletonData.Joints[JointID.Head].Position, out x, out y);
            x = Math.Max(0, Math.Min(x * 640, 640));
            y = Math.Max(0, Math.Min(y * 480, 480));
            int z = (int)((4017 - (skeletonData.Joints[JointID.Head].Position.Z * 1000)) / 24);

            byte[] faceBmp = new byte[z * z];

            BitmapSource.Create(colorFrame.Image.Width, colorFrame.Image.Height, 96, 96,
                System.Windows.Media.PixelFormats.Bgr32, null, colorFrame.Image.Bits,
                colorFrame.Image.Width * 4).CopyPixels(new Int32Rect((int)(x - (z / 2)), 
                    (int)y, z, z), faceBmp, z * 4, 0);

            //need to check settings here
            face = BitmapSource.Create(z, z, 96, 96, System.Windows.Media.PixelFormats.Bgr32, null,
                faceBmp, z * 4);
        }
    }
}
