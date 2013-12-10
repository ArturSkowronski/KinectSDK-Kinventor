using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Kinect;
using Nui=Microsoft.Kinect;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using SharpDX;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;


namespace Skowronski.Artur.Thesis
{
 
    public partial class MainWindow : Window
    {
        #region Member Variables
        
        private KinectSensor _KinectDevice;
        private Skeleton[] _FrameSkeletons;
        private AbstractController _DeviceControl;
        private SkeletonAnalyzerContainer _SkeletonAnalyzerContainer;
        private Skeleton skeletonData;
        private Dictionary<string, TextBlock> listTextBlock=new Dictionary<string,TextBlock>();
        private Dictionary<string, Slider> listSlider = new Dictionary<string, Slider>();
        private bool isActive = false;
        private bool mode3D = false;
        bool sourceParameterBool = true;
        private bool threadTest = true;
        private bool toogleSource = false;

        #endregion Member Variables
    
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            _SkeletonAnalyzerContainer = new SkeletonAnalyzerContainer();
            this.Loaded += (s,e) =>
            {
                KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
                this.KinectDevice = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            };
         }

        #endregion Constructor
        #region Methods
        public void createAnalyzerForParameter(string parameterName, List<JointType> bodySegments, int s2D, int s3D)
        {
            SkeletonAnalyzer analyzer = new SkeletonAnalyzer(_KinectDevice, this.LayoutRoot.ActualHeight, this.LayoutRoot.ActualWidth);
            analyzer.SetStabilization(s2D, s3D);
            analyzer.SetBodySegments(bodySegments[0], bodySegments[1], bodySegments[2]);
            _SkeletonAnalyzerContainer.AddParameter(parameterName, analyzer);
            updateParametersEditor();
            updateManualSlider();
        }
        public void createAnalyzerForConstraints(string parameterName, List<JointType> bodySegments, int s2D, int s3D)
        {
            SkeletonAnalyzer analyzer = new SkeletonAnalyzer(_KinectDevice, this.LayoutRoot.ActualHeight, this.LayoutRoot.ActualWidth);
            analyzer.SetStabilization(s2D, s3D);
            analyzer.SetBodySegments(bodySegments[0], bodySegments[1], bodySegments[2]);
            _SkeletonAnalyzerContainer.AddConstraints(parameterName, analyzer);
            updateParametersEditor();
            updateManualSlider();
        }
        private void KinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {      

            using(SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if(frame != null)
                {
                    frame.CopySkeletonDataTo(this._FrameSkeletons);
                    skeletonData = GetPrimarySkeleton(this._FrameSkeletons);
                    if (skeletonData != null)
                    {
                        SkeletonViewerElement.updateSkeletonDraw(skeletonData);
                        UpdateData();
                       
                    }
                 }                
            }
        }

        private void UpdateData()
        {

            Dictionary<string, SkeletonAnalyzer> listS = _SkeletonAnalyzerContainer.getParameterList();
            Dictionary<string, SkeletonAnalyzer> listS2 = _SkeletonAnalyzerContainer.getConstraintList();

            if (threadTest && toogleSource && isActive)
            {
                Thread worker = new Thread(() =>
                {
                    threadTest = false;
                    if (sourceParameterBool)
                    {
                        if (mode3D)
                        {
                            foreach (KeyValuePair<string, SkeletonAnalyzer> skeleton in listS)
                            {
                                _DeviceControl.updateAngleByParameter(skeleton.Key, (int)skeleton.Value.GetBodySegmentAngle3D(skeletonData));
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, SkeletonAnalyzer> constrainBySkeleton in listS)
                            {
                                _DeviceControl.updateAngleByParameter(constrainBySkeleton.Key, (int)constrainBySkeleton.Value.GetBodySegmentAngle2D(skeletonData));
                            }
                        }
                    }
                    else
                    {
                        alreadyChanged = true;
                        if (mode3D)
                        {
                            foreach (KeyValuePair<string, SkeletonAnalyzer> skeleton in listS2)
                            {
                                _DeviceControl.updateAngleByConstraints(skeleton.Key, -(int)skeleton.Value.GetBodySegmentAngle3D(skeletonData));
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, SkeletonAnalyzer> constrainBySkeleton in listS2)
                            {
                                _DeviceControl.updateAngleByConstraints(constrainBySkeleton.Key, (int)constrainBySkeleton.Value.GetBodySegmentAngle2D(skeletonData));
                            }
                        }
                    }

                    threadTest = true;
                });

                worker.Start();
            }
            UpdateParams();
        }

        private void updateTreeView()
        {
            TreeViewConstraints.Items.Clear();
            foreach (TreeViewItem tv in _DeviceControl.createTreeViewByOccurences())
            {
                TreeViewConstraints.Items.Add(tv);
            }
        }

