using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JoelParrish.NUI.Kinect;
using JoelParrish.NUI.FaceRecognition;
using System.Diagnostics;
using System.IO;

namespace SpaRCMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                User u = new User(1, new FaceRestAPIWrapper("deb6c07703726c639acab868020dc8af",
                    "3a671faa0cc9102fb613d5eef1aa80a7", "jparrish", null));

                FileStream f = new FileStream(@"C:\Users\Joel\Pictures\kinect\kinectUser.jpg", FileMode.Open);
                JpegBitmapDecoder decoder = new JpegBitmapDecoder(f, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource source = decoder.Frames[0];

                Debug.WriteLine("Test bitmapSource created");

                image1.Source = source;
                u.faceImage = source;
                Debug.WriteLine("faceImage => test image");
                u.recognize();
                Debug.WriteLine("Recognize complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
