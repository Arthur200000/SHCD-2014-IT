namespace Qisi.General.Controls
{
    using Qisi.General.Controls.Properties;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Timers;
    using System.Windows.Forms;
	/// <summary>
	/// Animated Clock.
	/// </summary>
    public class Clock : Control
    {
        private Bitmap clock = Resources.trad;
        private Image clockpad = Resources.trad;
        private Image dot = Resources.trad_dot;
        private Image hour = Resources.trad_h;
        private Image minute = Resources.trad_m;
        private System.Timers.Timer mytimer;
        private Image second = Resources.trad_s;

        public Clock()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;
            this.drawClock();
            this.mytimer = new System.Timers.Timer();
            this.mytimer.Interval = 500.0;
            this.mytimer.Elapsed += new ElapsedEventHandler(this.mytimer_Elapsed);
            this.mytimer.AutoReset = true;
            this.mytimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.mytimer != null))
            {
                this.mytimer.Stop();
                this.mytimer.Dispose();
            }
            base.Dispose(disposing);
        }

        private void drawClock()
        {
            try
            {
                this.clock = Resources.trad;
                Graphics graphics = Graphics.FromImage(this.clock);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TranslateTransform(((float) (this.clock.Width - 1)) / 2f, ((float) (this.clock.Height - 1)) / 2f);
                DateTime now = DateTime.Now;
                float angle = (((((now.Hour % 12) * 0xe10) + (now.Minute * 60)) + now.Second) * 360) / 0xa8c0;
                float num2 = (((now.Minute * 60) + now.Second) * 360) / 0xe10;
                float num3 = (now.Second * 360) / 60;
                graphics.RotateTransform(angle);
                graphics.DrawImage(this.hour, -((float) (this.hour.Width - 1)) / 2f, -((float) (this.clock.Height - 1)) / 2f, (float) this.hour.Width, (float) this.hour.Height);
                graphics.RotateTransform(num2 - angle);
                graphics.DrawImage(this.minute, -((float) (this.minute.Width - 1)) / 2f, -((float) (this.clock.Height - 1)) / 2f, (float) this.minute.Width, (float) this.minute.Height);
                graphics.RotateTransform(num3 - num2);
                graphics.DrawImage(this.second, -((float) (this.second.Width - 1)) / 2f, -((float) (this.clock.Height - 1)) / 2f, (float) this.second.Width, (float) this.second.Height);
                graphics.DrawImage(this.dot, -((float) (this.dot.Width - 1)) / 2f, -((float) (this.clock.Height - 1)) / 2f, (float) this.dot.Width, (float) this.dot.Height);
                Graphics graphics2 = base.CreateGraphics();
                graphics2.CompositingQuality = CompositingQuality.HighQuality;
                graphics2.InterpolationMode = InterpolationMode.HighQualityBilinear;
                graphics2.DrawImage(this.clock, new Rectangle(0, 0, base.Width, base.Height), new Rectangle(0, 0, this.clock.Width, this.clock.Height), GraphicsUnit.Pixel);
            }
            catch
            {
            }
        }

        private void mytimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            base.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                this.clock = Resources.trad;
                Graphics graphics = Graphics.FromImage(this.clock);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TranslateTransform(((float) (this.clock.Width - 1)) / 2f, ((float) (this.clock.Height - 1)) / 2f);
                DateTime now = DateTime.Now;
                float angle = (((((now.Hour % 12) * 0xe10) + (now.Minute * 60)) + now.Second) * 360) / 0xa8c0;
                float num2 = (((now.Minute * 60) + now.Second) * 360) / 0xe10;
                float num3 = (now.Second * 360) / 60;
                graphics.RotateTransform(angle);
                graphics.DrawImage(this.hour, -((float) (this.hour.Width - 1)) / 2f, -((float) (this.clock.Height - 1)) / 2f, (float) this.hour.Width, (float) this.hour.Height);
                graphics.RotateTransform(num2 - angle);
                graphics.DrawImage(this.minute, -((float) (this.minute.Width - 1)) / 2f, -((float) (this.clock.Height - 1)) / 2f, (float) this.minute.Width, (float) this.minute.Height);
                graphics.RotateTransform(num3 - num2);
                graphics.DrawImage(this.second, -((float) (this.second.Width - 1)) / 2f, -((float) (this.clock.Height - 1)) / 2f, (float) this.second.Width, (float) this.second.Height);
                graphics.DrawImage(this.dot, -((float) (this.dot.Width - 1)) / 2f, -((float) (this.clock.Height - 1)) / 2f, (float) this.dot.Width, (float) this.dot.Height);
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                e.Graphics.DrawImage(this.clock, new Rectangle(0, 0, base.Width, base.Height), new Rectangle(0, 0, this.clock.Width, this.clock.Height), GraphicsUnit.Pixel);
            }
            catch
            {
            }
            base.OnPaint(e);
        }
    }
}

