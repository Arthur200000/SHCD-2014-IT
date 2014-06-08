namespace ExamClientControlsLibrary
{
    using Qisi.General;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
	/// <summary>
	/// Test paper.
	/// </summary>
    internal class TestPaper : IDisposable
    {
        internal static bool allowExplorer;
        private int currentPageindex;
        private int endtiptime;
        public string formulaEditType;
        private int handintime;
        internal static string id;
        private string lockWin;
        private int pageChangeTime;
        private List<Page> pageList;
        private TreeNode paperTreeTopNode;
        internal static int questionCount;
        private string subject;
        private Thread t;
        private string testpaperName;
        private XmlNode testpaperNode;
        private bool threadstarted;
        private int totaltime;

        public event EventHandler HideWindow;

        public event Page.IndexEventHandler IndexChanged;

        public event Question.BoolEventHandler IsDown;

        public event EventHandler ShowWindow;
		/// <summary>
		/// Initializes a new instance of the <see cref="ExamClientControlsLibrary.TestPaper"/> class.
		/// </summary>
		/// <param name="testPaperXmlNode">Test paper XML node.</param>
        internal TestPaper(XmlNode testPaperXmlNode)
        {
            string[] strArray;
            int num;
            int num2;
            int num3;
            this.testpaperNode = testPaperXmlNode;
            if (this.testpaperNode.Attributes["name"] != null)
            {
                this.testpaperName = this.testpaperNode.Attributes["name"].Value;
            }
            else
            {
                this.testpaperName = "";
            }
            if (this.testpaperNode.Attributes["subject"] != null)
            {
                this.subject = this.testpaperNode.Attributes["subject"].Value;
            }
            else
            {
                this.subject = "";
            }
            if (this.testpaperNode.Attributes["id"] != null)
            {
                id = this.testpaperNode.Attributes["id"].Value;
            }
            else
            {
                id = "";
            }
            if (this.testpaperNode.Attributes["handintime"] != null)
            {
                strArray = this.testpaperNode.Attributes["handintime"].Value.Split(new char[] { ':' });
                this.handintime = 0;
                for (num = 0; num < strArray.Length; num++)
                {
                    num2 = 1;
                    num3 = 0;
                    while (num3 < num)
                    {
                        num2 *= 60;
                        num3++;
                    }
                    this.handintime += num2 * Convert.ToInt32(strArray[(strArray.Length - num) - 1]);
                }
            }
            else
            {
                this.handintime = 0;
            }
            if (this.testpaperNode.Attributes["endtiptime"] != null)
            {
                strArray = this.testpaperNode.Attributes["endtiptime"].Value.Split(new char[] { ':' });
                this.endtiptime = 0;
                for (num = 0; num < strArray.Length; num++)
                {
                    num2 = 1;
                    num3 = 0;
                    while (num3 < num)
                    {
                        num2 *= 60;
                        num3++;
                    }
                    this.endtiptime += num2 * Convert.ToInt32(strArray[(strArray.Length - num) - 1]);
                }
            }
            else
            {
                this.endtiptime = 0;
            }
            if (this.testpaperNode.Attributes["totaltime"] != null)
            {
                strArray = this.testpaperNode.Attributes["totaltime"].Value.Split(new char[] { ':' });
                this.totaltime = 0;
                for (num = 0; num < strArray.Length; num++)
                {
                    num2 = 1;
                    num3 = 0;
                    while (num3 < num)
                    {
                        num2 *= 60;
                        num3++;
                    }
                    this.totaltime += num2 * Convert.ToInt32(strArray[(strArray.Length - num) - 1]);
                }
            }
            else
            {
                this.totaltime = 0;
            }
            if (this.testpaperNode.Attributes["lockwinds"] != null)
            {
                this.lockWin = this.testpaperNode.Attributes["lockwinds"].Value;
            }
            else
            {
                this.lockWin = "";
            }
            if (this.testpaperNode.Attributes["fetype"] != null)
            {
                this.formulaEditType = this.testpaperNode.Attributes["fetype"].Value;
            }
            else
            {
                this.formulaEditType = "";
            }
            if (TestPaperPlayer.mode != TestPaperPlayer.RunMode.CDAnalysis)
            {
                TestPaperPlayer.answerINI.WriteValue("ANSWER", "PAPERID", id);
                TestPaperPlayer.answerINI.WriteValue("ANSWER", "DoTime", this.totaltime);
                TestPaperPlayer.answerINI.WriteValue("ANSWER", "Course", this.subject);
            }
            questionCount = 0;
            this.pageList = new List<Page>();
            XmlNodeList list = this.testpaperNode.SelectNodes("part");
            foreach (XmlNode node in list)
            {
                string str;
                if (node.Attributes["subject"] != null)
                {
                    str = node.Attributes["subject"].Value;
                }
                else
                {
                    str = "";
                }
                if (((str == TestPaperPlayer.subject) || (str == "")) || (TestPaperPlayer.subject == ""))
                {
                    Part part = new Part(node);
                    this.pageList.AddRange(part.PartPageList);
                }
            }
            int index = 1;
            this.paperTreeTopNode = new TreeNode();
            for (num = 0; num < this.pageList.Count; num++)
            {
                int num5 = index;
                this.pageList[num].IndexChanged += new Page.IndexEventHandler(this.TestPaper_IndexChanged);
                this.pageList[num].IsDown += new Page.BoolIndexEventHandler(this.TestPaper_isDown);
                this.pageList[num].setQuestionIndex(ref index);
                this.pageList[num].HideWindow += new EventHandler(this.TestPaper_HideWindow);
                this.pageList[num].ShowWindow += new EventHandler(this.TestPaper_ShowWindow);
                if (!this.paperTreeTopNode.Nodes.ContainsKey(this.pageList[num].SubjectName))
                {
                    this.paperTreeTopNode.Nodes.Add(this.pageList[num].SubjectName, this.pageList[num].SubjectName, "undo", "undo");
                }
                if (this.pageList[num].IsQuestion)
                {
                    for (num3 = num5; num3 < index; num3++)
                    {
                        this.paperTreeTopNode.Nodes[this.pageList[num].SubjectName].Nodes.Add(num.ToString(), "第" + num3.ToString().PadLeft(2, '0') + "题", "undo");
                    }
                }
            }
            this.currentPageindex = 0;
            allowExplorer = false;
            this.pageChangeTime = 0;
            if (TestPaperPlayer.mode != TestPaperPlayer.RunMode.CDAnalysis)
            {
                TestPaperPlayer.answerINI.WriteValue("Answer", "NUM", (index - 1).ToString());
            }
            this.t = new Thread(new ThreadStart(this.closeOtherWindow));
            this.t.Start();
            this.threadstarted = true;
            foreach (Page page in this.pageList)
            {
                page.PagePanel.TabStop = false;
                this.disableTab(page.PagePanel);
            }
        }

        private void closeOtherWindow()
        {
            while (this.threadstarted)
            {
                if (allowExplorer)
                {
                    ExamClientControlsLibrary.NativeMethods.ShutdownForms(this.lockWin, "考生目录");
                }
                else
                {
                    ExamClientControlsLibrary.NativeMethods.ShutdownForms(this.lockWin);
                }
                Thread.Sleep(0x7d0);
            }
        }

        private void disableTab(Control control)
        {
            foreach (Control control2 in control.Controls)
            {
                control2.TabStop = false;
                this.disableTab(control2);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Page page in this.pageList)
                {
                    page.Dispose();
                }
            }
            for (int i = 0; i < this.pageList.Count; i++)
            {
                this.pageList[i] = null;
            }
            this.pageList = null;
            this.threadstarted = false;
        }

        ~TestPaper()
        {
            this.Dispose(false);
        }

        internal void FreshTree()
        {
            foreach (Page page in this.pageList)
            {
                page.FreshTree();
            }
        }

        internal FlowLayoutPanel getPanel(int pageIndex, int questionIndex)
        {
            string defaultv = "";
            if (TestPaperPlayer.mode != TestPaperPlayer.RunMode.CDAnalysis)
            {
                TestPaperPlayer.answerINI.ReadValue("LOG", "PAGE_" + this.CurrentPageIndex.ToString(), defaultv);
                string[] strArray = new string[] { defaultv, "[", this.pageChangeTime.ToString(), "-", (TestPaperPlayer.examTotalTime - TestPaperPlayer.examTimeLeft).ToString(), "]" };
                TestPaperPlayer.answerINI.WriteValue("LOG", "PAGE_" + this.CurrentPageIndex.ToString(), string.Concat(strArray));
            }
            this.pageList[this.currentPageindex].ResetIndexFont();
            this.currentPageindex = pageIndex;
            if (TestPaperPlayer.mode != TestPaperPlayer.RunMode.CDAnalysis)
            {
                this.pageChangeTime = TestPaperPlayer.examTotalTime - TestPaperPlayer.examTimeLeft;
            }
            this.pageList[pageIndex].SetIndexFont(questionIndex);
            return this.pageList[pageIndex].PagePanel;
        }

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, int hwndCallback);
        internal void Play()
        {
            if (this.testpaperNode.Attributes["audio"] != null)
            {
                Play(Path.Combine(TestPaperPlayer.paperPath, this.testpaperNode.Attributes["audio"].Value), false);
            }
        }

        private static void Play(string MP3_FileName, bool Repeat)
        {
            mciSendString("open \"" + MP3_FileName + "\" type mpegvideo alias MediaFile", null, 0, 0);
            mciSendString("play MediaFile" + (Repeat ? " repeat" : string.Empty), null, 0, 0);
        }

        private void releaseControl(Control control)
        {
            foreach (Control control2 in control.Controls)
            {
                this.releaseControl(control2);
                control2.Dispose();
            }
            control.Dispose();
            control = null;
        }

        private void TestPaper_HideWindow(object sender, EventArgs e)
        {
            if (this.HideWindow != null)
            {
                this.HideWindow(sender, e);
            }
        }

        private void TestPaper_IndexChanged(object sender, IndexEventArgs e)
        {
            this.IndexChanged(this, e);
        }

        private void TestPaper_isDown(object sender, BoolIndexEventArgs e)
        {
            if (this.IsDown != null)
            {
                this.IsDown(this, new BoolEventArgs(e.Message));
            }
        }

        private void TestPaper_ShowWindow(object sender, EventArgs e)
        {
            if (this.ShowWindow != null)
            {
                this.ShowWindow(sender, e);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (allowExplorer)
            {
                ExamClientControlsLibrary.NativeMethods.ShutdownForms(this.lockWin, "考生目录");
            }
            else
            {
                ExamClientControlsLibrary.NativeMethods.ShutdownForms(this.lockWin);
            }
        }

        internal int CurrentPageIndex
        {
            get
            {
                return this.currentPageindex;
            }
        }

        internal Page Currentpanel
        {
            get
            {
                return this.pageList[this.currentPageindex];
            }
        }

        internal int EndTipTime
        {
            get
            {
                return this.endtiptime;
            }
        }

        internal FlowLayoutPanel FirstPanel
        {
            get
            {
                this.pageList[this.currentPageindex].ResetIndexFont();
                this.currentPageindex = 0;
                if (TestPaperPlayer.mode != TestPaperPlayer.RunMode.CDAnalysis)
                {
                    this.pageChangeTime = TestPaperPlayer.examTotalTime - TestPaperPlayer.examTimeLeft;
                }
                this.pageList[this.currentPageindex].SetIndexFont(0);
                return this.pageList[this.currentPageindex].PagePanel;
            }
        }

        internal int HandinTime
        {
            get
            {
                return this.handintime;
            }
        }

        internal bool HasNext
        {
            get
            {
                return (this.currentPageindex != (this.pageList.Count - 1));
            }
        }

        internal bool HasPrev
        {
            get
            {
                return (this.currentPageindex != 0);
            }
        }

        internal TreeNode TestPaperTreeNode
        {
            get
            {
                return this.paperTreeTopNode;
            }
        }

        internal int TotalTime
        {
            get
            {
                return this.totaltime;
            }
        }
    }
}

