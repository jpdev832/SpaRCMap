using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.Kinect.Nui;
using JoelParrish.NUI.FaceRecognition;

namespace JoelParrish.NUI.Kinect
{
    public class KinectController
    {
        public delegate void DepthImageHandler(object sender, BitmapSource e);
        public delegate void ColorImageHandler(object sender, BitmapSource e);
        public delegate void UserHandler(object sender, EventArgs e); //will change to custom args
        public delegate void SpeechRecognizeHandler(object sender, string recogText);

        public event DepthImageHandler DepthImageReady;
        public event ColorImageHandler ColorImageReady;
        public event UserHandler UserRemoved;
        public event UserHandler UserDetected;
        public event UserHandler UserRecognized;
        public event SpeechRecognizeHandler SpeechRecognized;

        public enum DeviceAngle
        {
            UP,
            DOWN
        };

        private static KinectController controller;
        private Camera camera;
        private UserManager userManager;

        internal Runtime nui;
        protected KinectOptions options;

        protected KinectController(KinectOptions options)
        {
            this.options = options;
            initialize();
        }

        public static KinectController getInstance()
        {
            if (controller == null)
                throw new Exception("KinectController must be initialized first. "+
                    "Use getInstance(KinectOptions option) to initialize a new instance");
            return controller;
        }

        public static KinectController getInstance(KinectOptions options)
        {
            if (controller == null)
                controller = new KinectController(options);

            return controller;
        }

        /// <summary>
        /// Unintialize
        /// </summary>
        public void uninitialize()
        {
            if (nui != null)
                nui.Uninitialize();

            if(options.recogEngine != null)
                

            nui = null;
        }
        
        private void initialize()
        {
            nui = Runtime.Kinects[0];

            nui.Initialize(options.getInitializeOptions());

            if(options.enableDepth && options.enableUserIndex)
                nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            else if(options.enableDepth)
                nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.Depth);
            if(options.enableVideo)
                nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);

            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            
            if (options.enableMotor)
                camera = nui.NuiCamera;

            if (options.enableSpeechRecog)
            {
                //speak = new KinectSpeak();
                //speak.SpeechRecognized += new KinectSpeak.SpeechRecognizeHandler(speak_SpeechRecognized);

                //Thread th = new Thread(new ThreadStart(speak.start));
                //th.IsBackground = true;
                //th.Start();
            }

