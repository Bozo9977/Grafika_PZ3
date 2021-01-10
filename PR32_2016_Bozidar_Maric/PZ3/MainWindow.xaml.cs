using PZ3.Helpers;
using PZ3.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PZ3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int xDimension = 100;
        private static int yDimension = 100;
        private static int zDimension = 100;

        private static double minX = 19.793909;
        private static double maxX = 19.894459;
        private static double minY = 45.2325;
        private static double maxY = 45.277031;

        public static Dictionary<ulong, Tuple<string, Entity>> elements = new Dictionary<ulong, Tuple<string, Entity>>();
        public static Dictionary<ulong, LineEntity> allLines = new Dictionary<ulong, LineEntity>();
        public static Tuple<string, object>[,,] matrix = new Tuple<string, object>[xDimension, yDimension, zDimension];

        public static Dictionary<ulong, GeometryModel3D> nodes = new Dictionary<ulong, GeometryModel3D>();
        public static Dictionary<ulong, GeometryModel3D> lines = new Dictionary<ulong, GeometryModel3D>();

        private List<Tuple<string, Entity>> nodeConnections3 = new List<Tuple<string, Entity>>();
        private List<Tuple<string, Entity>> nodeConnections35 = new List<Tuple<string, Entity>>();
        private List<Tuple<string, Entity>> nodeConnections5 = new List<Tuple<string, Entity>>();

        private bool hide3 = false;
        private bool hide35 = false;
        private bool hide5 = false;


        private bool middleButtonPressed = false;
        private bool leftButtonPressed = false;
        private Point start = new Point();
        private Point diffOffset = new Point();


        private GeometryModel3D hitgeo;
        private ToolTip toolTip = new ToolTip();

        private List<Tuple<ulong, SolidColorBrush>> coloredElements = new List<Tuple<ulong, SolidColorBrush>>();



        NetworkModel model = new NetworkModel();
        public MainWindow()
        {
            InitializeComponent();
            model = NetworkModel.InitModel(@"..\..\Files\Geographic.xml");
            //int count = allLines.Count;
            //int counter = elements.Count;
            
            DrawHelper.DrawElements(map, minX, maxX, minY, maxY, xDimension, yDimension, zDimension);

            nodeConnections3 = elements.Values.Where(x => x.Item2.Connections < 3).ToList();
            nodeConnections35 = elements.Values.Where(x => x.Item2.Connections >= 3 && x.Item2.Connections < 5).ToList();
            nodeConnections5 = elements.Values.Where(x => x.Item2.Connections >= 5).ToList();
        }


        #region MouseEvents

        private void ViewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                leftButtonPressed = true;
                ViewPort_LeftButtonDown(sender, e);
            }
            else if(e.MiddleButton == MouseButtonState.Pressed)
            {
                middleButtonPressed = true;
                ViewPort_MiddleButtonDown(sender, e);
            }
        }
        private void ViewPort_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Released)
            {
                leftButtonPressed = false;
            }
            else if(e.MiddleButton == MouseButtonState.Released)
            {
                middleButtonPressed = false;
            }
            ViewPort.ReleaseMouseCapture();
        }

        private void ViewPort_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewPort.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = panning.OffsetX;
            diffOffset.Y = panning.OffsetZ;

            toolTip.IsOpen = false;
            Point mousePosition = e.GetPosition(ViewPort);
            Point3D testPoint = new Point3D(mousePosition.X, mousePosition.Y, 0);
            Vector3D testDirection = new Vector3D(mousePosition.X, mousePosition.Y, 4);

            PointHitTestParameters pointparams = new PointHitTestParameters(mousePosition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testPoint, testDirection);

            hitgeo = null;
            VisualTreeHelper.HitTest(ViewPort, null, HTResult, pointparams);
        }

        private void ViewPort_MiddleButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewPort.CaptureMouse();
            start = e.GetPosition(ViewPort);
            diffOffset.X = panning.OffsetX;
            diffOffset.Y = panning.OffsetZ;
        }


        private int zMax = 50;
        private int uzMax = -10;
        private int zCurr = 1;

        private void ViewPort_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = e.MouseDevice.GetPosition(ViewPort);

            if(e.Delta > 0 && zCurr < zMax)
            {

                zCurr++;
                scale.ScaleX = scale.ScaleX + 0.1;
                scale.ScaleY = scale.ScaleY + 0.1;
                scale.ScaleZ = scale.ScaleZ + 0.1;
            }
            else if(e.Delta <= 0 && zCurr > uzMax)
            {

                zCurr--;
                scale.ScaleX = scale.ScaleX - 0.1;
                scale.ScaleY = scale.ScaleY - 0.1;
                scale.ScaleZ = scale.ScaleZ - 0.1;
            }
            ViewPort.CaptureMouse();
        }

        private void ViewPort_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewPort.IsMouseCaptured)
            {
                Point end = e.GetPosition(this);
                double offX = end.X - start.X;
                double offY = end.Y - start.Y;
                double width = this.Width;
                double height = this.Height;
                double translateX = (offX * 100) / width;
                double translateY = (offY * 100) / height;

                if(leftButtonPressed)
                {
                    panning.OffsetX = diffOffset.X + (translateX / (100 * scale.ScaleX));
                    panning.OffsetZ = diffOffset.Y + (translateY / (100 * scale.ScaleZ));
                }
                else if (middleButtonPressed)
                {
                    rotateY.Angle = (rotateY.Angle + translateY) % 360;
                    double rotationXAngle = rotateX.Angle + translateY % 360;
                    if(rotationXAngle > -20 && rotationXAngle < 65)
                    {
                        rotateX.Angle = rotationXAngle;
                    }
                    start = end;
                }
            }
        }
        #endregion

        #region HitTesting
        private HitTestResultBehavior HTResult(HitTestResult rawResult)
        {
            RayHitTestResult rayResult = rawResult as RayHitTestResult;

            if (rayResult != null)
            {
                bool hit = false;

                foreach (ulong key in nodes.Keys)
                {
                    if (nodes[key] == (GeometryModel3D)rayResult.ModelHit)
                    {
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        Tuple<string, Entity> powerEntity = elements[key];
                        toolTip.Content = "\t\t" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(powerEntity.Item1.ToLower()) + "\nId:\t" + powerEntity.Item2.Id + "\nName:\t" + powerEntity.Item2.Name;
                        if (powerEntity.Item1 == "switch")
                            toolTip.Content += "\nStatus:\t" + ((SwitchEntity)powerEntity.Item2).Status;
                        toolTip.IsOpen = true;
                        toolTip.StaysOpen = true;

                        hit = true;
                    }
                }

                if (!hit)
                {
                    foreach (ulong key in lines.Keys)
                    {
                        if (lines[key] == (GeometryModel3D)rayResult.ModelHit)
                        {
                            if (coloredElements.Count != 0)
                            {
                                lines[coloredElements[0].Item1].Material = new DiffuseMaterial(coloredElements[0].Item2);
                                nodes[coloredElements[1].Item1].Material = new DiffuseMaterial(coloredElements[1].Item2);
                                nodes[coloredElements[2].Item1].Material = new DiffuseMaterial(coloredElements[2].Item2);
                            }

                            coloredElements.Clear();

                            hitgeo = (GeometryModel3D)rayResult.ModelHit;
                            LineEntity lineEntity = allLines[key];

                            coloredElements.Add(new Tuple<ulong, SolidColorBrush>(key, Brushes.Purple));


                            if(elements[lineEntity.FirstEnd].Item2 is NodeEntity)
                                coloredElements.Add(new Tuple<ulong, SolidColorBrush>(lineEntity.FirstEnd, Brushes.Green));
                            else if(elements[lineEntity.FirstEnd].Item2 is SubstationEntity)
                                coloredElements.Add(new Tuple<ulong, SolidColorBrush>(lineEntity.FirstEnd, Brushes.Red));
                            else if(elements[lineEntity.FirstEnd].Item2 is SwitchEntity)
                                coloredElements.Add(new Tuple<ulong, SolidColorBrush>(lineEntity.FirstEnd, Brushes.Blue));

                            if (elements[lineEntity.SecondEnd].Item2 is NodeEntity)
                                coloredElements.Add(new Tuple<ulong, SolidColorBrush>(lineEntity.SecondEnd, Brushes.Green));
                            else if (elements[lineEntity.SecondEnd].Item2 is SubstationEntity)
                                coloredElements.Add(new Tuple<ulong, SolidColorBrush>(lineEntity.SecondEnd, Brushes.Red));
                            else if (elements[lineEntity.SecondEnd].Item2 is SwitchEntity)
                                coloredElements.Add(new Tuple<ulong, SolidColorBrush>(lineEntity.SecondEnd, Brushes.Blue));
                            


                            lines[key].Material = new DiffuseMaterial(Brushes.Yellow);
                            nodes[lineEntity.FirstEnd].Material = new DiffuseMaterial(Brushes.Yellow);
                            nodes[lineEntity.SecondEnd].Material = new DiffuseMaterial(Brushes.Yellow);

                            hit = true;
                        }
                    }
                }

                if (!hit)
                {
                    hitgeo = null;
                    toolTip.IsOpen = false;
                }
            }

            return HitTestResultBehavior.Stop;
        }

        #endregion

        #region Dodatni
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            hide3 = true;

            if (hide35)
            {
                foreach (var item in nodeConnections35)
                    map.Children.Add(nodes[item.Item2.Id]);
                hide35 = false;
            }
            if (hide5)
            {
                foreach (var item in nodeConnections5)
                {
                    map.Children.Add(nodes[item.Item2.Id]);
                }
                hide5 = false;
            }

            MessageBox.Show($"Number of elements with up to 3 connections: {nodeConnections3.Count}");

            foreach (var item in nodeConnections3)
            {
                map.Children.Remove(nodes[item.Item2.Id]);
            }
            
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            hide35 = true;
            
            if (hide3)
            {
                foreach (var item in nodeConnections3)
                {
                    map.Children.Add(nodes[item.Item2.Id]);
                }
                hide3 = false;
            }

            if (hide5)
            {
                foreach (var item in nodeConnections5)
                {
                    map.Children.Add(nodes[item.Item2.Id]);
                }
                hide5 = false;
            }
            MessageBox.Show($"Number of elements with up to 3 connections: {nodeConnections35.Count}");
            foreach (var item in nodeConnections35)
            {
                map.Children.Remove(nodes[item.Item2.Id]);
            }

            
            
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            hide5 = true;

            if (hide3)
            {
                foreach (var item in nodeConnections3)
                    map.Children.Add(nodes[item.Item2.Id]);
                hide3 = false;
            }

            if (hide35)
            {
                foreach (var item in nodeConnections35)
                    map.Children.Add(nodes[item.Item2.Id]);
                hide35 = false;
            }

            MessageBox.Show($"Number of elements with up to 3 connections: {nodeConnections5.Count}");

            foreach (var item in nodeConnections5)
            {
                map.Children.Remove(nodes[item.Item2.Id]);
            }
        }
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            if (hide3)
            {
                foreach (var item in nodeConnections3)
                    map.Children.Add(nodes[item.Item2.Id]);
                hide3 = false;
                MessageBox.Show($"Showing {nodeConnections3.Count} hidden elements.");
            }

            if (hide35)
            {
                foreach (var item in nodeConnections35)
                    map.Children.Add(nodes[item.Item2.Id]);
                hide35 = false;
                MessageBox.Show($"Showing {nodeConnections35.Count} hidden elements.");
            }
            if (hide5)
            {
                foreach (var item in nodeConnections5)
                {
                    map.Children.Add(nodes[item.Item2.Id]);
                }
                hide5 = false;
                MessageBox.Show($"Showing {nodeConnections5.Count} hidden elements.");
            }
        }
        #endregion


    }
}
