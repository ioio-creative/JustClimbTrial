using JustClimbTrial.DataAccess;
using JustClimbTrial.DataAccess.Entities;
using JustClimbTrial.Globals;
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
    /// Interaction logic for RescanWall.xaml
    /// </summary>
    public partial class RescanWall : Page
    {
        #region Resource Keys
        
        private const string BtnRecordDemoTemplateResourceKey = "btnRecordDemoTemplate";
        private const string BtnDemoDoneTemplateResourceKey = "btnDemoDoneTemplate";

        #endregion


        #region private members

        private IList<Point> rockPoints = new List<Point>();

        #endregion


        #region constructor

        public RescanWall()
        {
            InitializeComponent();

            SetControlTemplateByResourceKey(ctrlBtnDemo, BtnRecordDemoTemplateResourceKey);

            navHead.ParentPage = this;
        }

        #endregion


        #region event handlers

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void canvasWall_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePoint = e.GetPosition(sender as Canvas);
            rockPoints.Add(mousePoint);
            DrawCircleOnCanvas(mousePoint);
        }

        private void btnRecordDemo_Click(object sender, RoutedEventArgs e)
        {
            SetControlTemplateByResourceKey(ctrlBtnDemo, BtnDemoDoneTemplateResourceKey);
        }

        private void btnDemoDone_Click(object sender, RoutedEventArgs e)
        {
            SetControlTemplateByResourceKey(ctrlBtnDemo, BtnRecordDemoTemplateResourceKey);

            if (rockPoints.Any())
            {
                Rock[] rocks = rockPoints.Select(rockPoint => new Rock
                {
                    CoorX = rockPoint.X,
                    CoorY = rockPoint.Y,
                    Radius = 5,
                    Wall = AppGlobal.WallID
                }).ToArray();

                RockDataAccess.InsertAll(rocks);
            }
        }

        #endregion


        #region helpers

        private void SetControlTemplateByResourceKey(Control ctrl, string resourceKey)
        {
            ctrl.Template = Resources[resourceKey] as ControlTemplate;
        }

        private void DrawCircleOnCanvas(Point position)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            int radius = 20;

            Ellipse myEllipse = new Ellipse
            {
                Fill = mySolidColorBrush,
                StrokeThickness = 2,
                Stroke = Brushes.Black,
                Width = radius * 2,
                Height = radius * 2
            };

            Canvas.SetLeft(myEllipse, position.X - radius);
            Canvas.SetTop(myEllipse, position.Y - radius);

            canvasWall.Children.Add(myEllipse);
        }

        #endregion
    }
}
