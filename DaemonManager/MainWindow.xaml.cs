using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using DaemonManager.Tools;

namespace DaemonManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var service = new Task(SetText);
            service.Start();
        }

        private void SetText()
        {
            while (true)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UlteriusInstalledLabel.Content = Utils.UlteriusInstalled() ? "Ulterius is Installed" : "Ulterius is Not Installed";
                    ServerStatusBlock.Text = $"Server: {GetStatusString(Utils.ServerRunning())}";
                    AgentStatusBlock.Text = $"Agent: {GetStatusString(Utils.AgentRunning())}";
                    StateButton.Content = Utils.ServerRunning() ? "Kill Ulterius" : "Start Ulterius";
                });

                Thread.Sleep(1000);
            }
          
        }

        private string GetStatusString(bool status)
        {
            return status ? "Running" : "Stopped";
        }

        private bool ShowMissingError()
        {
            if (Utils.UlteriusInstalled()) return false;
            MessageBox.Show("Ulterius is not currently installed.", "Uh Oh", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        private void StateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowMissingError()) return;
            if (Utils.ServerRunning() || Utils.AgentRunning())
            {
                Utils.KillUlterius();
                MessageBox.Show("Ulterius has been stopped", "Sucess", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                Utils.RestartService();
                MessageBox.Show("Ulterius has been restarted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowMissingError()) return;
            Utils.RestartService();
            MessageBox.Show("Ulterius has been restarted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
