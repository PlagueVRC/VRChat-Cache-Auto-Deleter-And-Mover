using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;

namespace VRChat_Cache_Auto_Deleter
{
    public partial class CacheDeleter : Form
    {
        public CacheDeleter()
        {
            InitializeComponent();
        }

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
            if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\"))
            {
                foreach (var folder in Directory.GetDirectories(
                                Directory.GetParent(
                                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) +
                                "\\LocalLow\\VRChat\\vrchat\\"))
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
                if (Drive.AvailableSpaceInGB() == 5 || (Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\").GetDirectorySizeInGB() > 19) // Drive Space Is Less Than 5GB Or Cache Is Full
                {
                    ClearVRCCache();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsAdministrator() && comboBox2.Text.Contains(@":\"))
            {
                try
                {
                    Enabled = false;
                    Directory.CreateDirectory(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\");

                    if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\"))
                    {
                        CopyFolder(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\", Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\VRChat\\");
                    }

                    if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\"))
                    {
                        Directory.Delete(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\", true);
                    }

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/c mklink /d " + Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\" + " " + Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\VRChat\\";
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

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                comboBox2.Items.Add(drive.Name);
            }
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
            if (IsAdministrator() && Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\") && comboBox2.Text.Contains(@":\"))
            {
                Enabled = false;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/c rmdir " + Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\";
                process.StartInfo = startInfo;
                process.Start();

                if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\VRChat\\"))
                {
                    CopyFolder(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString().Replace(@"C:\", comboBox2.Text) + "\\LocalLow\\VRChat\\", Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\");
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
            foreach (var fileInfo in Info.GetFiles())
            {
                System.Threading.Interlocked.Add(ref startDirectorySize, fileInfo.Length);
            }

            //Loop on Sub Direcotries in the Current Directory and Calculate it's files size.
            System.Threading.Tasks.Parallel.ForEach(Info.GetDirectories(), (subDirectory) =>
                    System.Threading.Interlocked.Add(ref startDirectorySize, GetDirectorySizeInGB(subDirectory, recursive)));

            return startDirectorySize;  //Return full Size of this Directory.
        }
    }
}
