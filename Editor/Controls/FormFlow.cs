namespace Qisi.Editor.Controls
{
    using Qisi.General.Controls;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Timers;
    using System.Windows.Forms;

    internal class FormFlow : Form
    {
        private int backcount;
        private Button btnBack;
        private Button btnOpenDataPath;
        private IContainer components;
        private string dataDir;
        private int endTipTime;
        private DateTime lastbacktime;
        private int leftTime;
        private int myHeight;
        private System.Timers.Timer mytimer;
        private Panel panel1;
        private ReadOnlyRichTextBox readOnlyRichTextBox1;
        private const int SC_CLOSE = 0xf060;
        private const int SC_MAXIMIZE = 0xf030;
        private const int SC_MINIMIZE = 0xf020;
        private StatusStrip statusStrip1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private int tickCount;
        private int timecount;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private const int WM_SYSCOMMAND = 0x112;

        internal FormFlow(Image stuImg, string stuInfo, string rtfPath, string dataPath, int examLeftTime, int endtiptime, string answerPath, string gifPath, int countMode)
        {
            TabPage page;
            this.components = null;
            this.InitializeComponent();
            this.myHeight = 0;
            this.timecount = 0;
            this.backcount = 0;
            this.leftTime = examLeftTime;
            if (countMode == -1)
            {
                this.tickCount = 0;
            }
            else
            {
                this.tickCount = examLeftTime;
            }
            this.endTipTime = endtiptime;
            this.hasProcess = false;
            this.btnBack.Enabled = false;
            this.DoubleBuffered = true;
            this.panel1.BackColor = Color.SkyBlue;
            this.panel1.Location = new Point(0, 0);
            PictureBox box = new PictureBox {
                Image = stuImg,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            base.TopMost = true;
            box.Size = new Size(100, 0x84);
            this.panel1.Controls.Add(box);
            box.Location = new Point(0, 0);
            Label label = new Label {
                Text = stuInfo,
                Font = new Font("黑体", 9f, FontStyle.Bold),
                Visible = true,
                AutoSize = true
            };
            this.panel1.Controls.Add(label);
            label.Top = box.Bottom + 10;
            FileStream data = File.OpenRead(rtfPath);
            this.readOnlyRichTextBox1.LoadFile(data, RichTextBoxStreamType.RichText);
            data.Close();
            this.dataDir = dataPath;
            this.toolStripStatusLabel3.Text = "考生目录：" + this.dataDir;
            this.toolStripStatusLabel1.Font = new Font("黑体", 9f, FontStyle.Bold);
            this.toolStripStatusLabel2.Font = new Font("黑体", 9f, FontStyle.Bold);
            this.toolStripStatusLabel3.Font = new Font("黑体", 9f, FontStyle.Bold);
            if (countMode == -1)
            {
                this.mytimer = new System.Timers.Timer(500.0);
                this.mytimer.Elapsed += new ElapsedEventHandler(this.timer1_Tick);
                this.mytimer.AutoReset = true;
                this.mytimer.Start();
            }
            else if (countMode == 0)
            {
                this.toolStripStatusLabel2.Text = "";
            }
            else
            {
                this.mytimer = new System.Timers.Timer(500.0);
                this.mytimer.Elapsed += new ElapsedEventHandler(this.timer1_Tick_1);
                this.mytimer.AutoReset = true;
                this.mytimer.Start();
            }
            this.tabControl1.SelectedIndexChanged += new EventHandler(this.tabControl1_SelectedIndexChanged);
            this.btnOpenDataPath.Click += new EventHandler(this.button2_Click);
            this.btnBack.Click += new EventHandler(this.button1_Click);
            if ((gifPath != null) && (gifPath != ""))
            {
                page = new TabPage();
                Button button = new Button {
                    AutoSize = true,
                    Text = "播放"
                };
                page.Controls.Add(button);
                button.Location = new Point(3, 3);
                button.Click += new EventHandler(this.play_Click);
                Button button2 = new Button {
                    AutoSize = true,
                    Text = "停止"
                };
                page.Controls.Add(button2);
                button2.Location = new Point(3 + button.Width, 3);
                button2.Click += new EventHandler(this.stop_Click);
                animateImage image = new animateImage(gifPath);
                page.Controls.Add(image);
                image.Location = new Point(3, button.Bottom + 3);
                page.Text = "点此观看参考样例";
                this.tabControl1.TabPages.Add(page);
            }
            if ((answerPath != null) && (answerPath != ""))
            {
                page = new TabPage();
                ReadOnlyRichTextBox box2 = new ReadOnlyRichTextBox {
                    ReadOnly = true
                };
                box2.LoadFile(answerPath);
                box2.Dock = DockStyle.Fill;
                page.Controls.Add(box2);
                page.Text = "分析与解答";
                this.tabControl1.TabPages.Add(page);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process process = new Process {
                StartInfo = { FileName = "cmd.exe", UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, CreateNoWindow = true }
            };
            process.Start();
            process.StandardInput.WriteLine("start  " + this.dataDir);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.mytimer != null))
            {
                this.mytimer.Dispose();
            }
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FlowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.btnBack.Enabled)
            {
                e.Cancel = true;
            }
            else if (this.hasProcess)
            {
                TimeSpan span = new TimeSpan(this.lastbacktime.Ticks);
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                if (span.Subtract(ts).Duration().Seconds < 10)
                {
                    this.backcount++;
                    if (this.backcount >= 4)
                    {
                        if (MessageBox.Show("真的要强制返回主界面吗？", "注意", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            this.mytimer.Enabled = false;
                            return;
                        }
                        this.backcount = 0;
                    }
                    this.lastbacktime = DateTime.Now;
                    MessageBox.Show("请关闭操作软件窗口。\n关闭操作软件窗口就可以返回答题主界面。\n或连续点击四次“返回”按钮，强制返回答题主界面。", "注意", MessageBoxButtons.OK);
                }
                else
                {
                    this.backcount = 1;
                    this.lastbacktime = DateTime.Now;
                    MessageBox.Show("请关闭操作软件窗口。\n关闭操作软件窗口就可以返回答题主界面。\n或连续点击四次“返回”按钮，强制返回答题主界面。", "注意", MessageBoxButtons.OK);
                }
                e.Cancel = true;
            }
            else if (MessageBox.Show("确定要返回主界面吗？未保存的结果将会丢失！", "注意", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
            else
            {
                this.mytimer.Enabled = false;
            }
        }

        private void FlowForm_Load(object sender, EventArgs e)
        {
            foreach (TabPage page in this.tabControl1.TabPages)
            {
                if (this.tabControl1.TabPages.IndexOf(page) != 0)
                {
                    foreach (Control control in page.Controls)
                    {
                        this.tabControl1.Width = Math.Max(this.tabControl1.Width, control.Right + 6);
                        this.tabControl1.Height = Math.Max(this.tabControl1.Height, control.Bottom + 0x12);
                    }
                }
            }
            foreach (Control control in this.panel1.Controls)
            {
                this.panel1.Height = Math.Max(this.panel1.Height, control.Bottom);
                this.panel1.Width = Math.Max(this.panel1.Width, control.Right);
            }
            if (this.panel1.Height > this.tabControl1.Height)
            {
                this.tabControl1.Height = this.panel1.Height;
            }
            else
            {
                this.panel1.Height = this.tabControl1.Height;
            }
            this.btnBack.Top = this.panel1.Bottom;
            this.btnOpenDataPath.Top = this.btnBack.Top;
            this.panel1.Controls[0].Left = (this.panel1.Width - this.panel1.Controls[0].Width) / 2;
            this.tabControl1.Location = new Point(this.panel1.Right, 0);
            base.ClientSize = new Size(this.tabControl1.Right, this.btnBack.Bottom + this.statusStrip1.Height);
            base.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - base.Width, Screen.PrimaryScreen.WorkingArea.Height - base.Height);
        }

        private void FlowForm_SizeChanged(object sender, EventArgs e)
        {
            if (base.ClientSize.Height > 0)
            {
                base.MaximizeBox = false;
                base.MinimizeBox = true;
            }
            this.btnBack.Top = this.statusStrip1.Top - this.btnBack.Height;
            this.btnOpenDataPath.Top = this.btnBack.Top;
            this.panel1.Height = this.btnBack.Top;
            this.tabControl1.Height = this.panel1.Height;
            this.tabControl1.Width = base.ClientSize.Width - this.tabControl1.Left;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(FormFlow));
            this.statusStrip1 = new StatusStrip();
            this.toolStripStatusLabel1 = new ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new ToolStripStatusLabel();
            this.panel1 = new Panel();
            this.tabControl1 = new TabControl();
            this.tabPage1 = new TabPage();
            this.readOnlyRichTextBox1 = new ReadOnlyRichTextBox();
            this.btnBack = new Button();
            this.btnOpenDataPath = new Button();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            base.SuspendLayout();
            this.statusStrip1.Items.AddRange(new ToolStripItem[] { this.toolStripStatusLabel1, this.toolStripStatusLabel2, this.toolStripStatusLabel3 });
            this.statusStrip1.Location = new Point(0, 0x10f);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new Padding(1, 0, 0x13, 0);
            this.statusStrip1.Size = new Size(0x28d, 0x19);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            this.toolStripStatusLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Font = new Font("黑体", 9f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new Size(0x7e, 20);
            this.toolStripStatusLabel1.Text = "请及时保存文件";
            this.toolStripStatusLabel1.TextAlign = ContentAlignment.MiddleLeft;
            this.toolStripStatusLabel2.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new Size(0x47, 20);
            this.toolStripStatusLabel2.Text = "00:00:00";
            this.toolStripStatusLabel2.TextAlign = ContentAlignment.MiddleLeft;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new Size(0x195, 20);
            this.toolStripStatusLabel3.Spring = true;
            this.toolStripStatusLabel3.TextAlign = ContentAlignment.MiddleLeft;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Margin = new Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0xb5, 0xe5);
            this.panel1.TabIndex = 1;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new Point(0xb9, 0);
            this.tabControl1.Margin = new Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(0x1d4, 240);
            this.tabControl1.TabIndex = 2;
            this.tabPage1.Controls.Add(this.readOnlyRichTextBox1);
            this.tabPage1.Location = new Point(4, 0x19);
            this.tabPage1.Margin = new Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(4);
            this.tabPage1.Size = new Size(460, 0xd3);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "操作提示";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.readOnlyRichTextBox1.Cursor = Cursors.Arrow;
            this.readOnlyRichTextBox1.Dock = DockStyle.Fill;
            this.readOnlyRichTextBox1.Location = new Point(4, 4);
            this.readOnlyRichTextBox1.Margin = new Padding(4);
            this.readOnlyRichTextBox1.Name = "readOnlyRichTextBox1";
            this.readOnlyRichTextBox1.ReadOnly = true;
            this.readOnlyRichTextBox1.Size = new Size(0x1c4, 0xcb);
            this.readOnlyRichTextBox1.TabIndex = 0;
            this.readOnlyRichTextBox1.Text = "";
            this.btnBack.Cursor = Cursors.Hand;
            this.btnBack.Location = new Point(0, 0xec);
            this.btnBack.Margin = new Padding(4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new Size(100, 0x1d);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "返回";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnOpenDataPath.AutoSize = true;
            this.btnOpenDataPath.Cursor = Cursors.Hand;
            this.btnOpenDataPath.Location = new Point(120, 0xec);
            this.btnOpenDataPath.Margin = new Padding(4);
            this.btnOpenDataPath.Name = "btnOpenDataPath";
            this.btnOpenDataPath.Size = new Size(0x94, 0x1d);
            this.btnOpenDataPath.TabIndex = 4;
            this.btnOpenDataPath.Text = "打开考生目录";
            this.btnOpenDataPath.UseVisualStyleBackColor = true;
            base.AutoScaleDimensions = new SizeF(8f, 15f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x28d, 0x128);
            base.Controls.Add(this.btnOpenDataPath);
            base.Controls.Add(this.btnBack);
            base.Controls.Add(this.tabControl1);
            base.Controls.Add(this.panel1);
            base.Controls.Add(this.statusStrip1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Margin = new Padding(4);
            base.MaximizeBox = false;
            base.Name = "FormFlow";
            this.Text = "操作提示";
            base.TopMost = true;
            base.FormClosing += new FormClosingEventHandler(this.FlowForm_FormClosing);
            base.Load += new EventHandler(this.FlowForm_Load);
            base.SizeChanged += new EventHandler(this.FlowForm_SizeChanged);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void pexit()
        {
            if (base.InvokeRequired)
            {
                pexitcallback method = new pexitcallback(this.pexit);
                if (!((this == null) || base.IsDisposed))
                {
                    base.Invoke(method);
                }
            }
            else if (!((this == null) || base.IsDisposed))
            {
                base.Dispose();
            }
        }

        private void play_Click(object sender, EventArgs e)
        {
            try
            {
                int num = ((Button) sender).Parent.Controls.IndexOf((Button) sender) + 2;
                ((animateImage) ((Button) sender).Parent.Controls[num]).Play();
            }
            finally
            {
            }
        }

        private void SetColor(ToolStripStatusLabel c, Color col)
        {
            if (base.InvokeRequired)
            {
                SetColorCallBack method = new SetColorCallBack(this.SetColor);
                base.BeginInvoke(method, new object[] { c, col });
            }
            else
            {
                c.ForeColor = col;
            }
        }

        private void SetEnabled(Control c, bool Enabled)
        {
            if (c.InvokeRequired)
            {
                SetEnabledCallBack method = new SetEnabledCallBack(this.SetEnabled);
                c.BeginInvoke(method, new object[] { c, Enabled });
            }
            else
            {
                c.Enabled = Enabled;
            }
        }

        private void SetText(ToolStripStatusLabel c, string text)
        {
            if (base.InvokeRequired)
            {
                SetTextCallBack method = new SetTextCallBack(this.SetText);
                base.BeginInvoke(method, new object[] { c, text });
            }
            else
            {
                c.Text = text;
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            try
            {
                int num = ((Button) sender).Parent.Controls.IndexOf((Button) sender) + 1;
                ((animateImage) ((Button) sender).Parent.Controls[num]).Stop();
            }
            finally
            {
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.tabControl1.SelectedIndex;
            if ((selectedIndex != 0) && ((this.tabControl1.TabPages[selectedIndex].Controls.Count >= 3) && (this.tabControl1.TabPages[selectedIndex].Controls[2].GetType() == System.Type.GetType("ExamClientControlsLibrary.animateImage"))))
            {
                ((animateImage) this.tabControl1.TabPages[selectedIndex].Controls[2]).Play();
            }
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            this.tickCount++;
            if (this.timecount < 10)
            {
                this.timecount++;
            }
            else if (this.timecount == 10)
            {
                this.SetEnabled(this.btnBack, true);
            }
            this.tickCount = this.tickCount % 2;
            int num3 = this.leftTime % 60;
            int num2 = ((this.leftTime - num3) / 60) % 60;
            int num = (((this.leftTime - num3) / 60) - num2) / 60;
            if ((this.leftTime <= 0) && (this.tickCount == 1))
            {
                this.mytimer.Stop();
                this.pexit();
            }
            if ((this.leftTime > this.endTipTime) && (this.tickCount == 1))
            {
                this.SetText(this.toolStripStatusLabel2, num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'));
            }
            else if ((this.leftTime <= this.endTipTime) && (this.tickCount == 1))
            {
                this.SetColor(this.toolStripStatusLabel2, Color.Red);
                this.SetText(this.toolStripStatusLabel2, num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'));
            }
            else if ((this.leftTime <= this.endTipTime) && (this.tickCount == 0))
            {
                this.SetColor(this.toolStripStatusLabel2, Color.White);
                this.SetText(this.toolStripStatusLabel2, num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'));
            }
            if (this.tickCount == 1)
            {
                this.leftTime--;
            }
        }

        private void timer1_Tick_1(object sender, ElapsedEventArgs e)
        {
            this.tickCount++;
            if (this.timecount < 10)
            {
                this.timecount++;
            }
            else if (this.timecount == 10)
            {
                this.SetEnabled(this.btnBack, true);
            }
            this.tickCount = this.tickCount % 2;
            int num3 = this.leftTime % 60;
            int num2 = ((this.leftTime - num3) / 60) % 60;
            int num = (((this.leftTime - num3) / 60) - num2) / 60;
            if ((this.leftTime > this.endTipTime) && (this.tickCount == 1))
            {
                this.SetText(this.toolStripStatusLabel2, num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'));
                this.leftTime++;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x112)
            {
                if (m.WParam.ToInt32() == 0xf020)
                {
                    this.myHeight = Math.Max(200, base.ClientSize.Height);
                    base.Height -= base.ClientSize.Height;
                    base.MaximizeBox = true;
                    base.MinimizeBox = false;
                    return;
                }
                if (m.WParam.ToInt32() == 0xf030)
                {
                    base.Height += this.myHeight;
                    base.MaximizeBox = false;
                    base.MinimizeBox = true;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        internal bool hasProcess { get; set; }

        private delegate void pexitcallback();

        private delegate void SetColorCallBack(ToolStripStatusLabel control, Color color);

        private delegate void SetEnabledCallBack(Control control, bool enabled);

        private delegate void SetTextCallBack(ToolStripStatusLabel control, string text);
    }
}

