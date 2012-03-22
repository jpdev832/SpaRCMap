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
        protected int skeletonFrame;

        public BitmapSource faceImage;
        public int uid;
        public string id;
        public string name;

        public bool recognized = false;
        public DateTime lastUpdate { get; private set; }
        public DateTime lastRecogUpdate { get; private set; }

        private static IFaceRecognition faceRecog;
        private int recogTryLimit = 10;
        private int recogAttempts = 0;

        public User(int uid, IFaceRecognition faceRecognizer)
        {
            this.uid = uid;
            this.id = "tmp_jhhgfd"; //should be initialized with temp id

            if (faceRecognizer == null)
                throw new Exception("Could not initialize face recognition! No recognizer supplied");

            faceRecog = faceRecognizer.getNewInstance(id);
            faceRecog.detected += new EventHandler(faceRecog_detected);
            faceRecog.recognized += new EventHandler(faceRecog_recognized);

            lastUpdate = DateTime.Now;
            lastRecogUpdate = DateTime.Now.Subtract(new TimeSpan(0, 0, 10));    //set 10 sec behind current time
            Debug.WriteLine("New user create | tempId => " + id); 
        }

        ~User()
        {
            faceRecog.cleanup();
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
                    Debug.WriteLine("User Detected | recognition tempId => " + tempId);
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
                    if (fe.faceAPI.photos[0].tags[0].recognizable)
                    {
                        //label should be used to stored id
                        id = fe.faceAPI.photos[0].tags[0].uids[0].uid;
                        Debug.WriteLine("recognized: label => " + id);
                        Debug.WriteLine(id + " | confidence => " + fe.faceAPI.photos[0].tags[0].uids[0].confidence);
                        recognized = true;
                    }
                }
            }

            if (!recognized)
            {
                Debug.WriteLine("User not recognized");
                faceRecog.detect(faceImage, null);
            }
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

            if (!recognized && recogAttempts <= recogTryLimit &&
                lastRecogUpdate > DateTime.Now.Subtract(new TimeSpan(0,0,10)))
            {
                lastRecogUpdate = DateTime.Now;

                //can create new thread here
                recognize();
            }

            contextAnalysis();
            lastUpdate = DateTime.Now;
        }

        public void recognize()
        {
            //check skeleton position to see if user is facing camera. if position is
            //close then extract face and recognize
            extractFace();

            //if(closePosition)
            lock (faceImage)
            {
                faceRecog.recognize(faceImage, null);
            }
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
            faceImage = BitmapSource.Create(z, z, 96, 96, System.Windows.Media.PixelFormats.Bgr32, null,
                faceBmp, z * 4);
        }
    }
}
