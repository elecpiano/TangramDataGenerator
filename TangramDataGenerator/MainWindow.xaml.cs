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

namespace TangramDataGenerator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Windows Lifecycle

        public MainWindow()
        {
            InitializeComponent();
            CurrentStatus = ControlFlowEnum.WaitingForImage;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region Control Flow

        ControlFlowEnum _currentStatus = ControlFlowEnum.PlacingDot;
        ControlFlowEnum CurrentStatus
        {
            get
            {
                return _currentStatus;
            }
            set
            {
                _currentStatus = value;
                switch (_currentStatus)
                {
                    case ControlFlowEnum.WaitingForImage:
                        step0.Visibility = Visibility.Visible;
                        step1.Visibility = step2.Visibility = step3.Visibility = step4.Visibility = Visibility.Collapsed;
                        break;
                    case ControlFlowEnum.PlacingDot:
                        ResetDotsForGeneration();

                        step1.Visibility = Visibility.Visible;
                        step1.IsEnabled = true;
                        step2.Visibility = step3.Visibility = step4.Visibility = Visibility.Collapsed;
                        break;
                    case ControlFlowEnum.DotsForShape:
                        CanvasZoomOut();
                        ResetDotsForShape();

                        step1.Visibility = step2.Visibility = Visibility.Visible;
                        step3.Visibility = step4.Visibility = Visibility.Collapsed;
                        step1.IsEnabled = false;
                        step2.IsEnabled = true;
                        break;
                    case ControlFlowEnum.DotsForFrame:
                        ResetDotsForFrame();

                        step1.Visibility = step2.Visibility = step3.Visibility = Visibility.Visible;
                        step4.Visibility = Visibility.Collapsed;
                        step1.IsEnabled = false;
                        step2.IsEnabled = false;
                        step3.IsEnabled = true;
                        break;
                    case ControlFlowEnum.GeneratingData:
                        step1.Visibility = step2.Visibility = step3.Visibility = step4.Visibility = Visibility.Visible;
                        step1.IsEnabled = false;
                        step2.IsEnabled = false;
                        step3.IsEnabled = false;
                        step4.IsEnabled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Step 0

        string FileName = string.Empty;

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "png files (*.png)|*.png|All files (*.*)|*.*";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                img.Source = new BitmapImage(new Uri(ofd.FileName));
                string fullName = ofd.FileName;
                FileName = fullName.Substring(fullName.LastIndexOf("\\") + 1, (fullName.LastIndexOf(".") - fullName.LastIndexOf("\\") - 1)); 
                CurrentStatus = ControlFlowEnum.PlacingDot;
            }
        }

        #endregion

        #region Step 1

        private void DotGenerationReset_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus = ControlFlowEnum.PlacingDot;
        }

        private void DotGenerationComplete_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus = ControlFlowEnum.DotsForShape;
        }

        private void ResetDotsForGeneration()
        {
            while (Dots.Count > 0)
            {
                RemoveDot(Dots.Keys.First<Ellipse>());
            }

            this.lineCanvas.Children.Clear();
        }
        #endregion

        #region Step 2

        List<Ellipse> shapeQueue = new List<Ellipse>();

        Border _currentColor = null;
        Border CurrentColor
        {
            get
            {
                return _currentColor;
            }
            set
            {
                if (_currentColor != null)
                {
                    _currentColor.Width = CurrentColor.Height = 30;
                    _currentColor.BorderThickness = new Thickness(0);
                }
                _currentColor = value;
                if (_currentColor != null)
                {
                    _currentColor.Width = CurrentColor.Height = 50;
                    _currentColor.BorderThickness = new Thickness(3);
                }
            }
        }

        private void ResetDotsForShape()
        {
            shapeQueue.Clear();

            color1.DataContext = "";
            color2.DataContext = "";
            color3.DataContext = "";
            color4.DataContext = "";
            color5.DataContext = "";
            color6.DataContext = "";
            color7.DataContext = "";

            foreach (var dot in Dots)
            {
                dot.Key.Fill = BlackBrush;
            }

            btnDotsForShapeComplete.IsEnabled = false;

            CurrentColor = color1;
        }

        private void SetDotForShape(Ellipse dot)
        {
            shapeQueue.Add(dot);

            dot.Fill = CurrentColor.Background;

            // update dot count for shape
            if (shapeQueue.Count <= 3)
            {
                color1.DataContext = shapeQueue.Count;
            }
            else if (shapeQueue.Count > 3 && shapeQueue.Count <= 6)
            {
                color2.DataContext = shapeQueue.Count - 3;
            }
            else if (shapeQueue.Count > 6 && shapeQueue.Count <= 9)
            {
                color3.DataContext = shapeQueue.Count - 6;
            }
            else if (shapeQueue.Count > 9 && shapeQueue.Count <= 13)
            {
                color4.DataContext = shapeQueue.Count - 9;
            }
            else if (shapeQueue.Count > 13 && shapeQueue.Count <= 16)
            {
                color5.DataContext = shapeQueue.Count - 13;
            }
            else if (shapeQueue.Count > 16 && shapeQueue.Count <= 19)
            {
                color6.DataContext = shapeQueue.Count - 16;
            }
            else if (shapeQueue.Count > 19 && shapeQueue.Count <= 23)
            {
                color7.DataContext = shapeQueue.Count - 19;
            }

            // highlight current color
            if (shapeQueue.Count < 3)
            {
                CurrentColor = color1;
            }
            else if (shapeQueue.Count >= 3 && shapeQueue.Count < 6)
            {
                CurrentColor = color2;
            }
            else if (shapeQueue.Count >= 6 && shapeQueue.Count < 9)
            {
                CurrentColor = color3;
            }
            else if (shapeQueue.Count >= 9 && shapeQueue.Count < 13)
            {
                CurrentColor = color4;
            }
            else if (shapeQueue.Count >= 13 && shapeQueue.Count < 16)
            {
                CurrentColor = color5;
            }
            else if (shapeQueue.Count >= 16 && shapeQueue.Count < 19)
            {
                CurrentColor = color6;
            }
            else if (shapeQueue.Count >= 19 && shapeQueue.Count < 23)
            {
                CurrentColor = color7;
            }
            else if (shapeQueue.Count == 23)
            {
                btnDotsForShapeComplete.IsEnabled = true;
                CurrentColor = null;
            }


        }

        private void DotsForShapeReset_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus = ControlFlowEnum.DotsForShape;
        }

        private void DotsForShapeComplete_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus = ControlFlowEnum.DotsForFrame;
        }

        #endregion

        #region Step 3

        List<List<Ellipse>> frameQueue = new List<List<Ellipse>>();

        SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.White);
        SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);

        Ellipse lastFrameDot = null;

        private void ResetDotsForFrame()
        {
            frameQueue.Clear();

            foreach (var dot in Dots)
            {
                dot.Key.Fill = BlackBrush;
            }

            this.lineCanvas.Children.Clear();
            lastFrameDot = null;
        }

        private void SetDotForFrame(Ellipse dot)
        {
            dot.Fill = WhiteBrush;

            if (lastFrameDot == null)
            {
                var newIsland = new List<Ellipse>();
                newIsland.Add(dot);
                frameQueue.Add(newIsland);
            }
            else
            {
                var currentIsland = frameQueue[frameQueue.Count - 1];
                currentIsland.Add(dot);

                var from = Dots[lastFrameDot];
                var to = Dots[dot];
                DrawLine(from.X / 8, from.Y / 8, to.X / 8, to.Y / 8);
            }

            lastFrameDot = dot;
        }

        private void DotsForFrameReset_Click(object sender, RoutedEventArgs e)
        {
            ResetDotsForFrame();
        }

        private void DotsForFrameComplete_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus = ControlFlowEnum.GeneratingData;

            string data = GenerateData();
            Clipboard.SetText(data);
            textResult.Text = data;
        }

        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line();
            line.Stroke = WhiteBrush;
            line.StrokeThickness = 4;
            line.HorizontalAlignment = HorizontalAlignment.Left;
            line.VerticalAlignment = VerticalAlignment.Top;
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            this.lineCanvas.Children.Add(line);
        }

        #endregion

        #region Step 4

        private void GenerateData_Click(object sender, RoutedEventArgs e)
        {
            //CurrentStatus = ControlFlowEnum.WaitingForImage;
            string data = GenerateData();
            Clipboard.SetText(data);
            textResult.Text = data;
        }

        #endregion

        #region Canvas & Dot

        double scale = 1;
        double marginLeft;
        double marginTop;
        double imageOriginWidth = 512;
        double imageOriginHeight = 512;

        Dictionary<Ellipse, Point> Dots = new Dictionary<Ellipse, Point>();

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CurrentStatus == ControlFlowEnum.PlacingDot)
            {
                CanvasZoom(e);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z)
            {
                CanvasZoomByKeyboard();
            }
        }

        private void dotCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlaceADot(e);
        }

        private void CanvasZoom(MouseWheelEventArgs e)
        {
            Point pointAtCanvas = e.GetPosition(this.canvas);

            double pointAtImageX = pointAtCanvas.X - this.img.Margin.Left;
            double pointAtImageY = pointAtCanvas.Y - this.img.Margin.Top;

            if (e.Delta > 0 && scale < 2)
            {
                scale = 8;
                marginLeft = pointAtCanvas.X - pointAtImageX * scale;
                marginTop = pointAtCanvas.Y - pointAtImageY * scale;
            }
            else if (e.Delta < 0 && scale > 1)
            {
                scale = 1;
                marginLeft = 0;
                marginTop = 0;
            }

            this.img.Width = scale * imageOriginWidth;
            this.img.Height = scale * imageOriginHeight;

            this.img.Margin = new Thickness(marginLeft, marginTop, 0, 0);

            SyncDots();
        }


        private void CanvasZoomByKeyboard()
        {
            Point pointAtCanvas = Mouse.GetPosition(this.canvas);

            double pointAtImageX = pointAtCanvas.X - this.img.Margin.Left;
            double pointAtImageY = pointAtCanvas.Y - this.img.Margin.Top;

            if (scale < 2)
            {
                scale = 8;
                marginLeft = pointAtCanvas.X - pointAtImageX * scale;
                marginTop = pointAtCanvas.Y - pointAtImageY * scale;
            }
            else if (scale > 1)
            {
                scale = 1;
                marginLeft = 0;
                marginTop = 0;
            }

            this.img.Width = scale * imageOriginWidth;
            this.img.Height = scale * imageOriginHeight;

            this.img.Margin = new Thickness(marginLeft, marginTop, 0, 0);

            SyncDots();
        }

        private void CanvasZoomOut()
        {
            scale = 1;
            marginLeft = 0;
            marginTop = 0;

            this.img.Width = scale * imageOriginWidth;
            this.img.Height = scale * imageOriginHeight;

            this.img.Margin = new Thickness(marginLeft, marginTop, 0, 0);

            SyncDots();
        }

        private void PlaceADot(MouseButtonEventArgs e)
        {
            if (scale == 1)
            {
                return;
            }

            Ellipse dot = GetFreeDot();
            Point point = e.GetPosition(this.dotCanvas);
            this.dotCanvas.Children.Add(dot);
            dot.SetValue(Canvas.LeftProperty, point.X);
            dot.SetValue(Canvas.TopProperty, point.Y);
            this.dotCanvas.UpdateLayout();

            Dots.Add(dot, point);
        }

        private void SyncDots()
        {
            this.dotCanvas.Width = this.img.Width;
            this.dotCanvas.Height = this.img.Height;
            this.dotCanvas.Margin = this.img.Margin;

            foreach (var dot in Dots)
            {
                dot.Key.SetValue(Canvas.LeftProperty, dot.Value.X * scale / 8);
                dot.Key.SetValue(Canvas.TopProperty, dot.Value.Y * scale / 8);
            }
        }

        private void RemoveDot(Ellipse dot)
        {
            Dots.Remove(dot);
            this.dotCanvas.Children.Remove(dot);
            DotsPool.Push(dot);
        }

        #endregion

        #region Dot Lifecycle

        Stack<Ellipse> DotsPool = new Stack<Ellipse>();
        private Ellipse GetFreeDot()
        {
            Ellipse dot = null;
            if (DotsPool.Count > 0)
            {
                dot = DotsPool.Pop();
            }
            else
            {
                dot = new Ellipse();
                dot.Width = dot.Height = 14;
                dot.Margin = new Thickness(-7, -7, 0, 0);
                dot.MouseRightButtonDown += Dot_MouseRightButtonDown;
                dot.MouseLeftButtonDown += Dot_MouseLeftButtonDown;
            }

            dot.Fill = BlackBrush;
            dot.Opacity = 0.5;

            return dot;
        }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dot = sender as Ellipse;

            if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control) /* tagging start point for a frame */
            {
                if (CurrentStatus == ControlFlowEnum.DotsForFrame)
                {
                    lastFrameDot = null;
                    SetDotForFrame(dot);
                }
            }
            else if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift) 
            {
                if (CurrentStatus == ControlFlowEnum.PlacingDot)
                {
                    RemoveDot(dot);
                }
            }
            else
            {
                if (scale != 1)
                {
                    return;
                }

                if (CurrentStatus == ControlFlowEnum.DotsForShape)
                {
                    SetDotForShape(dot);
                }
                else if (CurrentStatus == ControlFlowEnum.DotsForFrame)
                {
                    SetDotForFrame(dot);
                }
            }

            e.Handled = true;
        }

        private void Dot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentStatus == ControlFlowEnum.PlacingDot)
            {
                var dot = sender as Ellipse;
                RemoveDot(dot);
            }
        }

        #endregion

        #region Data Generation

        int[] DataPattern = { 3, 3, 3, 4, 3, 3, 4 };

        private string GenerateData()
        {
            string data = string.Empty;

            int idx = 0;

            var dotKeyList = Dots.Keys.ToList();
            while (idx < Dots.Count)
            {
                var point = Dots[dotKeyList[idx]];
                data += (Math.Round(point.X / 8)).ToString() + "," + Math.Round((point.Y / 8)).ToString() + ";";
                idx++;
            }

            data = data.TrimEnd(';');
            data += "/";

            for (int i = 0; i < 7; i++)
            {
                var dotCount = DataPattern[i];
                for (int j = 0; j < dotCount; j++)
                {
                    var dot = shapeQueue[0];
                    data += dotKeyList.IndexOf(dot).ToString() + ",";
                    shapeQueue.RemoveAt(0);
                }

                data = data.TrimEnd(',');
                data += ";";
            }

            data = data.TrimEnd(';');
            data += "/";

            for (int i = 0; i < frameQueue.Count; i++)
            {
                for (int j = 0; j < frameQueue[i].Count; j++)
                {
                    var dot = frameQueue[i][j];
                    data += dotKeyList.IndexOf(dot).ToString() + ",";
                }
                data = data.TrimEnd(',');
                data += ";";
            }

            data = data.TrimEnd(';');
            data += "/";

            data += FileName;

            return data;
        }


        #endregion

    }
}
