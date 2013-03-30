using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoCapture
{
    public class ResizingAdorner : Adorner
    {
        Thumb topLeft, topRight, bottomLeft, bottomRight, middle;
        UIElement ratioEnforcer = null;
        VisualCollection visualChildren;
        bool constrained = false;
        double ratio = 1;
        TextBlock txtEnforcer;

        public ResizingAdorner(UIElement adornedElement, bool constrained, double ratio, UIElement ratioEnforcer, TextBlock txtEnforcer)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);

            BuildAdornerCorner(ref middle, Cursors.SizeAll);

            // Add handlers for resizing.

            bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
            bottomLeft.DragCompleted += new DragCompletedEventHandler(HandleComplete);
            bottomRight.DragCompleted += new DragCompletedEventHandler(HandleComplete);
            topLeft.DragCompleted += new DragCompletedEventHandler(HandleComplete);
            topRight.DragCompleted += new DragCompletedEventHandler(HandleComplete);
            middle.DragDelta += new DragDeltaEventHandler(HandleMiddle);
            this.constrained = constrained;
            this.ratio = ratio;
            this.ratioEnforcer = ratioEnforcer;
            this.txtEnforcer = txtEnforcer;
        }

        void HandleMiddle(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            Canvas.SetLeft(adornedElement, Canvas.GetLeft(adornedElement) + args.HorizontalChange);
            Canvas.SetTop(adornedElement, Canvas.GetTop(adornedElement) + args.VerticalChange);
            EnforceResize(adornedElement);
        }
        // Handler for resizing from the bottom-right.
        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);

            EnforceResize(adornedElement);
        }

        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            double height_old = adornedElement.Height;
            double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            double top_old = Canvas.GetTop(adornedElement);
            adornedElement.Height = height_new;
            Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
            EnforceResize(adornedElement);
        }

        // Handler for resizing from the top-left.
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);

            double width_old = adornedElement.Width;
            double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            double left_old = Canvas.GetLeft(adornedElement);
            adornedElement.Width = width_new;
            Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));

            double height_old = adornedElement.Height;
            double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            double top_old = Canvas.GetTop(adornedElement);
            adornedElement.Height = height_new;
            Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
            EnforceResize(adornedElement);
        }

        // Handler for resizing from the bottom-left.
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);
           
            //adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);

            double width_old = adornedElement.Width;
            double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            double left_old = Canvas.GetLeft(adornedElement);
            adornedElement.Width = width_new;
            Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));
            EnforceResize(adornedElement);
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.

            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            middle.Arrange(new Rect(0, 0, desiredWidth, desiredHeight));
            // Return the final size.
            return finalSize;
        }

        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 10;
            //cornerThumb.Opacity = 0.40;
            cornerThumb.Background = new SolidColorBrush(Colors.MediumBlue);

            visualChildren.Add(cornerThumb);
        }

        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        void EnforceResize(FrameworkElement adornedElement)
        {
            if (this.constrained)
            {
                FrameworkElement ratioEnforcer = this.ratioEnforcer as FrameworkElement;
                Canvas.SetTop(ratioEnforcer, Canvas.GetTop(adornedElement));
                Canvas.SetLeft(ratioEnforcer, Canvas.GetLeft(adornedElement));
                double width = adornedElement.Width;
                double height = adornedElement.Width * ratio;
                ratioEnforcer.Width = width;
                ratioEnforcer.Height = height;
                System.Windows.Media.Color strokeColor = Colors.Red;
                if (UserArgs.Configurations.PhotoParameters.Width <= width 
                    && UserArgs.Configurations.PhotoParameters.Height <= height)
                    strokeColor = Colors.Green;
                if(ratioEnforcer is System.Windows.Shapes.Rectangle)
                {
                    var enforcer = ratioEnforcer as System.Windows.Shapes.Rectangle;
                    enforcer.Stroke = new SolidColorBrush(strokeColor); 
                }

                this.ShowCaption(ratioEnforcer);
            }
        }
        private void ShowCaption(FrameworkElement ar)
        {
            var width = ar.Width;
            var height = ar.Height;

            string widthString = string.Format("Capture : ({0},{1})", Math.Round(width,0), Math.Round(height, 0));
            if (this.txtEnforcer != null)
            {
                this.txtEnforcer.Text = widthString;
                var point = System.Windows.Forms.Control.MousePosition;
                Canvas.SetLeft(this.txtEnforcer, point.X);
                Canvas.SetTop(this.txtEnforcer, point.Y);
            }
        }
        private void HandleComplete(object sender, DragCompletedEventArgs e)
        {
            if (this.constrained)
            {
                FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
                FrameworkElement ratioEnforcer = this.ratioEnforcer as FrameworkElement;
                adornedElement.Width = ratioEnforcer.Width;
                adornedElement.Height = ratioEnforcer.Height;
                //this.txtEnforcer.Text = string.Empty;
            }
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
