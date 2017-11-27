using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;

namespace JustClimbTrial.Kinect
{
    public class Wall
    {
        public IList<Boulder> boulderList;
        public Boulder SelectedBoulder
        {
            get { return selectedBoulder; }
            set { selectedBoulder = value; }
        }


        private Boulder selectedBoulder;

        DepthSpacePoint[] dCoordinatesInColorFrame { get; set; }
        //ColorFrame wallColorFrame;
        //DepthFrame wallDepthFrame;
        ushort[] wallDepthData;
        byte[] wallBitmap;

        public bool IsSnapshotTaken = false;


        public Wall()
        {
            boulderList = new List<Boulder>();
        }        

        public Wall( List<Boulder> bList)
        {
            boulderList = bList;
        }

        public void ResetWall()
        {
            boulderList = new List<Boulder>();
        }

        public void SnapShotWallData(DepthSpacePoint[] colorSpaceMap, ushort[] dFrameData, byte[] colFrameData)
        {
            dCoordinatesInColorFrame = colorSpaceMap;
            ExportDCoordinatesFile();
            IsSnapshotTaken = true;
            
            wallDepthData = dFrameData;
            wallBitmap = colFrameData;
        }

        public bool AddBoulder(double mouseX, double mouseY, 
            double sliderRadius,
            double canvasWidth, double canvasHeight, 
            SpaceMode spMode, CoordinateMapper coMapper)
        {
            bool isAddBoulderSuccess = false;

            if (!IsSnapshotTaken)
            {
                return isAddBoulderSuccess;
            }

            DepthSpacePoint _boulderDSP = new DepthSpacePoint { X = float.NegativeInfinity, Y = float.NegativeInfinity };
            CameraSpacePoint _boulderCAMSP = new CameraSpacePoint();

            Tuple<float, float> dimensions = KinectExtensions.frameDimensions[spMode];
            float x_temp = (float)(mouseX * dimensions.Item1 / canvasWidth);
            float y_temp = (float)(mouseY * dimensions.Item2 / canvasHeight);

            _boulderDSP = dCoordinatesInColorFrame[(int)(x_temp + 0.5f) + (int)(y_temp + 0.5f) * (int)dimensions.Item1];

            ushort depth = 0;
            if (_boulderDSP.X != float.NegativeInfinity && _boulderDSP.Y != float.NegativeInfinity)
            {
                depth = wallDepthData[(int)_boulderDSP.X + (int)(_boulderDSP.Y) * (int)KinectExtensions.frameDimensions[SpaceMode.Depth].Item1];
                _boulderCAMSP = coMapper.MapDepthPointToCameraSpace(_boulderDSP, depth);
            }
            System.Console.WriteLine($"Position: Color[{(int)(x_temp + 0.5f)}][{(int)(y_temp + 0.5f)}] ==> Depth[{_boulderDSP.X}][{_boulderDSP.Y}]");
            System.Console.WriteLine($"New Boulder: x = {_boulderCAMSP.X}; y = {_boulderCAMSP.Y}; z = {_boulderCAMSP.Z}");

            if (boulderList != null)
            {
                //boulderList.Add(new Boulder(_boulderDSP, depth, 10, 10, coMapper) );

                
                boulderList.Add(new Boulder(_boulderCAMSP, sliderRadius, sliderRadius) );
            }
            else
            {
                boulderList = new List<Boulder>();
                //boulderList.Add(new Boulder(_boulderDSP, depth, 10, 10, coMapper));
                
                boulderList.Add(new Boulder(_boulderCAMSP, sliderRadius, sliderRadius));
            }

            isAddBoulderSuccess = true;
            return isAddBoulderSuccess;
        }

        private void ExportDCoordinatesFile()
        {
            // Set a variable to the My Documents path.
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string wallMapPath = mydocpath + "\\JustClimb\\Wall Log";
            if (!Directory.Exists(wallMapPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(wallMapPath);
                Console.WriteLine("Log directory created successfully at {0}.", Directory.GetCreationTime(wallMapPath));
            }
            // Write the text to a new file named "WriteFile.txt".
            File.WriteAllText(wallMapPath + "\\Coordinate Map.txt", "Wall Coordinates");
            // Append text to an existing file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(wallMapPath + "\\Coordinate Map.txt", true))
            {
                int colorWidth = (int)KinectExtensions.frameDimensions[SpaceMode.Color].Item1;
                int CPIndex = 0;
                foreach (DepthSpacePoint dPoint in dCoordinatesInColorFrame)
                {
                    // Append new lines of text to the file
                    outputFile.WriteLine($"Color[{CPIndex%colorWidth}][{CPIndex/colorWidth}] = Depth[{dPoint.X}][{dPoint.Y}]");
                    CPIndex++;
                }
            }
            

        }

        //public void SetDCoordinatesInColorFrame(DepthSpacePoint[] someDepthSpacePoints)
        //{
        //  dCoordinatesInColorFrame = someDepthSpacePoints;
        //}
    }
}