            if (options.enableUserRecog)
            {
                if (options.recogEngine == null)
                    throw new Exception("No face recognition engine found");
                
                if (!typeof(IFaceRecognition).IsAssignableFrom(options.recogEngine.GetType()))
                    throw new Exception("Face recognition engine does not implement IFaceRecognition");

                userManager = new UserManager(options.recogEngine, new TimeSpan(0, 1, 0));
            }
        }

        #region static methods
        public static KinectOptions initialize(bool enableDepth, bool enableVideo, bool enableSpecchRecog, bool enableUserRecog)
        {
            return new KinectOptions()
            {
                enableDepth = enableDepth,
                enableVideo = enableVideo,
                enableSpeechRecog = enableSpecchRecog,
                enableUserRecog = enableUserRecog
            };
        }
        public static KinectOptions initialize(bool enableDepth, bool enableVideo, bool enableSpecchRecog, bool enableUserRecog, 
            bool enableTacking, bool enableHeadDetection, bool enableHandDetection, bool enableMotor)
        {
            return new KinectOptions()
            {
                enableDepth = enableDepth,
                enableVideo = enableVideo,
                enableSpeechRecog = enableSpecchRecog,
                enableUserRecog = enableUserRecog,
                enableTracking = enableTacking,
                enableHeadDetection = enableHeadDetection,
                enableHandDetection = enableHandDetection,
                enableMotor = enableMotor
            };
        }
        #endregion

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage image = e.ImageFrame.Image;

            byte[] frame = null;

            if (options.enableUserIndex)
                frame = convertDepthPlayerFrame(image.Bits);
            else
                frame = convertDepthFrame(image.Bits);

            BitmapSource bmp = BitmapSource.Create(image.Width, image.Height, 96, 96, 
                PixelFormats.Bgr32, null, frame, image.Width * 4);
            DepthImageReady(this, bmp);

            if (options.enableUserRecog && userManager.hasUsers)
                userManager.updateDepthFrame(e.ImageFrame);
        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage image = e.ImageFrame.Image;

            BitmapSource bmp = BitmapSource.Create(image.Width, image.Height, 96, 96, 
                PixelFormats.Bgr32, null, image.Bits, image.Width * 4);
            ColorImageReady(this, bmp);

            //for basic face recognition just analyze the color frames
            //NOTE: need to set a flag so detection only occurs when 
            //theres a hint a person is seen (skeleton) ***create a timeout {user manager can have a flag to notify of active usage}

            if (options.enableUserRecog && userManager.hasUsers)
                userManager.updateColorFrame(e.ImageFrame);
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (options.enableUserRecog)
            {
                userManager.updateSkeleton(e.SkeletonFrame);
            }
        }

        void speak_SpeechRecognized(object sender, string msgFound)
        {
            //possible use this the entry point for context differentiating
            /*
             * Note: If context differentiation is not maintained by this controller
             * then data may be lost among the various types of contextual entities
             */
            //SpeechRecognized(this, msgFound);
            //if(enableUserAnalysis) handle in user manager
        }

        public void changeAngle(DeviceAngle angle)
        {
            switch (angle)
            {
                case DeviceAngle.UP:
                    try
                    {
                        camera.ElevationAngle += 5;
                    }
                    catch (Exception e)
                    {
                    }
                    break;
                case DeviceAngle.DOWN:
                    try
                    {
                        camera.ElevationAngle -= 5;
                    }
                    catch (Exception e)
                    {
                    }
                    break;
            }
        }

        #region depth conversion
        /// <summary>
        /// Converts 16bit depth frame to a 32bit frame with color indexing
        /// </summary>
        /// <param name="depthFrame16"></param>
        /// <returns></returns>
        private byte[] convertDepthFrame(byte[] frame)
        {
            const int RED_IDX = 2;
            const int GREEN_IDX = 1;
            const int BLUE_IDX = 0;

            byte[] depthFrame32 = new byte[320 * 240 * 4];

            for (int i = 0, j = 0; i < frame.Length && j < frame.Length * 4;i+=2, j+=4)
            {
                int player = frame[i] & 7;
                int realDepth = frame[i] | (frame[i + 1] << 8);

                byte intensity = (byte)(255 - (255 * Math.Max(realDepth - 850, 0) / (4000 - 850)));

                depthFrame32[j + RED_IDX] = (byte)(intensity/2);
                depthFrame32[j + GREEN_IDX] = (byte)(intensity/2);
                depthFrame32[j + BLUE_IDX] = (byte)(intensity/2);
            }

            return depthFrame32;
        }

        /// <summary>
        /// This will be the actual player index depth. the previous is for testing and should change
        /// Converts 16bit depth frame to a 32bit frame with color indexing
        /// 
        /// 1-3 bits = player index
        /// 3-11 bits = depth <- check>
        /// 11-16 bits = 
        /// </summary>
        /// <param name="depthFrame16"></param>
        /// <returns></returns>
        private byte[] convertDepthPlayerFrame(byte[] frame)
        {
            const int RED_IDX = 2;
            const int GREEN_IDX = 1;
            const int BLUE_IDX = 0;

            byte[] depthFrame32 = new byte[320 * 240 * 4];

            const float MaxDepthDistance = 4000; // max value returned in mm
            const float MinDepthDistance = 850; // min value returned in mm
            const float MaxDepthDistanceOffset = MaxDepthDistance - MinDepthDistance;

            for (int i = 0, j = 0; i < frame.Length && j < frame.Length * 4; i += 2, j += 4)
            {
                int player = frame[i] & 7;
                int realDepth = (frame[i] >> 3) | (frame[i + 1] << 5);

                byte intensity = (byte)(255 - (255 * Math.Max(realDepth - MinDepthDistance, 0) / MaxDepthDistanceOffset));

                depthFrame32[j + RED_IDX] = 0;
                depthFrame32[j + GREEN_IDX] = 0;
                depthFrame32[j + BLUE_IDX] = 0;

                // choose different display colors based on player
                switch (player)
                {
                    case 0:
                        depthFrame32[j + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[j + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[j + BLUE_IDX] = (byte)(intensity / 2);
                        break;
                    case 1:
                        depthFrame32[j + RED_IDX] = intensity;
                        break;
                    case 2:
                        depthFrame32[j + GREEN_IDX] = intensity;
                        break;
                    case 3:
                        depthFrame32[j + RED_IDX] = (byte)(intensity / 4);
                        depthFrame32[j + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[j + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 4:
                        depthFrame32[j + RED_IDX] = (byte)(intensity);
                        depthFrame32[j + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[j + BLUE_IDX] = (byte)(intensity / 4);
                        break;
                    case 5:
                        depthFrame32[j + RED_IDX] = (byte)(intensity);
                        depthFrame32[j + GREEN_IDX] = (byte)(intensity / 4);
                        depthFrame32[j + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 6:
                        depthFrame32[j + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[j + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[j + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 7:
                        depthFrame32[j + RED_IDX] = (byte)(255 - intensity);
                        depthFrame32[j + GREEN_IDX] = (byte)(255 - intensity);
                        depthFrame32[j + BLUE_IDX] = (byte)(255 - intensity);
                        break;
                }
            }

            return depthFrame32;
        }
        #endregion
    }
}
