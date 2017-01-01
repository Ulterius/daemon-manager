using System;
using System.Runtime.InteropServices;
using System.Windows;
using DaemonManager.Tools;

namespace DaemonManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
               Utils.RefreshTrayArea();
            }
            finally
            {
                base.OnExit(e);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Utils.RefreshTrayArea();
        }
    }
}
