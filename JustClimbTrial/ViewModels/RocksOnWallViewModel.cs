using JustClimbTrial.Extensions;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
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


        #region add / remove rock

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
            RemoveRock(rockToBeRemoved);
        }

        public void RemoveRock(Boulder rock)
        {
            rocksOnWall.Remove(rock);
            rock.UndrawBoulder();
        }

        public void RemoveAllRocks()
        {
            DeselectRock();

            if (AnyRocksInList())
            {
                foreach (Boulder rock in rocksOnWall)
                {
                    rock.UndrawBoulder();
                }

                rocksOnWall.Clear();
            }
        }

        #endregion


        #region check if rock is in list

        public bool AnyRocksInList()
        {
            return rocksOnWall.Any();
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
                if (rockOnWall.IsCoincideWithCanvasPoint(canvasPt))                
                {
                    requiredBoulder = rockOnWall;
                    break;
                }
            }

            return requiredBoulder;
        }

        public bool IsOverlapWithRocksOnWall(
            Point ptOnCanvas, double widthOnCanvas,
            double heightOnCanvas)
        {
            return IsOverlapWithRocksOnWallOtherThanSomePredicate(
                ptOnCanvas, widthOnCanvas,
                heightOnCanvas, (rock) => false);  // false means skipping no rocks in list during checking for overlap
        }

        public bool IsOverlapWithRocksOnWallOtherThanSelectedRock(
            Point ptOnCanvas, double widthOnCanvas,
            double heightOnCanvas)
        {
            return IsOverlapWithRocksOnWallOtherThanSomePredicate(
                ptOnCanvas, widthOnCanvas,
                heightOnCanvas, (rock) => rock.Equals(SelectedRock));
        }

        public bool IsOverlapWithRocksOnWallOtherThanSomePredicate(
            Point ptOnCanvas, double widthOnCanvas,
            double heightOnCanvas, Predicate<Boulder> rockToSkipCheckingPredicate)
        {
            if (!AnyRocksInList())
            {
                return false;
            }

            bool doOverlapsExist = false;
            foreach (Boulder rockOnWall in rocksOnWall)
            {
                if (rockToSkipCheckingPredicate(rockOnWall))
                {
                    continue;
                }

                if (rockOnWall.IsOverlapWithAnotherBoulder(
                        ptOnCanvas, widthOnCanvas,
                        heightOnCanvas))
                {
                    doOverlapsExist = true;
                    break;
                }
            }
            return doOverlapsExist;
        }        

        #endregion


        #region manipulate selected rock

        public void DeselectRock()
        {
            if (SelectedRock != null)
            {
                SelectedRock = null;  // this causes selectedRockIndicator to be removed from the canvas in the setter 
            }
        }

        public void RemoveSelectedRock()
        {
            if (SelectedRock != null)
            {
                RemoveRock(SelectedRock);
                DeselectRock();
            }
        }

        public void ChangeWidthOfSelectedRock(double newWidth)
        {
            if (SelectedRock != null)
            {
                SelectedRock.ChangeBWidth(newWidth);
            }
        }

        public void ChangeHeightOfSelectedRock(double newHeight)
        {
            if (SelectedRock != null)
            {
                SelectedRock.ChangeBHeight(newHeight);
            }
        }

        private static Shape GetNewSelectedRockIndicator()
        {
            double radius = 3;

            return new Ellipse
            {
                Fill = Brushes.Red,
                StrokeThickness = 0,
                Stroke = Brushes.Red,
                Width = radius * 2,
                Height = radius * 2
            };
        }

        #endregion
    }
}
