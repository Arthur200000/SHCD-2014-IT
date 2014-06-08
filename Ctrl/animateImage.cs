namespace Qisi.General.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class animateImage : UserControl
    {
        private Bitmap animatedImage;
        private IContainer components;
        private bool currentlyAnimating;
        private string path;
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.Controls.animateImage"/> class, and make it double-buffed.
		/// </summary>
        public animateImage()
        {
            this.DoubleBuffered = true;
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="Qisi.General.Controls.animateImage"/> class, and make it double-buffed.
		/// </summary>
		/// <param name="Path">Path.</param>
        public animateImage(string Path)
        {
            this.path = Path;
            this.DoubleBuffered = true;
            this.animatedImage = new Bitmap(Path);
            base.Size = this.animatedImage.Size;
        }

        public void AnimateImage()
        {
            if (!this.currentlyAnimating)
            {
                ImageAnimator.Animate(this.animatedImage, new EventHandler(this.OnFrameChanged));
                this.currentlyAnimating = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.destory();
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void destory()
        {
            this.Stop();
            if (this.animatedImage != null)
            {
                this.animatedImage.Dispose();
                this.animatedImage = null;
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            base.AutoScaleMode = AutoScaleMode.Font;
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.animatedImage != null)
            {
                if (ImageAnimator.CanAnimate(this.animatedImage))
                {
                    ImageAnimator.UpdateFrames(this.animatedImage);
                }
                e.Graphics.DrawImage(this.animatedImage, new Point(0, 0));
            }
        }

        public void Play()
        {
            if (ImageAnimator.CanAnimate(this.animatedImage))
            {
                ImageAnimator.Animate(this.animatedImage, new EventHandler(this.OnFrameChanged));
            }
        }

        public void Stop()
        {
            ImageAnimator.StopAnimate(this.animatedImage, new EventHandler(this.OnFrameChanged));
        }


        [Description("gif图像的路径"), Localizable(true), Browsable(true), Category("外观"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>The path.</value>
		public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
                this.DoubleBuffered = true;
                this.animatedImage = new Bitmap(this.Path);
                base.Size = this.animatedImage.Size;
            }
        }
    }
}

