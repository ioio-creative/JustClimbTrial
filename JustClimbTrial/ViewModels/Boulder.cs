using JustClimbTrial.Extensions;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JustClimbTrial.ViewModels
{
    public class Boulder
    {
        //index of Boulder to be stored to database
        private ushort _index;

        //cameraspace positions (x/y/z)
        private CameraSpacePoint bCamPoint;

        //2D positions (x/y), normalized [0,1]
        private Point bPoint;
        //scaled point wrt canvas dimensions (x/y)
        private Point bCanvasPoint;

        //dimensions of boulder (ellipse/rectangle), normalized [0, 1]
        private double bWidth;
        private double bHeight;

        private Shape boulderShape;
        private Canvas bCanvas;


        public ushort Index { get { return _index; } set { _index = value; } }
        public double BWidth { get { return bWidth; } set { bWidth = value; } }
        public double BHeight { get { return bHeight; } set { bHeight = value; } }
        public Point BPoint { get { return bPoint; } set { bPoint = value; } }
        public Point BCanvasPoint { get { return bCanvasPoint; } set { bCanvasPoint = value; } }
        public Shape BoulderShape { get { return boulderShape; } set { boulderShape = value; } }

        // derive quantities
        // non-normalised
        public double BCanvasWidth
        {
            get
            {
                return boulderShape.Width;
            }
        }

        public double BCanvasHeight
        {
            get
            {
                return boulderShape.Height;
            }            
        }


        // derived quantities
        // normalised
        private double smallX
        {
            get
            {
                return bPoint.X - bWidth * 0.5;
            }
        }

        private double largeX
        {
            get
            {
                return bPoint.X + bWidth * 0.5;
            }
        }

        private double smallY
        {
            get
            {
                return bPoint.Y - bHeight * 0.5;
            }
        }

        private double largeY
        {
            get
            {
                return bPoint.Y + bHeight * 0.5;
            }
        }


        #region Constructors

        // used only within methods in this class
        private Boulder() { }

        public Boulder(CameraSpacePoint camPoint, Point ptOnCanvas, 
            double widthOnCanvas, double heightOnCanvas, Canvas canvas)
        {
            bCanvas = canvas;
            bCamPoint = camPoint;
            bPoint = bCanvas.GetNormalisedPoint(ptOnCanvas);
            bWidth = bCanvas.GetNormalisedLengthWrtWidth(widthOnCanvas);
            bHeight = bCanvas.GetNormalisedLengthWrtHeight(heightOnCanvas);

            bCanvasPoint = ptOnCanvas;
            boulderShape = CreateEllipse();
        }

        public Boulder(float xP, float yP, float zP, Point ptOnCanvas, double widthOnCanvas, double heightOnCanvas, Canvas canvas)
        : this(new CameraSpacePoint { X = xP, Y = yP, Z = zP }, ptOnCanvas, widthOnCanvas, heightOnCanvas, canvas) { }

        public Boulder(DepthSpacePoint depPoint, ushort dep, Point ptOnCanvas, double widthOnCanvas, double heightOnCanvas, Canvas canvas, CoordinateMapper coMapper)
        : this(coMapper.MapDepthPointToCameraSpace(depPoint, dep), ptOnCanvas, widthOnCanvas, heightOnCanvas, canvas) { }

        #endregion


        public void ChangeBWidth(double widthOnCanvas)
        {
            bWidth = bCanvas.GetNormalisedLengthWrtWidth(widthOnCanvas);            
            RedrawBoulder();            
        }

        public void ChangeBHeight(double heightOnCanvas)
        {
            bHeight = bCanvas.GetNormalisedLengthWrtHeight(heightOnCanvas);            
            RedrawBoulder();            
        }
        
        // we assume the boulder is a rectangle to determine its area
        public bool IsCoincideWithCanvasPoint(Point canvasPoint)
        {
            Point normedCanvasPoint = bCanvas.GetNormalisedPoint(canvasPoint);
            return (bPoint.X - bWidth * 0.5 < normedCanvasPoint.X && normedCanvasPoint.X < bPoint.X + bWidth * 0.5)
                && (bPoint.Y - bHeight * 0.5 < normedCanvasPoint.Y && normedCanvasPoint.Y < bPoint.Y + bHeight * 0.5);

        }

        // we assume the boulder is a rectangle
        public bool IsOverlapWithAnotherBoulder(Boulder anotherBoulder)
        {
            bool isWidthCoincide = !((anotherBoulder.largeX < this.smallX) || (this.largeX < anotherBoulder.smallX));
            bool isHeightCoincide = !((anotherBoulder.largeY < this.smallY) || (this.largeY < anotherBoulder.smallY));
            return isWidthCoincide && isHeightCoincide;
        }

        // we assume the boulder is a rectangle
        public bool IsOverlapWithAnotherBoulder(Point rectangleOnCanvasPt, double rectangleOnCanvasWidth, double rectangleOnCanvasHeight)
        {
            Point rectangleOnCanvasNormedPt = bCanvas.GetNormalisedPoint(rectangleOnCanvasPt);
            double rectangleOnCanvasNormedWidth = bCanvas.GetNormalisedLengthWrtWidth(rectangleOnCanvasWidth);
            double rectangleOnCanvasNormedHeight = bCanvas.GetNormalisedLengthWrtWidth(rectangleOnCanvasHeight);
            return IsOverlapWithAnotherBoulder(new Boulder
            {
                BPoint = rectangleOnCanvasNormedPt,
                BWidth = rectangleOnCanvasNormedWidth,
                BHeight = rectangleOnCanvasNormedHeight
            });
        }
        

        #region draw helpers

        public void DrawBoulder()
        {
            SetBoulderTopLeftPositionOnCanvas();
            bCanvas.Children.Add(boulderShape);
        }

        private void RedrawBoulder()
        {
            boulderShape.Width = bCanvas.GetActualLengthWrtWidth(bWidth);
            boulderShape.Height = bCanvas.GetActualLengthWrtHeight(bHeight);

            SetBoulderTopLeftPositionOnCanvas();
        }

        public void UndrawBoulder()
        {
            bCanvas.Children.Remove(boulderShape);
            boulderShape = null;
        }

        private void SetBoulderTopLeftPositionOnCanvas()
        {
            Canvas.SetLeft(boulderShape, bCanvas.GetActualLengthWrtWidth(bPoint.X - bWidth * .5));
            Canvas.SetTop(boulderShape, bCanvas.GetActualLengthWrtHeight(bPoint.Y - bHeight * .5));
        }

        private Shape CreateEllipse()
        {
            Ellipse boulderEllipse = new Ellipse
            {
                Width = bCanvas.GetActualLengthWrtWidth(bWidth),
                Height = bCanvas.GetActualLengthWrtHeight(bHeight),
                Fill = null,
                StrokeThickness = 7,
                Stroke = new SolidColorBrush(Colors.DarkRed)
            };

            return boulderEllipse;
        }

        #endregion
    }
}
