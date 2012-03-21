using System;
using System.Windows.Media.Imaging;

namespace JoelParrish.NUI.FaceRecognition
{
    public interface IFaceRecognition
    {
        //Create events so that detection/recognition can run async
        //remember to marshal when handling async results due to multiple threads
        event EventHandler detected;
        event EventHandler recognized;

        /// <summary>
        /// Allows implementing class to generate new instances of itself 
        /// through IFaceRecognition.
        /// </summary>
        /// <returns>New IFaceRecognition Instance</returns>
        IFaceRecognition getNewInstance();

        void detect(BitmapSource bmp, object extra);
        void recognize(BitmapSource bmp, object extra);
    }
}
