namespace Qisi.Editor.Documents
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    internal class Options : IDisposable
    {
        private int count;
        private List<Option> optionList;
        private System.Drawing.Region region;
        private int startIndex;

        public Options(bool multiple, bool randomized)
        {
            this.Multiple = multiple;
            this.Randomized = randomized;
            this.optionList = new List<Option>();
            this.startIndex = -1;
            this.count = 0;
            this.region = new System.Drawing.Region();
            this.Handled = false;
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
                if (this.optionList != null)
                {
                    foreach (Option option in this.optionList)
                    {
                        option.Dispose();
                    }
                }
                if (this.region != null)
                {
                    this.region.Dispose();
                }
            }
            this.region = null;
            if (this.optionList != null)
            {
                for (int i = 0; i < this.optionList.Count; i++)
                {
                    this.optionList[i] = null;
                }
            }
            this.optionList = null;
        }

        internal virtual void Draw(Graphics g)
        {
            for (int i = 0; i < this.OptionList.Count; i++)
            {
                char ch = (char) (0x41 + i);
                this.OptionList[i].Draw(g, ch.ToString());
            }
        }

        ~Options()
        {
            this.Dispose(false);
        }

        internal int Count
        {
            get
            {
                this.count = 0;
                foreach (Option option in this.OptionList)
                {
                    this.count += option.Count;
                }
                return this.count;
            }
        }

        internal bool Handled { get; set; }

        internal bool Multiple { get; set; }

        internal List<Option> OptionList
        {
            get
            {
                return this.optionList;
            }
            set
            {
                this.optionList = value;
            }
        }

        internal bool Randomized { get; set; }

        internal string RandOrder { get; set; }

        internal System.Drawing.Region Region
        {
            get
            {
                this.region = new System.Drawing.Region();
                this.region.MakeEmpty();
                foreach (Option option in this.OptionList)
                {
                    this.region.Union(option.Region);
                }
                return this.region;
            }
        }

        internal int StartIndex
        {
            get
            {
                this.startIndex = 0x7fffffff;
                foreach (Option option in this.OptionList)
                {
                    this.startIndex = Math.Min(option.StartIndex, this.startIndex);
                }
                return this.startIndex;
            }
        }
    }
}

