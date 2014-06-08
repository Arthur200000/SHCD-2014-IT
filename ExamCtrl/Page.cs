namespace ExamClientControlsLibrary
{
    using Qisi.General;
    using Qisi.General.Controls;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
	/// <summary>
	/// Page.
	/// </summary>
    internal class Page : IDisposable
    {
        private List<Label> answerLabelList = new List<Label>();
        private List<string> answerList = new List<string>();
        private List<Label> indexLabel = new List<Label>();
        private int indexLabelFontSize = 12;
        private XmlNode myNode;
        private bool pageIsQuestion = false;
        private FlowLayoutPanel pagePanel = new FlowLayoutPanel();
        private List<Question> questionList = new List<Question>();
        private bool showFormuleEditor = false;
        private bool showVolumnControl = false;
        private string subject;

        internal event EventHandler HideWindow;

        internal event IndexEventHandler IndexChanged;

        internal event BoolIndexEventHandler IsDown;

        internal event EventHandler ShowWindow;

        internal Page(XmlNode xmlnode)
        {
            this.myNode = xmlnode;
            this.pagePanel.AutoSize = true;
            this.pagePanel.MaximumSize = new Size(TestPaperPlayer.mainPanelSize.Width, 0);
            this.pagePanel.Margin = new Padding(0, 0, 0, 0);
            this.pagePanel.Padding = new Padding(0, 0, 0, 0);
            if ((this.myNode.Attributes["showvc"] != null) && (this.myNode.Attributes["showvc"].Value.ToLower() == "true"))
            {
                this.showVolumnControl = true;
            }
            if ((this.myNode.Attributes["showfe"] != null) && (this.myNode.Attributes["showfe"].Value.ToLower() == "true"))
            {
                this.showFormuleEditor = true;
            }
            if ((this.myNode.SelectNodes("question").Count != 0) || (this.myNode.SelectNodes("questions").Count != 0))
            {
                this.pageIsQuestion = true;
            }
            foreach (XmlNode node in this.myNode.ChildNodes)
            {
                if (node.Name == "richtext")
                {
                    ReadOnlyRichTextBox box = new ReadOnlyRichTextBox();
                    box.ContentsResized += new ContentsResizedEventHandler(this.RTF_ContentsResized);
                    box.Width = (TestPaperPlayer.mainPanelSize.Width - box.Margin.Left) - box.Margin.Right;
                    box.ReadOnly = true;
                    box.WordWrap = true;
                    box.BackColor = Color.White;
                    box.BorderStyle = BorderStyle.None;
                    box.Multiline = true;
                    box.LoadFile(TestPaperPlayer.paperPath + node.Attributes["src"].Value);
                    box.ScrollBars = RichTextBoxScrollBars.None;
                    this.pagePanel.Controls.Add(box);
                    this.pagePanel.SetFlowBreak(box, true);
                }
                else
                {
                    int num;
                    Question question;
                    Label Label;
                    ReadOnlyRichTextBox box2;
                    if (node.Name == "question")
                    {
                        num = this.indexLabelFontSize * 7;
                        int panelwidth = TestPaperPlayer.mainPanelSize.Width - num;
                        question = new Question(node, panelwidth, num);
                        question.IndexChanged += new EventHandler(this.q_IndexChanged);
                        question.HideWindow += new EventHandler(this.q_HideWindow);
                        question.ShowWindow += new EventHandler(this.q_ShowWindow);
                        question.isDone += new Question.BoolEventHandler(this.q_isDone);
                        this.pagePanel.Controls.Add(question.QuestionID);
                        this.pagePanel.Controls.Add(question.question);
                        this.indexLabel.Add(question.QuestionID);
                        this.questionList.Add(question);
                        if (TestPaperPlayer.mode == TestPaperPlayer.RunMode.CDExercise)
                        {
                            Label = new Label {
                                BackColor = Color.Transparent,
                                Text = "",
                                AutoSize = false,
                                Size = new Size(num, num)
                            };
                            this.pagePanel.Controls.Add(Label);
                            Label Label2 = new Label {
                                Font = new Font("宋体", 20f, FontStyle.Underline, GraphicsUnit.Pixel),
                                ForeColor = Color.Blue,
                                Text = "参考答案",
                                Cursor = Cursors.Hand
                            };
                            Label2.Click += new EventHandler(this.Label_Click);
                            this.pagePanel.Controls.Add(Label2);
                            this.pagePanel.SetFlowBreak(Label2, true);
                            this.answerList.Add(TestPaperPlayer.stdINI.ReadValue(node.Attributes["id"].Value, "std", ""));
                            this.answerLabelList.Add(Label2);
                        }
                        else if (TestPaperPlayer.mode == TestPaperPlayer.RunMode.CDAnalysis)
                        {
                            Label = new Label {
                                BackColor = Color.Transparent,
                                Text = "",
                                AutoSize = false,
                                Size = new Size(num, num)
                            };
                            this.pagePanel.Controls.Add(Label);
                            string optionAnswer = TestPaperPlayer.stdINI.ReadValue(node.Attributes["id"].Value, "std", "");
                            if (optionAnswer.EndsWith(".rtf"))
                            {
                                box2 = new ReadOnlyRichTextBox {
                                    Width = panelwidth - 20,
                                    BackColor = Color.White,
                                    ScrollBars = RichTextBoxScrollBars.None
                                };
                                box2.ContentsResized += new ContentsResizedEventHandler(this.RTF_ContentsResized);
                                box2.BorderStyle = BorderStyle.None;
                                box2.LoadFile(Path.Combine(TestPaperPlayer.stdAnswerDir, TestPaperPlayer.stdINI.ReadValue(node.Attributes["id"].Value, "std", "")));
                                this.pagePanel.Controls.Add(box2);
                                this.pagePanel.SetFlowBreak(box2, true);
                            }
                            else
                            {
                                question.LoadSTD(optionAnswer);
                            }
                        }
                    }
                    else if (node.Name == "questions")
                    {
                        XmlNodeList list2;
                        List<XmlNode> list = new List<XmlNode>();
                        if ((node.Attributes["randone"] != null) && (node.Attributes["randone"].Value.ToLower() == "true"))
                        {
                            int num3 = new Random(DateTime.Now.Millisecond).Next(node.ChildNodes.Count);
                            if (node.ChildNodes[num3].Name == "question")
                            {
                                list.Add(node.ChildNodes[num3]);
                            }
                            else
                            {
                                list2 = node.ChildNodes[num3].SelectNodes("question");
                                foreach (XmlNode node2 in list2)
                                {
                                    list.Add(node2);
                                }
                            }
                        }
                        else
                        {
                            list2 = node.SelectNodes("question");
                            foreach (XmlNode node2 in list2)
                            {
                                list.Add(node2);
                            }
                        }
                        num = this.indexLabelFontSize * 6;
                        for (int i = 0; i < list.Count; i++)
                        {
                            question = new Question(list[i], (this.pagePanel.ClientSize.Width - num) - 6, num);
                            question.IndexChanged += new EventHandler(this.q_IndexChanged);
                            question.HideWindow += new EventHandler(this.q_HideWindow);
                            question.ShowWindow += new EventHandler(this.q_ShowWindow);
                            question.isDone += new Question.BoolEventHandler(this.q_isDone);
                            this.pagePanel.Controls.Add(question.QuestionID);
                            this.pagePanel.Controls.Add(question.question);
                            this.pagePanel.SetFlowBreak(question.question, true);
                            this.indexLabel.Add(question.QuestionID);
                            this.questionList.Add(question);
                            if (TestPaperPlayer.mode == TestPaperPlayer.RunMode.CDExercise)
                            {
                                Label = new Label {
                                    BackColor = Color.Transparent,
                                    Text = "",
                                    AutoSize = false,
                                    Size = new Size(num, num)
                                };
                                this.pagePanel.Controls.Add(Label);
                                box2 = new ReadOnlyRichTextBox();
                                box2.LoadFile(Path.Combine(TestPaperPlayer.stdAnswerDir, TestPaperPlayer.stdINI.ReadValue(node.Attributes["id"].Value, "std", "")));
                                this.pagePanel.Controls.Add(box2);
                                this.pagePanel.SetFlowBreak(box2, true);
                            }
                        }
                    }
                }
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
                if (this.pagePanel != null)
                {
                    this.pagePanel.Dispose();
                }
                foreach (Question question in this.questionList)
                {
                    question.QuestionID.Dispose();
                    question.question.Dispose();
                }
            }
            this.pagePanel = null;
            this.indexLabel = null;
            this.questionList = null;
        }

        ~Page()
        {
            this.Dispose(false);
        }

        internal void FreshTree()
        {
            foreach (Question question in this.questionList)
            {
                question.freshtree();
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            if (sender is Label)
            {
                int index = this.answerLabelList.IndexOf(sender as Label);
                if (index >= 0)
                {
                    if (this.answerList[index].EndsWith(".rtf"))
                    {
                        new FormRTF(Path.Combine(TestPaperPlayer.stdAnswerDir, this.answerList[index])).ShowDialog();
                    }
                    else
                    {
                        this.questionList[index].LoadSTD(this.answerList[index]);
                    }
                }
            }
        }

        private void q_HideWindow(object sender, EventArgs e)
        {
            if (this.HideWindow != null)
            {
                this.HideWindow(sender, e);
            }
        }

        private void q_IndexChanged(object sender, EventArgs e)
        {
            if (sender is Question)
            {
                Question item = sender as Question;
                int index = this.questionList.IndexOf(item);
                if ((index >= 0) && (index < this.indexLabel.Count))
                {
                    this.IndexChanged(this, new IndexEventArgs(index));
                }
            }
        }

        private void q_isDone(object sender, BoolEventArgs e)
        {
            if (sender is Question)
            {
                Question item = sender as Question;
                int index = this.questionList.IndexOf(item);
                if ((index >= 0) && (index < this.indexLabel.Count))
                {
                    this.IsDown(this, new BoolIndexEventArgs(index, e.Message));
                }
            }
        }

        private void q_ShowWindow(object sender, EventArgs e)
        {
            if (this.ShowWindow != null)
            {
                this.ShowWindow(sender, e);
            }
        }

        internal void ResetIndexFont()
        {
            for (int i = 0; i < this.indexLabel.Count; i++)
            {
                this.indexLabel[i].Font = new Font("黑体", (float) this.indexLabelFontSize, FontStyle.Bold);
                this.indexLabel[i].Refresh();
            }
        }

        private void RTF_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            if (sender is ReadOnlyRichTextBox)
            {
                ReadOnlyRichTextBox box = sender as ReadOnlyRichTextBox;
                int num = box.Height - box.ClientSize.Height;
                box.Height = Math.Max(box.Height, e.NewRectangle.Height + num);
            }
        }

        internal void SetIndexFont(int index)
        {
            if (index < this.indexLabel.Count)
            {
                this.indexLabel[index].Font = new Font("黑体", (float) (this.indexLabelFontSize * 2), FontStyle.Bold);
                this.indexLabel[index].Refresh();
            }
        }

        internal void setQuestionIndex(ref int index)
        {
            for (int i = 0; i < this.indexLabel.Count; i++)
            {
                this.indexLabel[i].Text = ((int) index++).ToString() + ".";
            }
        }

        internal bool IsQuestion
        {
            get
            {
                return this.pageIsQuestion;
            }
        }

        internal FlowLayoutPanel PagePanel
        {
            get
            {
                return this.pagePanel;
            }
        }

        internal bool ShowFormuleEditor
        {
            get
            {
                return this.showFormuleEditor;
            }
        }

        internal bool ShowVolumnControl
        {
            get
            {
                return this.showVolumnControl;
            }
        }

        internal string SubjectName
        {
            get
            {
                return this.subject;
            }
            set
            {
                this.subject = value;
            }
        }

        internal delegate void BoolIndexEventHandler(object sender, BoolIndexEventArgs e);

        internal delegate void IndexEventHandler(object sender, IndexEventArgs e);
    }
}

