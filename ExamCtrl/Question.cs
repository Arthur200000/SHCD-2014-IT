namespace ExamClientControlsLibrary
{
    using Qisi.Editor.Controls;
    using Qisi.General;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.AccessControl;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    public class Question
    {
        private string answer;
        private string answerid;
        private string id;
        private Label indexLabel = new Label();
        private int indexlabelfontsize = 12;
        private static int questionChangeTime;
        private SuperBox superbox;

        public event EventHandler HideWindow;

        public event EventHandler IndexChanged;

        public event BoolEventHandler isDone;

        public event EventHandler ShowWindow;

        public Question(XmlNode xmlnode, int panelwidth, int QindexWidth)
        {
            this.id = xmlnode.Attributes["id"].Value;
            this.indexLabel.AutoSize = false;
            this.indexLabel.Font = new Font("黑体", (float) this.indexlabelfontsize, FontStyle.Bold, GraphicsUnit.Pixel);
            this.indexLabel.Size = new Size(QindexWidth, this.indexlabelfontsize * 3);
            this.indexLabel.TextAlign = ContentAlignment.MiddleRight;
            this.indexLabel.Text = "";
            this.indexLabel.Margin = new Padding(0, 0, 0, 0);
            this.superbox = new SuperBox(Math.Max(0, panelwidth - 30));
            this.superbox.ReadOnly = true;
            this.superbox.ContentChanged += new EventHandler(this.sb_ContentChanged);
            string file = Path.Combine(Path.Combine(TestPaperPlayer.paperPath, TestPaper.id + "_" + this.id), "Question.xml");
            this.superbox.LoadFromXml(file);
            this.superbox.Margin = new Padding(0, 0, 0, 0);
            this.superbox.OperateClicked += new EventHandler(this.superbox_OperateClicked);
            this.superbox.OperateDone += new SuperBox.MessageEventHandler(this.superbox_OperateDone);
            this.Done = false;
            if (TestPaperPlayer.mode == TestPaperPlayer.RunMode.CDAnalysis)
            {
                int num = TestPaperPlayer.answerINI.ReadValue("Answer", "NUM", 0);
                for (int i = 0; i < num; i++)
                {
                    if (TestPaperPlayer.answerINI.ReadValue(i.ToString(), "ID", "") == xmlnode.Attributes["id"].Value)
                    {
                        string str2 = TestPaperPlayer.answerINI.ReadValue(i.ToString(), "AnswerXML", "");
                        if (str2.StartsWith("["))
                        {
                            str2 = str2.Substring(1);
                        }
                        if (str2.EndsWith("]"))
                        {
                            str2 = str2.Substring(0, str2.Length - 1);
                        }
                        this.superbox.FillIn(str2.Split(new char[] { '\x001e' }), TestPaperPlayer.answerPath, this.id);
                    }
                }
            }
            this.superbox.MouseMove += new MouseEventHandler(this.QuestionPanel_MouseMove);
            this.answerid = TestPaper.questionCount.ToString();
            TestPaper.questionCount++;
            if (TestPaperPlayer.mode != TestPaperPlayer.RunMode.CDAnalysis)
            {
                this.superbox.Enter += new EventHandler(this.QuestionPanel_Enter);
                this.superbox.Leave += new EventHandler(this.QuestionPanel_Leave);
                TestPaperPlayer.answerINI.WriteValue(this.answerid, "ID", xmlnode.Attributes["id"].Value);
                TestPaperPlayer.answerINI.WriteValue(this.answerid, "Answer", "[" + this.superbox.getContent() + "]");
                TestPaperPlayer.answerINI.WriteValue(this.answerid, "AnswerXML", "[" + this.superbox.getContentXml() + "]");
            }
        }

        internal void freshtree()
        {
            string[] strArray = this.superbox.getContent().Split(new char[] { '\x001e' });
            bool flag = true;
            foreach (string str2 in strArray)
            {
                if (str2 == "")
                {
                    flag = false;
                }
            }
            if (flag != this.Done)
            {
                this.Done = flag;
                if (this.isDone != null)
                {
                    this.isDone(this, new BoolEventArgs(this.Done));
                }
            }
        }

        internal void LoadSTD(string OptionAnswer)
        {
            this.superbox.LoadOptionSTD(OptionAnswer);
        }

        private void QuestionPanel_Enter(object sender, EventArgs e)
        {
            if (TestPaperPlayer.mode != TestPaperPlayer.RunMode.CDAnalysis)
            {
                questionChangeTime = TestPaperPlayer.examTotalTime - TestPaperPlayer.examTimeLeft;
            }
        }

        private void QuestionPanel_Leave(object sender, EventArgs e)
        {
            if (TestPaperPlayer.mode != TestPaperPlayer.RunMode.CDAnalysis)
            {
                string str = "";
                TestPaperPlayer.answerINI.WriteValue("LOG", this.id, str);
                string[] strArray = new string[] { str, "[", questionChangeTime.ToString(), "-", (TestPaperPlayer.examTotalTime - TestPaperPlayer.examTimeLeft).ToString(), "]" };
                TestPaperPlayer.answerINI.ReadValue("LOG", this.id, string.Concat(strArray));
            }
        }

        private void QuestionPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsCurrent)
            {
                this.IndexChanged(this, new EventArgs());
                this.superbox.Focus();
            }
        }

        private void sb_ContentChanged(object sender, EventArgs e)
        {
            TestPaperPlayer.answerINI.WriteValue(this.answerid, "Answer", "[" + this.superbox.getContent() + "]");
            TestPaperPlayer.answerINI.WriteValue(this.answerid, "AnswerXML", "[" + this.superbox.getContentXml() + "]");
            string[] strArray = this.superbox.getContent().Split(new char[] { '\x001e' });
            bool flag = true;
            foreach (string str2 in strArray)
            {
                if (str2 == "")
                {
                    flag = false;
                }
            }
            if (flag != this.Done)
            {
                this.Done = flag;
                if (this.isDone != null)
                {
                    this.isDone(this, new BoolEventArgs(this.Done));
                }
            }
        }

        private void showAnswer_Click(object sender, EventArgs e)
        {
            this.ShowAnwser();
        }

        private void ShowAnwser()
        {
        }

        private void ShowStudentAnwser()
        {
        }

        private void superbox_OperateClicked(object sender, EventArgs e)
        {
            CommonMethods.ClearDirectory(TestPaperPlayer.stuDataPath);
            this.HideWindow(this, new EventArgs());
            TestPaper.allowExplorer = true;
            TestPaperPlayer.inOperate = true;
            if (TestPaperPlayer.mode == TestPaperPlayer.RunMode.CDAnalysis)
            {
                this.superbox.DoOperate(sender, TestPaperPlayer.stuBMP, TestPaperPlayer.stuInfo, TestPaperPlayer.stuDataPath, TestPaperPlayer.tickCount, -1, Path.Combine(TestPaperPlayer.stdAnswerDir, this.id));
            }
            else if (TestPaperPlayer.mode == TestPaperPlayer.RunMode.CDExercise)
            {
                this.superbox.DoOperate(sender, TestPaperPlayer.stuBMP, TestPaperPlayer.stuInfo, TestPaperPlayer.stuDataPath, TestPaperPlayer.tickCount, -1, Path.Combine(TestPaperPlayer.answerPath, ""));
            }
            else
            {
                this.superbox.DoOperate(sender, TestPaperPlayer.stuBMP, TestPaperPlayer.stuInfo, TestPaperPlayer.stuDataPath, TestPaperPlayer.examTimeLeft, TestPaperPlayer.endTipTime, "");
            }
        }

        private void superbox_OperateDone(object sender, MessageEventArgs e)
        {
            bool flag;
            DirectorySecurity directorySecurity = new DirectorySecurity();
            InheritanceFlags none = InheritanceFlags.None;
            none = InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit;
            FileSystemAccessRule rule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, none, PropagationFlags.None, AccessControlType.Allow);
            directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out flag);
            string path = Path.Combine(TestPaperPlayer.answerPath, this.id + e.Message);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path, directorySecurity);
            }
            string str2 = Path.Combine(TestPaperPlayer.answerPath, this.id + e.Message + ".zip");
            CommonMethods.ClearDirectory(path);
            CommonMethods.CopyFolder(TestPaperPlayer.stuDataPath, path, true);
            if (File.Exists(str2))
            {
                bool flag2;
                FileInfo info = new FileInfo(str2);
                if ((info.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    info.Attributes = FileAttributes.Normal;
                }
                FileSecurity accessControl = info.GetAccessControl(AccessControlSections.All);
                InheritanceFlags inheritanceFlags = InheritanceFlags.None;
                FileSystemAccessRule rule2 = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inheritanceFlags, PropagationFlags.None, AccessControlType.Allow);
                accessControl.ModifyAccessRule(AccessControlModification.Add, rule2, out flag2);
                info.SetAccessControl(accessControl);
                try
                {
                    File.Delete(str2);
                }
                catch
                {
                }
            }
            CommonMethods.Zip(path, str2, "CKKC37F423");
            CommonMethods.ClearDirectory(TestPaperPlayer.stuDataPath);
            TestPaperPlayer.inOperate = false;
            this.ShowWindow(this, new EventArgs());
            TestPaperPlayer.answerINI.WriteValue(this.answerid, "Answer", "[" + this.superbox.getContent() + "]");
            TestPaperPlayer.answerINI.WriteValue(this.answerid, "AnswerXML", "[" + this.superbox.getContentXml() + "]");
            string[] strArray = this.superbox.getContent().Split(new char[] { '\x001e' });
            bool flag3 = true;
            foreach (string str4 in strArray)
            {
                if (str4 == "")
                {
                    flag3 = false;
                }
            }
            if (flag3 != this.Done)
            {
                this.Done = flag3;
                if (this.isDone != null)
                {
                    this.isDone(this, new BoolEventArgs(this.Done));
                }
            }
            TestPaper.allowExplorer = false;
        }

        internal bool Done { get; set; }

        internal bool IsCurrent { get; set; }

        public SuperBox question
        {
            get
            {
                return this.superbox;
            }
        }

        public Label QuestionID
        {
            get
            {
                return this.indexLabel;
            }
        }

        public delegate void BoolEventHandler(object sender, BoolEventArgs e);
    }
}

