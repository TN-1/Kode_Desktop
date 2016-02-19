using System;
using System.Linq;
using Microsoft.Win32;
using System.IO;

namespace koside
{
    class KSPPathTools
    {
        static public bool IsLinux()
        {
            var drives = DriveInfo.GetDrives();
            if (drives.Where(data => data.Name == "Z:\\").Count() == 1)
            {
                RegistryKey OurKey = Registry.CurrentUser;
                OurKey = OurKey.OpenSubKey(@"SOFTWARE\", false);
                RegistryKey key = OurKey.OpenSubKey("Wine");
                if (key != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        static public string WindowsSteam()
        {
            string steam = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", @"SteamPath", null);

            if (steam != null && Directory.Exists(steam))
                return NormalizeWindowsPath(steam);
            else
                return null;
        }

        static public string LinuxSteam()
        {
            if (Directory.Exists("steam"))
                return "steam";
            else
                return null;
        }

        public static string KSPDirectory(string steam)
        {
            string ksp_path;
            if (steam == null)
                return null;

            ksp_path = Path.Combine(steam, "SteamApps", "common", "Kerbal Space Program");
            if (Directory.Exists(ksp_path))
                return ksp_path;

            ksp_path = Path.Combine(steam, "steamapps", "common", "Kerbal Space Program");
            if (Directory.Exists(ksp_path))
                return ksp_path;

            return null;

        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/').TrimEnd('/');
        }

        private static string NormalizeWindowsPath(string path)
        {
            return path.Replace('/', '\\');
        }

    }
}
