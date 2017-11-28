using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JustClimbTrial.Extensions;
using System;

namespace JustClimbTrial.Kinect
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

        public Shape BoulderShape;
        private Canvas bCanvas;
        

        public ushort Index { get { return _index; } set { _index = value; } }
        public double BWidth { get { return bWidth; } set { bWidth = value; } }
        public double BHeight { get { return bHeight; } set { bHeight = value; } }
        public Point BPoint { get { return bPoint; } set { bPoint = value; } }
        public Point BCanvasPoint { get { return bCanvasPoint; } set { bCanvasPoint = value; } }


        #region Constructors
        public Boulder() { }

        public Boulder(CameraSpacePoint camPoint, Point ptOnCanvas, 
            double widthOnCanvas, double heightOnCanvas, Canvas canvas)
        {
            bCanvas = canvas;
            bCamPoint = camPoint;
            bPoint = bCanvas.GetNormalisedPoint(ptOnCanvas);
            bWidth = bCanvas.GetNormalisedLengthWrtWidth(widthOnCanvas);
            bHeight = bCanvas.GetNormalisedLengthWrtHeight(heightOnCanvas);

            bCanvasPoint = bCanvas.GetActualPoint(bPoint);
            BoulderShape = CreateEllipse();
        }

        public Boulder(float xP, float yP, float zP, Point ptOnCanvas, double widthOnCanvas, double heightOnCanvas, Canvas canvas)
        : this(new CameraSpacePoint { X = xP, Y = yP, Z = zP }, ptOnCanvas, widthOnCanvas, heightOnCanvas, canvas) { }

        public Boulder(DepthSpacePoint depPoint, ushort dep, Point ptOnCanvas, double widthOnCanvas, double heightOnCanvas, Canvas canvas, CoordinateMapper coMapper)
        : this(coMapper.MapDepthPointToCameraSpace(depPoint, dep), ptOnCanvas, widthOnCanvas, heightOnCanvas, canvas) { }

        #endregion
        
        private Shape CreateEllipse()
        {
            Ellipse boulderEllipse = new Ellipse
            {
                Width = bCanvas.GetActualLengthWrtWidth(bWidth),
                Height = bCanvas.GetActualLengthWrtHeight(bHeight),
                Fill = null,
                StrokeThickness = 5,
                Stroke = new SolidColorBrush(Colors.DarkRed)
            };
            
            return boulderEllipse;
        }

        public void DrawBoulder()
        {
            Canvas.SetLeft(BoulderShape, bCanvas.GetActualLengthWrtWidth(bPoint.X - bWidth * .5));
            Canvas.SetTop(BoulderShape, bCanvas.GetActualLengthWrtHeight(bPoint.Y - bHeight * .5));

            bCanvas.Children.Add(BoulderShape);
        }

        public void RedrawBoulder()
        {
            BoulderShape.Width = bCanvas.GetNormalisedLengthWrtWidth(bWidth);
            BoulderShape.Height = bCanvas.GetNormalisedLengthWrtHeight(bHeight);
        }

        public void UndrawBoulder()
        {
            bCanvas.Children.Remove(BoulderShape);
        }

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

        //we assume the boulder is a rectangle to determine its area
        public bool IsCanvasPointCoincide(Point canvasPoint)
        {
            Point normedCanvasPoint = bCanvas.GetNormalisedPoint(canvasPoint);
            return (bPoint.X - bWidth * 0.5 < normedCanvasPoint.X && normedCanvasPoint.X < bPoint.X + bWidth * 0.5)
                && (bPoint.Y - bHeight * 0.5 < normedCanvasPoint.Y && normedCanvasPoint.Y < bPoint.Y + bHeight * 0.5);

        }
    }
}
