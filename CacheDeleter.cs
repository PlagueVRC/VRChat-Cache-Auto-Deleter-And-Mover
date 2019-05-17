using System;
using System.IO;
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
            if (checkBox1.Checked && IsAdministrator() == true)
            {
                AutoDeleteTimer.Enabled = true;
                AutoDeleteTimer.Start();
            }
            else if (IsAdministrator() == true)
            {
                AutoDeleteTimer.Stop();
                AutoDeleteTimer.Enabled = false;
            }
            else if (checkBox1.Checked && IsAdministrator() == false)
            {
                checkBox1.Checked = false;
                MessageBox.Show("This Tool MUST Be Ran As Administrator To Work!");
            }
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void AutoDeleteTimer_Tick(object sender, EventArgs e)
        {
            if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\"))
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.Name == @"C:\")
                    {
                        if (drive.AvailableFreeSpace <= 5368709120)
                        {
                            if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\Cookies"))
                            {
                                Directory.Delete(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\Cookies", true);
                            }

                            if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\HTTPCache-WindowsPlayer"))
                            {
                                Directory.Delete(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\HTTPCache-WindowsPlayer", true);
                            }

                            if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\Unity"))
                            {
                                Directory.Delete(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\Unity", true);
                            }

                            if (Directory.Exists(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\VRCHTTPCache"))
                            {
                                Directory.Delete(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\LocalLow\\VRChat\\vrchat\\VRCHTTPCache", true);
                            }
                        }
                    }
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
                MessageBox.Show("This Tool MUST Be Ran As Administrator To Work!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                comboBox2.Items.Add(drive.Name);
            }
        }

        static public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
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
                MessageBox.Show("This Tool MUST Be Ran As Administrator To Work!");
            }
        }
    }
}
