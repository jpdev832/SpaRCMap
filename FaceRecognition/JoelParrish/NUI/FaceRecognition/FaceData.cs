using System;
using System.Drawing;
using System.Windows.Media.Imaging;
//using Emgu.CV;
//using Emgu.CV.Structure;
//using Emgu.CV.CvEnum;

namespace JoelParrish.NUI.FaceRecognition
{
    public class FaceData
    {
        //public Image<Gray, Byte> grayImage;
        //public Image<Gray, Byte>[] faceImage;
        public BitmapSource grayImageSource;
        public BitmapSource faceImageSource;
        public Rectangle[] faceRect;
        public string[] id;

        /*public BitmapSource getGrayImage()
        {
            if (grayImage == null)
                throw new ArgumentNullException("No gray Image found");

            Image<Bgr, Byte> image = grayImage.Convert<Bgr, Byte>();

            return BitmapSource.Create(image.Width, image.Height, 96, 96,
                System.Windows.Media.PixelFormats.Bgr24, null, image.Bytes, image.Width * 3);
        }*/
    }
}
