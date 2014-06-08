namespace ExamClientControlsLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
	/// <summary>
	/// Part.
	/// </summary>
    internal class Part
    {
        private XmlNode myNode;
        private List<Section> objSectionList = new List<Section>();
        private List<Page> pageList = new List<Page>();
        public string partname;
        private string partSubject;

        internal Part(XmlNode xmlnode)
        {
            if (xmlnode.Attributes["name"] != null)
            {
                this.partname = xmlnode.Attributes["name"].Value;
            }
            else
            {
                this.partname = "";
            }
            if (xmlnode.Attributes["subject"] != null)
            {
                this.partSubject = xmlnode.Attributes["subject"].Value;
            }
            else
            {
                this.partSubject = "";
            }
            this.myNode = xmlnode;
            if (((this.partSubject == TestPaperPlayer.subject) || (this.partSubject == "")) || (TestPaperPlayer.subject == ""))
            {
                XmlNodeList list = this.myNode.SelectNodes("section");
                foreach (XmlNode node in list)
                {
                    Section item = new Section(node);
                    this.objSectionList.Add(item);
                }
            }
            foreach (Section section2 in this.objSectionList)
            {
                this.pageList.AddRange(section2.SectionPageList);
            }
            foreach (Page page in this.pageList)
            {
                page.SubjectName = this.partname;
            }
        }

        internal List<Page> PartPageList
        {
            get
            {
                return this.pageList;
            }
        }
    }
}

