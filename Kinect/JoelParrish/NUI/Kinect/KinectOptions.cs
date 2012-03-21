using System;
using System.Collections.Generic;
using Microsoft.Research.Kinect.Nui;
using JoelParrish.NUI.FaceRecognition;

namespace JoelParrish.NUI.Kinect
{
    public class KinectOptions
    {
        public bool enableDepth { get; set; }
        public bool enableHandDetection { get; set; }
        public bool enableHeadDetection { get; set; }
        public bool enableMotor { get; set; }
        public bool enableUserRecog { get; set; }
        public bool enableSpeechRecog { get; set; }
        public bool enableTracking { get; set; }
        public bool enableVideo { get; set; }
        public bool enableUserIndex { get; set; }
        public bool enableUserAnalysis { get; set; }

        public IFaceRecognition recogEngine { get; set; }

        public KinectOptions()
        {
            enableDepth = false;
            enableHandDetection = false;
            enableHeadDetection = false;
            enableMotor = false;
            enableUserRecog = false;
            enableSpeechRecog = false;
            enableTracking = false;
            enableVideo = false;
            enableUserIndex = false;
            enableUserAnalysis = false;
            recogEngine = null;
        }

        public RuntimeOptions getInitializeOptions()
        {
            if (enableDepth && enableUserIndex && enableTracking && enableVideo)
                return RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor;
            if (enableDepth && enableUserIndex && enableTracking)
                return RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking;
            if (enableDepth && enableTracking && enableVideo)
                return RuntimeOptions.UseDepth | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor;
            if (enableDepth && enableTracking)
                return RuntimeOptions.UseDepth | RuntimeOptions.UseSkeletalTracking;
            if (enableVideo && enableTracking)
                return RuntimeOptions.UseColor| RuntimeOptions.UseSkeletalTracking;
            if (enableDepth)
                return RuntimeOptions.UseDepth;
            if (enableVideo)
                return RuntimeOptions.UseColor;

            return 0;
        }
    }
}
