using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Kinect;
using System.Diagnostics;


namespace Skowronski.Artur.Thesis
{
    public partial class DepthViewer : UserControl
    { 

        #region Member Variables
        private const float FeetPerMeters = 3.2808399f;
        private readonly Brush[] _SkeletonBrushes = new Brush[] { Brushes.Black, Brushes.Crimson, Brushes.Indigo, Brushes.DodgerBlue, Brushes.Purple, Brushes.Pink };
        #endregion Member Variables
        
        #region Constructor
        public DepthViewer()
        {
            InitializeComponent();
        }
        #endregion Constructor

    

    }
}
