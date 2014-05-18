namespace Updater
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    public class Form1 : Form
    {
        private Button button1;
        private Button button2;
        private IContainer components = null;
        private int downIndex;
        private List<string> downloadList;
        private bool error;
        private List<string> fileList;
        private ProgressBar progressBar1;
        private TextBox textBox1;
		string SHCDProcess="SHCD";

        public Form1()
        {
            this.InitializeComponent();
            this.error = false;
            this.downloadList = new List<string>();
            this.fileList = new List<string>();
            string str = Program.URL.Substring(0, Program.URL.LastIndexOf("/"));
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(Program.URL);
                foreach (string str2 in Directory.GetFiles(Application.StartupPath, "*", SearchOption.AllDirectories))
                {
                    string extension = Path.GetExtension(str2);
                    if ((extension != null) && ((extension == ".dll") || (extension == ".exe")))
                    {
                        Version version = new Version(FileVersionInfo.GetVersionInfo(str2).FileVersion);
                        foreach (XmlNode node in document.DocumentElement.ChildNodes)
                        {
                            if (((node.Attributes["name"] != null) && (node.Attributes["version"] != null)) && (node.Attributes["name"].Value == str2.Replace(Application.StartupPath, "")))
                            {
                                Version version2 = new Version(node.Attributes["version"].Value);
                                if (version2.CompareTo(version) > 0)
                                {
                                    this.downloadList.Add(str + node.Attributes["name"].Value.Replace(@"\", "/"));
                                    this.fileList.Add(str2.Replace(Application.StartupPath, ""));
                                }
                            }
                        }
                    }
                    else
                    {
                        long ticks = System.IO.File.GetCreationTimeUtc(str2).Ticks;
                        foreach (XmlNode node in document.DocumentElement.ChildNodes)
                        {
                            if ((((node.Attributes["name"] != null) && (node.Attributes["time"] != null)) && (node.Attributes["name"].Value == str2.Replace(Application.StartupPath, ""))) && (Convert.ToInt64(node.Attributes["time"].Value) > ticks))
                            {
                                this.downloadList.Add(str + node.Attributes["name"].Value.Replace(@"\", "/"));
                                this.fileList.Add(str2.Replace(Application.StartupPath, ""));
                            }
                        }
                    }
                }
                if (document.DocumentElement.Attributes["details"] != null)
                {
                    this.textBox1.Text = new WebClient().DownloadString(str + document.DocumentElement.Attributes["details"].Value);
                }
            }
            catch
            {
                this.error = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.progressBar1.Maximum = this.downloadList.Count;
            this.progressBar1.Value = 0;
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
				if (process.ProcessName == SHCDProcess)
                {
                    try
                    {
                        if (process.Responding)
                        {
                            process.Kill();
                            process.WaitForExit();
                            Thread.Sleep(200);
                            for (int i = 0; !process.HasExited && (i < 5); i++)
                            {
                                Thread.Sleep(200);
                            }
                            Thread.Sleep(200);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.downIndex = 0;
            this.DownLoad();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        public static void CopyFolder(string sourceFolder, string destFolder, bool overwrite = true)
        {
            if (Directory.Exists(sourceFolder))
            {
                string fileName;
                string str3;
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string str in files)
                {
                    fileName = Path.GetFileName(str);
                    str3 = Path.Combine(destFolder, fileName);
                    try
                    {
                        System.IO.File.Copy(str, str3, overwrite);
                    }
                    catch (Exception)
                    {
                    }
                }
                string[] directories = Directory.GetDirectories(sourceFolder);
                foreach (string str4 in directories)
                {
                    fileName = Path.GetFileName(str4);
                    str3 = Path.Combine(destFolder, fileName);
                    CopyFolder(str4, str3, overwrite);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DownLoad()
        {
            if (this.downIndex < this.downloadList.Count)
            {
                WebClient client = new WebClient();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.wc_DownloadFileCompleted);
                try
                {
                    DirectoryInfo parent = Directory.GetParent(Path.Combine(Application.StartupPath, "update") + this.fileList[this.downIndex]);
                    if (!parent.Exists)
                    {
                        parent.Create();
                    }
                    client.DownloadFileAsync(new Uri(this.downloadList[this.downIndex]), Path.Combine(Application.StartupPath, "update") + this.fileList[this.downIndex]);
                }
                catch
                {
                    MessageBox.Show("更新过程发生错误！");
                    base.Close();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.error)
            {
                MessageBox.Show("获取更新失败！");
                base.Close();
            }
            else
            {
                this.textBox1.SelectedText = "";
                this.textBox1.SelectionLength = 0;
                this.textBox1.SelectionStart = 0;
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(Form1));
            this.button1 = new Button();
            this.button2 = new Button();
            this.textBox1 = new TextBox();
            this.progressBar1 = new ProgressBar();
            base.SuspendLayout();
            this.button1.AutoSize = true;
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.Location = new Point(0x7e, 0xed);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x4b, 0x18);
            this.button1.TabIndex = 1;
            this.button1.Text = "开始更新";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.AutoSize = true;
            this.button2.FlatStyle = FlatStyle.Flat;
            this.button2.Location = new Point(0xe5, 0xed);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x60, 0x18);
            this.button2.TabIndex = 2;
            this.button2.Text = "不了，谢谢！";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.textBox1.Dock = DockStyle.Top;
            this.textBox1.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.textBox1.HideSelection = false;
            this.textBox1.Location = new Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = ScrollBars.Vertical;
            this.textBox1.Size = new Size(0x151, 0xcf);
            this.textBox1.TabIndex = 0;
            this.progressBar1.Location = new Point(0, 0xd5);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(0x151, 12);
            this.progressBar1.TabIndex = 3;
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x151, 0x106);
            base.Controls.Add(this.progressBar1);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.textBox1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "Form1";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "软件更新";
            base.Load += new EventHandler(this.Form1_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.downIndex++;
            if (e.Error != null)
            {
                MessageBox.Show("更新过程发生错误！");
                try
                {
                    Directory.Delete(Path.Combine(Application.StartupPath, "update"));
                }
                catch
                {
                }
                base.Close();
            }
            else
            {
                this.progressBar1.Value = this.downIndex;
                if (this.downIndex < this.downloadList.Count)
                {
                    this.DownLoad();
                }
                else
                {
                    MessageBox.Show("更新完成，现在启动练习光盘！");
                    try
                    {
                        CopyFolder(Path.Combine(Application.StartupPath, "update"), Application.StartupPath, true);
                        Directory.Delete(Path.Combine(Application.StartupPath, "update"), true);
                    }
                    catch
                    {
                    }
                    Process.Start(Path.Combine(Application.StartupPath, "练习光盘.exe"));
                    base.Close();
                }
            }
        }
    }
}

