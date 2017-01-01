using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using DaemonManager.Tools;
using Hardcodet.Wpf.TaskbarNotification;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace DaemonManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Utils.RefreshTrayArea();
            Utils.KillAllButMe();
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1) return;
            InitializeComponent();
       
            var service = new Task(SetText);
            service.Start();
            this.Hide();
            TrayIcon.ShowBalloonTip("Ulterius", "Daemon manager started.", BalloonIcon.Info);

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
                    TrayIcon.ToolTipText = $"Server: {GetStatusString(Utils.ServerRunning())}\nAgent: {GetStatusString(Utils.AgentRunning())}";
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

            TrayIcon.ShowBalloonTip("Ulterius", "The server is not currently installed", BalloonIcon.Error);
            return true;
        }
        private void StateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowMissingError()) return;
            if (Utils.ServerRunning() || Utils.AgentRunning())
            {
                Utils.KillUlterius();
                TrayIcon.ShowBalloonTip("Ulterius", "The server has been stopped", BalloonIcon.Info);
            }
            else
            {
                Utils.RestartService();
                TrayIcon.ShowBalloonTip("Ulterius", "The server has been restarted", BalloonIcon.Info);

            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowMissingError()) return;
            Utils.RestartService();
            TrayIcon.ShowBalloonTip("Ulterius", "The server has been restarted", BalloonIcon.Info);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            var ip = Utils.GetDisplayAddress();
            var httpPort = 22006;
            Process.Start($"http://{ip}:{httpPort}");
        
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://ulterius.io/signup/");
        }

        private void Survey_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://ulterius.io/survey/");
        }

        private void Guide_CLick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://ulterius.io/guide/");
        }

        private void OpenGui(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;

        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            TrayIcon.Dispose();
            TrayIcon = null;
            Utils.RefreshTrayArea();
            Application.Current.Shutdown();
        }
    }
}
