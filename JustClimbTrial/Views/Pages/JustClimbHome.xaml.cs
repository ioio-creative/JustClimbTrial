using System.Windows;
using System.Windows.Controls;

namespace JustClimbTrial.Views.Pages
{
    /// <summary>
    /// Interaction logic for JustClimbHome.xaml
    /// </summary>
    public partial class JustClimbHome : Page
    {
        public JustClimbHome()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ModeSelect modeSelectPage = new ModeSelect();
            this.NavigationService.Navigate(modeSelectPage);
        }
    }
}
