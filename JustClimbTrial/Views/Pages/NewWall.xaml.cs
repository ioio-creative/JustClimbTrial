﻿using JustClimbTrial.DataAccess.Entities;
using JustClimbTrial.Helpers;
using JustClimbTrial.Kinect;
using JustClimbTrial.Mvvm.Infrastructure;
using JustClimbTrial.ViewModels;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JustClimbTrial.Views.Pages
{
    /// <summary>
    /// Interaction logic for NewWall.xaml
    /// </summary>
    public partial class NewWall : Page
    {
        #region constants

        private const string RockOverlapsWarningMsg = 
            "Please set a smaller rock size to avoid overlaps among rocks!";

        #endregion


        #region global objects & variables

        private SpaceMode _mode = SpaceMode.Color;

        // declare Kinect object and frame reader
        private KinectSensor kinectSensor;
        private MultiSourceFrameReader mulSourceReader;

        private float colorWidth, colorHeight, depthWidth, depthHeight;

        /// <summary>
        /// In NewWall mode, depthFrame must be used;
        /// Intermediate storage for the colorpoints to be mapped to depthframe
        /// </summary>
        private DepthSpacePoint[] colorMappedToDepthSpace;

        /// <summary>
        /// Instantaneous storage of frame data
        /// </summary>
        private ushort[] lastNotNullDepthData;
        private byte[] lastNotNullColorData;

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

        private IList<Body> _bodies;

        private bool _displayBody = false;
        //private bool _mirror = false;

        private KinectWall jcWall;

        private RocksOnWallViewModel rocksOnWallViewModel;

        private int newWallNo;

        private bool isSnapShotTaken = false;

        #endregion


        public NewWall()
        {
            // initialize Kinect object
            kinectSensor = KinectSensor.GetDefault();

            // activate sensor
            if (kinectSensor != null)
            {
                kinectSensor.Open();
                Console.WriteLine("Kinect activated!");

                mulSourceReader = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                mulSourceReader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

                colorWidth = KinectExtensions.FrameDimensions[SpaceMode.Color].Item1;
                colorHeight = KinectExtensions.FrameDimensions[SpaceMode.Color].Item2;
                depthWidth = KinectExtensions.FrameDimensions[SpaceMode.Depth].Item1;
                depthHeight = KinectExtensions.FrameDimensions[SpaceMode.Depth].Item2;

                colorMappedToDepthSpace = new DepthSpacePoint[(int)(colorWidth * colorHeight)];
                lastNotNullDepthData = new ushort[(int)depthWidth * (int)depthHeight];
                lastNotNullColorData = new byte[(int)colorWidth * (int)colorHeight * PixelFormats.Bgr32.BitsPerPixel / 8];

                bitmap = new WriteableBitmap((int)depthWidth, (int)depthHeight, 96.0, 96.0, PixelFormats.Bgra32, null);

                // Calculate the WriteableBitmap back buffer size
                bitmapBackBufferSize = (uint)((bitmap.BackBufferStride * (bitmap.PixelHeight - 1)) + (bitmap.PixelWidth * bytesPerPixel));
            }
            else
            {
                Console.WriteLine("Kinect not available!");
            }

            InitializeComponent();

            // set navHead
            newWallNo = WallDataAccess.LargestWallNo + 1;
            navHead.HeaderRowTitle = string.Format("Scan KinectWall - {0}", newWallNo);
            navHead.ParentPage = this;
        }

        private void InitialiseCommands()
        {
            snapshotWallBtn.Command = new RelayCommand(
                SnapShotWall, CanSnapShotWall);
            deselectRockBtn.Command = new RelayCommand(
                DeselectRock, CanDeselectRock);
            removeRockBtn.Command = new RelayCommand(
                RemoveRock, CanRemoveRock);
            removeAllRocksBtn.Command = new RelayCommand(
                RemoveAllRocks, CanRemoveAllRocks);            
            saveWallBtn.Command = new RelayCommand(
                SaveWall, CanSaveWall);
        }


        #region command methods executed when button clicked

        private bool CanSnapShotWall(object parameter = null)
        {
            return true;
        }

        private bool CanDeselectRock(object parameter = null)
        {
            return rocksOnWallViewModel.SelectedRock != null;
        }

        private bool CanRemoveRock(object parameter = null)
        {
            return rocksOnWallViewModel.SelectedRock != null;
        }

        private bool CanRemoveAllRocks(object parameter = null)
        {
            return rocksOnWallViewModel.AnyRocksInList();
        }

        private bool CanSaveWall(object parameter = null)
        {
            return rocksOnWallViewModel.AnyRocksInList();
        }

        private void SnapShotWall(object parameter = null)
        {
            isSnapShotTaken = jcWall.SnapShotWallData(
                colorMappedToDepthSpace, lastNotNullDepthData, lastNotNullColorData);
        }

        private void DeselectRock(object parameter = null)
        {
            rocksOnWallViewModel.DeselectRock();
        }

        private void RemoveRock(object parameter = null)
        {
            rocksOnWallViewModel.RemoveSelectedRock();
        }

        private void RemoveAllRocks(object parameter = null)
        {
            rocksOnWallViewModel.RemoveAllRocks();
        }

        private void SaveWall(object parameter = null)
        {
            rocksOnWallViewModel.SaveRocksOnWall(newWallNo.ToString());
        }

        #endregion


        #region event handlers

        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensor.Open();
            jcWall = new KinectWall(canvas, kinectSensor.CoordinateMapper);
            rocksOnWallViewModel = new RocksOnWallViewModel(canvas, kinectSensor.CoordinateMapper);

            InitialiseCommands();
        }

        private void Page_Unloaded(object sender, EventArgs e)
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
       
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSnapShotTaken)
            {
                Point mouseClickPt = e.GetPosition(cameraIMG);
                //ColorSpacePoint _boulderCSP = new ColorSpacePoint { X = (float)mouseClickPt.X, Y = (float)mouseClickPt.Y };

                RockViewModel rockCorrespondsToCanvasPt =
                    rocksOnWallViewModel.GetRockInListByCanvasPoint(mouseClickPt);

                if (rockCorrespondsToCanvasPt == null)  // new rock
                {
                    Size newBoulderSizeOnCanvas = GetNewBoulderSizeOnCanvasFromSliders();
                    
                    // check rock overlaps
                    if (rocksOnWallViewModel.IsOverlapWithRocksOnWall(
                            mouseClickPt, newBoulderSizeOnCanvas)
                        == false)
                    {
                        CameraSpacePoint csp = jcWall.GetCamSpacePointFromMousePoint(mouseClickPt, _mode);
                        if (!csp.Equals(default(CameraSpacePoint)))
                        {
                            rocksOnWallViewModel.AddRock(csp, newBoulderSizeOnCanvas);
                        }
                        else
                        {
                            UiHelper.NotifyUser("No depth info is captured for this point!");
                        }
                    }
                    else
                    {
                        UiHelper.NotifyUser(RockOverlapsWarningMsg);
                    }
                }
                else  // rock already in list
                {
                    rocksOnWallViewModel.SelectedRock = rockCorrespondsToCanvasPt;
                    boulderWidthSlider.Value = rockCorrespondsToCanvasPt.BoulderShape.Width;
                    boulderHeightSlider.Value = rockCorrespondsToCanvasPt.BoulderShape.Height;
                }
            }
            else
            {
                UiHelper.NotifyUser("Please take snap shot first.");
            }
        }

        private void boulderSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider boulderSizeSlider = sender as Slider;

            if (boulderSizeSlider.Value == 0)
            {
                UiHelper.NotifyUser("Zero size is not allowed.");
                boulderSizeSlider.Value = boulderSizeSlider.Minimum + boulderSizeSlider.TickFrequency;
            }

            if (rocksOnWallViewModel != null && rocksOnWallViewModel.SelectedRock != null)
            {
                Size newBoulderSizeOnCanvas = GetNewBoulderSizeOnCanvasFromSliders();

                // check rock overlaps
                if (rocksOnWallViewModel.IsOverlapWithRocksOnWallOtherThanSelectedRock(
                        rocksOnWallViewModel.SelectedRock.BCanvasPoint, newBoulderSizeOnCanvas)
                    == false)
                {
                    string boulderSizeSliderName = boulderSizeSlider.Name;
                    switch (boulderSizeSliderName)
                    {
                        case "boulderHeightSlider":
                            rocksOnWallViewModel.ChangeHeightOfSelectedRock(newBoulderSizeOnCanvas.Height);
                            break;
                        case "boulderWidthSlider":
                        default:
                            rocksOnWallViewModel.ChangeWidthOfSelectedRock(newBoulderSizeOnCanvas.Width);
                            break;
                    }
                }
                else
                {
                    UiHelper.NotifyUser(RockOverlapsWarningMsg);

                    // restore original value
                    boulderSizeSlider.Value -= boulderSizeSlider.TickFrequency;
                }
            }
        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var mSourceFrame = e.FrameReference.AcquireFrame();
            // If the Frame has expired by the time we process this event, return.
            if (mSourceFrame == null)
            {
                return;
            }

            DepthFrame depthFrame = null;
            ColorFrame colorFrame = mSourceFrame.ColorFrameReference.AcquireFrame();

            using (colorFrame)
            {
                if (colorFrame != null)
                {
                    cameraIMG.Source = KinectExtensions.ToBitmap(colorFrame);
                    colorFrame.CopyConvertedFrameDataToArray(lastNotNullColorData, ColorImageFormat.Bgra);

                    (Parent as MainWindow).PlaygroundWindow.ShowImage(cameraIMG.Source);
                }
                
                try
                {
                    depthFrame = mSourceFrame.DepthFrameReference.AcquireFrame();
                    if (depthFrame != null)
                    {                        
                        // Access the depth frame data directly via LockImageBuffer to avoid making a copy
                        using (KinectBuffer depthFrameData = depthFrame.LockImageBuffer())
                        {
                            kinectSensor.CoordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(
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


        #region slider value converters

        private static double ConvertSliderValueToSizeOnCanvas(double sliderValue)
        {
            double multiplicationFactor = 1;
            return multiplicationFactor * sliderValue;
        }

        private Size GetNewBoulderSizeOnCanvasFromSliders()
        {
            return new Size
            (
                ConvertSliderValueToSizeOnCanvas(boulderWidthSlider.Value),
                ConvertSliderValueToSizeOnCanvas(boulderHeightSlider.Value)
            );
        }

        #endregion
    }
}