        void selectedEvent_ItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }
        private void updateManualSlider() {
            ManualControl.Children.Clear();
            
            foreach (KeyValuePair<string, SkeletonAnalyzer> skeleton in _SkeletonAnalyzerContainer.getParameterList())
            {
                string parameter = skeleton.Key;
                TextBlock TextBlock1 = new TextBlock();
                TextBlock1.FontSize = 18;
                TextBlock1.Text = "Parametr: " + parameter;
                TextBlock1.Margin = new System.Windows.Thickness(10);
                Slider Slider1 = new Slider();
                Slider1.Minimum = 0;
                Slider1.Maximum = 180;
                Slider1.Name = parameter;
                Slider1.TickFrequency = 1;
                Slider1.ValueChanged += Slider_DragCompleted;
                ScaleTransform sc = new ScaleTransform();
                sc.ScaleY = 1.5;
                Slider1.LayoutTransform = sc;
                ManualControl.Children.Add(TextBlock1);
                ManualControl.Children.Add(Slider1);
            }
            UpdateParams();
        }
        private void Slider_DragCompleted(object sender,  RoutedPropertyChangedEventArgs<double> e)
        {
            double SliderVal=((Slider)sender).Value;
            String SliderName = ((Slider)sender).Name;
            if (mode3D)
            {
                _DeviceControl.updateAngleByParameter(SliderName, (int)SliderVal);
            }
            else {
                _DeviceControl.updateAngleByParameter(SliderName, (int)SliderVal);
            }
            UpdateParams();
        }

        private void updateParametersEditor()
        {
            ParameterList.Children.Clear();
            TextBlock TextBlock11 = new TextBlock();
            TextBlock11.FontSize = 18;
            TextBlock11.Text = "Sterowanie poprzez parametry";
            TextBlock11.FontFamily = new FontFamily("Segoe UI Semibold");
            TextBlock11.Margin = new System.Windows.Thickness(5);
            ParameterList.Children.Add(TextBlock11);

            foreach (KeyValuePair<string, SkeletonAnalyzer> skeleton in _SkeletonAnalyzerContainer.getParameterList())
            {
                List<JointType> bodySegments = skeleton.Value.GetBodySegments();
                StackPanel StackPanel1 = new StackPanel();
                StackPanel1.Orientation = Orientation.Horizontal;
                StackPanel1.Margin = new System.Windows.Thickness(5);

                TextBlock TextBlock1 = new TextBlock();
                TextBlock1.FontSize = 18;
                TextBlock1.Width=350;
                TextBlock1.Text = skeleton.Key + " :: " + bodySegments[0] + " : " + bodySegments[1] + " : " + bodySegments[2];
            
                StackPanel1.Children.Add(TextBlock1);
                ParameterList.Children.Add(StackPanel1);
            }
            Separator Separator1 = new Separator();
            ParameterList.Children.Add(Separator1);

            TextBlock TextBlock12 = new TextBlock();
            TextBlock12.FontSize = 18;
            TextBlock12.Text = "Sterowanie poprzez wiązania";
            TextBlock12.FontFamily = new FontFamily("Segoe UI Semibold");
            TextBlock12.Margin = new System.Windows.Thickness(5);
            ParameterList.Children.Add(TextBlock12);
            foreach (KeyValuePair<string, SkeletonAnalyzer> skeleton in _SkeletonAnalyzerContainer.getConstraintList())
            {
                List<JointType> bodySegments = skeleton.Value.GetBodySegments();
                StackPanel StackPanel1 = new StackPanel();
                StackPanel1.Orientation = Orientation.Horizontal;
                StackPanel1.Margin = new System.Windows.Thickness(5);

                TextBlock TextBlock1 = new TextBlock();
                TextBlock1.FontSize = 18;
                TextBlock1.Width = 350;
                TextBlock1.Text = skeleton.Key + " :: " + bodySegments[0] + " : " + bodySegments[1] + " : " + bodySegments[2];
           
                StackPanel1.Children.Add(TextBlock1);
                ParameterList.Children.Add(StackPanel1);
            }
            UpdateParams();
        }
      
        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }
            return skeleton;
        }
        
        #endregion Methods

        #region EventHandlers

        private void chooseDevice_Checked(object sender, RoutedEventArgs e)
        {
            bindDevice();

       }

        private void bindDevice()
        {
            if (_DeviceControl == null) _DeviceControl = new InventorController();
            Device_1.IsChecked = true;
            if(!_DeviceControl.isConstructed){
                MessageBox.Show("Nie udało się zainicjalizować kontrolera");
                this.Close();

            }
        }       
        private void createMock_Click(object sender, RoutedEventArgs e)
        {
            List<JointType> list = new List<JointType>();
            list.Add(JointType.HipLeft);
            list.Add(JointType.ShoulderLeft);
            list.Add(JointType.ElbowLeft);
            createAnalyzerForParameter("d658", list,-90, 0);
            list.Clear();
           
            list.Add(JointType.ElbowLeft);
            list.Add(JointType.WristLeft);
            list.Add(JointType.HandLeft);
            createAnalyzerForParameter("d701", list, -90, 0);
            createMock.IsEnabled = false;
        }
        private void TextBox_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
         this.Close();
        }
        private void TextBoxDrag_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            this.DragMove();
        }
        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Initializing:
                case KinectStatus.Connected:
                case KinectStatus.NotPowered:
                case KinectStatus.NotReady:
                case KinectStatus.DeviceNotGenuine:
                    this.KinectDevice = e.Sensor;
                    break;
                case KinectStatus.Disconnected:
                    this.KinectDevice = null;
                    break;
                default:
                    break;
            }
        }

        private void activateDevice_Click(object sender, RoutedEventArgs e)
        {
            SkeletonViewerElement.SkeletonViewerConstruct(_KinectDevice, this.LayoutRoot.ActualHeight, this.LayoutRoot.ActualWidth);

            var bc = new BrushConverter();
            if (isActive)
            {
                isActive = false;
                activateDevice.Content = "Aktywuj Kamerę Głębi";
                activateDevice.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            }

            else
            {
                isActive = true;
                activateDevice.Content = "Deaktywuj Kamerę Głębi";
                activateDevice.Background = (Brush)bc.ConvertFrom("#FF87F5B4");
            };
            UpdateParams();

        }
        private void changeMode_Click(object sender, RoutedEventArgs e)
        {
            if (mode3D)
            {
                mode3D = false;
            }
            else
            {
                mode3D = true;
            };
        }
        private void addByParameter_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Collapsed;
            addByParameterPanel.Visibility = Visibility.Visible;
            addByConstraintsPanel.Visibility = Visibility.Collapsed;
            UpdateParams();
        }
        private void addByConstrins_Click(object sender, RoutedEventArgs e)
        {
            if (_DeviceControl.createTreeViewByOccurences().Count > 0)
            {
                updateTreeView();
                Settings.Visibility = Visibility.Collapsed;
                addByParameterPanel.Visibility = Visibility.Collapsed;
                addByConstraintsPanel.Visibility = Visibility.Visible;
            }
            else { 
            
            }
            UpdateParams();
        }

        #endregion EventHandlers

        #region Properties

        public KinectSensor KinectDevice 
        {
            get { return this._KinectDevice; }
            set
            {
                if(this._KinectDevice != value)
                {
                    if(this._KinectDevice != null)
                    {
                        this._KinectDevice.Stop();
                        this._KinectDevice.SkeletonFrameReady -= KinectDevice_SkeletonFrameReady;
                         this._KinectDevice.SkeletonStream.Disable();
                        this._FrameSkeletons = null;
                    }
                   
                    this._KinectDevice = value;
                    if(this._KinectDevice != null)
                    {
                        if(this._KinectDevice.Status == KinectStatus.Connected)
                        {
                            this._KinectDevice.SkeletonStream.Enable();
                            this._FrameSkeletons = new Skeleton[this._KinectDevice.SkeletonStream.FrameSkeletonArrayLength];                        
                            this._KinectDevice.Start(); 
                            this.KinectDevice.SkeletonFrameReady += KinectDevice_SkeletonFrameReady;                            
                        }
                    }                
                }
            }
        }

        #endregion Properties      

    private void addByConstraintsAdd_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)TreeViewConstraints.SelectedItem;
            bool rSelectedItem = (selectedItem != null && selectedItem.Parent != TreeViewConstraints);
            int s2D;
            int s3D;
            bool r2D = Int32.TryParse(parameterS2D_Constraints.Text, out s2D);
            bool r3D = Int32.TryParse(parameterS3D_Constraints.Text, out s3D);
            bool rJoint = jointTypeCheckboxList_Constraints.Count == 3 ? true : false;
            List<JointType> list = new List<JointType>();

            foreach (CheckBox cb in jointTypeCheckboxList_Constraints)
            {
                foreach (JointType jt in Enum.GetValues(typeof(JointType)))
                {
                        if (cb.Name.Replace("_Constraints", "") == jt.ToString()) {
                        list.Add(jt); }
                };
            }

            if (rSelectedItem && r2D && r3D && rJoint)
            {
                createAnalyzerForConstraints(selectedItem.Header.ToString(), list, s2D, s3D);
                Settings.Visibility = Visibility.Visible;
                addByParameterPanel.Visibility = Visibility.Collapsed;
                MessageBox.Show("Dodano wiązanie " + selectedItem.Header);

            }
            else
            {
                MessageBox.Show("Niepoprawne dane" + r2D + r3D + r2D + rSelectedItem + rJoint);
            }
          
            Settings.Visibility = Visibility.Visible;
            addByConstraintsPanel.Visibility = Visibility.Collapsed;
            UpdateParams();
        }
        
    private void addByConstrinsCancel_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            addByConstraintsPanel.Visibility = Visibility.Collapsed;
        }
    
    List<CheckBox> jointTypeCheckboxList_Constraints = new List<CheckBox>();
    List<CheckBox> jointTypeCheckboxList = new List<CheckBox>();

    private void selectJoint_Click(object sender, RoutedEventArgs e)
        {
            jointTypeCheckboxList.Clear();
            int amount=0;
            foreach (CheckBox cb in FindVisualChildren<CheckBox>(jointsList))
            {
                if (cb.IsChecked == true)
                {
                    if (amount < 3)
                    {
                        jointTypeCheckboxList.Add(cb);
                        amount++;
                    }
                    else {
                        cb.IsChecked = false;
                    }
                }
            }
            UpdateParams();
        }
    private void selectJoint_Constraints_Click(object sender, RoutedEventArgs e)
    {
        jointTypeCheckboxList_Constraints.Clear();
        int amount = 0;
        foreach (CheckBox cb in FindVisualChildren<CheckBox>(jointsList_Constraints))
        {
            if (cb.IsChecked == true)
            {
                if (amount < 3)
                {
                    jointTypeCheckboxList_Constraints.Add(cb);
                    amount++;
                }
                else
                {
                    cb.IsChecked = false;
                }
            }
        }
        UpdateParams();
    }

    private void addByParameterAdd_Click(object sender, RoutedEventArgs e)
        {
            string parameter = parameterName.Text;
            int s2D;
            int s3D;
            bool rName = (parameter != null && !parameter.Equals(""));
            bool r2D = Int32.TryParse(parameterS2D.Text, out s2D);
            bool r3D = Int32.TryParse(parameterS3D.Text, out s3D);
            bool rJoint = jointTypeCheckboxList.Count == 3 ? true : false;
            List<JointType> list = new List<JointType>();

            foreach (CheckBox cb in jointTypeCheckboxList)
            {
                foreach (JointType jt in Enum.GetValues(typeof(JointType))) {
                    if (cb.Name == jt.ToString()) list.Add(jt);
                };
            }
         
            if (rName && r2D && r3D && rJoint)
            {
                createAnalyzerForParameter(parameter, list, s2D, s3D);
                Settings.Visibility = Visibility.Visible;
                addByParameterPanel.Visibility = Visibility.Collapsed;
                MessageBox.Show("Dodano parametr " + parameter);
            }
            else {
                MessageBox.Show("Niepoprawne dane");
            }
            UpdateParams();  
        }
        private void UpdateParams(){
            String textToShow = "";
            textToShow += "Odczyt za pomocą: " + (sourceParameterBool?"Parametrów":"Wiązań")+"\n";
            textToShow += "Wątek kontrolera wolny: " + (threadTest ? "Tak" : "Nie") + "\n";
            textToShow += "Włączono odczyt z kamery głębi: " + (isActive ? "Tak" : "Nie") + "\n";
            textToShow += "Można zmienić źródło odczytu: " + (alreadyChanged ? "Nie" : "Tak") + "\n";
            Block_left.Text = textToShow;
            String textToShow1 = "";
            textToShow1 += "Tryb odczytu: " + (mode3D ? "3D" : "2D") + "\n";
            textToShow1 += "Kontroler: " + Device_1.Content + "\n";
         
            Block_right.Text = textToShow1;
        }

    private void addByParameterCancel_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            addByParameterPanel.Visibility = Visibility.Collapsed;
        }
    bool alreadyChanged=false;
    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
{
    if (depObj != null)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
            if (child != null && child is T)
            {
                yield return (T)child;
            }

            foreach (T childOfChild in FindVisualChildren<T>(child))
            {
                yield return childOfChild;
            }
        }
    }
}
    private void setSource_Click(object sender, RoutedEventArgs e)
    {
        string content = (sender as Button).Name.ToString();
        var bc = new BrushConverter();
        toogleSource = true;
        switch (content) {
            case "sourceParameter":
                if (alreadyChanged) { MessageBox.Show("Niestety, zmiana jest już niemożliwa"); }
                else
                {
                    sourceParameterBool = true;
                    sourceParameter.Background = (Brush)bc.ConvertFrom("#FF87F5B4");
                    sourceConstraints.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
                }
                break;
            case "sourceConstraints":
                sourceParameterBool = false;
                sourceConstraints.Background = (Brush)bc.ConvertFrom("#FF87F5B4");
                sourceParameter.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
                break;
        };
        UpdateParams();
    }
    }

}

