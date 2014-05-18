namespace ExamClientControlsLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Xml;

    public class Section
    {
        private string commentText = null;
        private XmlNode myNode;
        private List<Page> objPageList = null;
        private bool rand = false;

        public Section(XmlNode xmlnode)
        {
            this.objPageList = new List<Page>();
            this.myNode = xmlnode;
            if ((this.myNode.Attributes["randomize"] != null) && (this.myNode.Attributes["randomize"].Value == "true"))
            {
                this.rand = true;
            }
            else
            {
                this.rand = false;
            }
            XmlNodeList list = this.myNode.SelectNodes("page");
            foreach (XmlNode node in list)
            {
                Page item = new Page(node);
                this.objPageList.Add(item);
            }
            if ((list.Count > 0) && (list[0].SelectSingleNode("text") != null))
            {
                this.commentText = list[0].SelectSingleNode("text").InnerText;
            }
            else
            {
                this.commentText = "";
            }
            if (this.rand)
            {
                Random random = new Random(DateTime.Now.Millisecond);
                for (int i = 0; i < this.objPageList.Count; i++)
                {
                    Page page2 = this.objPageList[i];
                    int index = random.Next(this.objPageList.Count);
                    this.objPageList.Remove(page2);
                    this.objPageList.Insert(index, page2);
                }
            }
            if (this.commentText != "")
            {
                Label label = new Label {
                    Visible = true,
                    Margin = new Padding(50, 10, 0, 30),
                    AutoSize = true,
                    MaximumSize = new Size(TestPaperPlayer.mainPanelSize.Width, 0),
                    Font = new Font("宋体", 20f, FontStyle.Regular, GraphicsUnit.Pixel),
                    Text = this.commentText
                };
                this.objPageList[0].PagePanel.Controls.Add(label);
                this.objPageList[0].PagePanel.SetFlowBreak(label, true);
                this.objPageList[0].PagePanel.Controls.SetChildIndex(label, 0);
            }
        }

        internal List<Page> SectionPageList
        {
            get
            {
                return this.objPageList;
            }
        }
    }
}

