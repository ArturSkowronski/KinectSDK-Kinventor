using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Kinect;
using System.Diagnostics;


namespace Skowronski.Artur.Thesis
{
    public partial class SkeletonViewer : UserControl
    { 

        #region Member Variables
        private const float FeetPerMeters = 3.2808399f;
        private readonly Brush[] _SkeletonBrushes = new Brush[] { Brushes.Black, Brushes.Crimson, Brushes.Indigo, Brushes.DodgerBlue, Brushes.Purple, Brushes.Pink };
        #endregion Member Variables
        
        #region Constructor
        public SkeletonViewer()
        {
            InitializeComponent();
        }
        #endregion Constructor

        public void updateSkeletonDraw(Skeleton skeleton) {
            if (skeleton != null)
            {
                SkeletonsPanel.Children.Clear();
                JointInfoPanel.Children.Clear();

                var brush = Brushes.Crimson;
                DrawSkeleton(skeleton, brush);
                TrackJoint(skeleton.Joints[JointType.HandLeft], brush);
                TrackJoint(skeleton.Joints[JointType.HandRight], brush);
                TrackJoint(skeleton.Joints[JointType.ElbowLeft], brush);
                TrackJoint(skeleton.Joints[JointType.ElbowRight], brush);
                TrackJoint(skeleton.Joints[JointType.ShoulderCenter], brush);
                TrackJoint(skeleton.Joints[JointType.ShoulderLeft], brush);
                TrackJoint(skeleton.Joints[JointType.ShoulderRight], brush);
                TrackJoint(skeleton.Joints[JointType.WristLeft], brush);
                TrackJoint(skeleton.Joints[JointType.WristRight], brush);
            }
            }
        

        #region Methods
        KinectSensor KinectDevice;
        double GridLayoutWidth;
        double GridLayoutHeight;

        public void SkeletonViewerConstruct(KinectSensor KinectDevice, double rootH, double rootW)
            {
                this.KinectDevice = KinectDevice;
                this.GridLayoutWidth = rootW;
                this.GridLayoutHeight = rootH;
            }


        private void DrawSkeleton(Skeleton skeleton, Brush brush)
        {
            if(skeleton != null)
            {            

                Polyline figure = CreateFigure(skeleton, brush, new [] {  JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.Spine, JointType.ShoulderRight, JointType.ShoulderCenter, JointType.HipCenter});              
                SkeletonsPanel.Children.Add(figure);
                figure = CreateFigure(skeleton, brush, new [] { JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft });
                SkeletonsPanel.Children.Add(figure);
                figure = CreateFigure(skeleton, brush, new [] { JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight });
                SkeletonsPanel.Children.Add(figure);

            }

        }

        public Skeleton skeletonElement;

        private Polyline CreateFigure(Skeleton skeleton, Brush brush, JointType[] joints)
        {
            Polyline figure = new Polyline();

            figure.StrokeThickness  = 6;
            figure.Stroke           = brush;

            for(int i = 0; i < joints.Length; i++)
            {                
                figure.Points.Add(GetJointPoint(skeleton.Joints[joints[i]]));
            }

            return figure;
        }


                                                                                  
        private Point GetJointPoint(Joint joint)
        {
            DepthImagePoint point = default(DepthImagePoint);
            if (KinectDevice != null)
            {
                point = this.KinectDevice.MapSkeletonPointToDepth(joint.Position, this.KinectDevice.DepthStream.Format);
                point.X *= (int)this.LayoutRoot.ActualWidth / KinectDevice.DepthStream.FrameWidth;
                point.Y *= (int)this.LayoutRoot.ActualHeight / KinectDevice.DepthStream.FrameHeight;
            }
            return new Point(point.X, point.Y);
        }
        
        
        private void TrackJoint(Joint joint, Brush brush)
        {
            if(joint.TrackingState != JointTrackingState.NotTracked)
            {
                Canvas container = new Canvas();
                Point jointPoint = GetJointPoint(joint);

                //FeetPerMeters is a class constant of 3.2808399f;
                double z = joint.Position.Z * FeetPerMeters;

                Ellipse element = new Ellipse();
                element.Height  = 15;
                element.Width   = 15;
                element.Fill    = brush;
                Canvas.SetLeft(element, 0 - (element.Width / 2));
                Canvas.SetTop(element, 0 - (element.Height / 2));
                container.Children.Add(element);

                TextBlock positionText  = new TextBlock();
                positionText.Text       = string.Format("<{0:0.00}, {1:0.00}, {2:0.00}>", jointPoint.X, jointPoint.Y, z);                
                positionText.Foreground = brush;
                positionText.FontSize   = 24;
                positionText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(positionText, 35);
                Canvas.SetTop(positionText, 15);
                container.Children.Add(positionText);

                Canvas.SetLeft(container, jointPoint.X);
                Canvas.SetTop(container, jointPoint.Y);
                
                JointInfoPanel.Children.Add(container);               
            }    
        }                                              
        #endregion Methods
          

    }
}
