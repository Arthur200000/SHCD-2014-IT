namespace Qisi.General.Controls
{
    using CoreAudioApi;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
	/// <summary>
	/// Volume control.
	/// </summary>
    public class VolumeControl : UserControl
    {
        private IContainer components;
		private TrackBar volumeValue;

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
		/// <summary>
		/// Gets the volume.
		/// </summary>
        private void GetVolume()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    MMDevice defaultAudioEndpoint = new MMDeviceEnumerator().GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                    this.volumeValue.Value = (int) (defaultAudioEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar * (this.volumeValue.Maximum - this.volumeValue.Minimum));
                }
                catch
                {
                    this.volumeValue.Value = 0;
                }
            }
            else
            {
                uint num2;
                uint deviceID = 0;
                waveOutGetVolume(deviceID, out num2);
                uint num3 = num2 & 65535;
                uint num4 = (uint) ((num2 & -65536) >> 16);
                this.volumeValue.Value = ((int.Parse (num3.ToString ()) | int.Parse (num4.ToString ())) * (this.volumeValue.Maximum - this.volumeValue.Minimum)) / 65535;
            }
        }

        private void InitializeComponent()
        {
            this.volumeValue = new TrackBar();
            this.volumeValue.BeginInit();
            base.SuspendLayout();
            this.volumeValue.AutoSize = false;
            this.volumeValue.BackColor = Color.White;
            this.volumeValue.Dock = DockStyle.Fill;
            this.volumeValue.Location = new Point(0, 0);
            this.volumeValue.Margin = new Padding(2);
            this.volumeValue.Maximum = 100;
            this.volumeValue.Name = "trackBar1";
            this.volumeValue.Size = new Size(0x76, 30);
            this.volumeValue.TabIndex = 0;
            this.volumeValue.Scroll += new EventHandler(this.trackBar1_Scroll);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            base.BorderStyle = BorderStyle.FixedSingle;
            base.Controls.Add(this.volumeValue);
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;
            base.Margin = new Padding(2);
            base.Name = "VolumeControl";
            base.Size = new Size(0x76, 30);
            base.Load += new EventHandler(this.VolumeControl_Load);
            this.volumeValue.EndInit();
            base.ResumeLayout(false);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
                    enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).AudioEndpointVolume.MasterVolumeLevelScalar = ((float) this.volumeValue.Value) / ((float) (this.volumeValue.Maximum - this.volumeValue.Minimum));
                }
                catch
                {
                }
            }
            else
            {
                uint num = (uint) ((65535.0 * this.volumeValue.Value) / ((double) (this.volumeValue.Maximum - this.volumeValue.Minimum)));
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

