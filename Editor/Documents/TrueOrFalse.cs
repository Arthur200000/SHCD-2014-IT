namespace Qisi.Editor.Documents
{
    using System;
    using System.Drawing;

    internal class TrueOrFalse : Options
    {
        internal TrueOrFalse(int index) : base(false, false)
        {
            base.OptionList.Add(new Option(index, 1, "T"));
            base.OptionList.Add(new Option(index + 1, 1, "F"));
        }

        internal override void Draw(Graphics g)
        {
            base.OptionList[0].Draw(g, "T");
            base.OptionList[1].Draw(g, "F");
        }
    }
}

