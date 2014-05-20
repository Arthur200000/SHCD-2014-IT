namespace SHCD
{
    using ExamClientControlsLibrary;
    using Qisi.General;
    using SHCD.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    public class FormSel : Form
    {
        private BackgroundWorker backgroundWorker1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private IContainer components = null;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private Label label2;
        private ListView listView1;
        private ListView listView2;
        private TestPaperPlayer.RunMode mode = TestPaperPlayer.RunMode.CDExercise;
        private Point mousePos;
        private string papername = "";

        public FormSel()
        {
            this.InitializeComponent();
            this.listView1.LargeImageList = new ImageList();
            this.listView1.LargeImageList.ImageSize = new Size(0x41, 0x41);
            this.listView1.LargeImageList.Images.Add(Resources.doneL);
            this.listView1.LargeImageList.Images.Add(Resources.undoL);
            this.listView1.GotFocus += new EventHandler(this.listView1_GotFocus);
            this.listView2.LargeImageList = new ImageList();
            this.listView2.LargeImageList.ImageSize = new Size(0x41, 0x41);
            this.listView2.LargeImageList.Images.Add(Resources.doneM);
            this.listView2.LargeImageList.Images.Add(Resources.undoM);
            this.listView2.GotFocus += new EventHandler(this.listView2_GotFocus);
            DirectoryInfo info = new DirectoryInfo(Path.Combine(Application.StartupPath, "EPF"));
            if (info.Exists)
            {
                foreach (FileInfo info2 in info.GetFiles())
                {
                    if (info2.Extension == ".epf")
                    {
                        if (info2.Name.StartsWith("练习"))
                        {
                            if (File.Exists(Path.Combine(Program.answerDir, Path.GetFileNameWithoutExtension(info2.FullName) + ".dat")))
                            {
                                this.listView1.Items.Add(Path.GetFileNameWithoutExtension(info2.FullName), 0);
                            }
                            else
                            {
                                this.listView1.Items.Add(Path.GetFileNameWithoutExtension(info2.FullName), 1);
                            }
                        }
                        else if (info2.Name.StartsWith("模拟"))
                        {
                            if (File.Exists(Path.Combine(Program.answerDir, Path.GetFileNameWithoutExtension(info2.FullName) + ".dat")))
                            {
                                this.listView2.Items.Add(Path.GetFileNameWithoutExtension(info2.FullName), 0);
                            }
                            else
                            {
                                this.listView2.Items.Add(Path.GetFileNameWithoutExtension(info2.FullName), 1);
                            }
                        }
                    }
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string str = e.Argument.ToString();
            if (File.Exists(Path.Combine(Program.answerDir, str + ".dat")))
            {
                byte[] buffer;
                CommonMethods.Decy(Path.Combine(Program.answerDir, str + ".dat"), out buffer);
                CommonMethods.Unzip(buffer, Program.tempAnswerDir, "CKKC37F423");
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(1000);
            string paper = "";
            if (this.button1.Enabled)
            {
                paper = this.listView1.SelectedItems[0].Text;
            }
            else
            {
                if (!this.button2.Enabled)
                {
                    return;
                }
                paper = this.listView2.SelectedItems[0].Text;
            }
            score score = new score(paper);
            score.Disposed += new EventHandler(this.sform_Disposed);
            score.ShowDialog(this);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.papername = this.listView1.SelectedItems[0].Text;
            this.mode = TestPaperPlayer.RunMode.CDExercise;
            FormTemplate template = new FormTemplate(this.mode, this.papername);
            base.Hide();
            base.ShowInTaskbar = false;
            template.Disposed += new EventHandler(this.t_Disposed);
            template.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.mode = TestPaperPlayer.RunMode.CDSimulate;
            this.papername = this.listView2.SelectedItems[0].Text;
            FormTemplate template = new FormTemplate(this.mode, this.papername);
            base.Hide();
            base.ShowInTaskbar = false;
            template.Disposed += new EventHandler(this.t_Disposed);
            template.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (this.button1.Enabled)
            {
                this.papername = this.listView1.SelectedItems[0].Text;
            }
            else
            {
                if (!this.button2.Enabled)
                {
                    return;
                }
                this.papername = this.listView2.SelectedItems[0].Text;
            }
            this.backgroundWorker1.RunWorkerAsync(this.papername);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormSel_MouseDown(object sender, MouseEventArgs e)
        {
            this.mousePos = e.Location;
        }

        private void FormSel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                base.Left += e.X - this.mousePos.X;
                base.Top += e.Y - this.mousePos.Y;
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(FormSel));
            this.label1 = new Label();
            this.label2 = new Label();
            this.listView1 = new ListView();
            this.listView2 = new ListView();
            this.button1 = new Button();
            this.button2 = new Button();
            this.button3 = new Button();
            this.button4 = new Button();
            this.backgroundWorker1 = new BackgroundWorker();
            this.groupBox1 = new GroupBox();
            this.groupBox2 = new GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.BackColor = Color.Transparent;
            this.label1.Font = new Font("微软雅黑", 15f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.label1.ForeColor = Color.White;
            this.label1.Location = new Point(1, 11);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(340, 0x20);
            this.label1.TabIndex = 0;
            this.label1.Text = "上海市普通高中学业水平考试";
            this.label1.MouseDown += new MouseEventHandler(this.label1_MouseDown);
            this.label1.MouseMove += new MouseEventHandler(this.label1_MouseMove);
            this.label2.AutoSize = true;
            this.label2.BackColor = Color.Transparent;
            this.label2.Font = new Font("微软雅黑", 21.75f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.label2.ForeColor = Color.White;
            this.label2.Location = new Point(0x41, 0x38);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x2a2, 0x30);
            this.label2.TabIndex = 1;
            this.label2.Text = "《信息科技》全真模拟练习 2014年专用";
            this.label2.MouseDown += new MouseEventHandler(this.label2_MouseDown);
            this.label2.MouseMove += new MouseEventHandler(this.label2_MouseMove);
            this.listView1.Alignment = ListViewAlignment.Default;
            this.listView1.BackColor = Color.White;
            this.listView1.Cursor = Cursors.Hand;
            this.listView1.Dock = DockStyle.Fill;
            this.listView1.Font = new Font("微软雅黑", 14f);
            this.listView1.HideSelection = false;
            this.listView1.Location = new Point(4, 0x23);
            this.listView1.Margin = new Padding(4, 4, 4, 4);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new Size(0x337, 0x98);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
            this.listView2.Alignment = ListViewAlignment.Default;
            this.listView2.BackColor = Color.White;
            this.listView2.Cursor = Cursors.Hand;
            this.listView2.Dock = DockStyle.Fill;
            this.listView2.Font = new Font("微软雅黑", 14f);
            this.listView2.HideSelection = false;
            this.listView2.Location = new Point(4, 0x23);
            this.listView2.Margin = new Padding(4, 4, 4, 4);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new Size(0x337, 0x98);
            this.listView2.TabIndex = 5;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.SelectedIndexChanged += new EventHandler(this.listView2_SelectedIndexChanged);
            this.button1.AutoSize = true;
            this.button1.BackColor = Color.White;
            this.button1.Cursor = Cursors.Hand;
            this.button1.Enabled = false;
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.button1.Location = new Point(0x8b, 0x222);
            this.button1.Margin = new Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x6b, 40);
            this.button1.TabIndex = 7;
            this.button1.Text = "操作练习";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new EventHandler(this.Button1_Click);
            this.button2.AutoSize = true;
            this.button2.BackColor = Color.White;
            this.button2.Cursor = Cursors.Hand;
            this.button2.Enabled = false;
            this.button2.FlatStyle = FlatStyle.Flat;
            this.button2.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.button2.Location = new Point(0x12b, 0x222);
            this.button2.Margin = new Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x6b, 40);
            this.button2.TabIndex = 8;
            this.button2.Text = "模拟考试";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.button3.AutoSize = true;
            this.button3.BackColor = Color.White;
            this.button3.Cursor = Cursors.Hand;
            this.button3.Enabled = false;
            this.button3.FlatStyle = FlatStyle.Flat;
            this.button3.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.button3.Location = new Point(0x1cb, 0x222);
            this.button3.Margin = new Padding(4, 4, 4, 4);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x6b, 40);
            this.button3.TabIndex = 9;
            this.button3.Text = "结果评分";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new EventHandler(this.Button3_Click);
            this.button4.AutoSize = true;
            this.button4.BackColor = Color.White;
            this.button4.Cursor = Cursors.Hand;
            this.button4.FlatStyle = FlatStyle.Flat;
            this.button4.Font = new Font("微软雅黑", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.button4.Location = new Point(0x26b, 0x222);
            this.button4.Margin = new Padding(4, 4, 4, 4);
            this.button4.Name = "button4";
            this.button4.Size = new Size(0x6b, 40);
            this.button4.TabIndex = 10;
            this.button4.Text = "退出";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new EventHandler(this.button4_Click);
            this.backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Font = new Font("微软雅黑", 14f);
            this.groupBox1.ForeColor = Color.White;
            this.groupBox1.Location = new Point(0x10, 130);
            this.groupBox1.Margin = new Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new Padding(4, 4, 4, 4);
            this.groupBox1.Size = new Size(0x33f, 0xbf);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作练习";
            this.groupBox2.Controls.Add(this.listView2);
            this.groupBox2.Font = new Font("微软雅黑", 14f);
            this.groupBox2.ForeColor = Color.White;
            this.groupBox2.Location = new Point(0x10, 0x15c);
            this.groupBox2.Margin = new Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new Padding(4, 4, 4, 4);
            this.groupBox2.Size = new Size(0x33f, 0xbf);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "模拟考试";
            base.AutoScaleDimensions = new SizeF(8f, 15f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(0x45, 0x89, 0x94);
            base.ClientSize = new Size(0x35f, 0x253);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.button4);
            base.Controls.Add(this.button3);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            base.FormBorderStyle = FormBorderStyle.None;
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Margin = new Padding(4, 4, 4, 4);
            base.Name = "FormSel";
            base.StartPosition = FormStartPosition.CenterScreen;
            base.MouseDown += new MouseEventHandler(this.FormSel_MouseDown);
            base.MouseMove += new MouseEventHandler(this.FormSel_MouseMove);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            this.mousePos = e.Location;
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                base.Left += e.X - this.mousePos.X;
                base.Top += e.Y - this.mousePos.Y;
            }
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            this.mousePos = e.Location;
        }

        private void label2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                base.Left += e.X - this.mousePos.X;
                base.Top += e.Y - this.mousePos.Y;
            }
        }

        private void listView1_GotFocus(object sender, EventArgs e)
        {
            this.listView2.SelectedItems.Clear();
            this.button2.Enabled = false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
            {
                this.button1.Enabled = false;
                this.button2.Enabled = false;
                this.button3.Enabled = false;
            }
            else if (this.listView1.SelectedItems[0].ImageIndex == 0)
            {
                this.button1.Enabled = true;
                this.button2.Enabled = false;
                this.button3.Enabled = true;
            }
            else
            {
                this.button1.Enabled = true;
                this.button2.Enabled = false;
                this.button3.Enabled = false;
            }
        }

        private void listView2_GotFocus(object sender, EventArgs e)
        {
            this.listView1.SelectedItems.Clear();
            this.button1.Enabled = false;
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView2.SelectedItems.Count == 0)
            {
                this.button1.Enabled = false;
                this.button2.Enabled = false;
                this.button3.Enabled = false;
            }
            else if (this.listView2.SelectedItems[0].ImageIndex == 0)
            {
                this.button1.Enabled = false;
                this.button2.Enabled = true;
                this.button3.Enabled = true;
            }
            else
            {
                this.button1.Enabled = false;
                this.button2.Enabled = true;
                this.button3.Enabled = false;
            }
        }

        private void sform_Disposed(object sender, EventArgs e)
        {
            CommonMethods.ClearDirectory(Program.tempAnswerDir);
        }

        private void t_Disposed(object sender, EventArgs e)
        {
            CommonMethods.ClearDirectory(Program.paperDir);
            CommonMethods.ClearDirectory(Program.dataDir);
            this.listView1.Clear();
            this.listView2.Clear();
            this.button3.Enabled = false;
            DirectoryInfo info = new DirectoryInfo(Path.Combine(Application.StartupPath, "EPF"));
            if (info.Exists)
            {
                foreach (FileInfo info2 in info.GetFiles())
                {
                    if (info2.Extension == ".epf")
                    {
                        if (info2.Name.StartsWith("练习"))
                        {
                            if (File.Exists(Path.Combine(Program.answerDir, Path.GetFileNameWithoutExtension(info2.FullName) + ".dat")))
                            {
                                this.listView1.Items.Add(Path.GetFileNameWithoutExtension(info2.FullName), 0);
                            }
                            else
                            {
                                this.listView1.Items.Add(Path.GetFileNameWithoutExtension(info2.FullName), 1);
                            }
                        }
                        else if (info2.Name.StartsWith("模拟"))
                        {
                            if (File.Exists(Path.Combine(Program.answerDir, Path.GetFileNameWithoutExtension(info2.FullName) + ".dat")))
                            {
                                this.listView2.Items.Add(Path.GetFileNameWithoutExtension(info2.FullName), 0);
                            }
                            else
                            {
                                this.listView2.Items.Add(Path.GetFileNameWithoutExtension(info2.FullName), 1);
                            }
                        }
                    }
                }
            }
            base.Show();
            base.ShowInTaskbar = true;
            if (File.Exists(Path.Combine(Program.answerDir, this.papername + ".dat")))
            {
                score score = new score(this.papername);
                score.Disposed += new EventHandler(this.sform_Disposed);
                score.ShowDialog(this);
            }
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
        }
    }
}

