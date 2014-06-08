namespace ExamClientControlsLibrary
{
    using ExamClientControlsLibrary.Properties;
    using Qisi.Editor.Controls;
    using Qisi.General;
    using Qisi.General.Controls;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Timers;
    using System.Windows.Forms;
    using System.Xml;

    public class TestPaperPlayer : Control
    {
        internal static MemoryIniFile answerINI;
        internal static string answerPath;
        private Label bottomNext;
        private Label bottomPre;
        private Label clockLabel;
        internal static int endTipTime;
        internal static int examTimeLeft;
        internal static int examTotalTime;
        private Panel foot;
        private FunctionLabelList formulaEditor;
        internal static int handinTime;
        private Qisi.General.Controls.imeBar imeBar;
        internal static bool inOperate;
        private FlowLayoutPanel mainPanel;
        internal static Size mainPanelSize;
        internal static RunMode mode;
        internal static string paperPath;
        private QisiTreeView paperTree;
        internal static string stdAnswerDir;
        internal static MemoryIniFile stdINI;
        private string stuAnswerFile;
        internal static Image stuBMP;
        internal static string stuDataPath;
        internal static string stuInfo;
        internal static string subject;
        private Label subjectTitleLabel;
        private Button submit;
        private TestPaper testpaper;
        internal static int tickCount;
        private System.Timers.Timer timer;
        private Panel title;
        private Label topNext;
        private Label topPre;
        private VolumeControl volumeControl;

        public event EventHandler HandOn;

        public event Qisi.General.MessageEventHandler QuestionChanged;

        public TestPaperPlayer(Size controlSize, List<string> information, string selectdeSubject, RunMode runMode, string unzippedPaperPath, string studentDataPath, string studentAnswerPath, Image studentPic, string keyName, string studentAnswerFileName, string stdAnswer = "", string stdDir = "")
        {
            int num10;
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.Selectable, false);
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            base.Size = controlSize;
            if ((unzippedPaperPath == null) || (unzippedPaperPath == ""))
            {
                throw new Exception("路径错误", new Exception("试卷路径为空"));
            }
            if (!unzippedPaperPath.EndsWith(@"\"))
            {
                unzippedPaperPath = unzippedPaperPath + @"\";
            }
            if ((studentAnswerPath == null) || (studentAnswerPath == ""))
            {
                throw new Exception("路径错误", new Exception("答案保存路径为空"));
            }
            if (!studentAnswerPath.EndsWith(@"\"))
            {
                studentAnswerPath = studentAnswerPath + @"\";
            }
            if ((studentDataPath == null) || (studentDataPath == ""))
            {
                throw new Exception("路径错误", new Exception("考生目录路径为空"));
            }
            if (!studentDataPath.EndsWith(@"\"))
            {
                studentDataPath = studentDataPath + @"\";
            }
            if (!Directory.Exists(unzippedPaperPath))
            {
                throw new Exception("路径错误", new Exception("试卷路径不存在"));
            }
            mode = runMode;
            paperPath = unzippedPaperPath;
            stuDataPath = studentDataPath;
            answerPath = studentAnswerPath;
            answerINI = new MemoryIniFile();
            this.stuAnswerFile = studentAnswerFileName;
            if (mode == RunMode.CDAnalysis)
            {
                if (!Directory.Exists(answerPath))
                {
                    throw new Exception("路径错误", new Exception("答案保存路径不存在"));
                }
                if (!File.Exists(answerPath + this.stuAnswerFile))
                {
                    throw new Exception("路径错误", new Exception("指定路径中答案不存在"));
                }
                answerINI.LoadFromEncodedFile(answerPath + this.stuAnswerFile);
                subject = answerINI.ReadValue("ANSWER", "SUBJECT", "");
            }
            else
            {
                if (!Directory.Exists(answerPath))
                {
                    try
                    {
                        Directory.CreateDirectory(answerPath);
                    }
                    catch
                    {
                        throw new Exception("路径错误", new Exception("创建答案保存路径时出错"));
                    }
                }
                subject = selectdeSubject;
                answerINI.WriteValue("ANSWER", "SUBJECT", subject);
                answerINI.WriteValue("ANSWER", "DoTimeLen", "0");
                answerINI.WriteValue("ANSWER", "MENU", "1");
                answerINI.WriteValue("ANSWER", "ReDo", "0");
            }
            stdINI = new MemoryIniFile();
            if (((mode == RunMode.CDAnalysis) || (mode == RunMode.CDExercise)) && ((stdAnswer != "") && File.Exists(stdAnswer)))
            {
                stdINI.LoadFromFile(stdAnswer);
                if ((stdDir == null) || (stdDir == ""))
                {
                    throw new Exception("路径错误", new Exception("考生目录路径为空"));
                }
                if (!stdDir.EndsWith(@"\"))
                {
                    stdDir = stdDir + @"\";
                }
                stdAnswerDir = stdDir;
            }
            int width = 180;
            int height = 270;
            int num3 = 50;
            int num4 = 30;
            int num5 = 40;
            int num6 = 40;
            int y = 10;
            Size size = new Size(100, 0x84);
            Panel panel = new Panel {
                Padding = new Padding(0, 0, 0, 0),
                Margin = new Padding(0, 0, 0, 0),
                BackColor = Color.SkyBlue,
                Size = new Size(width, height),
                Location = new Point(0, 0)
            };
            base.Controls.Add(panel);
            PictureBox box = new PictureBox {
                Padding = new Padding(0, 0, 0, 0),
                Margin = new Padding(0, 0, 0, 0),
                Location = new Point((width - size.Width) / 2, y),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Size = size
            };
            if (studentPic != null)
            {
                box.Image = studentPic;
            }
            else if (mode == RunMode.Player)
            {
                box.Image = Resources.logo;
            }
            else
            {
                box.Image = Resources.noPic;
            }
            box.Visible = true;
            panel.Controls.Add(box);
            stuBMP = box.Image;
            int num8 = 10;
            Label Label = new Label {
                Font = new Font("黑体", 15f, FontStyle.Bold, GraphicsUnit.Pixel),
                Location = new Point(0, box.Bottom + num8),
                AutoSize = true,
                MaximumSize = new Size(width, 0),
                Visible = true
            };
            if (mode != RunMode.CDAnalysis)
            {
                answerINI.WriteValue("ANSWER", "TESTID", information[3]);
                answerINI.WriteValue("ANSWER", "GENDER", information[1]);
                answerINI.WriteValue("ANSWER", "STUDENTNAME", information[0]);
                answerINI.WriteValue("ANSWER", "SEATNO", information[2]);
                Label.Text = Label.Text + "姓名：" + information[0] + "\r\n";
                Label.Text = Label.Text + "性别：" + information[1] + "\r\n";
                Label.Text = Label.Text + "座位号：" + information[2] + "\r\n";
                Label.Text = Label.Text + keyName + "：" + information[3] + "\r\n";
            }
            else
            {
                Label.Text = Label.Text + "姓名：" + answerINI.ReadValue("ANSWER", "STUDENTNAME", "") + "\r\n";
                Label.Text = Label.Text + "性别：" + answerINI.ReadValue("ANSWER", "GENDER", "") + "\r\n";
                Label.Text = Label.Text + "座位号：" + answerINI.ReadValue("ANSWER", "SEATNO", "") + "\r\n";
                Label.Text = Label.Text + keyName + "：" + answerINI.ReadValue("ANSWER", "TESTID", "") + "\r\n";
            }
            Label.Text = Label.Text + "选考模块：" + subject;
            panel.Controls.Add(Label);
            stuInfo = Label.Text;
            Clock clock = new Clock();
            base.Controls.Add(clock);
            clock.Size = new Size(num3, num3);
            clock.Location = new Point(15, base.Height - num3);
            clock.Visible = true;
            this.clockLabel = new Label();
            this.clockLabel.Location = new Point(clock.Right, clock.Top);
            this.clockLabel.Size = new Size(width - clock.Right, num3);
            this.clockLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.clockLabel.Font = new Font("黑体", 20f, FontStyle.Bold, GraphicsUnit.Pixel);
            base.Controls.Add(this.clockLabel);
            this.timer = new System.Timers.Timer(500.0);
            this.timer.Elapsed += new ElapsedEventHandler(this.timer1_Tick);
            this.timer.AutoReset = true;
            this.submit = new Button();
            this.submit.BackColor = Color.White;
            this.submit.Size = new Size(width, num4);
            this.submit.Location = new Point(0, clock.Top - num4);
            this.submit.Font = new Font("微软雅黑", 18f, FontStyle.Bold, GraphicsUnit.Pixel);
            this.submit.Cursor = Cursors.Hand;
            this.submit.TextAlign = ContentAlignment.MiddleCenter;
            base.Controls.Add(this.submit);
            if ((mode == RunMode.CDExercise) || (mode == RunMode.CDAnalysis))
            {
                this.submit.Text = "退出";
                this.submit.Enabled = true;
            }
            else if (mode == RunMode.Player)
            {
                this.submit.Text = "交卷";
                this.submit.Enabled = true;
            }
            else
            {
                this.submit.Text = "交卷";
                this.submit.Enabled = false;
            }
            this.submit.Click += new EventHandler(this.submit_Click);
            if (mode == RunMode.Player)
            {
                Label Label2 = new Label {
                    Font = new Font("微软雅黑", 14f, FontStyle.Regular, GraphicsUnit.Pixel),
                    Text = "字体",
                    Location = new Point(0, this.submit.Top - num5),
                    Size = new Size(num5, num5),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                base.Controls.Add(Label2);
                TrackBar bar = new TrackBar {
                    Location = new Point(Label2.Right, Label2.Top),
                    AutoSize = false,
                    Size = new Size(width - num5, num5),
                    Minimum = 0,
                    Maximum = 100,
                    Value = 50,
                    TickStyle = TickStyle.None,
                    Cursor = Cursors.Hand
                };
                base.Controls.Add(bar);
                Label Label3 = new Label {
                    Font = new Font("微软雅黑", 14f, FontStyle.Regular, GraphicsUnit.Pixel),
                    Text = "颜色",
                    Location = new Point(0, Label2.Top - num5),
                    Size = new Size(num6, num6),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                base.Controls.Add(Label3);
                int num9 = (width - num6) / 8;
                Color[] colorArray = new Color[] { Color.FromArgb(0xff, 0xff, 0xff), Color.FromArgb(250, 0xf9, 0xde), Color.FromArgb(0xff, 0xf2, 0xe2), Color.FromArgb(0xfd, 230, 0xe0), Color.FromArgb(0xe3, 0xed, 0xcd), Color.FromArgb(220, 0xe2, 0xf1), Color.FromArgb(0xe9, 0xeb, 0xfe), Color.FromArgb(0xea, 0xea, 0xef) };
                for (num10 = 0; num10 < colorArray.Length; num10++)
                {
                    Label Label4 = new Label {
                        Text = "",
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = colorArray[num10],
                        Size = new Size(num9, num9),
                        Location = new Point(Label3.Right + (num10 * num9), (Label3.Top + (Label3.Height / 2)) - (num9 / 2)),
                        Cursor = Cursors.Hand
                    };
                    base.Controls.Add(Label4);
                }
            }
            this.paperTree = new QisiTreeView();
            this.paperTree.Location = new Point(0, panel.Bottom);
            if (mode == RunMode.Player)
            {
                this.paperTree.Size = new Size(width, ((this.submit.Top - num5) - num5) - panel.Bottom);
            }
            else
            {
                this.paperTree.Size = new Size(width, this.submit.Top - panel.Bottom);
            }
            this.paperTree.ImageList = new ImageList();
            this.paperTree.ImageList.Images.Add("undo", Resources.undo);
            this.paperTree.ImageList.Images.Add("done", Resources.done);
            this.paperTree.Font = new Font("微软雅黑", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
            this.paperTree.ImeMode = ImeMode.Disable;
            this.paperTree.ShowPlusMinus = false;
            this.paperTree.ShowLines = false;
            this.paperTree.ShowRootLines = false;
            base.Controls.Add(this.paperTree);
            int x = 20;
            int num12 = 50;
            int num13 = 0x2d;
            this.title = new Panel();
            this.title.Location = new Point(width, 0);
            this.title.Size = new Size(base.Width - width, num13);
            this.title.Paint += new PaintEventHandler(this.Title_Paint);
            this.subjectTitleLabel = new Label();
            this.subjectTitleLabel.Font = new Font("宋体", 28f, FontStyle.Regular, GraphicsUnit.Pixel);
            this.subjectTitleLabel.BackColor = Color.Transparent;
            this.subjectTitleLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.subjectTitleLabel.AutoSize = true;
            this.subjectTitleLabel.Location = new Point(x, 0);
            this.title.Controls.Add(this.subjectTitleLabel);
            this.topNext = new Label();
            this.topNext.Text = "下一页";
            this.topNext.Enabled = false;
            this.topNext.Font = new Font("宋体", 20f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Pixel);
            this.topNext.ForeColor = Color.Blue;
            this.topNext.BackColor = Color.Transparent;
            this.topNext.Visible = true;
            this.topNext.Cursor = Cursors.Hand;
            this.topNext.AutoSize = true;
            this.topNext.Click += new EventHandler(this.Next_Click);
            this.topPre = new Label();
            this.topPre.Text = "上一页";
            this.topPre.Enabled = false;
            this.topPre.Font = new Font("宋体", 20f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Pixel);
            this.topPre.ForeColor = Color.Blue;
            this.topPre.BackColor = Color.Transparent;
            this.topPre.Visible = true;
            this.topPre.Cursor = Cursors.Hand;
            this.topPre.AutoSize = true;
            this.topPre.Click += new EventHandler(this.Pre_Click);
            this.title.Controls.Add(this.topNext);
            this.topNext.Location = new Point((this.title.Width - this.topNext.Width) - num12, (this.title.ClientSize.Height / 2) - (this.topNext.Height / 2));
            this.title.Controls.Add(this.topPre);
            this.topPre.Location = new Point(this.topNext.Left - this.topPre.Width, (this.title.ClientSize.Height / 2) - (this.topPre.Height / 2));
            base.Controls.Add(this.title);
            int num14 = 20;
            this.foot = new Panel();
            base.Controls.Add(this.foot);
            this.foot.Location = new Point(width, base.Height - num14);
            this.foot.Size = new Size(base.Width - width, num14);
            this.foot.Paint += new PaintEventHandler(this.Title_Paint);
            this.bottomNext = new Label();
            this.bottomNext.Text = "下一页";
            this.bottomNext.Font = new Font("宋体", 16f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Pixel);
            this.bottomNext.ForeColor = Color.Blue;
            this.bottomNext.BackColor = Color.Transparent;
            this.bottomNext.Visible = true;
            this.bottomNext.Cursor = Cursors.Hand;
            this.bottomNext.AutoSize = true;
            this.bottomNext.Click += new EventHandler(this.Next_Click);
            this.bottomPre = new Label();
            this.bottomPre.Text = "上一页";
            this.bottomPre.Enabled = false;
            this.bottomPre.Font = new Font("宋体", 16f, FontStyle.Underline | FontStyle.Bold, GraphicsUnit.Pixel);
            this.bottomPre.ForeColor = Color.Blue;
            this.bottomPre.BackColor = Color.Transparent;
            this.bottomPre.Visible = true;
            this.bottomPre.Cursor = Cursors.Hand;
            this.bottomPre.AutoSize = true;
            this.bottomPre.Click += new EventHandler(this.Pre_Click);
            this.foot.Controls.Add(this.bottomNext);
            this.bottomNext.Location = new Point((this.foot.Width - this.bottomNext.Width) - num12, (this.foot.ClientSize.Height / 2) - (this.bottomNext.Height / 2));
            this.foot.Controls.Add(this.bottomPre);
            this.bottomPre.Location = new Point(this.bottomNext.Left - this.bottomPre.Width, (this.foot.ClientSize.Height / 2) - (this.bottomPre.Height / 2));
            this.imeBar = new Qisi.General.Controls.imeBar();
            this.imeBar.Height = this.foot.ClientSize.Height;
            this.imeBar.Width = Math.Max(200, this.bottomPre.Left - ((base.Width - width) / 2));
            this.imeBar.Top = 1;
            this.imeBar.Left = (this.bottomPre.Left - this.imeBar.Width) - 20;
            this.imeBar.Visible = true;
            this.foot.Controls.Add(this.imeBar);
            this.volumeControl = new VolumeControl();
            this.volumeControl.Top = 0;
            this.volumeControl.Left = 30;
            this.volumeControl.Height = this.foot.ClientSize.Height;
            this.volumeControl.Visible = false;
            this.foot.Controls.Add(this.volumeControl);
            int verticalScrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            mainPanelSize = new Size((controlSize.Width - width) - verticalScrollBarWidth, this.foot.Top - this.title.Bottom);
            this.mainPanel = new FlowLayoutPanel();
            this.mainPanel.Margin = new Padding(0, 0, 0, 0);
            this.mainPanel.Padding = new Padding(0, 0, 0, 0);
            this.mainPanel.Size = new Size(controlSize.Width - width, this.foot.Top - this.title.Bottom);
            this.mainPanel.AutoScroll = true;
            XmlDocument document = new XmlDocument();
            string xml = File.ReadAllText(paperPath + "testpaper.xml", Encoding.UTF8);
            document.LoadXml(xml);
            this.testpaper = new TestPaper(document.DocumentElement);
            this.testpaper.IndexChanged += new Page.IndexEventHandler(this.testpaper_IndexChanged);
            this.testpaper.IsDown += new Question.BoolEventHandler(this.testpaper_isDown);
            this.testpaper.ShowWindow += new EventHandler(this.testpaper_ShowWindow);
            this.testpaper.HideWindow += new EventHandler(this.testpaper_HideWindow);
            examTotalTime = this.testpaper.TotalTime;
            examTimeLeft = examTotalTime;
            handinTime = this.testpaper.HandinTime;
            endTipTime = this.testpaper.EndTipTime;
            tickCount = 0;
            if (this.testpaper.formulaEditType != "")
            {
                this.formulaEditor = new FunctionLabelList(this.testpaper.formulaEditType, mainPanelSize.Width, true);
                this.formulaEditor.Visible = false;
                this.formulaEditor.Location = new Point(width, this.title.Bottom);
                this.formulaEditor.VisibleChanged += new EventHandler(this.fe_VisibleChanged);
                this.formulaEditor.SizeChanged += new EventHandler(this.fe_SizeChanged);
                this.formulaEditor.Visible = this.testpaper.Currentpanel.ShowFormuleEditor;
                base.Controls.Add(this.formulaEditor);
            }
            else
            {
                this.formulaEditor = null;
            }
            this.mainPanel.Controls.Add(this.testpaper.FirstPanel);
            this.volumeControl.Visible = this.testpaper.Currentpanel.ShowVolumnControl;
            base.Controls.Add(this.mainPanel);
            this.mainPanel.Location = new Point(width, this.title.Bottom);
            this.subjectTitleLabel.Text = this.testpaper.Currentpanel.SubjectName;
            for (num10 = 0; num10 < this.testpaper.TestPaperTreeNode.Nodes.Count; num10++)
            {
                this.paperTree.Nodes.Add(this.testpaper.TestPaperTreeNode.Nodes[num10]);
            }
            this.paperTree.ExpandAll();
            this.paperTree.AfterSelect += new TreeViewEventHandler(this.PaperTree_AfterSelect);
            this.bottomNext.Enabled = this.testpaper.HasNext;
            this.topNext.Enabled = this.testpaper.HasNext;
            this.bottomPre.Enabled = this.testpaper.HasPrev;
            this.topPre.Enabled = this.testpaper.HasPrev;
            inOperate = false;
            base.TabStop = false;
            this.disableTab(this);
            this.Refresh();
        }

        private void disableTab(Control control)
        {
            foreach (Control control2 in control.Controls)
            {
                control2.TabStop = false;
                this.disableTab(control2);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.timer != null)
                {
                    this.timer.Stop();
                    this.timer.Dispose();
                    this.timer = null;
                }
                if (answerINI != null)
                {
                    answerINI.Dispose();
                    answerINI = null;
                }
                if (stdINI != null)
                {
                    stdINI.Dispose();
                    stdINI = null;
                }
                if (this.testpaper != null)
                {
                    this.testpaper.Dispose();
                    this.testpaper = null;
                }
                if (stuBMP != null)
                {
                    stuBMP.Dispose();
                    stuBMP = null;
                }
                if (this.clockLabel != null)
                {
                    this.clockLabel.Dispose();
                    this.clockLabel = null;
                }
                if (this.paperTree != null)
                {
                    this.paperTree.Dispose();
                    this.paperTree = null;
                }
                if (this.submit != null)
                {
                    this.submit.Dispose();
                    this.submit = null;
                }
                if (this.subjectTitleLabel != null)
                {
                    this.subjectTitleLabel.Dispose();
                    this.subjectTitleLabel = null;
                }
                if (this.volumeControl != null)
                {
                    this.volumeControl.Dispose();
                    this.volumeControl = null;
                }
                if (this.formulaEditor != null)
                {
                    this.formulaEditor.Dispose();
                    this.formulaEditor = null;
                }
                if (this.imeBar != null)
                {
                    this.imeBar.Dispose();
                    this.imeBar = null;
                }
                if (this.topPre != null)
                {
                    this.topPre.Dispose();
                    this.topPre = null;
                }
                if (this.topNext != null)
                {
                    this.topNext.Dispose();
                    this.topNext = null;
                }
                if (this.bottomPre != null)
                {
                    this.bottomPre.Dispose();
                    this.bottomPre = null;
                }
                if (this.bottomNext != null)
                {
                    this.bottomNext.Dispose();
                    this.bottomNext = null;
                }
                if (this.title != null)
                {
                    this.title.Dispose();
                    this.title = null;
                }
                if (this.foot != null)
                {
                    this.foot.Dispose();
                    this.foot = null;
                }
                if (this.mainPanel != null)
                {
                    this.mainPanel.Dispose();
                    this.mainPanel = null;
                }
            }
            paperPath = "";
            stuDataPath = "";
            this.stuAnswerFile = "";
            answerPath = "";
            stdAnswerDir = "";
            examTotalTime = 0;
            examTimeLeft = 0;
            endTipTime = 0;
            handinTime = 0;
            tickCount = 0;
            subject = "";
            stuInfo = "";
            base.Dispose(disposing);
        }

        private void fe_SizeChanged(object sender, EventArgs e)
        {
            if (this.formulaEditor != null)
            {
                this.mainPanel.Location = new Point(this.mainPanel.Left, this.formulaEditor.Bottom);
                this.mainPanel.Size = new Size(this.mainPanel.Size.Width, this.foot.Top - this.formulaEditor.Bottom);
            }
        }

        private void fe_VisibleChanged(object sender, EventArgs e)
        {
            if ((this.formulaEditor != null) && this.formulaEditor.Visible)
            {
                this.mainPanel.Location = new Point(this.mainPanel.Left, this.formulaEditor.Bottom);
                this.mainPanel.Size = new Size(this.mainPanel.Size.Width, this.foot.Top - this.formulaEditor.Bottom);
            }
            else
            {
                this.mainPanel.Location = new Point(this.mainPanel.Left, this.title.Bottom);
                this.mainPanel.Size = new Size(this.mainPanel.Size.Width, this.foot.Top - this.title.Bottom);
            }
        }

        ~TestPaperPlayer()
        {
            this.Dispose(false);
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if (this.paperTree.SelectedNode == null)
            {
                this.paperTree.SelectedNode = this.paperTree.Nodes[0];
            }
            else if (this.paperTree.SelectedNode.Level == 0)
            {
                if (this.paperTree.SelectedNode.Nodes.Count > 0)
                {
                    this.paperTree.SelectedNode = this.paperTree.SelectedNode.Nodes[0];
                }
            }
            else if (this.paperTree.SelectedNode.NextNode != null)
            {
                TreeNode selectedNode = this.paperTree.SelectedNode;
                while ((selectedNode.NextNode != null) && (selectedNode.NextNode.Name == selectedNode.Name))
                {
                    selectedNode = selectedNode.NextNode;
                }
                if (selectedNode.NextNode != null)
                {
                    selectedNode = selectedNode.NextNode;
                }
                else if ((selectedNode.Parent != null) && (selectedNode.Parent.NextNode != null))
                {
                    selectedNode = selectedNode.Parent.NextNode;
                }
                this.paperTree.SelectedNode = selectedNode;
            }
            else if (this.paperTree.SelectedNode.Parent.NextNode != null)
            {
                this.paperTree.SelectedNode = this.paperTree.SelectedNode.Parent.NextNode;
            }
        }

        private void PaperTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int num;
            int questionIndex = 0;
            if (e.Node.Level == 0)
            {
                num = Convert.ToInt32(e.Node.Nodes[0].Name) - 1;
            }
            else
            {
                num = Convert.ToInt32(e.Node.Name);
                if (e.Node.PrevNode != null)
                {
                    for (TreeNode node = e.Node; (node.PrevNode != null) && (node.PrevNode.Name == e.Node.Name); node = node.PrevNode)
                    {
                        questionIndex++;
                    }
                }
            }
            if (num != this.testpaper.CurrentPageIndex)
            {
                this.mainPanel.Controls.Clear();
                this.mainPanel.Controls.Add(this.testpaper.getPanel(num, questionIndex));
                this.volumeControl.Visible = this.testpaper.Currentpanel.ShowVolumnControl;
                if (this.formulaEditor != null)
                {
                    this.formulaEditor.Visible = this.testpaper.Currentpanel.ShowFormuleEditor;
                }
            }
            else
            {
                this.testpaper.getPanel(num, questionIndex);
            }
            this.bottomNext.Enabled = this.testpaper.HasNext;
            this.topNext.Enabled = this.testpaper.HasNext;
            this.bottomPre.Enabled = this.testpaper.HasPrev;
            this.topPre.Enabled = this.testpaper.HasPrev;
            this.subjectTitleLabel.Text = this.testpaper.Currentpanel.SubjectName;
            if (this.QuestionChanged != null)
            {
                this.QuestionChanged(this, new MessageEventArgs(this.paperTree.SelectedNode.Text));
            }
        }

        private void PaperTree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void PerformClick(Button b)
        {
            if (b.InvokeRequired)
            {
                performClickCallBack method = new performClickCallBack(this.PerformClick);
                b.BeginInvoke(method, new object[] { b });
            }
            else
            {
                b.PerformClick();
            }
        }

        private void Pre_Click(object sender, EventArgs e)
        {
            if (this.paperTree.SelectedNode == null)
            {
                this.paperTree.SelectedNode = this.paperTree.Nodes[0];
            }
            else if (this.paperTree.SelectedNode.Level == 0)
            {
                if ((this.paperTree.SelectedNode.PrevNode != null) && (this.paperTree.SelectedNode.PrevNode.Nodes.Count > 0))
                {
                    this.paperTree.SelectedNode = this.paperTree.SelectedNode.PrevNode.LastNode;
                }
                else if (this.paperTree.SelectedNode.PrevNode != null)
                {
                    this.paperTree.SelectedNode = this.paperTree.SelectedNode.PrevNode;
                }
                else
                {
                    this.mainPanel.Controls.Clear();
                    this.mainPanel.Controls.Add(this.testpaper.getPanel(0, 0));
                    this.bottomNext.Enabled = this.testpaper.HasNext;
                    this.topNext.Enabled = this.testpaper.HasNext;
                    this.bottomPre.Enabled = this.testpaper.HasPrev;
                    this.topPre.Enabled = this.testpaper.HasPrev;
                    this.subjectTitleLabel.Text = this.testpaper.Currentpanel.SubjectName;
                }
            }
            else if (this.paperTree.SelectedNode.PrevNode != null)
            {
                TreeNode selectedNode = this.paperTree.SelectedNode;
                while ((selectedNode.PrevNode != null) && (selectedNode.PrevNode.Name == selectedNode.Name))
                {
                    selectedNode = selectedNode.PrevNode;
                }
                if (selectedNode.PrevNode != null)
                {
                    selectedNode = selectedNode.PrevNode;
                }
                else if (selectedNode.Parent != null)
                {
                    selectedNode = selectedNode.Parent;
                }
                this.paperTree.SelectedNode = selectedNode;
            }
            else if (this.paperTree.SelectedNode.Parent != null)
            {
                this.paperTree.SelectedNode = this.paperTree.SelectedNode.Parent;
            }
        }

        private void setControlColor(Control control, Color color)
        {
            if (control.InvokeRequired)
            {
                setControlColorCallBack method = new setControlColorCallBack(this.setControlColor);
                control.BeginInvoke(method, new object[] { control, color });
            }
            else
            {
                control.ForeColor = color;
            }
        }

        private void setControlEnabled(Control c, bool b)
        {
            if (c.InvokeRequired)
            {
                setControlEnabledCallBack method = new setControlEnabledCallBack(this.setControlEnabled);
                c.BeginInvoke(method, new object[] { c, b });
            }
            else
            {
                c.Enabled = b;
                c.ForeColor = Color.Black;
            }
        }

        private void setControlText(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                setControlTextCallBack method = new setControlTextCallBack(this.setControlText);
                control.BeginInvoke(method, new object[] { control, text });
            }
            else
            {
                control.Text = text;
            }
        }

        private void setControlVisible(Control control, bool visible)
        {
            if (control.InvokeRequired)
            {
                setControlVisibleCallBack method = new setControlVisibleCallBack(this.setControlVisible);
                base.Parent.Invoke(method, new object[] { control, visible });
            }
            else
            {
                control.Visible = visible;
            }
        }

        private void setPaperTreeImageKey(bool isDone)
        {
            if (this.paperTree.InvokeRequired)
            {
                setPaperTreeImageKeyCallBack method = new setPaperTreeImageKeyCallBack(this.setPaperTreeImageKey);
                this.paperTree.Invoke(method, new object[] { isDone });
            }
            else if (isDone)
            {
                this.paperTree.SelectedNode.ImageKey = "done";
                this.paperTree.SelectedNode.SelectedImageKey = "done";
            }
            else
            {
                this.paperTree.SelectedNode.ImageKey = "undo";
                this.paperTree.SelectedNode.SelectedImageKey = "undo";
            }
        }

        private void setPictureBoxImage(PictureBox pic, Bitmap bmp)
        {
            if (pic.InvokeRequired)
            {
                setPictureBoxImageCallBack method = new setPictureBoxImageCallBack(this.setPictureBoxImage);
                pic.BeginInvoke(method, new object[] { pic, bmp });
            }
            else
            {
                pic.Image = bmp;
                pic.Refresh();
            }
        }

        public void StartCounting()
        {
            this.timer.Start();
            this.testpaper.Play();
        }

        public void StopCounting()
        {
            this.timer.Stop();
        }

        private void submit_Click(object sender, EventArgs e)
        {
            if (FlatMessageBox.Show(this, "确定要" + this.submit.Text + "吗?", "提示", FlatMessageBox.KeysButtons.OKCancel, FlatMessageBox.KeysIcon.Information) == DialogResult.OK)
            {
                while (inOperate)
                {
                    Thread.Sleep(500);
                }
                this.timer.Stop();
                if (this.HandOn != null)
                {
                    this.HandOn(this, new EventArgs());
                }
            }
        }

        private void testpaper_HideWindow(object sender, EventArgs e)
        {
            base.Parent.Hide();
        }

        private void testpaper_IndexChanged(object sender, IndexEventArgs e)
        {
            if (this.paperTree != null)
            {
                TreeNode[] nodeArray = this.paperTree.Nodes.Find(this.testpaper.CurrentPageIndex.ToString(), true);
                if ((nodeArray.Length > e.Index) && (this.paperTree.SelectedNode != nodeArray[e.Index]))
                {
                    this.paperTree.SelectedNode = nodeArray[e.Index];
                }
            }
        }

        private void testpaper_isDown(object sender, BoolEventArgs e)
        {
            this.setPaperTreeImageKey(e.Message);
        }

        private void testpaper_ShowWindow(object sender, EventArgs e)
        {
            if (base.Parent != null)
            {
                this.setControlVisible(base.Parent, true);
            }
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            int num;
            int num2;
            int num3;
            tickCount++;
            if ((mode == RunMode.CDAnalysis) || (mode == RunMode.CDExercise))
            {
                num3 = (tickCount / 2) % 60;
                num2 = (((tickCount / 2) - num3) / 60) % 60;
                num = ((((tickCount / 2) - num3) / 60) - num2) / 60;
                this.setControlText(this.clockLabel, num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'));
                if (mode == RunMode.CDExercise)
                {
                    answerINI.WriteValue("ANSWER", "DoTimeLen", tickCount.ToString());
                    answerINI.SaveToEncryptedFile(answerPath + this.stuAnswerFile);
                }
            }
            else if ((mode == RunMode.CDSimulate) || (mode == RunMode.Exam))
            {
                int num4;
                tickCount = tickCount % 2;
                if ((examTimeLeft <= 0) && (tickCount == 1))
                {
                    num3 = examTimeLeft % 60;
                    num2 = ((examTimeLeft - num3) / 60) % 60;
                    num = (((examTimeLeft - num3) / 60) - num2) / 60;
                    this.setControlText(this.clockLabel, num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'));
                    num4 = examTotalTime - examTimeLeft;
                    answerINI.WriteValue("ANSWER", "DoTimeLen", num4.ToString());
                    answerINI.SaveToEncryptedFile(answerPath + this.stuAnswerFile);
                    this.PerformClick(this.submit);
                }
                else if ((examTimeLeft > endTipTime) && (tickCount == 1))
                {
                    num3 = examTimeLeft % 60;
                    num2 = ((examTimeLeft - num3) / 60) % 60;
                    num = (((examTimeLeft - num3) / 60) - num2) / 60;
                    this.setControlText(this.clockLabel, num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'));
                    examTimeLeft--;
                    num4 = examTotalTime - examTimeLeft;
                    answerINI.WriteValue("ANSWER", "DoTimeLen", num4.ToString());
                    answerINI.SaveToEncryptedFile(answerPath + this.stuAnswerFile);
                }
                else if ((examTimeLeft <= endTipTime) && (tickCount == 1))
                {
                    num3 = examTimeLeft % 60;
                    num2 = ((examTimeLeft - num3) / 60) % 60;
                    num = (((examTimeLeft - num3) / 60) - num2) / 60;
                    this.setControlColor(this.clockLabel, Color.Red);
                    this.setControlText(this.clockLabel, num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'));
                    examTimeLeft--;
                    answerINI.WriteValue("ANSWER", "DoTimeLen", (examTotalTime - examTimeLeft).ToString());
                    answerINI.SaveToEncryptedFile(answerPath + this.stuAnswerFile);
                }
                else if ((examTimeLeft <= endTipTime) && (tickCount == 0))
                {
                    this.setControlText(this.clockLabel, "");
                }
                if (examTimeLeft <= handinTime)
                {
                    this.setControlEnabled(this.submit, true);
                }
            }
        }

        private void Title_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Color white = Color.White;
            Color green = Color.Green;
            Brush brush = new LinearGradientBrush(((Panel) sender).ClientRectangle, white, green, LinearGradientMode.ForwardDiagonal);
            graphics.FillRectangle(brush, ((Panel) sender).ClientRectangle);
        }

        private delegate void performClickCallBack(Button button);

        public enum RunMode
        {
            Exam,
            CDExercise,
            CDSimulate,
            CDAnalysis,
            Player
        }

        private delegate void setControlColorCallBack(Control control, Color color);

        private delegate void setControlEnabledCallBack(Control control, bool enabled);

        private delegate void setControlTextCallBack(Control control, string text);

        private delegate void setControlVisibleCallBack(Control control, bool visible);

        private delegate void setPaperTreeImageKeyCallBack(bool isDone);

        private delegate void setPictureBoxImageCallBack(PictureBox picturebox, Bitmap bitmap);
    }
}

