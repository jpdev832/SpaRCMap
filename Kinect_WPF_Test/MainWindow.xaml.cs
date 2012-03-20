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

namespace Kinect_WPF_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectController controller;

        public MainWindow()
        {
            InitializeComponent();
            controller = KinectController.getInstance();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            KinectOptions options = new KinectOptions()
            {
                enableDepth = true,
                enableTracking = true
            };

            controller.initialize(options);
            controller.DepthImageReady += new KinectController.DepthImageHandler(controller_DepthImageReady);
        }

        void controller_DepthImageReady(object sender, BitmapSource e)
        {
            depthImage.Source = e;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            controller.uninitialize();
        }
    }
}
