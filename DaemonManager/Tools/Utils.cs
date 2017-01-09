using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace DaemonManager.Tools
{
    public static class Utils
    {
     
        public static void KillAllButMe()
        {
            try
            {
                var current = Process.GetCurrentProcess();
                //kill any other manager that may be running
                var processes = Process.GetProcessesByName(current.ProcessName)
                    .Where(t => t.Id != current.Id)
                    .ToList();
                foreach (var process in processes)
                {
                    process.Kill();
                    process.WaitForExit();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static string GetDisplayAddress()
        {
            //if VMware or VMPlayer installed, we get the wrong address, so try getting the physical first.
            var address = GetPhysicalIpAdress();
            //Default since we couldn't.
            if (string.IsNullOrEmpty(address))
            {
                address = GetIPv4Address();
            }
            return address;
        }
        public static string GetPhysicalIpAdress()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr == null || addr.Address.ToString().Equals("0.0.0.0")) continue;
                if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }
            }
            return string.Empty;
        }


        private static string GetIPv4Address()
        {
            var ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var i in ips.Where(i => i.AddressFamily == AddressFamily.InterNetwork))
            {
                return i.ToString();
            }
            return "127.0.0.1";
        }

        public static string UlteriusPath = Environment.GetEnvironmentVariable("ulteriuspath", EnvironmentVariableTarget.Machine);
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
