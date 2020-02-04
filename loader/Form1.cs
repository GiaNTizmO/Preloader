using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace loader
{
    
    public partial class Form1 : Form
    {

        public Form1()
        {
            
            InitializeComponent();
            label2.Parent = pictureBox1;
            label2.BackColor = Color.Transparent;
            progressBar.Maximum = 100;

            if (!Directory.Exists("C:\\VexarMC"))
            {
                Directory.CreateDirectory("C:\\VexarMC");
            }
            
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e) //Java Verify
        {
            timer1.Stop();

            var wc = new WebClient();
            progressBar.Value = 1;
            var md5 = MD5.Create();

            if (IntPtr.Size == 8)
            {
                if (File.Exists("C:\\VexarMC\\java_64.zip"))
                {
                    var stream = File.OpenRead("C:\\VexarMC\\java_64.zip");
                    byte[] checksum = md5.ComputeHash(stream);
                    string output = BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
                    stream.Close();
                    Stream streamphp = wc.OpenRead("http://launch.vexarmc.ru/launch.php?starter=64");
                    StreamReader reader = new StreamReader(streamphp);
                    String content = reader.ReadToEnd();
                    Match[] MD5HashesServer = Regex.Matches(content, @"[A-z0-9]{32}")
                       .Cast<Match>()
                       .ToArray();
                    if (MD5HashesServer[1].ToString() == output)
                    {
                        //Java on client is valid!
                        timer3.Start();
                        progressBar.Value = 75;
                    }
                    else
                    {
                        timer2.Start();

                    }
                }
                else
                {
                    timer2.Start();
                }
            }
            else if (IntPtr.Size == 4)
            {
                if (File.Exists("C:\\VexarMC\\java_32.zip"))
                {
                    var stream = File.OpenRead("C:\\VexarMC\\java_32.zip");
                    byte[] checksum = md5.ComputeHash(stream);
                    string output = BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
                    stream.Close();
                    Stream streamphp = wc.OpenRead("http://launch.vexarmc.ru/launch.php?starter=32");
                    StreamReader reader = new StreamReader(streamphp);
                    String content = reader.ReadToEnd();
                    /* Regex word = new Regex(@";[A-z0-9]{32};U");
                     MessageBox.Show(word.Match(content).ToString());*/
                    Match[] MD5HashesServer = Regex.Matches(content, @"[A-z0-9]{32}")
                       .Cast<Match>()
                       .ToArray();
                    if (MD5HashesServer[1].ToString() == output)
                    {
                        timer3.Start();
                        progressBar.Value = 75;
                    }
                    else
                    {
                        timer2.Start();

                    }
                }
                else
                {
                    timer2.Start();
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e) //Java Downloader
        {
            timer2.Stop();
           
                var wc1 = new WebClient();
            wc1.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            wc1.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            if (IntPtr.Size == 8)
            {
                // 64 bit OS
                wc1.DownloadFileAsync(new Uri("http://launch.vexarmc.ru/clients/java_64.zip"), "C:\\VexarMC\\java_64.zip");
            }
            else if (IntPtr.Size == 4)
            {
                // 32 bit OS
                wc1.DownloadFileAsync(new Uri("http://launch.vexarmc.ru/clients/java_32.zip"), "C:\\VexarMC\\java_32.zip");
            }
            else
            {
                MessageBox.Show("Произошла ошибка в работе стартера! Пожалуйста, отправьте администратору скриншот данной ошибки!" + "\nUnable get OsArch, current is:" + Environment.OSVersion.Platform, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 70;
                label2.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
                progressBar.Value = (progressBar.Value/10) + int.Parse(Math.Truncate(percentage).ToString());
            });
        }

        private void timer3_Tick(object sender, EventArgs e) // Verify launcher.jar
        {
            timer3.Stop();
            if (File.Exists("C:\\VexarMC\\launcher.jar"))
            {
                var md5 = MD5.Create();
                var wc = new WebClient();
                if (IntPtr.Size == 8)
                {
                    Stream streamphp = wc.OpenRead("http://launch.vexarmc.ru/launch.php?starter=64");
                    StreamReader reader = new StreamReader(streamphp);
                    String content = reader.ReadToEnd();
                    Match[] MD5HashesServer_launcher = Regex.Matches(content, @"[A-z0-9]{32}")
                               .Cast<Match>()
                               .ToArray();
                    var stream = File.OpenRead("C:\\VexarMC\\launcher.jar");
                    
                    byte[] checksum = md5.ComputeHash(stream);
                    stream.Close();
                    string output = BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
                    if (MD5HashesServer_launcher[0].ToString() == output)
                    {
                        //Launcher on client is valid!
                        timer5.Start();
                    }
                    else
                    {
                        timer4.Start();
                    }

                }
                if (IntPtr.Size == 4)
                {
                    Stream streamphp = wc.OpenRead("http://launch.vexarmc.ru/launch.php?starter=32");
                    StreamReader reader = new StreamReader(streamphp);
                    String content = reader.ReadToEnd();
                    Match[] MD5HashesServer_launcher = Regex.Matches(content, @"[A-z0-9]{32}")
                               .Cast<Match>()
                               .ToArray();
                    var stream = File.OpenRead("C:\\VexarMC\\launcher.jar");
                    
                    byte[] checksum = md5.ComputeHash(stream);
                    stream.Close();
                    string output = BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
                    
                    if (MD5HashesServer_launcher[0].ToString() == output)
                    {
                        //Launcher on client is valid!
                        progressBar.Value = progressBar.Value + (progressBar.Value / 100 * 5);
                        timer5.Start();
                    }
                    else
                    {
                        timer4.Start();
                    }
                }

            }
            else
            {
                timer4.Start();
            }
        }

        private void timer4_Tick(object sender, EventArgs e) // download launcher.jar
        {
            timer4.Stop();
            var wc = new WebClient();
            //wc.DownloadFileAsync(new Uri("http://launch.vexarmc.ru/launcher.jar"), "C:\\VexarMC\\launcher.jar");
            wc.DownloadFile("http://launch.vexarmc.ru/launcher.jar", "C:\\VexarMC\\launcher.jar");
            progressBar.Value = progressBar.Value + (progressBar.Value / 100 * 5);
            timer3.Start();
        }

        private void timer5_Tick(object sender, EventArgs e) //unzip java
        {
            timer5.Stop();
            progressBar.Value = progressBar.Value + 1;
            if (IntPtr.Size == 8)
            {
                    DirectoryInfo directoryinfo = new DirectoryInfo("C:\\VexarMC\\java_64");
                    if (directoryinfo.Exists) directoryinfo.Delete(true);
                Task.Factory.StartNew(() =>
                {
                    ZipFile.ExtractToDirectory("C:\\VexarMC\\java_64.zip", "C:\\VexarMC\\java_64");
                });


            }
            if (IntPtr.Size == 4)
            {
                ZipFile.ExtractToDirectory("C:\\VexarMC\\java_32.zip", "C:\\VexarMC\\java_32");
            }

            progressBar.Value = progressBar.Value + (progressBar.Value / 100 * 15);
            timer6.Start();
        }


        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
            this.BeginInvoke((MethodInvoker)delegate {
                timer1.Start();
            });
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            timer6.Stop();
            int checkCount = Int32.Parse(posLable.Text);
            checkCount = checkCount + 1;
            posLable.Text = checkCount.ToString();
            MessageBox.Show(checkCount.ToString());
            if (checkCount == 1)
            {
                timer1.Start();
                return;
            }
            progressBar.Value = progressBar.Value + (progressBar.Value / 100 * 4);
            System.Diagnostics.Process.Start("C:\\VexarMC\\java_64\\jre-8-51-x64\\bin\\javaw.exe", "C:\\VexarMC\\launcher.jar");
        }
    }
}
