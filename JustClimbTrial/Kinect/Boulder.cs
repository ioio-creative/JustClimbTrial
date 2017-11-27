using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JustClimbTrial.Kinect
{
    public class Boulder
    {
        //unit radius of boulder = 2cm
        private const double unitRadius = 1;
        //index of Boulder to be stored to database
        private ushort _index;
        //positions
        private float xPos;
        private float yPos;
        private float zPos;
        private CameraSpacePoint bCamPoint;

        //radii of boulder (ellipse)
        private double xr;
        private double yr;

        public Shape BoulderShape;
        

        public ushort Index { get { return _index; } set { _index = value; } }
        public float XPos { get { return xPos; } set { xPos = value; } }
        public float YPos { get { return yPos; } set { yPos = value; } }
        public double Xr { get { return xr; } set { xr = value; } }
        public double Yr { get { return yr; } set { yr = value; } }
        public float Depth { get { return zPos; } set { zPos = value; } }

        public Boulder() { }
        public Boulder(CameraSpacePoint camPoint, double sliderRX, double sliderRY) 
        {
            bCamPoint = camPoint;
            xPos = bCamPoint.X;
            yPos = bCamPoint.Y;
            zPos = bCamPoint.Z;
            xr = sliderRX * unitRadius;
            yr = sliderRY * unitRadius;

            //System.Console.WriteLine($"New Boulder: x = {xPos}; y = {yPos}; z = {zPos}");
        }

        public Boulder(float xP, float yP, float zP, double sliderRX, double sliderRY)
        : this(new CameraSpacePoint { X = xP, Y = yP, Z = zP }, sliderRX, sliderRY) { }

        public Boulder(DepthSpacePoint depPoint, ushort dep, double sliderRX, double sliderRY, CoordinateMapper coMapper)
        : this(coMapper.MapDepthPointToCameraSpace(depPoint, dep), sliderRX, sliderRY) { }

        public void DrawBoulder(Canvas canvas, CoordinateMapper coMapper)
        {
            Ellipse boulderEllipse = new Ellipse
            {
                Width = xr*2,
                Height = yr*2,
                Fill = null,
                StrokeThickness = 5,
                Stroke = new SolidColorBrush(Colors.DarkRed)
            };

            Point canvasPos = MapCameraPointToCanvas(canvas, coMapper);            
            Canvas.SetLeft(boulderEllipse, (int)(canvasPos.X - xr));
            Canvas.SetTop(boulderEllipse, (int)(canvasPos.Y - yr));

            canvas.Children.Add(boulderEllipse);

            BoulderShape = boulderEllipse;
        }

        public Point MapCameraPointToCanvas(Canvas canvas, CoordinateMapper coMapper)
        {
            ColorSpacePoint _camToColor = coMapper.MapCameraPointToColorSpace(bCamPoint);
            double canvasPosX = _camToColor.X * canvas.ActualWidth / KinectExtensions.frameDimensions[SpaceMode.Color].Item1;
            double canvasPosY = _camToColor.Y * canvas.ActualHeight / KinectExtensions.frameDimensions[SpaceMode.Color].Item2;
            return new Point(canvasPosX, canvasPosY);
        }

        //public bool IsCanvasPointCoincide(Point canvasPoint, Canvas canvas, CoordinateMapper coMapper)
        //{
        //    Point boulderOnCanvasPoint = MapCameraPointToCanvas(canvas, coMapper);
        //    return boulderOnCanvasPoint.X -
        //}
    }

 
}
