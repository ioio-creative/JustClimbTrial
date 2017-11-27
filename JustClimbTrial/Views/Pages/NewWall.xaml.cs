using JustClimbTrial.Kinect;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JustClimbTrial.Views.Pages
{
    /// <summary>
    /// Interaction logic for NewWall.xaml
    /// </summary>
    public partial class NewWall : Page
    {
        #region global objects & variables

        SpaceMode _mode = SpaceMode.Color;

        // declare Kinect object and frame reader
        KinectSensor kinectSensor;
        MultiSourceFrameReader mulSourceReader;

        CoordinateMapper coMapper;

        float colorWidth, colorHeight, depthWidth, depthHeight;

        /// <summary>
        /// In NewWall mode, depthFrame must be used;
        /// Intermediate storage for the colorpoints to be mapped to depthframe
        /// </summary>
        DepthSpacePoint[] colorMappedToDepthSpace;

        /// <summary>
        /// Instantaneous storage of frame data
        /// </summary>
        ushort[] lastNotNullDepthData;
        byte[] lastNotNullColorData;

        /// <summary>
        ///Bitmap to display
        /// </summary>
        private WriteableBitmap bitmap = null;

        /// <summary>
        /// The size in bytes of the bitmap back buffer
        /// </summary>
        private uint bitmapBackBufferSize = 0;

        /// <summary>
        /// Size of the RGB pixel in the bitmap
        /// </summary>
        private readonly int bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

        IList<Body> _bodies;

        bool _displayBody = false;
        //bool _mirror = false;

        Wall jcWall;

        #endregion
        

        public NewWall()
        {
            // initialize Kinect object
            kinectSensor = KinectSensor.GetDefault();

            // activate sensor
            if (kinectSensor != null)
            {
                kinectSensor.Open();
                System.Console.WriteLine("Kinect Activated");

                mulSourceReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                mulSourceReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

                coMapper = kinectSensor.CoordinateMapper;

                colorWidth = KinectExtensions.frameDimensions[SpaceMode.Color].Item1;
                colorHeight = KinectExtensions.frameDimensions[SpaceMode.Color].Item2;
                depthWidth = KinectExtensions.frameDimensions[SpaceMode.Depth].Item1;
                depthHeight = KinectExtensions.frameDimensions[SpaceMode.Depth].Item2;

                colorMappedToDepthSpace = new DepthSpacePoint[(int)(colorWidth * colorHeight)];
                lastNotNullDepthData = new ushort[(int)depthWidth * (int)depthHeight];
                lastNotNullColorData = new byte[(int)colorWidth * (int)colorHeight * PixelFormats.Bgr32.BitsPerPixel / 8];

                bitmap = new WriteableBitmap((int)depthWidth, (int)depthHeight, 96.0, 96.0, PixelFormats.Bgra32, null);

                // Calculate the WriteableBitmap back buffer size
                bitmapBackBufferSize = (uint)((bitmap.BackBufferStride * (bitmap.PixelHeight - 1)) + (bitmap.PixelWidth * bytesPerPixel));

            }

            InitializeComponent();
        }


        #region event handlers

        public void NewWall_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensor.Open();
            jcWall = new Wall();

        }

        private void NewWall_Unloaded(object sender, EventArgs e)
        {
            if (mulSourceReader != null)
            {
                // MultiSourceFrameReder is IDisposable
                mulSourceReader.Dispose();
                mulSourceReader = null;
            }

            if (kinectSensor != null)
            {
                kinectSensor.Close();
                kinectSensor = null;
            }
        }

        private void SaveWallBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(jcWall.SelectedBoulder.BoulderShape.Width.ToString());
        }

        private void RadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (radiusSlider.Value == 0)
            {
                MessageBox.Show("Zero radius is not allowed.");
                radiusSlider.Value = radiusSlider.Minimum + radiusSlider.TickFrequency;
            }            
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mouseClickPt = e.GetPosition(cameraIMG);
            //ColorSpacePoint _boulderCSP = new ColorSpacePoint { X = (float)mouseClickPt.X, Y = (float)mouseClickPt.Y };

            bool isAddBoulderSuccess = jcWall.AddBoulder(mouseClickPt.X, mouseClickPt.Y, radiusSlider.Value, canvas.ActualWidth, canvas.ActualHeight, _mode, coMapper);
            if (isAddBoulderSuccess)
            {
                jcWall.SelectedBoulder = jcWall.boulderList.Last();
                jcWall.SelectedBoulder.DrawBoulder(canvas, coMapper);
            }
            else
            {                
                MessageBox.Show("Please take snap shot first.");
            }
        }

        private void SnapshotWallBtn_Clicked(object sender, RoutedEventArgs e)
        {
            jcWall.ResetWall();
            jcWall.SnapShotWallData(colorMappedToDepthSpace, lastNotNullDepthData, lastNotNullColorData);
        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var mSourceFrame = e.FrameReference.AcquireFrame();
            // If the Frame has expired by the time we process this event, return.
            if (mSourceFrame == null) return;

            DepthFrame depthFrame = null;
            ColorFrame colorFrame = mSourceFrame.ColorFrameReference.AcquireFrame();

            using (colorFrame)
            {
                if (colorFrame != null)
                {
                    cameraIMG.Source = KinectExtensions.ToBitmap(colorFrame);
                    colorFrame.CopyConvertedFrameDataToArray(lastNotNullColorData, ColorImageFormat.Bgra);
                }



                try
                {
                    depthFrame = mSourceFrame.DepthFrameReference.AcquireFrame();
                    if (depthFrame != null)
                    {


                        // Access the depth frame data directly via LockImageBuffer to avoid making a copy
                        using (KinectBuffer depthFrameData = depthFrame.LockImageBuffer())
                        {
                            coMapper.MapColorFrameToDepthSpaceUsingIntPtr(
                                                 depthFrameData.UnderlyingBuffer,
                                                 depthFrameData.Size,
                                                 colorMappedToDepthSpace);

                            depthFrame.CopyFrameDataToArray(lastNotNullDepthData);
                        }
                        // We're done with the DepthFrame 
                        depthFrame.Dispose();
                        depthFrame = null;
                    }
                }
                finally
                {
                    if (depthFrame != null)
                    {
                        depthFrame.Dispose();
                    }

                    if (colorFrame != null)
                    {
                        colorFrame.Dispose();
                    }
                }

            }
        }

        #endregion


        //private Boulder GetSelectedBoulder()
        //{
        //    Boulder selectedBoulder = null;

        //    foreach (Boulder boulder in jcWall.boulderList)
        //    {               
        //        Point boulderPoint = boulder.MapCameraPointToCanvas(canvas, coMapper);

        //    }
        //}
    }
}
