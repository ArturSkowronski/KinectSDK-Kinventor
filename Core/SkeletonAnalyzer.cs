using Microsoft.Kinect;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Skowronski.Artur.Thesis
{
        public class SkeletonAnalyzer
        {
            private KinectSensor KinectDevice;
            private JointType _JointId1;
            private JointType _JointId2;
            private JointType _JointId3;
            private double s2D;
            private double s3D;
            private int _RotationOffset = 0;
            private bool _ReverseCoordinates = false;
            private double GridLayoutWidth;
            private double GridLayoutHeight;

            public SkeletonAnalyzer(KinectSensor KinectDevice, double rootH, double rootW)
            {
                this.KinectDevice = KinectDevice;
                this.GridLayoutWidth = rootW;
                this.GridLayoutHeight = rootH;
            }

            private Point GetJointPoint(Joint joint)
            {
                DepthImagePoint point = KinectDevice.MapSkeletonPointToDepth(joint.Position, KinectDevice.DepthStream.Format);
                point.X *= (int)GridLayoutWidth / KinectDevice.DepthStream.FrameWidth;
                point.Y *= (int)GridLayoutHeight / KinectDevice.DepthStream.FrameHeight;
                return new Point(point.X, point.Y);
            }      
            private int RotationOffset
            {
                get { return _RotationOffset; }
                set
                {
                    _RotationOffset = value % 360;
                }
            }
            private bool ReverseCoordinates
            {
                get { return _ReverseCoordinates; }
                set { _ReverseCoordinates = value; }
            }
            private double CalculateReverseCoordinates(double degrees)
            {
                return (-degrees + 180) % 360;
            }
          
            public void SetBodySegments(JointType jointId1, JointType jointId2, JointType jointId3)
            {
                _JointId1 = jointId1;
                _JointId2 = jointId2;
                _JointId3 = jointId3;
            }
            public void SetStabilization(int s2D, int s3D)
            {
                this.s2D = s2D;
                this.s3D = s3D;
            }
            
            public List<JointType> GetBodySegments()
            {
                List<JointType> list = new List<JointType>();
                list.Add(_JointId1);
                list.Add(_JointId2);
                list.Add(_JointId3);
                return list;
            }
            public double GetBodySegmentAngle2D(Skeleton skeletonData)
            {
            try
                {
                Point zeroPoint = GetJointPoint(skeletonData.Joints[_JointId2]);
                Point anglePoint = GetJointPoint(skeletonData.Joints[_JointId3]);
                Point x = new Point(zeroPoint.X + anglePoint.X, zeroPoint.Y);
                double a;
                double b;
                double c;
                a = Math.Sqrt(Math.Pow(zeroPoint.X - anglePoint.X, 2) +
                Math.Pow(zeroPoint.Y - anglePoint.Y, 2));
                b = anglePoint.X;
                c = Math.Sqrt(Math.Pow(anglePoint.X - x.X, 2) + Math.Pow(anglePoint.Y - x.Y, 2));
                double angleRad = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
                double angleDeg = angleRad * 180 / Math.PI;
                if (zeroPoint.Y < anglePoint.Y)
                {
                    angleDeg = 360 - angleDeg;
                }
                return angleDeg + s2D;

                }
 catch { return 0; }
                
            }         
            public double GetBodySegmentAngle3D(Skeleton skeletonData)
            {
                try
                {
                    Joint joint1 = skeletonData.Joints[_JointId1];
                    Joint joint2 = skeletonData.Joints[_JointId2];
                    Joint joint3 = skeletonData.Joints[_JointId3];

                    Vector3 vectorJoint1ToJoint2 = new Vector3(joint1.Position.X - joint2.Position.X, joint1.Position.Y - joint2.Position.Y, 0);
                    Vector3 vectorJoint2ToJoint3 = new Vector3(joint2.Position.X - joint3.Position.X, joint2.Position.Y - joint3.Position.Y, 0);
                    vectorJoint1ToJoint2.Normalize();
                    vectorJoint2ToJoint3.Normalize();

                    Vector3 crossProduct = Vector3.Cross(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
                    double crossProductLength = crossProduct.Z;
                    double dotProduct = Vector3.Dot(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
                    double segmentAngle = Math.Atan2(crossProductLength, dotProduct);

                    double degrees = segmentAngle * (180 / Math.PI);

                    degrees = (degrees + _RotationOffset) % 360;
                    if (_ReverseCoordinates)
                    {
                        degrees = CalculateReverseCoordinates(degrees);
                    }
                    return degrees + s3D;

                }
                catch { return 0; }
            }
        }
    }
