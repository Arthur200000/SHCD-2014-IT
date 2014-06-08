namespace SHCD
{
    using ExamClientControlsLibrary;
    using Qisi.General;
    using Qisi.General.Controls;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.AccessControl;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    public class FormTemplate : Form
    {
        private BackgroundWorker backgroundWorker1;
        private BackgroundWorker backgroundWorker3;
        private ChooseSubject choose;
        private IContainer components = null;
        private List<Control> ControlList = new List<Control>();
        private CountDown countdown;
        private string gender;
        private string ID;
        private string IDName;
        private Login login;
        private TestPaperPlayer.RunMode mymode;
        private string paperName;
        private string seatNo;
        private string studentName;
        private string subject;
        private string subjectList;
        private Test test;
        private TestPaperPlayer testpaper;
        private Label Title;

        public FormTemplate(TestPaperPlayer.RunMode mode, string papername)
        {
            this.InitializeComponent();
            this.mymode = mode;
            this.paperName = papername;
            base.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;
            if (this.mymode == TestPaperPlayer.RunMode.CDExercise)
            {
                this.countdown = new CountDown();
                this.choose = new ChooseSubject();
                base.Controls.Add(this.choose);
                base.Controls.Add(this.countdown);
            }
            else if (this.mymode == TestPaperPlayer.RunMode.CDSimulate)
            {
                this.countdown = new CountDown();
                this.choose = new ChooseSubject();
                this.test = new Test();
                this.login = new Login();
                base.Controls.Add(this.choose);
                base.Controls.Add(this.countdown);
                base.Controls.Add(this.login);
                base.Controls.Add(this.test);
            }
            else
            {
                this.countdown = new CountDown();
                base.Controls.Add(this.countdown);
            }
            this.Title.Text = "上海市普通高中学业水平考试信息科技全真模拟练习(2014)";
            base.Visible = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] buffer;
            if (!Directory.Exists(Program.paperDir))
            {
                Directory.CreateDirectory(Program.paperDir);
            }
            else
            {
                CommonMethods.ClearDirectory(Program.paperDir);
            }
            CommonMethods.Decy(Path.Combine(Application.StartupPath, @"EPF\" + this.paperName + ".epf"), out buffer);
            CommonMethods.Unzip(buffer, Program.paperDir, "CKKC37F423");
            if (Directory.Exists(Program.tempAnswerDir))
            {
                CommonMethods.ClearDirectory(Program.tempAnswerDir);
            }
            else
            {
                Directory.CreateDirectory(Program.tempAnswerDir);
            }
            if (this.mymode == TestPaperPlayer.RunMode.CDAnalysis)
            {
                CommonMethods.Decy(Path.Combine(Program.answerDir, this.paperName + ".dat"), out buffer);
                CommonMethods.Unzip(buffer, Program.tempAnswerDir, "CKKC37F423");
            }
            else if (File.Exists(Path.Combine(Program.answerDir, this.paperName + ".dat")))
            {
                File.Delete(Path.Combine(Program.answerDir, this.paperName + ".dat"));
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int num;
            if (Directory.Exists(Program.dataDir))
            {
                try
                {
                    bool flag;
                    DirectorySecurity directorySecurity = new DirectorySecurity();
                    InheritanceFlags none = InheritanceFlags.None;
                    none = InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit;
                    FileSystemAccessRule rule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, none, PropagationFlags.None, AccessControlType.Allow);
                    directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out flag);
                    Directory.CreateDirectory(Program.dataDir, directorySecurity);
                }
                catch
                {
                    FlatMessageBox.Show(this, "打开试卷时，发生了错误。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                    base.Dispose();
                    return;
                }
            }
            else
            {
                CommonMethods.ClearDirectory(Program.dataDir);
            }
            if (this.mymode == TestPaperPlayer.RunMode.CDAnalysis)
            {
                Thread.Sleep(1000);
                MemoryIniFile file = new MemoryIniFile();
                num = 0;
                while (!File.Exists(Path.Combine(Program.tempAnswerDir, "Answer.ini")) && (num < 5))
                {
                    Thread.Sleep(500);
                    num++;
                }
                if (num >= 5)
                {
                    FlatMessageBox.Show(this, "加载答卷时，发生了错误。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                    base.Dispose();
                }
                else
                {
                    file.LoadFromEncodedFile(Path.Combine(Program.tempAnswerDir, "Answer.ini"));
                    this.subject = file.ReadValue("ANSWER", "SUBJECT", "");
                    this.generateTestPaper();
                }
            }
            else if (this.mymode == TestPaperPlayer.RunMode.CDExercise)
            {
                XmlDocument document = new XmlDocument();
                num = 0;
                while (!File.Exists(Path.Combine(Program.paperDir, "TestPaper.xml")) && (num < 5))
                {
                    Thread.Sleep(500);
                    num++;
                }
                if (num >= 5)
                {
                    FlatMessageBox.Show(this, "打开试卷时，发生了错误。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                    base.Dispose();
                }
                else
                {
                    try
                    {
                        string xml = File.ReadAllText(Path.Combine(Program.paperDir, "TestPaper.xml"));
                        document.LoadXml(xml);
                    }
                    catch
                    {
                        FlatMessageBox.Show(this, "打开试卷时，发生了错误。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                        base.Dispose();
                        return;
                    }
                    string text = "";
                    foreach (XmlNode node in document.ChildNodes[0].ChildNodes)
                    {
                        if (node.Attributes["subject"].Value != "")
                        {
                            text = text + "_" + node.Attributes["subject"].Value;
                        }
                    }
                    if (text.Length > 1)
                    {
                        text = text.Substring(1);
                    }
                    this.choose.AddItem(text);
                    this.choose.SubjectSelected += new EventHandler(this.choose_SubjectSelected);
                    this.choose.Top = (base.Height - this.choose.Height) / 2;
                    this.choose.Left = (base.Width - this.choose.Width) / 2;
                    this.choose.Visible = true;
                    this.choose.Invalidate();
                    this.choose.BringToFront();
                }
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.mymode != TestPaperPlayer.RunMode.CDAnalysis)
            {
                bool flag;
                DirectorySecurity directorySecurity = new DirectorySecurity();
                InheritanceFlags none = InheritanceFlags.None;
                none = InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit;
                FileSystemAccessRule rule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, none, PropagationFlags.None, AccessControlType.Allow);
                directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out flag);
                if (!Directory.Exists(Path.Combine(Program.paperDir, "Answerbackup")))
                {
                    Directory.CreateDirectory(Path.Combine(Program.paperDir, "Answerbackup"), directorySecurity);
                }
                CommonMethods.CopyFolder(Program.tempAnswerDir, Path.Combine(Program.paperDir, "Answerbackup"), true);
                string fileToZip = Path.Combine(Program.paperDir, "Answerbackup");
                fileToZip = fileToZip.EndsWith(@"\") ? fileToZip : (fileToZip + @"\");
                if (CommonMethods.Zip(fileToZip, Path.Combine(Program.answerDir, this.paperName + ".dat"), "CKKC37F423"))
                {
                    CommonMethods.Encrypt(Path.Combine(Program.answerDir, this.paperName + ".dat"));
                }
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(0x7d0);
            base.Hide();
            base.Dispose();
        }

        private void choose_SubjectSelected(object sender, EventArgs e)
        {
            this.subject = this.choose.SelectSubject;
            this.choose.Visible = false;
            this.generateTestPaper();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            if (!((this.testpaper == null) || this.testpaper.IsDisposed))
            {
                this.testpaper.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormInputNo_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Color color = Color.FromArgb(0xb9, 220, 0xff);
            Color color2 = Color.FromArgb(0, 0x80, 0xc0);
            Brush brush = new LinearGradientBrush(base.ClientRectangle, color2, color, LinearGradientMode.ForwardDiagonal);
            graphics.FillRectangle(brush, base.ClientRectangle);
        }

        private void FormTemplate_Load(object sender, EventArgs e)
        {
            if (this.mymode == TestPaperPlayer.RunMode.CDExercise)
            {
                this.countdown.Visible = true;
                this.countdown.Top = (base.Height - this.countdown.Height) / 2;
                this.countdown.Left = (base.Width - this.countdown.Width) / 2;
                this.backgroundWorker1.RunWorkerAsync();
            }
            else if (this.mymode == TestPaperPlayer.RunMode.CDSimulate)
            {
                this.login.Visible = true;
                this.countdown.Top = (base.Height - this.countdown.Height) / 2;
                this.countdown.Left = (base.Width - this.countdown.Width) / 2;
                this.login.Top = (base.Height - this.login.Height) / 2;
                this.login.Left = (base.Width - this.login.Width) / 2;
                this.login.Exit += new EventHandler(this.login_Exit);
                this.login.Logon += new EventHandler(this.login_Logon);
                this.backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                this.countdown.Visible = true;
                this.countdown.Top = (base.Height - this.countdown.Height) / 2;
                this.countdown.Left = (base.Width - this.countdown.Width) / 2;
                this.backgroundWorker1.RunWorkerAsync();
            }
            base.Visible = true;
        }

        private void generateTestPaper()
        {
            if (base.InvokeRequired)
            {
                generateTestPaperCallBack back = new generateTestPaperCallBack(this.generateTestPaper);
                back();
            }
            else
            {
                List<string> information = new List<string> {
                    this.studentName,
                    this.gender,
                    this.seatNo,
                    this.ID
                };
                bool flag = false;
                int num = 0;
                while (!flag && (num < 5))
                {
                    try
                    {
                        this.testpaper = new TestPaperPlayer(base.Size, information, this.subject, this.mymode, Program.paperDir, Program.dataDir, Program.tempAnswerDir, null, "报名号", "Answer.ini", Path.Combine(Application.StartupPath, @"STD\" + this.paperName + @"\std.ini"), Path.Combine(Application.StartupPath, @"STD\" + this.paperName));
                        flag = true;
                        this.testpaper.HandOn += new EventHandler(this.tp_HandOn);
                        this.ShowTestPaper();
                    }
                    catch (Exception)
                    {
                        num++;
                    }
                    Thread.Sleep(500);
                }
                if (num >= 5)
                {
                    FlatMessageBox.Show(this, "打开试卷时，发生了错误。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                    base.Dispose();
                }
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(FormTemplate));
            this.Title = new Label();
            this.backgroundWorker1 = new BackgroundWorker();
            this.backgroundWorker3 = new BackgroundWorker();
            base.SuspendLayout();
            this.Title.AutoSize = true;
            this.Title.BackColor = Color.Transparent;
            this.Title.Font = new Font("宋体", 15.75f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Title.ForeColor = Color.White;
            this.Title.Location = new Point(0x21, 30);
            this.Title.Name = "Title";
            this.Title.Size = new Size(0x4c, 0x15);
            this.Title.TabIndex = 0;
            this.Title.Text = "Label1";
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker3.WorkerReportsProgress = true;
            this.backgroundWorker3.WorkerSupportsCancellation = true;
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.ControlDark;
            base.ClientSize = new Size(800, 600);
            base.Controls.Add(this.Title);
            this.DoubleBuffered = true;
            base.FormBorderStyle = FormBorderStyle.None;
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "FormTemplate";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            base.WindowState = FormWindowState.Maximized;
            base.Load += new EventHandler(this.FormTemplate_Load);
            base.Paint += new PaintEventHandler(this.FormInputNo_Paint);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void login_Exit(object sender, EventArgs e)
        {
            if (FlatMessageBox.Show(this, "确定要退出吗？", "警告", FlatMessageBox.KeysButtons.OKCancel, FlatMessageBox.KeysIcon.Information) == DialogResult.OK)
            {
                base.Dispose();
            }
        }

        private void login_Logon(object sender, EventArgs e)
        {
            this.ID = this.login.ID;
            this.studentName = this.login.ID;
            this.login.Visible = false;
            this.test.Top = (base.Height - this.test.Height) / 2;
            this.test.Left = (base.Width - this.test.Width) / 2;
            this.test.stuID = this.login.ID;
            this.test.stuName = this.login.ID;
            this.test.Exit += new EventHandler(this.test_Exit);
            this.test.Complete += new EventHandler(this.test_Complete);
            this.test.Visible = true;
            this.test.Focus();
            this.backgroundWorker1.RunWorkerAsync();
        }

        private void ShowTestPaper()
        {
            this.testpaper.Location = new Point(0, 0);
            if (this.mymode == TestPaperPlayer.RunMode.CDExercise)
            {
                this.countdown.Visible = false;
                this.choose.Visible = false;
            }
            else if (this.mymode == TestPaperPlayer.RunMode.CDSimulate)
            {
                this.countdown.Visible = false;
                this.choose.Visible = false;
                this.test.Visible = false;
                this.login.Visible = false;
            }
            else
            {
                this.countdown.Visible = false;
            }
            this.Title.Visible = false;
            this.testpaper.Visible = true;
            Thread.Sleep(1000);
            base.Controls.Add(this.testpaper);
            this.testpaper.StartCounting();
        }

        private void test_Complete(object sender, EventArgs e)
        {
            while (this.backgroundWorker1.IsBusy)
            {
                Thread.Sleep(500);
            }
            XmlDocument document = new XmlDocument();
            int num = 0;
            while (!File.Exists(Path.Combine(Program.paperDir, "TestPaper.xml")) && (num < 5))
            {
                Thread.Sleep(500);
                num++;
            }
            if (num >= 5)
            {
                FlatMessageBox.Show(this, "打开试卷时，发生了错误。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                base.Dispose();
            }
            try
            {
                string xml = File.ReadAllText(Path.Combine(Program.paperDir, "TestPaper.xml"));
                document.LoadXml(xml);
            }
            catch
            {
                FlatMessageBox.Show(this, "打开试卷时，发生了错误。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                base.Dispose();
            }
            string text = "";
            foreach (XmlNode node in document.ChildNodes[0].ChildNodes)
            {
                if (node.Attributes["subject"].Value != "")
                {
                    text = text + "_" + node.Attributes["subject"].Value;
                }
            }
            if (text.Length > 1)
            {
                text = text.Substring(1);
            }
            this.test.Visible = false;
            this.choose.AddItem(text);
            this.choose.SubjectSelected += new EventHandler(this.choose_SubjectSelected);
            this.choose.Top = (base.Height - this.choose.Height) / 2;
            this.choose.Left = (base.Width - this.choose.Width) / 2;
            this.choose.Visible = true;
            this.choose.Invalidate();
            this.choose.BringToFront();
        }

        private void test_Exit(object sender, EventArgs e)
        {
            if (FlatMessageBox.Show(this, "确定要退出吗？", "警告", FlatMessageBox.KeysButtons.OKCancel, FlatMessageBox.KeysIcon.Information) == DialogResult.OK)
            {
                this.backgroundWorker1.CancelAsync();
                base.Dispose();
            }
        }

        private void tp_HandOn(object sender, EventArgs e)
        {
            this.testpaper.Visible = false;
            this.countdown.Text = "正在生成答卷....";
            this.countdown.Visible = true;
            this.countdown.BringToFront();
            this.backgroundWorker3.DoWork += new DoWorkEventHandler(this.backgroundWorker3_DoWork);
            this.backgroundWorker3.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker3_RunWorkerCompleted);
            this.backgroundWorker3.RunWorkerAsync();
        }

        private delegate void generateTestPaperCallBack();
    }
}

