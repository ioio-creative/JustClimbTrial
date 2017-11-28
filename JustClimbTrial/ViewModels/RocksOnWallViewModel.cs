using JustClimbTrial.Extensions;
using JustClimbTrial.Kinect;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JustClimbTrial.ViewModels
{
    public class RocksOnWallViewModel
    {
        #region private members

        private IList<Boulder> rocksOnWall = new List<Boulder>();       
        private Canvas canvas;
        private Boulder selectedRock;        
        private Shape selectedRockIndicator;

        #endregion


        #region public members

        public Boulder SelectedRock
        {
            get { return selectedRock; }
            set
            {
                if (selectedRock != value)
                {
                    selectedRock = value;

                    // remove old selected rock indicator
                    if (selectedRockIndicator != null)
                    {
                        canvas.RemoveChild(selectedRockIndicator);
                    }

                    if (selectedRock != null)
                    {
                        // draw selected rock indicator
                        selectedRockIndicator = GetNewSelectedRockIndicator();
                        canvas.DrawShape(selectedRockIndicator, selectedRock.BCanvasPoint);
                    }
                }
            }
        }

        #endregion


        #region constructors

        public RocksOnWallViewModel(Canvas aCanvas)
        {
            canvas = aCanvas;
        }

        #endregion


        public Boulder AddRock(CameraSpacePoint camSpacePt, 
            Point canvasPt, double rockWidth, double rockHeight)
        {
            SelectedRock = new Boulder(camSpacePt, canvasPt, rockWidth,
                    rockHeight, canvas);
            rocksOnWall.Add(SelectedRock);
            SelectedRock.DrawBoulder();
            return SelectedRock;
        }

        public void RemoveRock(Point canvasPt)
        {
            Boulder rockToBeRemoved = GetRockInListByCanvasPoint(canvasPt);
            rocksOnWall.Remove(rockToBeRemoved);
        }

        public bool IsRockInList(Point canvasPt)
        {
            return GetRockInListByCanvasPoint(canvasPt) != null;
        }

        public Boulder GetRockInListByCanvasPoint(Point canvasPt)
        {
            Boulder requiredBoulder = null;

            foreach (Boulder rockOnWall in rocksOnWall)
            {
                if (rockOnWall.IsCanvasPointCoincide(canvasPt))                
                {
                    requiredBoulder = rockOnWall;
                    break;
                }
            }

            return requiredBoulder;
        }
        

        private static Shape GetNewSelectedRockIndicator()
        {
            double radius = 2;

            return new Ellipse
            {
                Fill = Brushes.Red,
                StrokeThickness = 0,
                Stroke = Brushes.Red,
                Width = radius * 2,
                Height = radius * 2
            };
        }
    }
}
