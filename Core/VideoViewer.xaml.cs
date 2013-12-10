using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Kinect;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System;


namespace Skowronski.Artur.Thesis
{
    public partial class VideoViewer : UserControl
    { 

        #region Member Variables
        private const float FeetPerMeters = 3.2808399f;
        private readonly Brush[] _SkeletonBrushes = new Brush[] { Brushes.Black, Brushes.Crimson, Brushes.Indigo, Brushes.DodgerBlue, Brushes.Purple, Brushes.Pink };
        #endregion Member Variables
        KinectSensor myKinect;

        #region Constructor
        public VideoViewer()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myKinect = KinectSensor.KinectSensors[0];

            myKinect.ColorStream.Enable();

            myKinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(myKinect_ColorFrameReady);

            myKinect.Start();
        }
        #endregion Constructor

        void myKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null) return;

                byte[] colorData = new byte[colorFrame.PixelDataLength];

                colorFrame.CopyPixelDataTo(colorData);

                kinectVideo.Source = BitmapSource.Create(
                                    colorFrame.Width, colorFrame.Height, // image dimensions
                                    96, 96,  // resolution - 96 dpi for video frames
                                    PixelFormats.Bgr32, // video format
                                    null,               // palette - none
                                    colorData,          // video data
                                    colorFrame.Width * colorFrame.BytesPerPixel // stride
                                    );
            }
        }
        KinectSensor KinectDevice;
        double GridLayoutWidth;
        double GridLayoutHeight;

    
       
     

    }
}
