using System;
using System.Collections.Generic;
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

namespace GraphicSurface
{
	public partial class Surface
	{
        private double zoomFactor = 1.0;

        public double ZoomFactor
        {
            get { return zoomFactor; }
            set
            {
                if (zoomFactor != value)
                {
                    lastZoomFactor = zoomFactor;
                    zoomFactor = value;
                    //RefreshSurface();
                    PopulateSurface();
                }
            }
        }
        private Point zoomPoint = new Point(0, 0);

        public Point ZoomPoint
        {
            get { return zoomPoint; }
            set
            {
                zoomPoint = value;
            }
        }

        private Point panPoint;

        public Point PanPoint
        {
            get { return panPoint; }
            set { panPoint = value; }
        }

        private double lastZoomFactor = 1.0;
        private bool populating = false;
        private double width;
        private double height;

        private double thinLineWidth;
        private double thickLineWidth;

        private double standardRadius;

        public bool Populating
        {
            get { return populating; }
            set { populating = value; }
        }

        //private BitmapImage bgBitmap = null;
        private BitmapSource bgBmpSource = null;
        private double bgBmpWidth = 1;
        private double bgBmpHeight = 1;

        public List<VisioItem> VisioItems;

        public Surface()
		{
			this.InitializeComponent();
            VisioItems = new List<VisioItem>();
		}

        public void SetBackground(BitmapSource aBmpSource, double bWidth, double bHeight, double sWidth, double sHeight)
        {
            if (aBmpSource != null)
            {
                bgBmpSource = aBmpSource;
                bgBmpWidth = bWidth;
                bgBmpHeight = bHeight;
                width = sWidth;
                height = sHeight;
                ZoomFactor = 1;
                PopulateSurface();
            }
        }

        public VisioItem FindVisioItem(string aName)
        {
            return VisioItems.Find(delegate(VisioItem aShape)
            {
                return aShape.ItemName == aName;
            });
        }

        //public void DeleteVisioItem(string aName)
        //{
        //    VisioItem aShape = FindVisioItem(aName);
        //    if (aShape != null)
        //    {
        //        VisioItems.Remove(aShape);
        //    }
        //}

        //public bool VisioItemExists(string aName)
        //{
        //    VisioItem aShape = FindVisioItem(aName);
        //    return (aShape != null);
        //}

        //public void AddVisioItem(VisioItem visioItem)
        //{
        //    VisioItems.Add(visioItem);
        //}

        public void AddBackground(BitmapImage aBitmap, double aWidth, double aHeight)
        {

        }

        public void AddUpdateElement(VisioItem visioItem)
        {
            bool isNew = false;
            if (visioItem.Element == null) // is it already on the canvas?
            {
                isNew = true;
                if (visioItem.ItemType == 1) //its a shape
                {
                    if (visioItem.ShapeID == 0)
                    {
                        visioItem.Element = new Ellipse();
                    }
                    if (visioItem.ShapeID == 1)
                    {
                        visioItem.Element = new Rectangle();
                        Rectangle r = (Rectangle)visioItem.Element;
                        r.RadiusX = standardRadius;
                        r.RadiusY = standardRadius;
                    }
                }
                if (visioItem.ItemType == 2) //it's text
                {
                    visioItem.ShapeID = 1;
                    visioItem.Element = new TextBlock();
                }
            }
            if (visioItem.Element != null)
            {
                visioItem.Element.Name = visioItem.ItemName;
                visioItem.Element.Height = ContentToSurfaceDimension(visioItem.Height);
                visioItem.Element.Width = ContentToSurfaceDimension(visioItem.Width);

                if (visioItem.Element is Shape)
                {
                    Shape aShape = (Shape)visioItem.Element;
                    aShape.Fill = visioItem.Fillbrush;
                    aShape.StrokeThickness = thickLineWidth;
                    aShape.Stroke = Brushes.Black;
                }
                if (visioItem.Element is TextBlock)
                {
                    TextBlock tb = (TextBlock)visioItem.Element;
                    tb.Text = visioItem.Text;
                    tb.FontSize = 36 * zoomFactor;
                }
                if (visioItem.Element != null)
                {
                    Canvas.SetLeft(visioItem.Element, ContentToSurfaceDimension(visioItem.X) - visioItem.Element.Width / 2);
                    Canvas.SetTop(visioItem.Element, ContentToSurfaceDimension(visioItem.Y) - visioItem.Element.Height / 2);
                    Canvas.SetZIndex(visioItem.Element, visioItem.Z);
                    if (isNew)
                        surfaceCanvas.Children.Add(visioItem.Element);
                }
            }
        }

