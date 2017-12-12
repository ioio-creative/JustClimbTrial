using JustClimbTrial.Views.Windows;
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

namespace JustClimbTrial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        private Playground playgroundWindow;

        public Playground PlaygroundWindow
        {
            get { return playgroundWindow; }            
        }

        public MainWindow()
        {
            InitializeComponent();

            playgroundWindow = new Playground();
            playgroundWindow.Show();

<<<<<<< HEAD
=======
        private void NavigationWindow_Closed(object sender, EventArgs e)
        {
            KinectManagerClient.ColorImageSourceArrived -= HandleColorImageSourceArrived;
            KinectManagerClient.CloseKinect();
        }

        public void HandleColorImageSourceArrived(object sender, ColorImgSrcEventArgs e)
        {
            playgroundWindow.ShowImage( e.GetColorImgSrc() );
>>>>>>> parent of 5182c16... [171211]Video Player UI
        }
    }
}
