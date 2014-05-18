namespace Qisi.Editor.Documents
{
    using Qisi.Editor;
    using Qisi.Editor.Documents.Elements;
    using Qisi.Editor.Documents.Table;
    using Qisi.Editor.Expression;
    using Qisi.General;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    internal class Document : IDisposable
    {
        private List<Blank> blanks = new List<Blank>();
        private float docHeight;
        private PointF docLocation;
        private float docWidth;
        private List<Element> elements = new List<Element>();
        private Color highLightColor;
        private const float lineInterval = 8f;
        private List<Line> lines = new List<Line>();
        private const float minWidth = 10f;
        private List<Options> optionss = new List<Options>();
        private Pic_Tab parent;

        internal event EventHandler ContentChanged;

        internal event EventHandler HeightChanged;

        internal event EventHandler OperateClicked;

        internal event EventHandler OperateDone;

        internal Document(Padding margin, Font font, Pic_Tab parentObj, float width, PointF location, Color backcolor)
        {
            this.parent = parentObj;
            this.Margin = margin;
            this.DocWidth = width - this.Margin.Horizontal;
            this.DocHeight = 0f;
            this.docLocation = location;
            this.DefaultFont = font;
            this.highLightColor = Qisi.Editor.NativeMethods.MixColor(Qisi.Editor.NativeMethods.getRevColor(backcolor), SystemColors.Highlight);
            this.AppendLine(this.docLocation.Y + this.Margin.Top, 0);
        }

        internal void AppendLine(float top, int startIndex)
        {
            new Line(top, this.DefaultFont, startIndex, this, this.docWidth, this.docLocation.X + this.Margin.Left);
        }

        internal void Checked(PointF MousePos)
        {
            foreach (Options options in this.optionss)
            {
                if (options.Region.IsVisible(MousePos))
                {
                    Option option = null;
                    foreach (Option option2 in options.OptionList)
                    {
                        if (option2.Region.IsVisible(MousePos))
                        {
                            option = option2;
                        }
                    }
                    if (option != null)
                    {
                        if (options.Multiple)
                        {
                            option.Checked = !option.Checked;
                        }
                        else
                        {
                            foreach (Option option2 in options.OptionList)
                            {
                                if (option2 != option)
                                {
                                    option2.Checked = false;
                                }
                                else
                                {
                                    option2.Checked = !option.Checked;
                                }
                            }
                        }
                    }
                }
            }
            foreach (Element element in this.elements)
            {
                if (element is TableInfo)
                {
                    TableInfo info = element as TableInfo;
                    foreach (Cell cell in info.Items)
                    {
                        cell.Checked(MousePos);
                    }
                }
                else if (element is PictureInfo)
                {
                    PictureInfo info2 = element as PictureInfo;
                    foreach (Document document in info2.Documents)
                    {
                        document.Checked(MousePos);
                    }
                }
            }
        }

        private bool CheckOptionsAndBlank()
        {
            CharInfo info2;
            for (int i = 0; i < this.elements.Count; i++)
            {
                if (this.elements[i] is OperationInfo)
                {
                    OperationInfo info = this.elements[i] as OperationInfo;
                    if ((i > 0) && (!(this.elements[i - 1] is CharInfo) || ((this.elements[i - 1] as CharInfo).Char != '\r')))
                    {
                        info2 = new CharInfo('\r', this.elements[i - 1].Font);
                        this.elements.Insert(i, info2);
                        this.MoveBlankOptionsRight(i, true);
                        return false;
                    }
                    if ((i < (this.elements.Count - 1)) && (!(this.elements[i + 1] is CharInfo) || ((this.elements[i + 1] as CharInfo).Char != '\r')))
                    {
                        info2 = new CharInfo('\r', this.elements[i + 1].Font);
                        this.elements.Insert(i + 1, info2);
                        this.MoveBlankOptionsRight(i + 1, true);
                        return false;
                    }
                }
            }
            foreach (Options options in this.optionss)
            {
                if ((options.StartIndex > 0) && !((this.elements[options.StartIndex - 1] is CharInfo) && ((this.elements[options.StartIndex - 1] as CharInfo).Char == '\r')))
                {
                    info2 = new CharInfo('\r', this.elements[options.StartIndex - 1].Font);
                    this.elements.Insert(options.StartIndex, info2);
                    this.MoveBlankOptionsRight(options.StartIndex, true);
                    return false;
                }
                if ((options.StartIndex + options.Count) < this.elements.Count)
                {
                    if (!((this.elements[options.StartIndex + options.Count] is CharInfo) && ((this.elements[options.StartIndex + options.Count] as CharInfo).Char == '\r')))
                    {
                        info2 = new CharInfo('\r', this.elements[options.StartIndex + options.Count].Font);
                        this.elements.Insert(options.StartIndex + options.Count, info2);
                        this.MoveBlankOptionsRight(options.StartIndex + options.Count, false);
                        return false;
                    }
                }
                else if ((options.StartIndex + options.Count) == this.elements.Count)
                {
                    info2 = new CharInfo('\r', this.DefaultFont);
                    this.elements.Insert(options.StartIndex + options.Count, info2);
                    return false;
                }
            }
            foreach (Blank blank in this.blanks)
            {
                if (blank.StartIndex > 0)
                {
                    if (blank.AllowCR)
                    {
                        if (!((this.elements[blank.StartIndex - 1] is CharInfo) && ((this.elements[blank.StartIndex - 1] as CharInfo).Char == '\r')))
                        {
                            info2 = new CharInfo('\r', this.elements[blank.StartIndex - 1].Font);
                            this.elements.Insert(blank.StartIndex, info2);
                            this.MoveBlankOptionsRight(blank.StartIndex, true);
                            return false;
                        }
                    }
                    else if (!((this.elements[blank.StartIndex - 1] is CharInfo) && ((this.elements[blank.StartIndex - 1] as CharInfo).Char == ' ')))
                    {
                        info2 = new CharInfo(' ', this.elements[blank.StartIndex - 1].Font);
                        this.elements.Insert(blank.StartIndex, info2);
                        this.MoveBlankOptionsRight(blank.StartIndex, true);
                        return false;
                    }
                }
                if ((blank.StartIndex + blank.Count) < this.elements.Count)
                {
                    if (blank.AllowCR)
                    {
                        if (!((this.elements[blank.StartIndex + blank.Count] is CharInfo) && ((this.elements[blank.StartIndex + blank.Count] as CharInfo).Char == '\r')))
                        {
                            info2 = new CharInfo('\r', this.elements[blank.StartIndex + blank.Count].Font);
                            this.elements.Insert(blank.StartIndex + blank.Count, info2);
                            this.MoveBlankOptionsRight(blank.StartIndex + blank.Count, false);
                            return false;
                        }
                    }
                    else if (!((this.elements[blank.StartIndex + blank.Count] is CharInfo) && ((this.elements[blank.StartIndex + blank.Count] as CharInfo).Char == ' ')))
                    {
                        info2 = new CharInfo(' ', this.elements[blank.StartIndex + blank.Count].Font);
                        this.elements.Insert(blank.StartIndex + blank.Count, info2);
                        this.MoveBlankOptionsRight(blank.StartIndex + blank.Count, false);
                        return false;
                    }
                }
                else if ((blank.StartIndex + blank.Count) == this.elements.Count)
                {
                    if (blank.AllowCR)
                    {
                        info2 = new CharInfo('\r', this.DefaultFont);
                        this.elements.Insert(blank.StartIndex + blank.Count, info2);
                        return false;
                    }
                    info2 = new CharInfo(' ', this.DefaultFont);
                    this.elements.Insert(blank.StartIndex + blank.Count, info2);
                    return false;
                }
            }
            return true;
        }

        internal void ClearAll()
        {
            this.elements = new List<Element>();
            this.lines = new List<Line>();
            this.blanks = new List<Blank>();
            this.optionss = new List<Options>();
            this.AppendLine(this.docLocation.Y + this.Margin.Top, 0);
        }

        internal bool DeleteElement(int index, bool readOnly)
        {
            foreach (Options options in this.optionss)
            {
                if ((index >= (options.StartIndex - 1)) && (index <= (options.StartIndex + options.Count)))
                {
                    return false;
                }
            }
            foreach (Blank blank in this.blanks)
            {
                if ((index == (blank.StartIndex - 1)) || (index == (blank.StartIndex + blank.Count)))
                {
                    return false;
                }
            }
            bool flag = false;
            foreach (Blank blank in this.blanks)
            {
                if ((blank.StartIndex <= index) && ((blank.StartIndex + blank.Count) > index))
                {
                    flag = true;
                }
            }
            if (readOnly ^ flag)
            {
                return false;
            }
            foreach (Blank blank in this.Blanks)
            {
                if ((blank.StartIndex <= index) && ((blank.StartIndex + blank.Count) > index))
                {
                    blank.Count--;
                }
                else if (blank.StartIndex > index)
                {
                    blank.StartIndex--;
                }
            }
            foreach (Options options in this.optionss)
            {
                if (options.StartIndex > index)
                {
                    foreach (Option option in options.OptionList)
                    {
                        option.StartIndex--;
                    }
                }
            }
            this.elements.RemoveAt(index);
            if (index < this.elements.Count)
            {
                this.elements[index].Settled = false;
            }
            if (this.ContentChanged != null)
            {
                this.ContentChanged(this, new EventArgs());
            }
            return true;
        }

        internal int DeleteElement(int start, int count, bool readOnly)
        {
            int num = 0;
            int index = start;
            while (count > 0)
            {
                bool flag = false;
                foreach (Options options in this.optionss)
                {
                    if ((index >= (options.StartIndex - 1)) && (index <= (options.StartIndex + options.Count)))
                    {
                        flag = true;
                    }
                }
                foreach (Blank blank in this.blanks)
                {
                    if ((index == (blank.StartIndex - 1)) || (index == (blank.StartIndex + blank.Count)))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    num++;
                    index++;
                    count--;
                }
                else
                {
                    bool flag2 = false;
                    foreach (Blank blank in this.blanks)
                    {
                        if ((blank.StartIndex <= index) && ((blank.StartIndex + blank.Count) > index))
                        {
                            flag2 = true;
                        }
                    }
                    if (readOnly ^ flag2)
                    {
                        num++;
                        index++;
                        count--;
                        continue;
                    }
                    foreach (Blank blank in this.Blanks)
                    {
                        if ((blank.StartIndex <= index) && ((blank.StartIndex + blank.Count) > index))
                        {
                            blank.Count--;
                        }
                        else if (blank.StartIndex > index)
                        {
                            blank.StartIndex--;
                        }
                    }
                    foreach (Options options in this.optionss)
                    {
                        if (options.StartIndex > index)
                        {
                            foreach (Option option in options.OptionList)
                            {
                                option.StartIndex--;
                            }
                        }
                    }
                    this.elements.RemoveAt(index);
                    count--;
                }
            }
            if (start < this.elements.Count)
            {
                this.elements[start].Settled = false;
            }
            if (this.ContentChanged != null)
            {
                this.ContentChanged(this, new EventArgs());
            }
            return num;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            int num;
            if (disposing)
            {
                if (this.lines != null)
                {
                    foreach (Line line in this.lines)
                    {
                        line.Dispose();
                    }
                }
                if (this.elements != null)
                {
                    foreach (Element element in this.elements)
                    {
                        element.Dispose();
                    }
                }
                if (this.blanks != null)
                {
                    foreach (Blank blank in this.blanks)
                    {
                        blank.Dispose();
                    }
                }
                if (this.optionss != null)
                {
                    foreach (Options options in this.optionss)
                    {
                        options.Dispose();
                    }
                }
            }
            if (this.lines != null)
            {
                for (num = 0; num < this.lines.Count; num++)
                {
                    this.lines[num] = null;
                }
            }
            this.lines = null;
            if (this.elements != null)
            {
                for (num = 0; num < this.elements.Count; num++)
                {
                    this.elements[num] = null;
                }
            }
            this.elements = null;
            if (this.blanks != null)
            {
                for (num = 0; num < this.blanks.Count; num++)
                {
                    this.blanks[num] = null;
                }
            }
            this.blanks = null;
            if (this.optionss != null)
            {
                for (num = 0; num < this.optionss.Count; num++)
                {
                    this.optionss[num] = null;
                }
            }
            this.optionss = null;
            this.parent = null;
        }

        internal void Draw(Graphics g)
        {
            this.PrepareToDraw(g);
            g.Clip = new Region(new RectangleF(this.docLocation.X, this.docLocation.Y, this.OutWidth, this.OutHeight));
            foreach (Element element in this.elements)
            {
                element.Draw(g);
            }
            foreach (Blank blank in this.blanks)
            {
                blank.Draw(g);
            }
            foreach (Options options in this.optionss)
            {
                options.Draw(g);
            }
            g.ResetClip();
        }

        internal void DrawHighLight(Graphics g)
        {
            foreach (Element element in this.elements)
            {
                element.DrawHighLight(g);
            }
        }

        internal void DrawHighLight(Graphics g, int start, int count)
        {
            for (int i = start; i < (start + count); i++)
            {
                this.elements[i].DrawHighLight(g);
            }
        }

        internal void FillIn(string[] strs, string filepath, string id)
        {
            if (strs.Length == this.AnswerCount)
            {
                int num2;
                List<int> list = new List<int>();
                List<object> list2 = new List<object>();
                foreach (Blank blank in this.blanks)
                {
                    while (blank.Count > 0)
                    {
                        this.DeleteElement(blank.StartIndex, true);
                    }
                }
                foreach (Options options in this.optionss)
                {
                    foreach (Option option in options.OptionList)
                    {
                        option.Checked = false;
                    }
                }
                foreach (Blank blank in this.blanks)
                {
                    list2.Add(blank);
                    list.Add(blank.StartIndex);
                }
                foreach (Options options in this.optionss)
                {
                    list.Add(options.StartIndex);
                    list2.Add(options);
                }
                foreach (Element element in this.elements)
                {
                    if (element is OperationInfo)
                    {
                        list.Add(element.Index);
                        list2.Add(element as OperationInfo);
                    }
                }
                for (num2 = 0; num2 < list.Count; num2++)
                {
                    for (int i = 0; i < ((list.Count - 1) - num2); i++)
                    {
                        if (list[i] > list[i + 1])
                        {
                            int num = list[i];
                            list[i] = list[i + 1];
                            list[i + 1] = num;
                            object obj2 = list2[i];
                            list2[i] = list2[i + 1];
                            list2[i + 1] = obj2;
                        }
                    }
                }
                num2 = 0;
                while (num2 < list2.Count)
                {
                    if (list2[num2] is Options)
                    {
                        this.setOption(list2[num2] as Options, strs[num2]);
                    }
                    else if (list2[num2] is Blank)
                    {
                        this.setBlank(list2[num2] as Blank, strs[num2]);
                    }
                    else if (list2[num2] is OperationInfo)
                    {
                        this.setOperate(list2[num2] as OperationInfo, strs[num2], filepath, id);
                    }
                    num2++;
                }
                int count = list2.Count;
                foreach (Element element in this.Elements)
                {
                    int answerCount;
                    string[] strArray;
                    if (element is TableInfo)
                    {
                        TableInfo info = element as TableInfo;
                        foreach (Cell cell in info.Items)
                        {
                            answerCount = cell.AnswerCount;
                            strArray = new string[answerCount];
                            num2 = count;
                            while (count < answerCount)
                            {
                                strArray[num2 - count] = strs[count];
                                num2++;
                            }
                            cell.FillIn(strArray, filepath, id);
                            count += answerCount;
                        }
                    }
                    else if (element is PictureInfo)
                    {
                        PictureInfo info2 = element as PictureInfo;
                        foreach (Document document in info2.Documents)
                        {
                            answerCount = document.AnswerCount;
                            strArray = new string[answerCount];
                            for (num2 = count; count < answerCount; num2++)
                            {
                                strArray[num2 - count] = strs[count];
                            }
                            document.FillIn(strArray, filepath, id);
                            count += answerCount;
                        }
                    }
                }
            }
        }

        ~Document()
        {
            this.Dispose(false);
        }

        internal static string FromEscape(string str)
        {
            str = str.Replace("&amp;", "&");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("&quot;", "\"");
            str = str.Replace("&apos;", "'");
            str = str.Replace(@"\s", " ");
            str = str.Replace(@"\\", @"\");
            str = str.Replace(@"\r", "\r");
            str = str.Replace(@"\n", "\r");
            return str;
        }

        internal string getContent()
        {
            int startIndex;
            string str;
            char ch;
            List<int> list = new List<int>();
            List<string> list2 = new List<string>();
            StringBuilder builder = new StringBuilder();
            foreach (Blank blank in this.blanks)
            {
                StringBuilder builder2 = new StringBuilder();
                startIndex = blank.StartIndex;
                while (startIndex < (blank.StartIndex + blank.Count))
                {
                    if (this.Elements[startIndex] is CharInfo)
                    {
                        CharInfo info = this.Elements[startIndex] as CharInfo;
                        if (info.Char == '\r')
                        {
                            builder2.Append('\x001d');
                        }
                        else if (!((info.Char == ' ') && builder2.ToString().EndsWith(" ")))
                        {
                            builder2.Append(info.Char);
                        }
                    }
                    else if (this.Elements[startIndex] is ExpressionInfo)
                    {
                        builder2.Append((this.Elements[startIndex] as ExpressionInfo).ContainerExpression.ToString());
                    }
                    startIndex++;
                }
                list.Add(blank.StartIndex);
                list2.Add(builder2.ToString());
            }
            foreach (Options options in this.optionss)
            {
                str = "";
                startIndex = 0;
                while (startIndex < options.OptionList.Count)
                {
                    if (options.OptionList[startIndex].Checked)
                    {
                        ch = (char) (('A' + options.RandOrder[startIndex]) - 0x30);
                        str = str + ch.ToString();
                    }
                    startIndex++;
                }
                list.Add(options.StartIndex);
                list2.Add(str);
            }
            foreach (Element element in this.elements)
            {
                if (element is OperationInfo)
                {
                    if ((element as OperationInfo).Opened)
                    {
                        str = (element as OperationInfo).OperationID + ".zip";
                    }
                    else
                    {
                        str = "";
                    }
                    list.Add(element.Index);
                    list2.Add(str);
                }
            }
            for (startIndex = 0; startIndex < list.Count; startIndex++)
            {
                for (int i = 0; i < ((list.Count - 1) - startIndex); i++)
                {
                    if (list[i] > list[i + 1])
                    {
                        int num2 = list[i];
                        list[i] = list[i + 1];
                        list[i + 1] = num2;
                        string str2 = list2[i];
                        list2[i] = list2[i + 1];
                        list2[i + 1] = str2;
                    }
                }
            }
            for (startIndex = 0; startIndex < list.Count; startIndex++)
            {
                ch = '\x001e';
                builder.Append(list2[startIndex] + ch.ToString());
            }
            foreach (Element element in this.elements)
            {
                if (element is TableInfo)
                {
                    TableInfo info2 = element as TableInfo;
                    foreach (Cell cell in info2.Items)
                    {
                        str = cell.getContent();
                        if (str != "")
                        {
                            ch = '\x001e';
                            builder.Append(str + ch.ToString());
                        }
                    }
                }
                else if (element is PictureInfo)
                {
                    PictureInfo info3 = element as PictureInfo;
                    foreach (Document document in info3.Documents)
                    {
                        str = document.getContent();
                        if (str != "")
                        {
                            builder.Append(str + '\x001e'.ToString());
                        }
                    }
                }
            }
            string str3 = builder.ToString();
            if (str3.Length > 0)
            {
                str3 = str3.Substring(0, str3.Length - 1);
            }
            return str3;
        }

        internal string getContentXml()
        {
            string str2;
            int num;
            char ch;
            List<int> list = new List<int>();
            List<string> list2 = new List<string>();
            string str = "";
            foreach (Blank blank in this.Blanks)
            {
                str2 = "";
                if (blank.Count > 0)
                {
                    ContentType text;
                    string str5;
                    if (this.elements[blank.StartIndex] is CharInfo)
                    {
                        text = ContentType.Text;
                    }
                    else if (this.elements[blank.StartIndex] is ExpressionInfo)
                    {
                        text = ContentType.Expr;
                    }
                    else if (this.elements[blank.StartIndex] is TableInfo)
                    {
                        text = ContentType.Table;
                    }
                    else
                    {
                        text = ContentType.Image;
                    }
                    Font font = this.elements[blank.StartIndex].Font;
                    Color black = Color.Black;
                    Element element = this.Elements[blank.StartIndex];
                    if (element is ExpressionInfo)
                    {
                        text = ContentType.Expr;
                        str5 = str2;
                        str2 = str5 + "<expr Font=\"" + element.Font.FontFamily.Name + "\" FontSize=\"" + element.Font.Size.ToString() + "\" Style=\"" + element.Font.Style.ToString() + "\">" + (element as ExpressionInfo).ContainerExpression.ToXml() + "</expr>";
                    }
                    else if (element is CharInfo)
                    {
                        text = ContentType.Text;
                        CharInfo info = element as CharInfo;
                        black = info.Color;
                        str5 = str2;
                        str2 = str5 + "<text Font=\"" + info.Font.FontFamily.Name + "\" FontSize=\"" + info.Font.Size.ToString() + "\" Style=\"" + info.Font.Style.ToString() + "\" Color=\"" + info.Color.ToArgb().ToString("x8") + "\">" + ToEscape(info.Char);
                    }
                    num = blank.StartIndex + 1;
                    while (num < (blank.StartIndex + blank.Count))
                    {
                        element = this.Elements[num];
                        if (!((text != ContentType.Text) || (element is CharInfo)))
                        {
                            str2 = str2 + "</text>";
                        }
                        if (element is ExpressionInfo)
                        {
                            str5 = str2;
                            str2 = str5 + "<expr Font=\"" + element.Font.FontFamily.Name + "\" FontSize=\"" + element.Font.Size.ToString() + "\" Style=\"" + element.Font.Style.ToString() + "\">" + (element as ExpressionInfo).ContainerExpression.ToXml() + "</expr>";
                        }
                        else if (element is CharInfo)
                        {
                            CharInfo info2 = element as CharInfo;
                            if (((info2.Font == font) && (info2.Color == black)) && (text == ContentType.Text))
                            {
                                str2 = str2 + ToEscape(info2.Char);
                            }
                            else
                            {
                                if (text == ContentType.Text)
                                {
                                    str2 = str2 + "</text>";
                                }
                                font = info2.Font;
                                black = info2.Color;
                                str5 = str2;
                                str2 = str5 + "<text Font=\"" + info2.Font.FontFamily.Name + "\" FontSize=\"" + info2.Font.Size.ToString() + "\" Style=\"" + info2.Font.Style.ToString() + "\" Color=\"" + info2.Color.ToArgb().ToString("x8") + "\">" + ToEscape(info2.Char);
                            }
                        }
                        if (element is CharInfo)
                        {
                            text = ContentType.Text;
                        }
                        else if (element is ExpressionInfo)
                        {
                            text = ContentType.Expr;
                        }
                        else if (element is TableInfo)
                        {
                            text = ContentType.Table;
                        }
                        else if (element is PictureInfo)
                        {
                            text = ContentType.Image;
                        }
                        num++;
                    }
                    if (text == ContentType.Text)
                    {
                        str2 = str2 + "</text>";
                    }
                }
                list.Add(blank.StartIndex);
                list2.Add(str2);
            }
            foreach (Options options in this.optionss)
            {
                str2 = "rand:" + options.RandOrder + " answer:";
                num = 0;
                while (num < options.OptionList.Count)
                {
                    if (options.OptionList[num].Checked)
                    {
                        ch = (char) (('A' + options.RandOrder[num]) - 0x30);
                        str2 = str2 + ch.ToString();
                    }
                    num++;
                }
                list.Add(options.StartIndex);
                list2.Add(str2);
            }
            foreach (Element element in this.elements)
            {
                if (element is OperationInfo)
                {
                    if ((element as OperationInfo).Opened)
                    {
                        str2 = (element as OperationInfo).OperationID + ".zip";
                    }
                    else
                    {
                        str2 = "";
                    }
                    list.Add(element.Index);
                    list2.Add(str2);
                }
            }
            for (num = 0; num < list.Count; num++)
            {
                for (int i = 0; i < ((list.Count - 1) - num); i++)
                {
                    if (list[i] > list[i + 1])
                    {
                        int num2 = list[i];
                        list[i] = list[i + 1];
                        list[i + 1] = num2;
                        string str3 = list2[i];
                        list2[i] = list2[i + 1];
                        list2[i + 1] = str3;
                    }
                }
            }
            str = "";
            for (num = 0; num < list.Count; num++)
            {
                ch = '\x001e';
                str = str + list2[num] + ch.ToString();
            }
            foreach (Element element in this.Elements)
            {
                if (element is TableInfo)
                {
                    TableInfo info3 = element as TableInfo;
                    foreach (Cell cell in info3.Items)
                    {
                        str2 = cell.getContentXml();
                        if (str2 != "")
                        {
                            ch = '\x001e';
                            str = str + str2 + ch.ToString();
                        }
                    }
                }
                else if (element is PictureInfo)
                {
                    PictureInfo info4 = element as PictureInfo;
                    foreach (Document document in info4.Documents)
                    {
                        str2 = document.getContentXml();
                        if (str2 != "")
                        {
                            str = str + str2 + '\x001e'.ToString();
                        }
                    }
                }
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str;
        }

        internal void InsertBlank(int index, int maxCharsCount, float width, bool allowCR)
        {
            foreach (Options options in this.optionss)
            {
                if ((index >= options.StartIndex) && (index <= (options.StartIndex + options.Count)))
                {
                    return;
                }
            }
            foreach (Blank blank in this.blanks)
            {
                if ((index >= blank.StartIndex) && (index <= (blank.StartIndex + blank.Count)))
                {
                    return;
                }
            }
            int num = -1;
            for (int i = 0; i < this.blanks.Count; i++)
            {
                if (this.blanks[i].StartIndex < index)
                {
                    num = i;
                }
            }
            this.blanks.Insert(num + 1, new Blank(index, 0, maxCharsCount, width, allowCR));
            if (this.ContentChanged != null)
            {
                this.ContentChanged(this, new EventArgs());
            }
        }

        internal bool InsertChar(int index, char chr, Font font, bool readOnly)
        {
            return this.InsertElement(index, new CharInfo(chr, font), readOnly);
        }

        internal bool InsertElement(int index, Element element, bool readOnly)
        {
            bool flag;
            foreach (Options options in this.optionss)
            {
                if ((index >= options.StartIndex) && (index <= (options.StartIndex + options.Count)))
                {
                    return false;
                }
            }
            if (!readOnly)
            {
                flag = false;
                foreach (Blank blank in this.blanks)
                {
                    if ((blank.StartIndex <= index) && ((blank.StartIndex + blank.Count) >= index))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            else
            {
                flag = false;
                foreach (Blank blank in this.blanks)
                {
                    if ((((blank.StartIndex <= index) && ((blank.StartIndex + blank.Count) >= index)) && (blank.Count < blank.MaxCharsCount)) && ((!(element is CharInfo) || ((element as CharInfo).Char != '\r')) || blank.AllowCR))
                    {
                        blank.Count++;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
                goto Label_01BA;
            }
            if (flag)
            {
                return false;
            }
        Label_01BA:
            foreach (Blank blank in this.blanks)
            {
                if (blank.StartIndex > index)
                {
                    blank.StartIndex++;
                }
            }
            foreach (Options options in this.optionss)
            {
                if (options.StartIndex > index)
                {
                    foreach (Option option in options.OptionList)
                    {
                        option.StartIndex++;
                    }
                }
            }
            this.elements.Insert(index, element);
            this.elements[index].Settled = false;
            if (this.ContentChanged != null)
            {
                this.ContentChanged(this, new EventArgs());
            }
            return true;
        }

        internal bool InsertExpression(int index, containerexpression containerExpr, Font font, bool readOnly)
        {
            return this.InsertElement(index, new ExpressionInfo(containerExpr, font), readOnly);
        }

        internal bool InsertOperation(int index, OperationInfo.OperationType operType, Font font, string dirpath, string rtf, string gif, bool readOnly)
        {
            return this.InsertElement(index, new OperationInfo(operType, font, dirpath, rtf, gif), readOnly);
        }

        internal void InsertOptions(int index, Options optionsToInsert, List<Element> elementList)
        {
            int num;
            foreach (Options options in this.optionss)
            {
                if ((index >= options.StartIndex) && (index <= (options.StartIndex + options.Count)))
                {
                    return;
                }
            }
            foreach (Blank blank in this.blanks)
            {
                if ((index >= blank.StartIndex) && (index <= (blank.StartIndex + blank.Count)))
                {
                    return;
                }
            }
            for (num = 0; num < elementList.Count; num++)
            {
                elementList[num].Settled = false;
                this.InsertElement(index + num, elementList[num], false);
            }
            int num2 = index;
            for (num = 0; num < optionsToInsert.OptionList.Count; num++)
            {
                optionsToInsert.OptionList[num].StartIndex = num2;
                num2 += optionsToInsert.OptionList[num].Count;
            }
            int num3 = -1;
            for (num = 0; num < this.optionss.Count; num++)
            {
                if (this.optionss[num].StartIndex < index)
                {
                    num3 = num;
                }
            }
            this.optionss.Insert(num3 + 1, optionsToInsert);
            if (this.ContentChanged != null)
            {
                this.ContentChanged(this, new EventArgs());
            }
        }

        internal bool InsertPic(int index, Image image, Font font, bool readOnly)
        {
            return this.InsertElement(index, new PictureInfo(image, font), readOnly);
        }

        internal bool InsertTable(int index, Point tableSize, Font font, bool readOnly)
        {
            return this.InsertElement(index, new TableInfo(tableSize, this.DocWidth, font, 1f), readOnly);
        }

        private void LayOut(int index)
        {
            if ((this.elements != null) && (index < this.elements.Count))
            {
                Element elementToAdd = this.elements[index];
                foreach (Blank blank in this.blanks)
                {
                    if (index == (blank.StartIndex + blank.Count))
                    {
                        this.RefreshBlank(blank);
                        break;
                    }
                }
                foreach (Options options in this.optionss)
                {
                    if (index == (options.StartIndex + options.Count))
                    {
                        this.RefreshOptions(options);
                        break;
                    }
                }
                Line lastLine = this.LastLine;
                elementToAdd.LineContainer = lastLine;
                elementToAdd.Location = new PointF(lastLine.Right, (lastLine.Top + lastLine.BaseLine) - elementToAdd.BaseLine);
                lastLine.ResetVertical(elementToAdd);
                if (((elementToAdd.OutSize.Width + elementToAdd.OutLocation.X) > ((this.DocLocation.X + this.Margin.Left) + this.DocWidth)) && ((elementToAdd.OutSize.Width < this.docWidth) || (lastLine.ElementCount != 0)))
                {
                    lastLine.ResetVertical();
                    lastLine.Separate();
                    this.AppendLine((lastLine.Top + lastLine.Height) + 8f, index);
                    this.LayOut(index);
                }
                else
                {
                    if (((elementToAdd is CharInfo) && (lastLine.ElementCount == 0)) && (lastLine != this.Lines[0]))
                    {
                        Element element2;
                        int num = 0;
                        Line line2 = this.Lines[this.Lines.IndexOf(lastLine) - 1];
                        for (element2 = elementToAdd; ((num < line2.ElementCount) && (element2 is CharInfo)) && !CharInfo.IsPunctuationLegalTOL((element2 as CharInfo).Char); element2 = this.elements[index - num])
                        {
                            num++;
                        }
                        if (num < line2.ElementCount)
                        {
                            element2 = this.elements[(index - num) - 1];
                            while (((num < line2.ElementCount) && (element2 is CharInfo)) && !CharInfo.IsPunctuationLegalEOL(((CharInfo) element2).Char))
                            {
                                num++;
                                if ((index - num) > 0)
                                {
                                    element2 = this.elements[(index - num) - 1];
                                }
                            }
                            if ((num < line2.ElementCount) && (num > 0))
                            {
                                line2.ElementCount -= num;
                                line2.Separate();
                                List<Element> list = new List<Element>();
                                this.lines.Remove(lastLine);
                                this.DocHeight = ((this.LastLine.Top + this.LastLine.Height) - this.DocLocation.Y) - this.Margin.Top;
                                this.AppendLine((line2.Top + line2.Height) + 8f, line2.StartIndex + line2.ElementCount);
                                for (int i = 0; i < num; i++)
                                {
                                    this.LayOut((line2.StartIndex + line2.ElementCount) + i);
                                }
                                this.LayOut(index);
                                return;
                            }
                        }
                    }
                    elementToAdd.DocumentContainer = this;
                    elementToAdd.LineContainer = lastLine;
                    Line lineContainer = elementToAdd.LineContainer;
                    lineContainer.ElementCount++;
                    elementToAdd.LineContainer.Right = elementToAdd.OutLocation.X + elementToAdd.OutSize.Width;
                    elementToAdd.Settled = true;
                    lastLine.ResetVertical();
                    if ((elementToAdd is CharInfo) && ((elementToAdd as CharInfo).Char == '\r'))
                    {
                        this.AppendLine((lastLine.Top + lastLine.Height) + 8f, index + 1);
                    }
                }
            }
        }

        internal void LoadFromXml(XmlNode xml, string dir, bool newOutLine = true)
        {
            string str2;
            int num5;
            int num6;
            int num11;
            Option option;
            this.elements = new List<Element>();
            this.lines = new List<Line>();
            this.blanks = new List<Blank>();
            this.optionss = new List<Options>();
            FontStyle regular = FontStyle.Regular;
            string familyName = "宋体";
            float emSize = 13f;
            if (!newOutLine)
            {
                string[] strArray;
                string str3;
                if (xml.Attributes["padding"] != null)
                {
                    strArray = xml.Attributes["padding"].Value.Split(new char[] { ',' });
                    if (strArray.Length == 4)
                    {
                        this.Margin = new Padding(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), Convert.ToInt32(strArray[3]));
                    }
                }
                else if (xml.Attributes["Padding"] != null)
                {
                    strArray = xml.Attributes["Padding"].Value.Split(new char[] { ',' });
                    if (strArray.Length == 4)
                    {
                        this.Margin = new Padding(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), Convert.ToInt32(strArray[3]));
                    }
                }
                if (xml.Attributes["style"] != null)
                {
                    str2 = xml.Attributes["style"].Value;
                    if (str2.Contains("Bold"))
                    {
                        regular |= FontStyle.Bold;
                    }
                    if (str2.Contains("Italic"))
                    {
                        regular |= FontStyle.Italic;
                    }
                    if (str2.Contains("Strikeout"))
                    {
                        regular |= FontStyle.Strikeout;
                    }
                    if (str2.Contains("Underline"))
                    {
                        regular |= FontStyle.Underline;
                    }
                }
                else if (xml.Attributes["Style"] != null)
                {
                    str2 = xml.Attributes["Style"].Value;
                    if (str2.Contains("Bold"))
                    {
                        regular |= FontStyle.Bold;
                    }
                    if (str2.Contains("Italic"))
                    {
                        regular |= FontStyle.Italic;
                    }
                    if (str2.Contains("Strikeout"))
                    {
                        regular |= FontStyle.Strikeout;
                    }
                    if (str2.Contains("Underline"))
                    {
                        regular |= FontStyle.Underline;
                    }
                }
                if (xml.Attributes["font"] != null)
                {
                    familyName = xml.Attributes["font"].Value;
                }
                else if (xml.Attributes["Font"] != null)
                {
                    familyName = xml.Attributes["Font"].Value;
                }
                if (xml.Attributes["fontsize"] != null)
                {
                    emSize = Convert.ToSingle(xml.Attributes["fontsize"].Value);
                }
                else if (xml.Attributes["FontSize"] != null)
                {
                    emSize = Convert.ToSingle(xml.Attributes["FontSize"].Value);
                }
                this.DefaultFont = new Font(familyName, emSize, regular, GraphicsUnit.Pixel);
                this.docWidth = 10f;
                if (xml.Attributes["width"] != null)
                {
                    this.docWidth = Convert.ToSingle(xml.Attributes["width"].Value);
                }
                else if (xml.Attributes["Width"] != null)
                {
                    this.docWidth = Convert.ToSingle(xml.Attributes["Width"].Value);
                }
                if (this.parent == null)
                {
                    this.docLocation = new PointF(0f, 0f);
                }
                else
                {
                    this.docLocation = this.parent.OutLocation;
                }
                if (xml.Attributes["location"] != null)
                {
                    strArray = xml.Attributes["location"].Value.Split(new char[] { ',' });
                    if ((strArray.Length == 2) && (this.Parent != null))
                    {
                        this.docLocation = new PointF(Convert.ToSingle(strArray[0]) + this.Parent.OutLocation.X, Convert.ToSingle(strArray[1]) + this.Parent.OutLocation.Y);
                    }
                }
                else if (xml.Attributes["Location"] != null)
                {
                    strArray = xml.Attributes["Location"].Value.Split(new char[] { ',' });
                    if ((strArray.Length == 2) && (this.Parent != null))
                    {
                        this.docLocation = new PointF(Convert.ToSingle(strArray[0]) + this.Parent.OutLocation.X, Convert.ToSingle(strArray[1]) + this.Parent.OutLocation.Y);
                    }
                }
                this.highLightColor = Qisi.Editor.NativeMethods.MixColor(Color.White, SystemColors.Highlight);
                if (xml.Attributes["color"] != null)
                {
                    str3 = xml.Attributes["color"].Value;
                    try
                    {
                        this.highLightColor = Qisi.Editor.NativeMethods.MixColor(Color.FromArgb(Convert.ToInt32(str3, 0x10)), SystemColors.Highlight);
                    }
                    catch
                    {
                        this.highLightColor = Qisi.Editor.NativeMethods.MixColor(Color.White, SystemColors.Highlight);
                    }
                }
                else if (xml.Attributes["Color"] != null)
                {
                    str3 = xml.Attributes["Color"].Value;
                    try
                    {
                        this.highLightColor = Qisi.Editor.NativeMethods.MixColor(Color.FromArgb(Convert.ToInt32(str3, 0x10)), SystemColors.Highlight);
                    }
                    catch
                    {
                        this.highLightColor = Qisi.Editor.NativeMethods.MixColor(Color.White, SystemColors.Highlight);
                    }
                }
            }
            Font defaultFont = this.DefaultFont;
            Padding margin = this.Margin;
            this.AppendLine(this.docLocation.Y + margin.Top, 0);
            foreach (XmlNode node in xml.ChildNodes)
            {
                if (node.Attributes["style"] != null)
                {
                    str2 = node.Attributes["style"].Value;
                    if (str2.Contains("Bold"))
                    {
                        regular |= FontStyle.Bold;
                    }
                    if (str2.Contains("Italic"))
                    {
                        regular |= FontStyle.Italic;
                    }
                    if (str2.Contains("Strikeout"))
                    {
                        regular |= FontStyle.Strikeout;
                    }
                    if (str2.Contains("Underline"))
                    {
                        regular |= FontStyle.Underline;
                    }
                }
                else if (node.Attributes["Style"] != null)
                {
                    str2 = node.Attributes["Style"].Value;
                    if (str2.Contains("Bold"))
                    {
                        regular |= FontStyle.Bold;
                    }
                    if (str2.Contains("Italic"))
                    {
                        regular |= FontStyle.Italic;
                    }
                    if (str2.Contains("Strikeout"))
                    {
                        regular |= FontStyle.Strikeout;
                    }
                    if (str2.Contains("Underline"))
                    {
                        regular |= FontStyle.Underline;
                    }
                }
                if (node.Attributes["font"] != null)
                {
                    familyName = node.Attributes["font"].Value;
                }
                else if (node.Attributes["Font"] != null)
                {
                    familyName = node.Attributes["Font"].Value;
                }
                if (node.Attributes["fontsize"] != null)
                {
                    emSize = Convert.ToSingle(node.Attributes["fontsize"].Value);
                }
                else if (node.Attributes["FontSize"] != null)
                {
                    emSize = Convert.ToSingle(node.Attributes["FontSize"].Value);
                }
                defaultFont = new Font(familyName, emSize, regular, GraphicsUnit.Pixel);
                if (node.Name == "text")
                {
                    string str4 = FromEscape(node.InnerText);
                    foreach (char ch in str4.ToCharArray())
                    {
                        this.elements.Add(new CharInfo(ch, defaultFont));
                    }
                }
                else if (node.Name == "expr")
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        containerexpression expr = new containerexpression(node2, defaultFont);
                        this.elements.Add(new ExpressionInfo(expr, defaultFont));
                    }
                }
                else
                {
                    float width;
                    if (node.Name == "img")
                    {
                        float height;
                        FileStream stream = File.OpenRead(Path.Combine(dir, node.Attributes["src"].Value));
                        int length = 0;
                        length = (int) stream.Length;
                        Image image = Image.FromStream(stream);
                        stream.Close();
                        if (node.Attributes["width"] != null)
                        {
                            width = Convert.ToSingle(node.Attributes["width"].Value);
                        }
                        else if (node.Attributes["Width"] != null)
                        {
                            width = Convert.ToSingle(node.Attributes["Width"].Value);
                        }
                        else
                        {
                            width = image.Width;
                        }
                        if (node.Attributes["height"] != null)
                        {
                            height = Convert.ToSingle(node.Attributes["height"].Value);
                        }
                        else if (node.Attributes["Height"] != null)
                        {
                            height = Convert.ToSingle(node.Attributes["Height"].Value);
                        }
                        else
                        {
                            height = image.Height;
                        }
                        PictureInfo item = new PictureInfo(image, defaultFont, new SizeF(width, height));
                        this.elements.Add(item);
                        num5 = 0;
                        while (num5 < node.ChildNodes.Count)
                        {
                            margin = new Padding();
                            new Document(margin, defaultFont, item, 0f, new PointF(0f, 0f), Color.Transparent).LoadFromXml(node.ChildNodes[num5], dir, true);
                            num5++;
                        }
                    }
                    else if (node.Name == "table")
                    {
                        num6 = 0;
                        int y = 0;
                        if (node.Attributes["x"] != null)
                        {
                            num6 = Convert.ToInt32(node.Attributes["x"].Value);
                        }
                        else if (node.Attributes["X"] != null)
                        {
                            num6 = Convert.ToInt32(node.Attributes["X"].Value);
                        }
                        if (node.Attributes["y"] != null)
                        {
                            y = Convert.ToInt32(node.Attributes["y"].Value);
                        }
                        else if (node.Attributes["Y"] != null)
                        {
                            y = Convert.ToInt32(node.Attributes["Y"].Value);
                        }
                        width = 0f;
                        float lineWidth = 0f;
                        if (node.Attributes["width"] != null)
                        {
                            width = Convert.ToSingle(node.Attributes["width"].Value);
                        }
                        else if (node.Attributes["Width"] != null)
                        {
                            width = Convert.ToSingle(node.Attributes["Width"].Value);
                        }
                        if (node.Attributes["linewidth"] != null)
                        {
                            lineWidth = Convert.ToSingle(node.Attributes["linewidth"].Value);
                        }
                        else if (node.Attributes["LineWidth"] != null)
                        {
                            lineWidth = Convert.ToSingle(node.Attributes["LineWidth"].Value);
                        }
                        if ((((num6 > 0) && (y > 0)) && (width > 0f)) && (lineWidth > 0f))
                        {
                            Point tableSize = new Point(num6, y);
                            TableInfo info2 = new TableInfo(tableSize, width, defaultFont, lineWidth);
                            this.elements.Add(info2);
                            num5 = 0;
                            while (num5 < Math.Min(info2.Items.Count, node.ChildNodes.Count))
                            {
                                info2.Items[num5].LoadFromXml(node.ChildNodes[num5], dir, false);
                                num5++;
                            }
                        }
                    }
                    else if (node.Name == "operate")
                    {
                        if (((node.Attributes["Engine"] != null) && (node.Attributes["Path"] != null)) && (node.Attributes["RTF"] != null))
                        {
                            string str8;
                            string str5 = node.Attributes["Engine"].Value;
                            string str6 = node.Attributes["Path"].Value;
                            string str7 = node.Attributes["RTF"].Value;
                            if (node.Attributes["GIF"] != null)
                            {
                                str8 = Path.Combine(dir, node.Attributes["GIF"].Value);
                            }
                            else
                            {
                                str8 = "";
                            }
                            if (str5.ToLower() == "flash")
                            {
                                this.elements.Add(new OperationInfo(OperationInfo.OperationType.Flash, defaultFont, Path.Combine(dir, str6), Path.Combine(dir, str7), str8));
                            }
                            else if (str5.ToLower() == "vb")
                            {
                                this.elements.Add(new OperationInfo(OperationInfo.OperationType.VisualBasic, defaultFont, Path.Combine(dir, str6), Path.Combine(dir, str7), str8));
                            }
                            else if (str5.ToLower() == "access")
                            {
                                this.elements.Add(new OperationInfo(OperationInfo.OperationType.Access, defaultFont, Path.Combine(dir, str6), Path.Combine(dir, str7), str8));
                            }
                            else if (str5.ToLower() == "photoshop")
                            {
                                this.elements.Add(new OperationInfo(OperationInfo.OperationType.PhotoShop, defaultFont, Path.Combine(dir, str6), Path.Combine(dir, str7), str8));
                            }
                        }
                    }
                    else if (node.Name == "edit")
                    {
                        int num9;
                        int num10;
                        if (node.Attributes["width"] != null)
                        {
                            num9 = Convert.ToInt32(node.Attributes["width"].Value);
                        }
                        else if (node.Attributes["Width"] != null)
                        {
                            num9 = Convert.ToInt32(node.Attributes["Width"].Value);
                        }
                        else
                        {
                            num9 = -1;
                        }
                        if (node.Attributes["minlength"] != null)
                        {
                            num10 = Convert.ToInt32(node.Attributes["minlength"].Value);
                        }
                        else if (node.Attributes["MinLength"] != null)
                        {
                            num10 = Convert.ToInt32(node.Attributes["MinLength"].Value);
                        }
                        else if (num9 <= 0)
                        {
                            num10 = 50;
                        }
                        else
                        {
                            num10 = num9 * 10;
                        }
                        bool cr = false;
                        if (node.Attributes["allowcr"] != null)
                        {
                            cr = Convert.ToBoolean(node.Attributes["allowcr"].Value);
                        }
                        else if (node.Attributes["AllowCR"] != null)
                        {
                            cr = Convert.ToBoolean(node.Attributes["AllowCR"].Value);
                        }
                        if (node.Attributes["count"] != null)
                        {
                            num11 = Convert.ToInt32(node.Attributes["count"].Value);
                        }
                        else if (node.Attributes["Count"] != null)
                        {
                            num11 = Convert.ToInt32(node.Attributes["Count"].Value);
                        }
                        else
                        {
                            num11 = 0;
                        }
                        this.Blanks.Add(new Blank(this.Elements.Count, num11, num9, (float) num10, cr));
                    }
                    else if (node.Name == "select")
                    {
                        bool flag2;
                        bool flag3;
                        if (node.Attributes["multiple"] != null)
                        {
                            flag2 = Convert.ToBoolean(node.Attributes["multiple"].Value);
                        }
                        else if (node.Attributes["Multiple"] != null)
                        {
                            flag2 = Convert.ToBoolean(node.Attributes["Multiple"].Value);
                        }
                        else
                        {
                            flag2 = false;
                        }
                        if (node.Attributes["randomized"] != null)
                        {
                            flag3 = Convert.ToBoolean(node.Attributes["randomized"].Value);
                        }
                        else if (node.Attributes["Randomized"] != null)
                        {
                            flag3 = Convert.ToBoolean(node.Attributes["Randomized"].Value);
                        }
                        else
                        {
                            flag3 = false;
                        }
                        Options options = new Options(flag2, flag3);
                        int count = this.Elements.Count;
                        options.RandOrder = "";
                        num5 = 0;
                        while (num5 < node.ChildNodes.Count)
                        {
                            if (node.ChildNodes[num5].Attributes["count"] != null)
                            {
                                num11 = Convert.ToInt32(node.ChildNodes[num5].Attributes["count"].Value);
                            }
                            else if (node.ChildNodes[num5].Attributes["Count"] != null)
                            {
                                num11 = Convert.ToInt32(node.ChildNodes[num5].Attributes["Count"].Value);
                            }
                            else
                            {
                                num11 = 0;
                            }
                            char ch2 = (char) (0x41 + num5);
                            option = new Option(count, num11, ch2.ToString());
                            bool flag4 = false;
                            if (node.ChildNodes[num5].Attributes["checked"] != null)
                            {
                                flag4 = Convert.ToBoolean(node.ChildNodes[num5].Attributes["checked"].Value);
                            }
                            else if (node.ChildNodes[num5].Attributes["Checked"] != null)
                            {
                                flag4 = Convert.ToBoolean(node.ChildNodes[num5].Attributes["Checked"].Value);
                            }
                            else
                            {
                                flag4 = false;
                            }
                            option.Checked = flag4;
                            options.OptionList.Add(option);
                            count += num11;
                            options.RandOrder = options.RandOrder + num5.ToString();
                            num5++;
                        }
                        this.optionss.Add(options);
                    }
                    else if (node.Name == "judge")
                    {
                        TrueOrFalse @false = new TrueOrFalse(this.Elements.Count);
                        this.optionss.Add(@false);
                    }
                }
            }
            foreach (Options options in this.optionss)
            {
                if (options.Randomized)
                {
                    int startIndex = options.StartIndex;
                    num11 = options.Count;
                    List<Element> list = new List<Element>();
                    List<Option> list2 = new List<Option>();
                    list2.AddRange(options.OptionList);
                    while (num11 > 0)
                    {
                        list.Add(this.elements[startIndex]);
                        this.elements.RemoveAt(startIndex);
                        num11--;
                    }
                    long ticks = DateTime.Now.Ticks;
                    Random random = new Random(((int) (((ulong) ticks) & 0xffffffffL)) | ((int) (ticks >> 0x20)));
                    options.OptionList = new List<Option>();
                    options.RandOrder = "";
                    for (num6 = 0; num6 < list2.Count; num6++)
                    {
                        option = list2[num6];
                        int index = random.Next(options.OptionList.Count + 1);
                        int num16 = startIndex;
                        num5 = 0;
                        while (num5 < index)
                        {
                            num16 += options.OptionList[num5].Count;
                            num5++;
                        }
                        num5 = 0;
                        while (num5 < option.Count)
                        {
                            this.elements.Insert(num5 + num16, list[(option.StartIndex - startIndex) + num5]);
                            num5++;
                        }
                        option.StartIndex = num16;
                        for (num5 = index; num5 < options.OptionList.Count; num5++)
                        {
                            Option local1 = options.OptionList[num5];
                            local1.StartIndex += option.Count;
                        }
                        options.OptionList.Insert(index, option);
                        options.RandOrder = options.RandOrder.Insert(index, num6.ToString());
                    }
                }
            }
        }

        internal void LoadOptionSTD(string answer)
        {
            string[] strArray = answer.Split(new char[] { '\x001d' });
            for (int i = 0; i < Math.Min(this.optionss.Count, strArray.Length); i++)
            {
                Options options = this.optionss[i];
                for (int j = 0; j < options.OptionList.Count; j++)
                {
                    char ch = (char) ((options.RandOrder[j] + 'A') - 0x30);
                    if (strArray[i].Contains(ch.ToString()))
                    {
                        Option option = options.OptionList[j];
                        option.Answer = true;
                        for (int k = option.StartIndex; k < (option.StartIndex + option.Count); k++)
                        {
                            if (this.elements[k] is CharInfo)
                            {
                                (this.elements[k] as CharInfo).Color = Color.Red;
                                this.elements[k].Settled = false;
                            }
                            else if (this.elements[k] is ExpressionInfo)
                            {
                                (this.elements[k] as ExpressionInfo).Color = Color.Red;
                                this.elements[k].Settled = false;
                            }
                        }
                    }
                }
            }
        }

        internal void LoadXmlFromFile(string file)
        {
            string directoryName = Path.GetDirectoryName(file);
            XmlDocument document = new XmlDocument();
            string xml = File.ReadAllText(file, Encoding.UTF8);
            document.LoadXml(xml);
            this.LoadFromXml(document.ChildNodes[0], directoryName, true);
        }

        private void MoveBlankOptionsRight(int index, bool addFront)
        {
            if (addFront)
            {
                foreach (Options options in this.optionss)
                {
                    if (options.StartIndex >= index)
                    {
                        foreach (Option option in options.OptionList)
                        {
                            option.StartIndex++;
                        }
                    }
                }
                foreach (Blank blank in this.blanks)
                {
                    if (blank.StartIndex >= index)
                    {
                        blank.StartIndex++;
                    }
                }
            }
            else
            {
                foreach (Options options in this.optionss)
                {
                    if (options.StartIndex > index)
                    {
                        foreach (Option option in options.OptionList)
                        {
                            option.StartIndex++;
                        }
                    }
                }
                foreach (Blank blank in this.blanks)
                {
                    if (blank.StartIndex > index)
                    {
                        blank.StartIndex++;
                    }
                }
            }
        }

        internal void Operate(PointF MousePos)
        {
            foreach (Element element in this.elements)
            {
                if (((element != null) && (element is OperationInfo)) && element.Region.IsVisible(MousePos))
                {
                    OperationInfo sender = element as OperationInfo;
                    if (sender.ReDoButtonRegion.IsVisible(MousePos) && sender.Opened)
                    {
                        sender.ReDo();
                        if (this.OperateClicked != null)
                        {
                            this.OperateClicked(sender, new EventArgs());
                        }
                    }
                    else if ((sender.DoButtonRegion.IsVisible(MousePos) || sender.ImageRegion.IsVisible(MousePos)) && (this.OperateClicked != null))
                    {
                        this.OperateClicked(sender, new EventArgs());
                    }
                }
            }
            foreach (Element element in this.elements)
            {
                if (element is TableInfo)
                {
                    TableInfo info2 = element as TableInfo;
                    foreach (Cell cell in info2.Items)
                    {
                        cell.Checked(MousePos);
                    }
                }
                else if (element is PictureInfo)
                {
                    PictureInfo info3 = element as PictureInfo;
                    foreach (Document document in info3.Documents)
                    {
                        document.Checked(MousePos);
                    }
                }
            }
        }

        internal void OperateFinished(OperationInfo operate)
        {
            if (this.OperateDone != null)
            {
                this.OperateDone(operate, new EventArgs());
            }
            if (this.ContentChanged != null)
            {
                this.ContentChanged(this, new EventArgs());
            }
        }

        internal void PrepareToDraw(Graphics g)
        {
            int num2;
            while (!this.CheckOptionsAndBlank())
            {
            }
            int count = this.elements.Count;
            for (num2 = 0; num2 < this.elements.Count; num2++)
            {
                Element element = this.elements[num2];
                if (!element.Sized)
                {
                    if (element is CharInfo)
                    {
                        element.Size = CharInfo.CalCharSize(g, element as CharInfo);
                    }
                    else if (element is ExpressionInfo)
                    {
                        (element as ExpressionInfo).ContainerExpression.RefreshRegion(g);
                    }
                    else if (element is OperationInfo)
                    {
                        (element as OperationInfo).LayOut(g);
                    }
                    else if (element is TableInfo)
                    {
                        (element as TableInfo).LayOut(g);
                    }
                    element.OutWidth = element.Size.Width;
                    element.Sized = true;
                    element.Settled = false;
                    count = Math.Min(num2, count);
                }
            }
            for (num2 = 0; num2 < this.elements.Count; num2++)
            {
                if (!this.elements[num2].Settled)
                {
                    count = Math.Min(num2, count);
                    break;
                }
            }
            if (count < this.elements.Count)
            {
                if (count == 0)
                {
                    this.lines = new List<Line>();
                    this.DocHeight = 0f;
                    this.AppendLine(this.docLocation.Y + this.Margin.Top, 0);
                }
                else
                {
                    int index = this.lines.IndexOf(this.elements[count - 1].LineContainer);
                    if ((this.elements[count - 1] is CharInfo) && ((this.elements[count - 1] as CharInfo).Char == '\r'))
                    {
                        this.lines.RemoveRange(index + 2, (this.lines.Count - index) - 2);
                        this.DocHeight = ((this.LastLine.Top + this.LastLine.Height) - this.docLocation.Y) - this.Margin.Top;
                        this.LastLine.Right = this.docLocation.X + this.Margin.Left;
                        this.LastLine.ElementCount = 0;
                        this.LastLine.ResetVertical();
                    }
                    else
                    {
                        this.lines.RemoveRange(index + 1, (this.lines.Count - index) - 1);
                        this.DocHeight = ((this.LastLine.Top + this.LastLine.Height) - this.docLocation.Y) - this.Margin.Top;
                        this.LastLine.Right = this.elements[count - 1].OutLocation.X + this.elements[count - 1].OutSize.Width;
                        this.LastLine.ElementCount = count - this.LastLine.StartIndex;
                        this.LastLine.ResetVertical();
                    }
                }
                foreach (Blank blank in this.blanks)
                {
                    if ((blank.StartIndex + blank.Count) >= count)
                    {
                        blank.Refreshed = false;
                    }
                }
                foreach (Options options in this.optionss)
                {
                    if ((options.StartIndex + options.Count) >= count)
                    {
                        options.Handled = false;
                    }
                }
                for (num2 = count; num2 < this.elements.Count; num2++)
                {
                    this.LayOut(num2);
                }
                foreach (Blank blank in this.blanks)
                {
                    if (!blank.Refreshed)
                    {
                        this.RefreshBlank(blank);
                    }
                }
                foreach (Options options in this.optionss)
                {
                    if (!options.Handled)
                    {
                        this.RefreshOptions(options);
                    }
                }
                if (this.HeightChanged != null)
                {
                    this.HeightChanged(this, new EventArgs());
                }
            }
            else
            {
                this.LastLine.ElementCount = this.elements.Count - this.LastLine.StartIndex;
                this.LastLine.ResetVertical();
                this.DocHeight = ((this.LastLine.Top + this.LastLine.Height) - this.docLocation.Y) - this.Margin.Top;
            }
        }

        private void RefreshBlank(Blank blank)
        {
            float num3;
            Line lastLine = this.LastLine;
            blank.UnderLines = new List<UnderLine>();
            float num = 0f;
            for (int i = 0; i < blank.Count; i++)
            {
                Element element = this.Elements[i + blank.StartIndex];
                if ((element is CharInfo) && ((element as CharInfo).Char == '\r'))
                {
                    num3 = (this.docWidth - element.OutLocation.X) - element.OutWidth;
                    foreach (RectangleF ef in new List<RectangleF> { new RectangleF(element.OutLocation.X + element.OutWidth, element.OutLocation.Y, num3, element.OutSize.Height) })
                    {
                        blank.UnderLines.Add(new UnderLine(element.LineContainer, ef.X, ef.Right));
                        num += ef.Width;
                    }
                }
                else
                {
                    num += element.OutSize.Width;
                    blank.UnderLines.Add(new UnderLine(element.LineContainer, element.OutLocation.X, element.OutLocation.X + element.OutSize.Width));
                }
            }
            float num4 = blank.MinLength - num;
            while (num4 > 0f)
            {
                if (lastLine.Right >= ((this.docLocation.X + this.docWidth) + this.Margin.Left))
                {
                    this.AppendLine((lastLine.Top + lastLine.Height) + 8f, blank.StartIndex + blank.Count);
                    lastLine = this.LastLine;
                }
                num3 = Math.Min(num4, ((this.docLocation.X + this.Margin.Left) + this.docWidth) - lastLine.Right);
                foreach (RectangleF ef2 in new List<RectangleF> { new RectangleF(lastLine.Right, lastLine.Top, num3, lastLine.Height) })
                {
                    blank.UnderLines.Add(new UnderLine(lastLine, ef2.X, ef2.Right));
                    num4 -= ef2.Width;
                    lastLine.Right = Math.Max(lastLine.Right, ef2.Right);
                }
            }
            if (blank.AllowCR)
            {
                blank.UnderLines.Add(new UnderLine(lastLine, lastLine.Right, (this.docLocation.X + this.Margin.Left) + this.docWidth));
                lastLine.Right = (this.docWidth + this.docLocation.X) + this.Margin.Left;
            }
            blank.Refreshed = true;
        }

        private void RefreshOptions(Options options)
        {
            Option current;
            int startIndex;
            float num3;
            Line lastLine;
            float num4;
            int num5;
            List<Option>.Enumerator enumerator;
            if (options.StartIndex == 0)
            {
                this.lines = new List<Line>();
                this.DocHeight = 0f;
            }
            else
            {
                Line lineContainer = this.elements[options.StartIndex - 1].LineContainer;
                int index = this.Lines.IndexOf(lineContainer);
                this.lines.RemoveRange(index + 1, (this.Lines.Count - index) - 1);
                this.DocHeight = ((this.LastLine.Top + this.LastLine.Height) - this.docLocation.Y) - this.Margin.Top;
            }
            using (enumerator = options.OptionList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    current.Width = 0f;
                    current.Region.MakeEmpty();
                    startIndex = current.StartIndex;
                    while (startIndex < (current.StartIndex + current.Count))
                    {
                        current.Width += this.Elements[startIndex].Size.Width;
                        startIndex++;
                    }
                }
            }
            bool flag = true;
            using (enumerator = options.OptionList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    if ((current.Width + (current.Font.Size * 3f)) > (this.docWidth / ((float) options.OptionList.Count)))
                    {
                        flag = false;
                        goto Label_01C7;
                    }
                }
            }
        Label_01C7:
            if (this.LastLine == null)
            {
                num3 = this.docLocation.Y + this.Margin.Top;
            }
            else
            {
                num3 = (this.LastLine.Top + this.LastLine.Height) + 8f;
            }
            if (flag)
            {
                this.AppendLine(num3, options.StartIndex);
                lastLine = this.LastLine;
                for (startIndex = 0; startIndex < options.OptionList.Count; startIndex++)
                {
                    current = options.OptionList[startIndex];
                    num4 = ((this.docLocation.X + this.Margin.Left) + ((startIndex * this.docWidth) / ((float) options.OptionList.Count))) + (current.Font.Size * 1.5f);
                    current.Left = (this.docLocation.X + this.Margin.Left) + ((startIndex * this.docWidth) / ((float) options.OptionList.Count));
                    current.Line = lastLine;
                    num5 = current.StartIndex;
                    while (num5 < (current.StartIndex + current.Count))
                    {
                        this.Elements[num5].Location = new PointF(num4, lastLine.Top);
                        this.Elements[num5].LineContainer = lastLine;
                        this.Elements[num5].OutWidth = this.Elements[num5].Size.Width;
                        lastLine.ElementCount++;
                        num4 += this.Elements[num5].Size.Width;
                        num5++;
                    }
                }
                lastLine.ResetVertical();
                for (startIndex = 0; startIndex < options.OptionList.Count; startIndex++)
                {
                    current = options.OptionList[startIndex];
                    current.Region.Union(new RectangleF(current.Left, current.Line.Top, current.Font.Size * 1.5f, current.Line.Height));
                    num5 = current.StartIndex;
                    while (num5 < (current.StartIndex + current.Count))
                    {
                        current.Region.Union(this.Elements[num5].Region);
                        num5++;
                    }
                }
                goto Label_0AF3;
            }
            bool flag2 = true;
            using (enumerator = options.OptionList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    if ((current.Width + (current.Font.Size * 3f)) > (this.docWidth / 2f))
                    {
                        flag2 = false;
                        goto Label_04F2;
                    }
                }
            }
        Label_04F2:
            if (flag2)
            {
                this.AppendLine(num3, options.StartIndex);
                lastLine = this.LastLine;
                for (startIndex = 0; startIndex < options.OptionList.Count; startIndex++)
                {
                    current = options.OptionList[startIndex];
                    num4 = ((this.docLocation.X + this.Margin.Left) + (((startIndex % 2) * this.docWidth) / 2f)) + (current.Font.Size * 1.5f);
                    current.Left = (this.docLocation.X + this.Margin.Left) + (((startIndex % 2) * this.docWidth) / 2f);
                    current.Line = lastLine;
                    num5 = current.StartIndex;
                    while (num5 < (current.StartIndex + current.Count))
                    {
                        this.Elements[num5].Location = new PointF(num4, lastLine.Top);
                        this.Elements[num5].LineContainer = lastLine;
                        this.Elements[num5].OutWidth = this.Elements[num5].Size.Width;
                        lastLine.ElementCount++;
                        num4 += this.Elements[num5].Size.Width;
                        num5++;
                    }
                    if (((startIndex % 2) == 1) && ((startIndex + 1) < options.OptionList.Count))
                    {
                        lastLine.ResetVertical();
                        this.AppendLine((lastLine.Top + lastLine.Height) + 8f, options.OptionList[startIndex + 1].StartIndex);
                        lastLine = this.LastLine;
                    }
                }
                lastLine.ResetVertical();
                for (startIndex = 0; startIndex < options.OptionList.Count; startIndex++)
                {
                    current = options.OptionList[startIndex];
                    current.Region.Union(new RectangleF(current.Left, current.Line.Top, current.Font.Size * 1.5f, current.Line.Height));
                    num5 = current.StartIndex;
                    while (num5 < (current.StartIndex + current.Count))
                    {
                        current.Region.Union(this.Elements[num5].Region);
                        num5++;
                    }
                }
            }
            else
            {
                this.AppendLine(num3, options.StartIndex);
                lastLine = this.LastLine;
                for (startIndex = 0; startIndex < options.OptionList.Count; startIndex++)
                {
                    current = options.OptionList[startIndex];
                    num4 = (this.DocLocation.X + this.Margin.Left) + (current.Font.Size * 1.5f);
                    current.Left = this.DocLocation.X + this.Margin.Left;
                    current.Line = lastLine;
                    num5 = current.StartIndex;
                    while (num5 < (current.StartIndex + current.Count))
                    {
                        if ((num4 + this.Elements[num5].Size.Width) > ((this.DocWidth + this.DocLocation.X) + this.Margin.Left))
                        {
                            num4 = (this.DocLocation.X + this.Margin.Left) + (current.Font.Size * 1.5f);
                            lastLine.ResetVertical();
                            this.AppendLine((lastLine.Top + lastLine.Height) + 8f, num5);
                            lastLine = this.LastLine;
                        }
                        this.Elements[num5].Location = new PointF(num4, lastLine.Top);
                        this.Elements[num5].LineContainer = lastLine;
                        this.Elements[num5].OutWidth = this.Elements[num5].Size.Width;
                        lastLine.ElementCount++;
                        num4 += this.Elements[num5].Size.Width;
                        num5++;
                    }
                    if ((startIndex + 1) < options.OptionList.Count)
                    {
                        lastLine.ResetVertical();
                        this.AppendLine((lastLine.Top + lastLine.Height) + 8f, options.OptionList[startIndex + 1].StartIndex);
                        lastLine = this.LastLine;
                    }
                }
                lastLine.ResetVertical();
                for (startIndex = 0; startIndex < options.OptionList.Count; startIndex++)
                {
                    current = options.OptionList[startIndex];
                    current.Region.Union(new RectangleF(current.Left, current.Line.Top, current.Font.Size * 1.5f, current.Line.Height));
                    for (num5 = current.StartIndex; num5 < (current.StartIndex + current.Count); num5++)
                    {
                        current.Region.Union(this.Elements[num5].Region);
                    }
                }
            }
        Label_0AF3:
            options.Handled = true;
        }

        public void setBlank(Blank blank, string strr)
        {
            strr = strr.Replace('\x001d', '\r');
            if (strr != "")
            {
                XmlDocument document = new XmlDocument();
                try
                {
                    document.LoadXml("<doc>" + strr + "</doc>");
                }
                catch
                {
                }
                StringFormat genericTypographic = StringFormat.GenericTypographic;
                genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                FontStyle regular = FontStyle.Regular;
                string familyName = "宋体";
                float emSize = 13f;
                Font defaultFont = SystemFonts.DefaultFont;
                foreach (XmlNode node in document.ChildNodes[0].ChildNodes)
                {
                    string str2;
                    if (node.Attributes["style"] != null)
                    {
                        str2 = node.Attributes["style"].Value;
                        if (str2.Contains("Bold"))
                        {
                            regular |= FontStyle.Bold;
                        }
                        if (str2.Contains("Italic"))
                        {
                            regular |= FontStyle.Italic;
                        }
                        if (str2.Contains("Strikeout"))
                        {
                            regular |= FontStyle.Strikeout;
                        }
                        if (str2.Contains("Underline"))
                        {
                            regular |= FontStyle.Underline;
                        }
                    }
                    else if (node.Attributes["Style"] != null)
                    {
                        str2 = node.Attributes["Style"].Value;
                        if (str2.Contains("Bold"))
                        {
                            regular |= FontStyle.Bold;
                        }
                        if (str2.Contains("Italic"))
                        {
                            regular |= FontStyle.Italic;
                        }
                        if (str2.Contains("Strikeout"))
                        {
                            regular |= FontStyle.Strikeout;
                        }
                        if (str2.Contains("Underline"))
                        {
                            regular |= FontStyle.Underline;
                        }
                    }
                    if (node.Attributes["font"] != null)
                    {
                        familyName = node.Attributes["font"].Value;
                    }
                    else if (node.Attributes["Font"] != null)
                    {
                        familyName = node.Attributes["Font"].Value;
                    }
                    if (node.Attributes["fontsize"] != null)
                    {
                        emSize = Convert.ToSingle(node.Attributes["fontsize"].Value);
                    }
                    else if (node.Attributes["FontSize"] != null)
                    {
                        emSize = Convert.ToSingle(node.Attributes["FontSize"].Value);
                    }
                    defaultFont = new Font(familyName, emSize, regular, GraphicsUnit.Pixel);
                    if (node.Name == "text")
                    {
                        string str3 = FromEscape(node.InnerText);
                        foreach (char ch in str3.ToCharArray())
                        {
                            this.InsertChar(blank.StartIndex + blank.Count, ch, defaultFont, true);
                        }
                    }
                    else if (node.Name == "expr")
                    {
                        foreach (XmlNode node2 in node.ChildNodes)
                        {
                            containerexpression containerExpr = new containerexpression(node2, defaultFont);
                            this.InsertExpression(blank.StartIndex + blank.Count, containerExpr, defaultFont, true);
                        }
                    }
                }
            }
        }

        internal void setOperate(OperationInfo operate, string str, string filepath, string id)
        {
            operate.Review = true;
            string path = Path.Combine(filepath, id + operate.OperationID + ".zip");
            if (File.Exists(path))
            {
                List<string> list;
                List<byte[]> list2;
                Qisi.General.CommonMethods.Unzip(File.ReadAllBytes(path), out list2, out list, "CKKC37F423");
                operate.LoadAnswer(list2, list);
            }
        }

        internal void setOption(Options options, string str)
        {
            int index;
            int num3;
            string str4;
            char ch;
            string str2 = "";
            if (str.Contains("rand:") && str.Contains(" "))
            {
                index = str.IndexOf("rand:");
                int num2 = str.IndexOf(" ");
                str2 = str.Substring(index + 5, (num2 - index) - 5);
                string randOrder = options.RandOrder;
                options.RandOrder = "";
                for (num3 = 0; num3 < Math.Min(str2.Length, randOrder.Length); num3++)
                {
                    options.RandOrder = options.RandOrder + randOrder.IndexOf(str2[num3]).ToString();
                }
            }
            else
            {
                options.RandOrder = "";
                num3 = 0;
                while (num3 < options.OptionList.Count)
                {
                    options.RandOrder = options.RandOrder + num3.ToString();
                    num3++;
                }
            }
            int startIndex = options.StartIndex;
            int num5 = options.StartIndex;
            int count = options.Count;
            List<Element> list = new List<Element>();
            List<Option> list2 = new List<Option>();
            list2.AddRange(options.OptionList);
            while (count > 0)
            {
                list.Add(this.elements[startIndex]);
                this.elements.RemoveAt(startIndex);
                count--;
            }
            options.OptionList = new List<Option>();
            for (int i = 0; i < options.RandOrder.Length; i++)
            {
                ch = options.RandOrder[i];
                int num8 = Convert.ToInt32(ch.ToString());
                Option item = list2[num8];
                for (num3 = 0; num3 < item.Count; num3++)
                {
                    list[(item.StartIndex - num5) + num3].Settled = false;
                    this.elements.Insert(num3 + startIndex, list[(item.StartIndex - num5) + num3]);
                }
                item.StartIndex = startIndex;
                options.OptionList.Add(item);
                startIndex += item.Count;
            }
            options.RandOrder = str2;
            if (str.Contains("answer:"))
            {
                index = str.IndexOf("answer:");
                str4 = str.Substring(index + 7);
            }
            else
            {
                str4 = "";
            }
            for (num3 = 0; num3 < options.OptionList.Count; num3++)
            {
                ch = (char) ((options.RandOrder[num3] + 'A') - 0x30);
                if (str4.Contains(ch.ToString()))
                {
                    options.OptionList[num3].Checked = true;
                }
                else
                {
                    options.OptionList[num3].Checked = false;
                }
            }
            options.Handled = false;
        }

        private static string ToEscape(char chr)
        {
            string str = "";
            if (chr == '&')
            {
                return (str + "&amp;");
            }
            if (chr == '<')
            {
                return (str + "&lt;");
            }
            if (chr == '>')
            {
                return (str + "&gt;");
            }
            if (chr == '"')
            {
                return (str + "&quot;");
            }
            if (chr == '\'')
            {
                return (str + "&apos;");
            }
            if (chr == ' ')
            {
                return (str + @"\s");
            }
            if (chr == '\\')
            {
                return (str + @"\\");
            }
            if (chr == '\r')
            {
                return (str + @"\n");
            }
            if (chr == '\n')
            {
                return (str + @"\n");
            }
            return (str + chr.ToString());
        }

        internal string toXml(string PathToSave)
        {
            float num;
            float num2;
            int num3;
            object obj2;
            string str = "";
            if (this.Parent == null)
            {
                num = 0f;
                num2 = 0f;
            }
            else
            {
                num = this.docLocation.X - this.Parent.OutLocation.X;
                num2 = this.docLocation.Y - this.Parent.OutLocation.Y;
            }
            string str5 = "<doc Padding=\"" + this.Margin.Left.ToString() + "," + this.Margin.Top.ToString() + "," + this.Margin.Right.ToString() + "," + this.Margin.Bottom.ToString() + "\"";
            str5 = str5 + " Font=\"" + this.DefaultFont.FontFamily.Name + "\" FontSize=\"" + this.DefaultFont.Size.ToString() + "\" Style=\"" + this.DefaultFont.Style.ToString() + "\"";
            str = (str5 + " Width=\"" + this.DocWidth.ToString() + "\" Location=\"" + num.ToString() + "," + num2.ToString() + "\"") + ">";
            if (this.Elements.Count > 0)
            {
                ContentType text;
                TableInfo info;
                PictureInfo info2;
                string str2;
                string str3;
                if (this.elements[0] is CharInfo)
                {
                    text = ContentType.Text;
                }
                else if (this.elements[0] is ExpressionInfo)
                {
                    text = ContentType.Expr;
                }
                else if (this.elements[0] is TableInfo)
                {
                    text = ContentType.Table;
                }
                else
                {
                    text = ContentType.Image;
                }
                Font font = this.elements[0].Font;
                Color black = Color.Black;
                foreach (Blank blank in this.blanks)
                {
                    if (blank.StartIndex == 0)
                    {
                        str5 = str;
                        str = str5 + "<edit Width=\"" + blank.MaxCharsCount.ToString() + "\" MinLength=\"" + blank.MinLength.ToString() + "\" Count=\"" + blank.Count.ToString() + "\"/>";
                        break;
                    }
                }
                foreach (Options options in this.optionss)
                {
                    if (options.StartIndex == 0)
                    {
                        str5 = str;
                        str = str5 + "<select Multiple=\"" + options.Multiple.ToString() + "\" Randomized=\"" + options.Randomized.ToString() + "\">";
                        for (num3 = 0; num3 < options.OptionList.Count; num3++)
                        {
                            obj2 = str;
                            str = string.Concat(new object[] { obj2, "<option Count=\"", options.OptionList[num3].Count, "\" Checked=\"", options.OptionList[num3].Checked, "\"/>" });
                        }
                        str = str + "</select>";
                        break;
                    }
                }
                Element element = this.Elements[0];
                if (element is ExpressionInfo)
                {
                    text = ContentType.Expr;
                    str5 = str;
                    str = str5 + "<expr Font=\"" + element.Font.FontFamily.Name + "\" FontSize=\"" + element.Font.Size.ToString() + "\" Style=\"" + element.Font.Style.ToString() + "\">" + (element as ExpressionInfo).ContainerExpression.ToXml() + "</expr>";
                }
                else if (element is TableInfo)
                {
                    text = ContentType.Table;
                    info = element as TableInfo;
                    str5 = str;
                    str = str5 + "<table X=\"" + info.TableSize.X.ToString() + "\" Y=\"" + info.TableSize.Y.ToString() + "\" Width=\"" + info.Width.ToString() + "\" LineWidth=\"" + info.LineWidth.ToString() + "\" Font=\"" + info.Font.FontFamily.Name + "\" FontSize=\"" + info.Font.Size.ToString() + "\" Style=\"" + info.Font.Style.ToString() + "\">";
                    foreach (Cell cell in info.Items)
                    {
                        str = str + cell.toXml(PathToSave);
                    }
                    str = str + "</table>";
                }
                else if (element is PictureInfo)
                {
                    text = ContentType.Image;
                    info2 = element as PictureInfo;
                    str2 = "tmp";
                    info2.Image.Save(Path.Combine(PathToSave, str2), info2.Image.RawFormat);
                    str3 = Qisi.Editor.NativeMethods.GetMD5HashFromFile(Path.Combine(PathToSave, str2));
                    File.Move(Path.Combine(PathToSave, str2), Path.Combine(PathToSave, str3));
                    str5 = str;
                    str = str5 + "<img src=\"" + str3 + "\" Width=\"" + info2.ImageShowSize.Width.ToString() + "\" Height=\"" + info2.ImageShowSize.Height.ToString() + "\" Font=\"" + info2.Font.FontFamily.Name + "\" FontSize=\"" + info2.Font.Size.ToString() + "\" Style=\"" + info2.Font.Style.ToString() + "\">";
                    foreach (Document document in info2.Documents)
                    {
                        str = str + document.toXml(PathToSave);
                    }
                    str = str + "</img>";
                }
                else
                {
                    text = ContentType.Text;
                    CharInfo info3 = element as CharInfo;
                    black = info3.Color;
                    str5 = str;
                    str = str5 + "<text Font=\"" + info3.Font.FontFamily.Name + "\" FontSize=\"" + info3.Font.Size.ToString() + "\" Style=\"" + info3.Font.Style.ToString() + "\" Color=\"" + info3.Color.ToArgb().ToString("x8") + "\">" + ToEscape(info3.Char);
                }
                num3 = 1;
                while (num3 < this.Elements.Count)
                {
                    bool flag = false;
                    foreach (Blank blank in this.blanks)
                    {
                        if (blank.StartIndex == num3)
                        {
                            if (text == ContentType.Text)
                            {
                                str = str + "</text>";
                            }
                            str5 = str;
                            str = str5 + "<edit Width=\"" + blank.MaxCharsCount.ToString() + "\" MinLength=\"" + blank.MinLength.ToString() + "\" Count=\"" + blank.Count.ToString() + "\"/>";
                            flag = true;
                        }
                    }
                    foreach (Options options in this.optionss)
                    {
                        if (options.StartIndex == num3)
                        {
                            if (text == ContentType.Text)
                            {
                                str = str + "</text>";
                            }
                            str5 = str;
                            str = str5 + "<select Multiple=\"" + options.Multiple.ToString() + "\" Randomized=\"" + options.Randomized.ToString() + "\">";
                            for (int i = 0; i < options.OptionList.Count; i++)
                            {
                                obj2 = str;
                                str = string.Concat(new object[] { obj2, "<option Count=\"", options.OptionList[i].Count, "\" Checked=\"", options.OptionList[i].Checked, "\"/>" });
                            }
                            str = str + "</select>";
                            flag = true;
                        }
                    }
                    element = this.Elements[num3];
                    if (!(((text != ContentType.Text) || (element is CharInfo)) || flag))
                    {
                        str = str + "</text>";
                    }
                    if (element is ExpressionInfo)
                    {
                        str5 = str;
                        str = str5 + "<expr Font=\"" + element.Font.FontFamily.Name + "\" FontSize=\"" + element.Font.Size.ToString() + "\" Style=\"" + element.Font.Style.ToString() + "\">" + (element as ExpressionInfo).ContainerExpression.ToXml() + "</expr>";
                    }
                    else if (element is TableInfo)
                    {
                        info = element as TableInfo;
                        str5 = str;
                        str = str5 + "<table X=\"" + info.TableSize.X.ToString() + "\" Y=\"" + info.TableSize.Y.ToString() + "\" Width=\"" + info.Width.ToString() + "\" LineWidth=\"" + info.LineWidth.ToString() + "\" Font=\"" + info.Font.FontFamily.Name + "\" FontSize=\"" + info.Font.Size.ToString() + "\" Style=\"" + info.Font.Style.ToString() + "\">";
                        foreach (Cell cell in info.Items)
                        {
                            str = str + cell.toXml(PathToSave);
                        }
                        str = str + "</table>";
                    }
                    else if (element is PictureInfo)
                    {
                        info2 = element as PictureInfo;
                        str2 = "tmp";
                        info2.Image.Save(Path.Combine(PathToSave, str2), info2.Image.RawFormat);
                        str3 = Qisi.Editor.NativeMethods.GetMD5HashFromFile(Path.Combine(PathToSave, str2));
                        File.Move(Path.Combine(PathToSave, str2), Path.Combine(PathToSave, str3));
                        str5 = str;
                        str = str5 + "<img src=\"" + str3 + "\" Width=\"" + info2.ImageShowSize.Width.ToString() + "\" Height=\"" + info2.ImageShowSize.Height.ToString() + "\" Font=\"" + info2.Font.FontFamily.Name + "\" FontSize=\"" + info2.Font.Size.ToString() + "\" Style=\"" + info2.Font.Style.ToString() + "\">";
                        foreach (Document document in info2.Documents)
                        {
                            str = str + document.toXml(PathToSave);
                        }
                        str = str + "</img>";
                    }
                    else
                    {
                        CharInfo info4 = element as CharInfo;
                        if (!flag)
                        {
                            if (((info4.Font == font) && (info4.Color == black)) && (text == ContentType.Text))
                            {
                                str = str + ToEscape(info4.Char);
                            }
                            else
                            {
                                if (text == ContentType.Text)
                                {
                                    str = str + "</text>";
                                }
                                font = info4.Font;
                                black = info4.Color;
                                str5 = str;
                                str = str5 + "<text Font=\"" + info4.Font.FontFamily.Name + "\" FontSize=\"" + info4.Font.Size.ToString() + "\" Style=\"" + info4.Font.Style.ToString() + "\" Color=\"" + info4.Color.ToArgb().ToString("x8") + "\">" + ToEscape(info4.Char);
                            }
                        }
                        else
                        {
                            font = info4.Font;
                            black = info4.Color;
                            str5 = str;
                            str = str5 + "<text Font=\"" + info4.Font.FontFamily.Name + "\" FontSize=\"" + info4.Font.Size.ToString() + "\" Style=\"" + info4.Font.Style.ToString() + "\" Color=\"" + info4.Color.ToArgb().ToString("x8") + "\">" + ToEscape(info4.Char);
                        }
                    }
                    if (element is CharInfo)
                    {
                        text = ContentType.Text;
                    }
                    else if (element is ExpressionInfo)
                    {
                        text = ContentType.Expr;
                    }
                    else if (element is TableInfo)
                    {
                        text = ContentType.Table;
                    }
                    else if (element is PictureInfo)
                    {
                        text = ContentType.Image;
                    }
                    num3++;
                }
                if (text == ContentType.Text)
                {
                    str = str + "</text>";
                }
            }
            foreach (Blank blank in this.blanks)
            {
                if (blank.StartIndex == this.elements.Count)
                {
                    str5 = str;
                    str = str5 + "<edit Width=\"" + blank.MaxCharsCount.ToString() + "\" MinLength=\"" + blank.MinLength.ToString() + "\" Count=\"" + blank.Count.ToString() + "\"/>";
                    break;
                }
            }
            foreach (Options options in this.optionss)
            {
                if (options.StartIndex == this.elements.Count)
                {
                    str5 = str;
                    str = str5 + "<select Multiple=\"" + options.Multiple.ToString() + "\" Randomized=\"" + options.Randomized.ToString() + "\"/>";
                    for (num3 = 0; num3 < options.OptionList.Count; num3++)
                    {
                        obj2 = str;
                        str = string.Concat(new object[] { obj2, "<option Count=\"", options.OptionList[num3].Count, "\" Checked=\"", options.OptionList[num3].Checked, "\"/>" });
                    }
                    str = str + "</select>";
                    break;
                }
            }
            return (str + "</doc>");
        }

        internal int AnswerCount
        {
            get
            {
                int num = 0;
                foreach (Blank blank in this.Blanks)
                {
                    num++;
                }
                foreach (Options options in this.optionss)
                {
                    num++;
                }
                foreach (Element element in this.Elements)
                {
                    if (element is TableInfo)
                    {
                        TableInfo info = element as TableInfo;
                        foreach (Cell cell in info.Items)
                        {
                            num += cell.AnswerCount;
                        }
                    }
                    else if (element is PictureInfo)
                    {
                        PictureInfo info2 = element as PictureInfo;
                        foreach (Document document in info2.Documents)
                        {
                            num += document.AnswerCount;
                        }
                    }
                    else if (element is OperationInfo)
                    {
                        num++;
                    }
                    else if (element is DrawInfo)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        internal List<Blank> Blanks
        {
            get
            {
                return this.blanks;
            }
            set
            {
                this.blanks = value;
            }
        }

        public Font DefaultFont { get; set; }

        internal virtual float DocHeight
        {
            get
            {
                return this.docHeight;
            }
            set
            {
                if (this.docHeight != value)
                {
                    this.docHeight = value;
                }
            }
        }

        public PointF DocLocation
        {
            get
            {
                return this.docLocation;
            }
            set
            {
                if (this.docLocation != value)
                {
                    foreach (Element element in this.elements)
                    {
                        element.Location = new PointF((element.Location.X - this.docLocation.X) + value.X, (element.Location.Y - this.docLocation.Y) + value.Y);
                    }
                    foreach (Line line in this.lines)
                    {
                        line.Top = (line.Top - this.docLocation.Y) + value.Y;
                        line.Left = (line.Left - this.docLocation.X) + value.X;
                        line.Right = (line.Right - this.docLocation.X) + value.X;
                    }
                    this.docLocation = value;
                }
            }
        }

        internal virtual float DocWidth
        {
            get
            {
                return this.docWidth;
            }
            set
            {
                if (this.docWidth != Math.Max(10f, value))
                {
                    this.docWidth = Math.Max(10f, value);
                    if (this.elements.Count > 0)
                    {
                        this.elements[0].Settled = false;
                    }
                }
            }
        }

        internal List<Element> Elements
        {
            get
            {
                return this.elements;
            }
            set
            {
                this.elements = value;
            }
        }

        public Color HighLightColor
        {
            get
            {
                return this.highLightColor;
            }
        }

        internal Element LastElement
        {
            get
            {
                if ((this.elements != null) && (this.elements.Count > 0))
                {
                    return this.elements[this.elements.Count - 1];
                }
                return null;
            }
        }

        internal Line LastLine
        {
            get
            {
                if ((this.lines == null) || (this.lines.Count == 0))
                {
                    return null;
                }
                return this.lines[this.lines.Count - 1];
            }
        }

        internal float LineInterval
        {
            get
            {
                return 8f;
            }
        }

        internal List<Line> Lines
        {
            get
            {
                return this.lines;
            }
            set
            {
                this.lines = value;
            }
        }

        internal Padding Margin { get; set; }

        internal List<Options> Optionss
        {
            get
            {
                return this.optionss;
            }
            set
            {
                this.optionss = value;
            }
        }

        internal virtual float OutHeight
        {
            get
            {
                return (this.docHeight + this.Margin.Vertical);
            }
        }

        internal float OutWidth
        {
            get
            {
                return (this.DocWidth + this.Margin.Horizontal);
            }
        }

        internal Pic_Tab Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }
    }
}

