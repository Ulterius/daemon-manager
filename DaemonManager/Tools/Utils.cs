using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DaemonManager.Tools
{
    public static class Utils
    {
        public static string UlteriusPath = System.Environment.GetEnvironmentVariable("ulteriuspath", EnvironmentVariableTarget.Machine);
        public static bool UlteriusInstalled()
        {
            if (string.IsNullOrEmpty(UlteriusPath))
            {
                return false;
            }
            var ulteriusDirectory = new DirectoryInfo(UlteriusPath);
            var files = ulteriusDirectory.GetFiles("*.exe");
            return files.Any(file => file.Name.Contains("Ulterius Server"));
        }

        public static bool ServerRunning()
        {
            return Process.GetProcessesByName("Ulterius Server").Length > 0;
        }


        public static bool AgentRunning()
        {

            return Process.GetProcessesByName("UlteriusAgent").Length > 0;
        }

        public static void KillUlterius()
        {
            var agentList = Process.GetProcessesByName("UlteriusAgent");
            foreach (var agent in agentList)
            {
                try
                {
                    agent.Kill();
                    agent.WaitForExit();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            var serverInstanceList = Process.GetProcessesByName("Ulterius Server");
            foreach (var server in serverInstanceList)
            {
                try
                {
                    server.Kill();
                    server.WaitForExit();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
           
        }
        public static bool RestartService()
        {
            if (!UlteriusInstalled()) return false;
            var restartServiceScript = Path.Combine(UlteriusPath,
                "restartservice.bat");
            var serviceRestartInfo = new ProcessStartInfo(restartServiceScript)
            {
                WindowStyle = ProcessWindowStyle.Minimized
            };

            var process = Process.Start(serviceRestartInfo);
            if (process == null) return false;
            process.WaitForExit();
            return true;
        }
    }
}