        public void AddShape(string aName, int shapeID, double x, double y, double h, double w, int z, Brush fillBrush)
        {
            Shape aShape = null;
            if (shapeID == 0)
            {
                aShape = new Ellipse();
            }
            if (shapeID == 1)
            {
                aShape = new Rectangle();
                Rectangle r = (Rectangle)aShape;
                r.RadiusX = standardRadius;
                r.RadiusY = standardRadius;
            }
            aShape.SnapsToDevicePixels = true;
            aShape.Name = aName;
            aShape.Height = ContentToSurfaceDimension(h);
            aShape.Width = ContentToSurfaceDimension(w);

            aShape.Fill = fillBrush;
            aShape.StrokeThickness = thickLineWidth;
            aShape.Stroke = Brushes.Black;
            if (aShape != null)
            {
                Canvas.SetLeft(aShape, ContentToSurfaceDimension(x) - aShape.Width / 2);
                Canvas.SetTop(aShape, ContentToSurfaceDimension(y) - aShape.Height / 2);
                Canvas.SetZIndex(aShape, z);
                surfaceCanvas.Children.Add(aShape);
            }
        }

        public void SetScrollbars(bool hOn, bool vOn)
        {
            if (hOn)
            {
                surfaceViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                surfaceViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            if (vOn)
            {
                surfaceViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                surfaceViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }


        public void Pan(Point newPoint)
        {
            if (panPoint != newPoint)
            {
                double widthToMove = (newPoint.X - panPoint.X);
                double heightToMove = (newPoint.Y - panPoint.Y);

                if ((widthToMove != 0) || (heightToMove != 0))
                {
                    surfaceViewer.ScrollToHorizontalOffset(surfaceViewer.HorizontalOffset - widthToMove);
                    surfaceViewer.ScrollToVerticalOffset(surfaceViewer.VerticalOffset - heightToMove);
                }
            }
            panPoint = newPoint;
        }

        public void VerticalPanPercent(double panPercent)
        {
            surfaceViewer.ScrollToVerticalOffset(panPercent * height / 100.0);
        }

        public void HorizontalPanPercent(double panPercent)
        {
            surfaceViewer.ScrollToHorizontalOffset(panPercent * width / 100.0);
        }

        public Point ScreenPoint() // the real mouse location in logical units
        {
            Point aPoint = Mouse.GetPosition(surfaceViewer);
            return aPoint;
        }

        public Point DiagramPoint() // the virtual mouse position on the embedded image
        {
            Point aPoint = SurfaceCanvasToContentCoordinates(Mouse.GetPosition(surfaceCanvas));
            return aPoint;
        }

        public Point CanvasPoint() // the scaled point on the canvas
        {
            Point aPoint = Mouse.GetPosition(surfaceCanvas);
            return aPoint;
        }

        public Point ViewportOrigin
        {
            get
            {
                Point aPoint = SurfaceToContentCoordinates(new Point(0, 0));
                return aPoint;
            }
        }

        public Point ViewPortBottomRight
        {
            get
            {
                Point aPoint = SurfaceToContentCoordinates(new Point(surfaceViewer.ActualWidth, surfaceViewer.ActualHeight));
                return aPoint;
            }
        }

        public string ViewPortExtentsString
        {
            get
            {
                string aString = ViewportOrigin.X.ToString("0.0") + ", " + ViewportOrigin.Y.ToString("0.0") + ", " +
                    ViewPortBottomRight.X.ToString("0.0") + ", " + ViewPortBottomRight.Y.ToString("0.0");
                return aString;
            }
        }

        public string ZoomPercentString
        {
            get
            {
                string aString = "100%";
                aString = (100 * zoomFactor).ToString("0")+"%";
                return aString;
            }
        }

        public Point SurfaceToContentCoordinates(Point viewportPoint)
        {
            Point contentPoint = new Point((viewportPoint.X + surfaceViewer.HorizontalOffset) / zoomFactor,
                (viewportPoint.Y + surfaceViewer.VerticalOffset) / zoomFactor);
            return contentPoint; 
        }

        public Point ContentToSurfaceCoodinates(Point contentPoint)
        {
            Point viewportPoint = new Point((contentPoint.X * zoomFactor) - surfaceViewer.HorizontalOffset,
                (contentPoint.Y * zoomFactor) - surfaceViewer.VerticalOffset);
            return viewportPoint;
        }

        public Point SurfaceCanvasToContentCoordinates(Point surfaceCanvasPoint)
        {
            Point contentPoint = new Point((surfaceCanvasPoint.X / zoomFactor), surfaceCanvasPoint.Y / zoomFactor);
            return contentPoint;
        }

        private double ContentToSurfaceDimension(double aDimension)
        {
            return aDimension * zoomFactor;
        }

        public Point ContentToSurfaceCanvasCoordinates(Point surfaceCanvasPoint)
        {
            Point contentPoint = new Point((surfaceCanvasPoint.X * zoomFactor), surfaceCanvasPoint.Y * zoomFactor);
            return contentPoint;
        }

        public void ZoomToLimits(Point contentTopLeft, Point contentBottomRight)
        {
            double contentZoomWidth = contentBottomRight.X - contentTopLeft.X;
            double zfw = 1;
            if (contentZoomWidth > 0)
                zfw = surfaceViewer.ActualWidth / contentZoomWidth;
            double contentZoomHeight = contentBottomRight.Y - contentTopLeft.Y;
            double zfh = 1;
            if (contentZoomHeight > 0)
                zfh = surfaceViewer.ActualHeight / contentZoomHeight;
            if (zfw < zfh)
                zoomFactor = zfw;
            else
                zoomFactor = zfh;
            PopulateSurface();
            Point surfacePoint = ContentToSurfaceCanvasCoordinates(contentTopLeft);
            surfaceViewer.ScrollToVerticalOffset(surfacePoint.Y);
            surfaceViewer.ScrollToHorizontalOffset(surfacePoint.X);
        }

        public void RefreshContent()
        {
            //how to reference child items of canvas?

        }

        public void ResizeSurface()
        {

        }

        public void Populate()
        {
            PopulateSurface();
        }

        public void Refresh()
        {
            RefreshSurface();
        }

        private void RefreshSurface() // nothings changed
        {
            populating = true;
            thinLineWidth = (width / 4000) * zoomFactor;
            if (thinLineWidth < 0.75) thinLineWidth = 0.75;
            thickLineWidth = (width / 2000) * zoomFactor;
            if (thickLineWidth < 1) thickLineWidth = 1;
            standardRadius = (width / 500) * zoomFactor;
            foreach (VisioItem visioItem in VisioItems)
            {
                AddUpdateElement(visioItem);
            }
            //double zoomDelta = 1;
            //if (lastZoomFactor > 0)
            //    zoomDelta = zoomFactor / lastZoomFactor;
            //Point vPoint = new Point(zoomPoint.X * zoomDelta, zoomPoint.Y * zoomDelta);

            //if (zoomPoint != vPoint)
            //{
            //    double widthToMove = (vPoint.X - zoomPoint.X);
            //    double heightToMove = (vPoint.Y - zoomPoint.Y);

            //    if ((widthToMove != 0) || (heightToMove != 0))
            //    {
            //        surfaceViewer.ScrollToHorizontalOffset(surfaceViewer.HorizontalOffset + widthToMove);
            //        surfaceViewer.ScrollToVerticalOffset(surfaceViewer.VerticalOffset + heightToMove);
            //    }
            //}


            populating = false;
        }

        public VisioItem HitTest(Point point)
        {
            VisioItem item = null;
            HitTestResult result = VisualTreeHelper.HitTest(surfaceCanvas, point);
            if (result != null)
            {
                FrameworkElement hit = (FrameworkElement)result.VisualHit;
                if (hit is Shape)
                {
                    item = VisioItems.Find(vi => vi.Element == hit); 
                }
            }
            return item;
        }

        private void PopulateSurface()
        {
            populating = true;
            if (bgBmpSource != null)
            {
                Image myImage = new Image();
                myImage.Source = bgBmpSource;
                //myImage.Width = width * zoomFactor;
                //myImage.Height = height * zoomFactor;
                myImage.Width = bgBmpWidth * zoomFactor;
                myImage.Height = bgBmpHeight * zoomFactor;
                thinLineWidth = (width / 4000) * zoomFactor;
                if (thinLineWidth < 0.75) thinLineWidth = 0.75;
                thickLineWidth = (width / 2000) * zoomFactor;
                if (thickLineWidth < 1) thickLineWidth = 1;
                standardRadius = (width / 500) * zoomFactor;
                surfaceCanvas.Children.Clear();
                surfaceCanvas.Children.Add(myImage);
                surfaceCanvas.Width = width * zoomFactor;
                surfaceCanvas.Height = height * zoomFactor;

                foreach (VisioItem visioItem in VisioItems)
                {
                    visioItem.Element = null; //kill all orphaned children!
                    AddUpdateElement(visioItem);
                }
                //zoom on mouse calculation
                double zoomDelta=1;
                if (lastZoomFactor > 0)
                    zoomDelta = zoomFactor / lastZoomFactor;
                Point vPoint = new Point(zoomPoint.X * zoomDelta, zoomPoint.Y * zoomDelta);

                if (zoomPoint != vPoint)
                {
                    double widthToMove = (vPoint.X - zoomPoint.X);
                    double heightToMove = (vPoint.Y - zoomPoint.Y);

                    if ((widthToMove != 0) || (heightToMove != 0))
                    {
                        surfaceViewer.ScrollToHorizontalOffset(surfaceViewer.HorizontalOffset + widthToMove);
                        surfaceViewer.ScrollToVerticalOffset(surfaceViewer.VerticalOffset + heightToMove);
                    }
                }


                populating = false;
            }
        }

        private void GraphicSurface_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //needs to know in native coordinates what area it was looking at!
        }
	}

    public class VisioItem
    {
        public string ItemName { get; set; }
        public int ItemType { get; set; } //1:Shape, 2:Text -- refactor for enum.
        private int shapeID = -1;
        public int ShapeID
        {
            get { return shapeID; }
            set { shapeID = value; }
        }
        private int itemID = -1;
        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
       
        public double X { get; set; }
        public double Y { get; set; }
        public int Z { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        private Brush fillbrush = Brushes.Red;

        private FrameworkElement shape = null;

        public FrameworkElement Element
        {
            get { return shape; }
            set { shape = value; }
        }

        public Brush Fillbrush
        {
            get { return fillbrush; }
            set { fillbrush = value; }
        }
    }
}