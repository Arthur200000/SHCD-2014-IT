namespace Qisi.General.Controls
{
    using Qisi.General.Controls.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

	/// <summary>
	/// Start form.
	/// </summary>
    public class StartForm : Form
    {
        private IContainer components;
        private int decrease = 1500;
        private double deltdecrease;
        private double deltincrease;
        private int increase = 1500;
        private int maintence = 2000;
        private Bitmap mybit;
        private int state;
        private Timer timer1;

        public StartForm()
        {
            this.InitializeComponent();
            base.Size = Resources.logo.Size;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.timer1 = new Timer(this.components);
            base.SuspendLayout();
            this.timer1.Tick += new EventHandler(this.Timer1_Tick);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackgroundImage = Resources.logo;
            base.ClientSize = new Size (284, 262);
            this.DoubleBuffered = true;
            base.FormBorderStyle = FormBorderStyle.None;
            base.Name = "StartForm";
            base.Opacity = 0.0;
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            base.TransparencyKey = SystemColors.Control;
            base.Load += new EventHandler(this.StartForm_Load);
            base.ResumeLayout(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.mybit, new Point(0, 0));
        }

        private void StartForm_Load(object sender, EventArgs e)
        {
            this.mybit = Resources.logo;
            this.mybit.MakeTransparent(Color.Yellow);
            base.TopMost = true;
            this.timer1.Interval = 40;
            this.deltdecrease = ((double) this.timer1.Interval) / ((double) this.decrease);
            this.deltincrease = ((double) this.timer1.Interval) / ((double) this.increase);
            this.timer1.Enabled = true;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (this.state == 0)
            {
                base.Opacity += this.deltincrease;
                if (base.Opacity >= 0.95)
                {
                    this.state = 1;
                }
            }
            else if (this.state == 1)
            {
                this.maintence -= this.timer1.Interval;
                if (this.maintence <= 0)
                {
                    this.state = 2;
                }
            }
            else
            {
                base.Opacity -= this.deltdecrease;
                if (base.Opacity <= 0.05)
                {
                    base.Close();
                }
            }
        }

		/// <summary>
		/// Gets or sets the keep opacity milliseconds.
		/// </summary>
		/// <value>The keep opacity milliseconds.</value>
        public int KeepOpacityMilliseconds
        {
            get
            {
                return this.maintence;
            }
            set
            {
                this.maintence = value;
            }
        }

        public int OpacityDecreaseMilliseconds
        {
            get
            {
                return this.decrease;
            }
            set
            {
                this.decrease = value;
            }
        }

        public int OpacityIncreaseMilliseconds
        {
            get
            {
                return this.increase;
            }
            set
            {
                this.increase = value;
            }
        }
    }
}

