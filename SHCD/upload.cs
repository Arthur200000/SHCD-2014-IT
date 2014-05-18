namespace SHCD
{
    using Qisi.General;
    using Qisi.General.Controls;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;

    public class upload : Form
    {
        private Button button1;
        private IContainer components = null;
        private Label label1;
        private string mypaper;
        private TextBox textBox1;

        public upload(string paper)
        {
            this.InitializeComponent();
            this.mypaper = paper;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Contains("&") || this.textBox1.Text.Contains("="))
            {
                FlatMessageBox.Show(this, "邮箱输入错误，请重新输入邮箱地址！", "提示", FlatMessageBox.KeysButtons.YesNo, FlatMessageBox.KeysIcon.Information);
            }
            else if ((FlatMessageBox.Show(this, "每张模拟卷只能获取一次评估报告，你确定现在就要获取评估报告吗？", "提示", FlatMessageBox.KeysButtons.YesNo, FlatMessageBox.KeysIcon.Information) == DialogResult.Yes) && (FlatMessageBox.Show(this, "你的邮箱是：" + this.textBox1.Text + "\r\n确定吗?", "提示", FlatMessageBox.KeysButtons.YesNo, FlatMessageBox.KeysIcon.Information) != DialogResult.No))
            {
                string newDir = "";
                string str2 = Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini";
                string s = "";
                try
                {
                    s = System.IO.File.ReadAllText(Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini");
                }
                catch
                {
                    s = "";
                }
                finally
                {
                    byte[] bytes = Convert.FromBase64String(s);
                    s = Encoding.ASCII.GetString(bytes);
                    if (s.Length >= 0x22)
                    {
                        newDir = s.Substring(0, 0x12);
                    }
                }
                WebClient client = new WebClient();
                string str4 = "";
                try
                {
                    str4 = client.DownloadString("http://www.keys-edu.com/register/upload.asp?LISTCODE=" + newDir + "&PAPERNAME=" + this.mypaper + "&EMAIL=" + this.textBox1.Text);
                }
                catch
                {
                }
                if (str4 == "1")
                {
                    if (FlatMessageBox.Show(this, "点击“确定”开始上传你的答题数据！", "提示", FlatMessageBox.KeysButtons.YesNo, FlatMessageBox.KeysIcon.Information) == DialogResult.Yes)
                    {
                        FTPClient client2 = new FTPClient("www.keys-edu.com", "shcd", "Keys0801");
                        if (client2.login())
                        {
                            client2.makeDir(newDir);
                            client2.Upload(Path.Combine(Program.answerDir, this.mypaper + ".dat"));
                        }
                        FlatMessageBox.Show(this, "数据上传完成，我们会在两周内返回你的评估报告。", "提示", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Information);
                        base.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    FlatMessageBox.Show(this, "你可能已经获取过评估报告，如有任何疑问，请拨打咨询热线。", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Information);
                }
            }
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
            this.label1 = new Label();
            this.textBox1 = new TextBox();
            this.button1 = new Button();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.label1.ForeColor = Color.White;
            this.label1.Location = new Point(1, 0x20);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x200, 0x10);
            this.label1.TabIndex = 0;
            this.label1.Text = "请输入你的邮箱地址，我们将在2周内，将评估报告发往你填写的邮箱。";
            this.textBox1.Location = new Point(0x44, 0x43);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(0x180, 0x15);
            this.textBox1.TabIndex = 1;
            this.button1.Cursor = Cursors.Hand;
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.Location = new Point(0xd9, 0x73);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x4b, 0x17);
            this.button1.TabIndex = 4;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(0x45, 0x89, 0x94);
            base.ClientSize = new Size(0x1fd, 0xa9);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.textBox1);
            base.Controls.Add(this.label1);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Name = "upload";
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "upload";
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

