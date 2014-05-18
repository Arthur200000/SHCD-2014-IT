namespace Qisi.Editor.Documents.Elements
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    internal class CharInfo : Element
    {
        public char chr;

        internal CharInfo(char ch, Font font) : base(font)
        {
            this.chr = ch;
            this.Color = System.Drawing.Color.Black;
            FontFamily fontFamily = font.FontFamily;
            int cellAscent = fontFamily.GetCellAscent(font.Style);
            this.BaseLine = (font.Size * cellAscent) / ((float) fontFamily.GetEmHeight(font.Style));
        }

        internal static SizeF CalCharSize(Graphics g, CharInfo charInfo)
        {
            StringFormat genericTypographic = StringFormat.GenericTypographic;
            genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            SizeF ef = g.MeasureString(charInfo.chr.ToString(), charInfo.Font, 0, genericTypographic);
            genericTypographic.Dispose();
            return ef;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        internal override void Draw(Graphics g)
        {
            if (base.Font != null)
            {
                base.Draw(g);
                StringFormat genericTypographic = StringFormat.GenericTypographic;
                genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                SolidBrush brush = new SolidBrush(this.Color);
                g.DrawString(this.chr.ToString(), base.Font, brush, this.Location, genericTypographic);
                brush.Dispose();
                genericTypographic.Dispose();
            }
        }

        internal override void DrawHighLight(Graphics g)
        {
            if (base.Font != null)
            {
                base.Draw(g);
                StringFormat genericTypographic = StringFormat.GenericTypographic;
                genericTypographic.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                g.DrawString(this.chr.ToString(), base.Font, SystemBrushes.HighlightText, this.Location, genericTypographic);
                genericTypographic.Dispose();
            }
        }

        ~CharInfo()
        {
            this.Dispose(false);
        }

        internal static bool IsPunctuationLegalEOL(char c)
        {
            if ("([{\x00b7‘“〈《「『【〔〖（．［｛￡￥".Contains(c.ToString()))
            {
                return false;
            }
            return true;
        }

        internal static bool IsPunctuationLegalTOL(char c)
        {
            if ("!),.:;?]}\x00a8\x00b7ˇˉ―‖’”…∶、。〃々〉》」』】〕〗！＂＇），．：；？］｀｜｝～￠".Contains(c.ToString()))
            {
                return false;
            }
            return true;
        }

        internal char Char
        {
            get
            {
                return this.chr;
            }
        }

        internal System.Drawing.Color Color { get; set; }
    }
}

