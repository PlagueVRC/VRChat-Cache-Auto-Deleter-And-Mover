using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace VRChat_Cache_Auto_Deleter
{
    public partial class CacheDeleter : Form
    {
        public CacheDeleter()
        {
            InitializeComponent();
        }

        public long LastKnownDriveSpace = 120;
        public long LastKnownCacheSize = 0;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                AutoDeleteTimer.Enabled = true;
                AutoDeleteTimer.Start();
            }
            else
            {
                AutoDeleteTimer.Stop();
                AutoDeleteTimer.Enabled = false;
            }
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Clears VRChat's Cache
        /// </summary>
        public void ClearVRCCache()
        {
            if (Directory.Exists(VRChatCacheDir))
            {
                foreach (var folder in Directory.GetDirectories(
                    VRChatCacheDir + "\\vrchat\\"))
                {
                    var DirInfo = new DirectoryInfo(folder);

                    switch (DirInfo.Name)
                    {
                        default:
                            MessageBox.Show(DirInfo.Name);
                            break;
                        case "Cookies":
                            try
                            {
                                Directory.Delete(folder, true);
                            }
                            catch
                            {

                            }
                            break;
                        case "Cache-WindowsPlayer":
                            try
                            {
                                Directory.Delete(folder, true);
                            }
                            catch
                            {

                            }
                            break;
                        case "HTTPCache-WindowsPlayer":
                            try
                            {
                                Directory.Delete(folder, true);
                            }
                            catch
                            {

                            }
                            break;
                        case "Unity":
                            try
                            {
                                Directory.Delete(folder, true);
                            }
                            catch
                            {

                            }
                            break;
                        case "VRCHTTPCache":
                            try
                            {
                                Directory.Delete(folder, true);
                            }
                            catch
                            {

                            }
                            break;
                    }
                }
            }
        }

        private void AutoDeleteTimer_Tick(object sender, EventArgs e)
        {
            var Drive = DriveInfo.GetDrives().First(o => o.Name == @"C:\");

            if (Drive != null)
            {
                if (Drive.AvailableSpaceInGB() == 5 || VRChatCacheDir.GetDirectorySizeInGB() > 19) // Drive Space Is Less Than 5GB Or Cache Is Full
                {
                    AutoDeleteTimer.Stop();
                    AutoDeleteTimer.Enabled = false;

                    ClearVRCCache();

                    AutoDeleteTimer.Enabled = true;
                    AutoDeleteTimer.Start();
                }
            }

            label2.Text =
                $"Last Known Free Drive Space: {Drive.AvailableSpaceInGB()}GB - Last Known Cache Size: {VRChatCacheDir.GetDirectorySizeInGB()}GB";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsAdministrator() && comboBox2.Text.Contains(@":\"))
            {
                try
                {
                    Enabled = false;
                    Directory.CreateDirectory(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\");

                    if (Directory.Exists(VRChatCacheDir))
                    {
                        CopyFolder(VRChatCacheDir, Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\VRChat\\");
                    }

                    if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\"))
                    {
                        Directory.Delete(VRChatCacheDir, true);
                    }

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/c mklink /d " + VRChatCacheDir + " " + Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\VRChat\\";
                    process.StartInfo = startInfo;
                    process.Start();

                    Enabled = true;
                }
                catch (DirectoryNotFoundException)
                {
                    Enabled = true;
                    MessageBox.Show("You Do Not Have A VRChat Cache Or You Have Already Moved It Before.");
                }
            }
            else if (IsAdministrator() == true && !comboBox2.Text.Contains(@":\"))
            {
                MessageBox.Show("You Must Select A Drive To Proceed!");
            }
            else if (IsAdministrator() == false)
            {
                Enabled = true;
                MessageBox.Show("This Tool MUST Be Ran As Administrator For This To Work!");
            }
        }

        public string VRChatCacheDir =
            (Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) +
             "\\LocalLow\\VRChat");

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                comboBox2.Items.Add(drive.Name);
            }

            try
            {
                VRChatCacheDir = GetRealPath(VRChatCacheDir);
            }
            catch
            {
                VRChatCacheDir =
                    (Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) +
                     "\\LocalLow\\VRChat");
            }

            if (!IsAdministrator())
            {
                button1.Enabled = false;
                button2.Enabled = false;
                comboBox2.Enabled = false;
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr securityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetFinalPathNameByHandle([In] SafeFileHandle hFile, [Out] StringBuilder lpszFilePath, [In] int cchFilePath, [In] int dwFlags);

        private const int CREATION_DISPOSITION_OPEN_EXISTING = 3;
        private const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        public static string GetRealPath(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                throw new IOException("Path not found");
            }

            SafeFileHandle directoryHandle = CreateFile(path, 0, 2, IntPtr.Zero, CREATION_DISPOSITION_OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero); //Handle file / folder

            if (directoryHandle.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            StringBuilder result = new StringBuilder(512);
            int mResult = GetFinalPathNameByHandle(directoryHandle, result, result.Capacity, 0);

            if (mResult < 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (result.Length >= 4 && result[0] == '\\' && result[1] == '\\' && result[2] == '?' && result[3] == '\\')
            {
                return result.ToString().Substring(4); // "\\?\" remove
            }
            return result.ToString();
        }

        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IsAdministrator() && Directory.Exists(VRChatCacheDir) && comboBox2.Text.Contains(@":\"))
            {
                Enabled = false;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/c rmdir " + VRChatCacheDir;
                process.StartInfo = startInfo;
                process.Start();

                if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\VRChat\\"))
                {
                    CopyFolder(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\VRChat\\", VRChatCacheDir);
                }

                Enabled = true;
            }
            else if (IsAdministrator() == true && !comboBox2.Text.Contains(@":\"))
            {
                MessageBox.Show("You Must Select A Drive To Proceed!");
            }
            else if (IsAdministrator() == true)
            {
                Enabled = true;
                MessageBox.Show("VRChat Cache Does Not Exist, Or Is Not A Redirect Link");
            }
            else
            {
                Enabled = true;
                MessageBox.Show("This Tool MUST Be Ran As Administrator For This To Work!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (string file in Directory.GetFiles(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\", "output_log_*.txt"))
            {
                File.Delete(file);
            }

            ClearVRCCache();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\HTTPCache-WindowsPlayer"))
            {
                Directory.Delete(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\HTTPCache-WindowsPlayer", true);
            }
        }
    }

    public static class Extensions
    {
        public static long AvailableSpaceInMB(this DriveInfo drive)
        {
            return (drive.AvailableFreeSpace / 1048576);
        }

        public static long AvailableSpaceInGB(this DriveInfo drive)
        {
            return (drive.AvailableFreeSpace / 1073741824);
        }

        public static long GetDirectorySizeInGB(this string Dir)
        {
            return GetDirectorySize(Dir) / 1073741824;
        }

        public static long GetDirectorySize(this string Dir)
        {
            var Info = new DirectoryInfo(Dir);

            var startDirectorySize = default(long);

            if (Info == null || !Info.Exists)
            {
                return startDirectorySize; //Return 0 while Directory does not exist.
            }

            //Add size of files in the Current Directory to main size.
            foreach (var fileInfo in Info.GetFiles("*.*", SearchOption.AllDirectories))
            {
                startDirectorySize += fileInfo.Length;
            }

            return startDirectorySize;  //Return full Size of this Directory.
        }
    }
}
