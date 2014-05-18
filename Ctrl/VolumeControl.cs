namespace Qisi.General.Controls
{
    using CoreAudioApi;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class VolumeControl : UserControl
    {
        private IContainer components;
        private TrackBar trackBar1;

        public VolumeControl()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GetVolume()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    MMDevice defaultAudioEndpoint = new MMDeviceEnumerator().GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                    this.trackBar1.Value = (int) (defaultAudioEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar * (this.trackBar1.Maximum - this.trackBar1.Minimum));
                }
                catch
                {
                    this.trackBar1.Value = 0;
                }
            }
            else
            {
                uint num2;
                uint deviceID = 0;
                waveOutGetVolume(deviceID, out num2);
                uint num3 = num2 & 0xffff;
                uint num4 = (uint) ((num2 & -65536) >> 0x10);
                this.trackBar1.Value = ((int.Parse(num3.ToString()) | int.Parse(num4.ToString())) * (this.trackBar1.Maximum - this.trackBar1.Minimum)) / 0xffff;
            }
        }

        private void InitializeComponent()
        {
            this.trackBar1 = new TrackBar();
            this.trackBar1.BeginInit();
            base.SuspendLayout();
            this.trackBar1.AutoSize = false;
            this.trackBar1.BackColor = Color.White;
            this.trackBar1.Dock = DockStyle.Fill;
            this.trackBar1.Location = new Point(0, 0);
            this.trackBar1.Margin = new Padding(2);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new Size(0x76, 30);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Scroll += new EventHandler(this.trackBar1_Scroll);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            base.BorderStyle = BorderStyle.FixedSingle;
            base.Controls.Add(this.trackBar1);
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;
            base.Margin = new Padding(2);
            base.Name = "VolumeControl";
            base.Size = new Size(0x76, 30);
            base.Load += new EventHandler(this.VolumeControl_Load);
            this.trackBar1.EndInit();
            base.ResumeLayout(false);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
                    enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume.MasterVolumeLevelScalar = ((float) this.trackBar1.Value) / ((float) (this.trackBar1.Maximum - this.trackBar1.Minimum));
                }
                catch
                {
                }
            }
            else
            {
                uint num = (uint) ((65535.0 * this.trackBar1.Value) / ((double) (this.trackBar1.Maximum - this.trackBar1.Minimum)));
                if (num < 0)
                {
                    num = 0;
                }
                if (num > 0xffff)
                {
                    num = 0xffff;
                }
                uint num2 = num;
                uint num3 = num;
                waveOutSetVolume(0, (num2 << 0x10) | num3);
            }
        }

        private void VolumeControl_Load(object sender, EventArgs e)
        {
            this.GetVolume();
        }

        [DllImport("winmm.dll")]
        public static extern long waveOutGetVolume(uint deviceID, out uint Volume);
        [DllImport("winmm.dll")]
        public static extern long waveOutSetVolume(uint deviceID, uint Volume);
    }
}

