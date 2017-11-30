using JustClimbTrial.DataAccess;
using JustClimbTrial.DataAccess.Entities;
using JustClimbTrial.Enums;
using JustClimbTrial.Extensions;
using JustClimbTrial.Globals;
using JustClimbTrial.Helpers;
using JustClimbTrial.Mvvm.Infrastructure;
using JustClimbTrial.ViewModels;
using JustClimbTrial.Views.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JustClimbTrial.Views.Pages
{
    /// <summary>
    /// Interaction logic for RouteSet.xaml
    /// </summary>
    public partial class RouteSet : Page
    {
        #region resource keys

        private const string TrainingRockStatusTemplateResourceKey = "trainingRockStatusTemplate";
        private const string BoulderRockStatusTemplateResourceKey = "boulderRockStatusTemplate";

        private const string BtnRecordDemoTemplateResourceKey = "btnRecordDemoTemplate";
        private const string BtnDemoDoneTemplateResourceKey = "btnDemoDoneTemplate";

        #endregion        


        #region private members

        private ClimbMode routeSetClimbMode;
        private RouteSetViewModel viewModel;
        private int newRouteNo;
        private IEnumerable<Rock> rocksOnWall;
        private IList<RockOnRouteViewModel> rocksOnRoute = new List<RockOnRouteViewModel>();
        private RockOnRouteViewModel _selectedRockOnRoute;
        private Ellipse selectedRockIndicator;

        #endregion


        private RockOnRouteViewModel selectedRockOnRoute
        {
            get { return _selectedRockOnRoute; }
            set
            {
                if (_selectedRockOnRoute != value)
                {
                    _selectedRockOnRoute = value;

                    // remove old selected rock indicator               
                    if (selectedRockIndicator != null)
                    {
                        RemoveShapeFromCanvas(selectedRockIndicator);
                    }

                    if (_selectedRockOnRoute != null)
                    {
                        // draw selected rock indicator
                        selectedRockIndicator = GetNewSelectedRockIndicatorCircle();
                        DrawCircleOnCanvas(selectedRockIndicator,
                            _selectedRockOnRoute.MyRock.CoorX.GetValueOrDefault(0),
                            _selectedRockOnRoute.MyRock.CoorY.GetValueOrDefault(0));
                    }
                }
            }
        }


        public RouteSet() : this(ClimbMode.Boulder) { }

        public RouteSet(ClimbMode aClimbMode)
        {
            routeSetClimbMode = aClimbMode;

            InitializeComponent();

            viewModel = DataContext as RouteSetViewModel;
            if (viewModel != null)
            {
                viewModel.SetClimbMode(aClimbMode);
            }

            // set titles
            string titleFormat = "Just Climb - {0} Route Set";
            string headerRowTitleFormat = "Route Set - {0} {1}";
            string rockStatusTemplateResourceKey;            

            switch (aClimbMode)
            {
                case ClimbMode.Training:
                    newRouteNo = TrainingRouteDataAccess.LargestTrainingRouteNo + 1;
                    Title = string.Format(titleFormat, "Training");
                    navHead.HeaderRowTitle =
                        string.Format(headerRowTitleFormat, "Training", newRouteNo);
                    rockStatusTemplateResourceKey = TrainingRockStatusTemplateResourceKey;
                    break;
                case ClimbMode.Boulder:
                default:
                    newRouteNo = BoulderRouteDataAccess.LargestBoulderRouteNo + 1;
                    Title = string.Format(titleFormat, "Boulder");
                    navHead.HeaderRowTitle =
                        string.Format(headerRowTitleFormat, "Boulder", newRouteNo);                            
                    rockStatusTemplateResourceKey = BoulderRockStatusTemplateResourceKey;
                    break;                                    
            }

            WindowTitle = Title;            
            SetTemplateOfControlFromResource(ctrlBtnDemo, BtnRecordDemoTemplateResourceKey);
            SetTemplateOfControlFromResource(ctrlRockStatus, rockStatusTemplateResourceKey);            

            navHead.ParentPage = this;                              
        }

        // !!! Important !!!
        // don't call this method in page's constructor
        // call it in the page_load event
        // https://stackoverflow.com/questions/21482291/access-textbox-from-controltemplate-in-usercontrol-resources
        private void SetUpBtnCommandsInRockStatusUserControls()
        {
            switch (routeSetClimbMode)
            {
                case ClimbMode.Training:
                    break;
                case ClimbMode.Boulder:
                default:
                    BoulderRockStatus ucboulderRockStatus = GetBoulderRockStatusUserControl();
                    ucboulderRockStatus.btnStartStatus.Command =
                        new RelayCommand(SetSelectedBoulderRockToStart, CanSetSelectedBoulderRockToStart);
                    ucboulderRockStatus.btnEndStatus.Command =
                        new RelayCommand(SetSelectedBoulderRockToEnd, CanSetSelectedBoulderRockToEnd);
                    ucboulderRockStatus.btnIntermediateStatus.Command =
                        new RelayCommand(SetSelectedBoulderRockToIntermediate, CanSetSelectedBoulderRockToIntermediate);
                    ucboulderRockStatus.btnNoneStatus.Command =
                        new RelayCommand(RemoveSelectedBoulderRockFromRoute, CanRemoveSelectedBoulderRockFromRoute);
                    break;
            }
        }


        #region event handlers

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadData();
            rocksOnWall = RockDataAccess.ValidRocksOnWall(AppGlobal.WallID);

            if (rocksOnWall.Any())
            {
                DrawRocksOnWallOnCanvas(rocksOnWall);
            }
            else
            {
                UiHelper.NotifyUser("No rocks registered with the wall!");
            }

            SetUpBtnCommandsInRockStatusUserControls();
        }

        private void canvasWall_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePoint = e.GetPosition(sender as Canvas);
            Rock nearestRockOnWall = FindNearestRockOnWallFromTouchPoint(mousePoint);
            if (nearestRockOnWall != null)
            {
                selectedRockOnRoute = FindRockOnRouteViewModel(nearestRockOnWall);
                bool isRockAlreadyOnTheRoute = selectedRockOnRoute != null;

                if (routeSetClimbMode == ClimbMode.Training)
                {

                }
                else if (routeSetClimbMode == ClimbMode.Boulder)
                {
                    if (!isRockAlreadyOnTheRoute)  // new rock on route
                    {
                        selectedRockOnRoute = new RockOnRouteViewModel
                        {
                            MyRock = nearestRockOnWall
                        };
                        rocksOnRoute.Add(selectedRockOnRoute);
                        SetSelectedBoulderRockToIntermediate();
                    }
                }                
            }
        }

        private void btnRecordDemo_Click(object sender, RoutedEventArgs e)
        {
            SetTemplateOfControlFromResource(ctrlBtnDemo, BtnDemoDoneTemplateResourceKey);
        }

        private void btnDemoDone_Click(object sender, RoutedEventArgs e)
        {
            SetTemplateOfControlFromResource(ctrlBtnDemo, BtnRecordDemoTemplateResourceKey);
            
            if (routeSetClimbMode == ClimbMode.Boulder)
            {
                if (rocksOnRoute.Any())
                {
                    BoulderRoute newBoulderRoute = CreateBoulderRouteFromUi();

                    BoulderRouteAndRocksDataAccess.InsertRouteAndRocksOnRoute(
                        newBoulderRoute, rocksOnRoute, true);                        
                }
            }
        }

        #endregion


        #region command methods for BoulderRockStatus UserControl

        private bool CanSetSelectedBoulderRockToStart(object parameter = null)
        {            
            return selectedRockOnRoute != null;
        }

        private bool CanSetSelectedBoulderRockToIntermediate(object parameter = null)
        {            
            return selectedRockOnRoute != null;
        }

        private bool CanSetSelectedBoulderRockToEnd(object parameter = null)
        {            
            return selectedRockOnRoute != null;
        }

        private bool CanRemoveSelectedBoulderRockFromRoute(object parameter = null)
        {            
            return selectedRockOnRoute != null && IsRockOnTheRoute(selectedRockOnRoute.MyRock);
        }

        private void SetSelectedBoulderRockToStart(object parameter = null)
        {
            SetSelectedBoulderRockStatus(RockOnBoulderStatus.Start);
        }

        private void SetSelectedBoulderRockToIntermediate(object parameter = null)
        {
            SetSelectedBoulderRockStatus(RockOnBoulderStatus.Int);
        }

        private void SetSelectedBoulderRockToEnd(object parameter = null)
        {
            SetSelectedBoulderRockStatus(RockOnBoulderStatus.End);
        }

        private void SetSelectedBoulderRockStatus(RockOnBoulderStatus status)
        {
            if (selectedRockOnRoute != null)
            {
                // if selectedRockOnRoute not already in rocksOnRoute,
                // add it into the rocksOnRoute list
                if (!rocksOnRoute.Contains(selectedRockOnRoute))
                {
                    rocksOnRoute.Add(selectedRockOnRoute);
                }

                if (selectedRockOnRoute.ShapeOnCanvas == null || selectedRockOnRoute.BoulderStatus != status)
                {
                    RemoveRockShapeFromCanvas(selectedRockOnRoute);
                    selectedRockOnRoute.BoulderStatus = status;
                    selectedRockOnRoute.ShapeOnCanvas = DrawBoulderRockOnCanvas(selectedRockOnRoute);
                }
            }
        }

        private void RemoveSelectedBoulderRockFromRoute(object parameter = null)
        {
            rocksOnRoute.Remove(selectedRockOnRoute);
            RemoveRockShapeFromCanvas(selectedRockOnRoute);
        }

        #endregion


        #region rock helpers

        // by position
        private Rock FindNearestRockOnWallFromTouchPoint(Point touchPt)
        {
            if (!rocksOnWall.Any())
            {
                return null;
            }

            foreach (Rock rock in rocksOnWall)
            {
                // TODO: change find nearest rock logic
                double rockRadius = Math.Max(rock.Width.GetValueOrDefault(0), rock.Height.GetValueOrDefault(0)) * 0.5;
                if ((rock.GetPoint() - touchPt).LengthSquared < rockRadius * rockRadius)
                {
                    return rock;
                }
            }

            return null;
        }

        // by rock id
        private bool IsRockOnTheRoute(Rock selectedRock)
        {
            return FindRockOnRouteViewModel(selectedRock) != null;
        }        

        // by rock id
        private RockOnRouteViewModel FindRockOnRouteViewModel(Rock selectedRock)
        {
            if (!rocksOnRoute.Any())
            {
                return null;
            }

            IEnumerable<RockOnRouteViewModel> selectedRockOnRouteViewModels =
                   rocksOnRoute.Where(x => x.MyRock.RockID == selectedRock.RockID);

            if (selectedRockOnRouteViewModels.Any())
            {
                return selectedRockOnRouteViewModels.Single();
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region control template helpers

        private void SetTemplateOfControlFromResource(Control ctrl, string resourceKey)
        {
            ctrl.Template = GetControlTemplateFromResource(resourceKey);
        }

        private ControlTemplate GetControlTemplateFromResource(string resourceKey)
        {
            return Resources[resourceKey] as ControlTemplate;
        }

        // https://stackoverflow.com/questions/8126700/how-do-i-access-an-element-of-a-control-template-from-within-code-behind
        private BoulderRockStatus GetBoulderRockStatusUserControl()
        {
            ControlTemplate template = ctrlRockStatus.Template;            
            return template.FindName("ucBoulderRockStatus", ctrlRockStatus) as BoulderRockStatus;
        }

        private TrainingRockStatus GetTrainingRockStatusUserControl()
        {
            ControlTemplate template = ctrlRockStatus.Template;
            return template.FindName("ucTrainingRockStatus", ctrlRockStatus) as TrainingRockStatus;
        }

        #endregion


        #region draw helpers        

        private void DrawRocksOnWallOnCanvas(IEnumerable<Rock> rocks)
        {
            if (rocks.Any())
            {
                foreach (Rock rock in rocks)
                {
                    DrawRockOnWallOnCanvas(rock);
                }
            }
        }

        private Shape DrawRockOnWallOnCanvas(Rock rock)
        {
            // TODO: change draw ellipse logic
            double radius = Math.Max(rock.Width.GetValueOrDefault(0), rock.Height.GetValueOrDefault(0));
            Ellipse rockOnWallCircle = GetNewRockOnWallCircle(radius);
            DrawCircleOnCanvas(rockOnWallCircle, rock.CoorX.GetValueOrDefault(0), rock.CoorY.GetValueOrDefault(0));
            return rockOnWallCircle;
        }

        private Shape DrawBoulderRockOnCanvas(RockOnRouteViewModel rockOnBoulderRoute)
        {
            Shape shapeToReturn;
            switch (rockOnBoulderRoute.BoulderStatus)
            {
                case RockOnBoulderStatus.Start:
                    shapeToReturn = DrawStartRockOnCanvas(rockOnBoulderRoute.MyRock);
                    break;
                case RockOnBoulderStatus.End:
                    shapeToReturn = DrawEndRockOnCanvas(rockOnBoulderRoute.MyRock);
                    break;
                case RockOnBoulderStatus.Int:
                default:
                    shapeToReturn = DrawIntermediateRockOnCanvas(rockOnBoulderRoute.MyRock);
                    break;
            }
            return shapeToReturn;
        }

        private Shape DrawStartRockOnCanvas(Rock rock)
        {
            // TODO: change draw ellipse logic
            double radius = Math.Max(rock.Width.GetValueOrDefault(0), rock.Height.GetValueOrDefault(0));
            Ellipse startRockCircle = GetNewStartRockCircle(radius);
            DrawCircleOnCanvas(startRockCircle, rock.CoorX.GetValueOrDefault(0), rock.CoorY.GetValueOrDefault(0));
            return startRockCircle;
        }

        private Shape DrawIntermediateRockOnCanvas(Rock rock)
        {
            // TODO: change draw ellipse logic
            double radius = Math.Max(rock.Width.GetValueOrDefault(0), rock.Height.GetValueOrDefault(0));
            Ellipse intermediateRockCircle = GetNewIntermediateRockCircle(radius);
            DrawCircleOnCanvas(intermediateRockCircle, rock.CoorX.GetValueOrDefault(0), rock.CoorY.GetValueOrDefault(0));
            return intermediateRockCircle;
        }

        private Shape DrawEndRockOnCanvas(Rock rock)
        {
            // TODO: change draw ellipse logic
            double radius = Math.Max(rock.Width.GetValueOrDefault(0), rock.Height.GetValueOrDefault(0));
            Ellipse endRockCircle = GetNewEndRockCircle(radius);
            DrawCircleOnCanvas(endRockCircle, rock.CoorX.GetValueOrDefault(0), rock.CoorY.GetValueOrDefault(0));
            return endRockCircle;
        }

        private void DrawCircleOnCanvas(Ellipse circle, Point position)
        {
            DrawCircleOnCanvas(circle, position.X, position.Y);
        }

        private void DrawCircleOnCanvas(Ellipse circle, double x, double y)
        {
            double radius = circle.SemiMajorAxis(); 
            
            Canvas.SetLeft(circle, x - radius);
            Canvas.SetTop(circle, y - radius);

            canvasWall.Children.Add(circle);            
        }

        private void RemoveRockShapeFromCanvas(RockOnRouteViewModel rockOnRoute)
        {
            RemoveShapeFromCanvas(rockOnRoute.ShapeOnCanvas);
        }

        private void RemoveShapeFromCanvas(Shape aShape)
        {
            canvasWall.Children.Remove(aShape);
        }

        #endregion        


        #region circles

        private static Ellipse GetNewRockOnWallCircle(double radius)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(123, 255, 255, 0);
            return new Ellipse
            {
                Fill = mySolidColorBrush,
                StrokeThickness = 2,
                Stroke = Brushes.Black,
                Width = radius * 2,
                Height = radius * 2
            };
        }

        private static Ellipse GetNewStartRockCircle(double radius)
        {
            return new Ellipse
            {
                Fill = Brushes.Transparent,
                StrokeThickness = 4,
                Stroke = Brushes.Green,
                Width = radius * 2,
                Height = radius * 2
            };
        }

        private static Ellipse GetNewIntermediateRockCircle(double radius)
        {
            return new Ellipse
            {
                Fill = Brushes.Transparent,
                StrokeThickness = 4,
                Stroke = Brushes.Yellow,
                Width = radius * 2,
                Height = radius * 2
            };
        }

        private static Ellipse GetNewEndRockCircle(double radius)
        {
            return new Ellipse
            {
                Fill = Brushes.Transparent,
                StrokeThickness = 4,
                Stroke = Brushes.Red,
                Width = radius * 2,
                Height = radius * 2
            };
        }

        private static Ellipse GetNewSelectedRockIndicatorCircle()
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

        #endregion


        #region retrieve data from UI helpers

        private class RouteFromUiModel
        {
            public string AgeGroup { get; set; }
            public string Difficulty { get; set; }
            public string RouteNo { get; set; }
            public string Wall { get; set; }
        }

        private RouteFromUiModel CreateRouteFromUiModel()
        {
            return new RouteFromUiModel
            {
                AgeGroup = (ddlAge.SelectedItem as AgeGroup).AgeGroupID,
                Difficulty = (ddlDifficulty.SelectedItem as RouteDifficulty).RouteDifficultyID,
                RouteNo = newRouteNo.ToString(),
                Wall = AppGlobal.WallID
            };
        }

        private BoulderRoute CreateBoulderRouteFromUi()
        {
            RouteFromUiModel routeModel = CreateRouteFromUiModel();
            return new BoulderRoute
            {
                AgeGroup = routeModel.AgeGroup,
                Difficulty = routeModel.Difficulty,
                RouteNo = routeModel.RouteNo,
                Wall = routeModel.Wall
            };
        }

        private TrainingRoute CreateTrainingRouteFromUi()
        {
            RouteFromUiModel routeModel = CreateRouteFromUiModel();
            return new TrainingRoute
            {
                AgeGroup = routeModel.AgeGroup,
                Difficulty = routeModel.Difficulty,
                RouteNo = routeModel.RouteNo,
                Wall = routeModel.Wall
            };
        }

        #endregion
    }
}
