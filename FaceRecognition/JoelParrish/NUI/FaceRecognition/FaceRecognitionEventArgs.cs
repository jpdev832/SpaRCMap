using System;
using JoelParrish.NUI.FaceRecognition.utils;

namespace JoelParrish.NUI.FaceRecognition
{
    public class FaceRecognitionEventArgs :EventArgs
    {
        public FaceRestAPI.FaceAPI faceAPI;

        public FaceRecognitionEventArgs(FaceRestAPI.FaceAPI faceAPI)
        {
            this.faceAPI = faceAPI;
        }
    }
}
