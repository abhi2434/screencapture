using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double x;
        public double y;
        public double w;
        public double h;
        public bool isMouseDown = false;
        public System.Drawing.Size desiredSize;
        public System.Windows.Shapes.Rectangle adornerRectangle = null;
        public System.Windows.Shapes.Rectangle absoluteAdornerRectangle = null;
        public string watermark;
        public double ratio = 1;
        public MainWindow()
        {
            InitializeComponent();

            desiredSize = new System.Drawing.Size(UserArgs.Configurations.PhotoParameters.Width, UserArgs.Configurations.PhotoParameters.Height);
            watermark = UserArgs.Configurations.PhotoWatermarking.Text;
            ratio = (double)UserArgs.Configurations.PhotoParameters.Height / (double)UserArgs.Configurations.PhotoParameters.Width;
        }

       
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (adornerRectangle == null)
            {
                isMouseDown = true;
                x = e.GetPosition(null).X;
                y = e.GetPosition(null).Y;
            }
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                double curx = e.GetPosition(null).X;
                double cury = e.GetPosition(null).Y;
                adornerRectangle = new System.Windows.Shapes.Rectangle();
                if (!UserArgs.Configurations.IsConstrained)
                {
                    SolidColorBrush brush = new SolidColorBrush(Colors.White);
                    adornerRectangle.Stroke = brush;
                    adornerRectangle.Fill = brush;
                    adornerRectangle.StrokeThickness = 2;
                }
                var width = Math.Abs(curx - x);
                var height = Math.Abs(cury - y);
                adornerRectangle.Width = width;
                adornerRectangle.Height = height;
                adornerRectangle.Opacity = 0.5;
                cnv.Children.Clear();

                cnv.Children.Add(adornerRectangle);
                Canvas.SetLeft(adornerRectangle, x);
                Canvas.SetTop(adornerRectangle, y);
                TextBlock txt = null;
                if (UserArgs.Configurations.IsConstrained)
                {
                    this.DrawActualRectangle(curx, cury);
                    txt = this.ShowCaption(absoluteAdornerRectangle, cnv, curx, cury);
                }
                if (e.LeftButton == MouseButtonState.Released)
                {
                    if (UserArgs.Configurations.IsConstrained)
                    {
                        adornerRectangle.Height = absoluteAdornerRectangle.Height;
                        adornerRectangle.Width = absoluteAdornerRectangle.Width;
                    }
                    var aLayer = AdornerLayer.GetAdornerLayer(adornerRectangle);
                    aLayer.Add(new ResizingAdorner(adornerRectangle, UserArgs.Configurations.IsConstrained, ratio, absoluteAdornerRectangle, txt));
                    //txt.Text = string.Empty;
                    this.isMouseDown = false;
                }
            }
        }

        private TextBlock ShowCaption(System.Windows.Shapes.Rectangle ar, Canvas cnv, double x, double y)
        {
            var width = ar.Width;
            var height = ar.Height;

            string widthString = string.Format("Capture : ({0},{1})", Math.Round(width, 0), Math.Round(height, 0));

            TextBlock txtWidth = new TextBlock();
            txtWidth.Text = widthString;
            txtWidth.FontSize = 10;
            txtWidth.Foreground = new SolidColorBrush(System.Windows.Media.Colors.Black);
            txtWidth.Background = new SolidColorBrush(System.Windows.Media.Colors.White);
            txtWidth.Visibility = Visibility.Visible;

            cnv.Children.Add(txtWidth);

            Canvas.SetTop(txtWidth, y);
            Canvas.SetLeft(txtWidth, x);

            return txtWidth;
        }
        private void DrawActualRectangle(double curx, double cury)
        {
            absoluteAdornerRectangle = new System.Windows.Shapes.Rectangle();
            SolidColorBrush brush = new SolidColorBrush(Colors.White);

            absoluteAdornerRectangle.Fill = brush;
            //ar.StrokeThickness = 2;
            var width = Math.Abs(curx - x);
            var height = Math.Abs(cury - y);
            var curheight = width * ratio;
            absoluteAdornerRectangle.Width = width;
            absoluteAdornerRectangle.Height = curheight;
            System.Windows.Media.Color strokeColor = Colors.Red;
            if (UserArgs.Configurations.PhotoParameters.Width <= width && UserArgs.Configurations.PhotoParameters.Height <= curheight)
                strokeColor = Colors.Green;
            absoluteAdornerRectangle.Stroke = new SolidColorBrush(strokeColor);
            absoluteAdornerRectangle.StrokeThickness = 2;
            absoluteAdornerRectangle.Opacity = 0.5;
            //cnv.Children.Clear();
            cnv.Children.Add(absoluteAdornerRectangle);
            Canvas.SetLeft(absoluteAdornerRectangle, x);
            Canvas.SetTop(absoluteAdornerRectangle, y);

        }
        private void Window_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (adornerRectangle != null)
            {
                x = Canvas.GetLeft(adornerRectangle);
                y = Canvas.GetTop(adornerRectangle);
                w = adornerRectangle.ActualWidth;
                h = adornerRectangle.ActualHeight;
                var aLayer = AdornerLayer.GetAdornerLayer(adornerRectangle);
                var objs = aLayer.GetAdorners(adornerRectangle);
                aLayer.Remove(objs[0]);
                this.Hide();
                x = this.AdjustX(x);
                y = this.AdjustY(y);
                ImageUtil.CaptureScreen(x, y, w, h, desiredSize, watermark);
                this.x = this.y = 0;
                this.Close();
            }
        }
        private double AdjustX(double x)
        {
            return x + this.Left;
        }
        private double AdjustY(double y)
        {
            return y + this.Top;
        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //if (UserArgs.Configurations.IsConstrained)
            //    this.DrawRectangle(desiredSize);
        }

        private void Window_KeyDown_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (adornerRectangle != null && !UserArgs.Configurations.IsConstrained)
                {
                    var aLayer = AdornerLayer.GetAdornerLayer(adornerRectangle);
                    var objs = aLayer.GetAdorners(adornerRectangle);
                    aLayer.Remove(objs[0]);

                    this.cnv.Children.Remove(this.adornerRectangle);
                    this.adornerRectangle = null;
                    return;
                }
                this.Close();
            }
        }

    }
}
