namespace SHCD
{
    using Qisi.General.Controls;
    using SHCD.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Windows.Forms;

    public class FormReg : Form
    {
        private Button button1;
        private IContainer components = null;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label5;
        private Label label9;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private ReadOnlyRichTextBox readOnlyRichTextBox1;
        private TextBox textBox5;
        private TextBox textBox6;
        private TextBox textBox7;

        public FormReg()
        {
            this.InitializeComponent();
            this.readOnlyRichTextBox1.BorderStyle = BorderStyle.None;
            this.readOnlyRichTextBox1.LoadFile(Path.Combine(Application.StartupPath, "SHCD.inf"));
            this.readOnlyRichTextBox1.ReadOnly = true;
            this.radioButton1.Checked = true;
            this.label10.Visible = false;
            this.label9.Visible = false;
            this.label5.Visible = false;
            this.textBox5.Visible = false;
            this.textBox6.Visible = false;
            this.textBox6.Text = Program.doString(Program.getCpuId()).PadLeft(2, '0') + Program.doString(Program.getBaseBoardId()).PadLeft(2, '0') + Program.doString(Program.getBIOSId()).PadLeft(2, '0') + Program.doString(Program.getPhysicalMediaId()).PadLeft(2, '0');
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Program.CheckListCode(this.textBox7.Text))
            {
                FlatMessageBox.Show(this, "请输入正确的密码！", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
            }
            else
            {
                string str;
                if (this.radioButton2.Checked)
                {
                    int num;
                    int[] array = new int[8];
                    for (num = 0; num < this.textBox6.Text.Length; num++)
                    {
                        array[num] = Convert.ToInt32(this.textBox6.Text[num].ToString());
                    }
                    Array.Sort<int>(array);
                    int num2 = 0;
                    for (num = 0; num < array.Length; num++)
                    {
                        num2 += array[num] * ((int) Math.Pow(10.0, (double) ((array.Length - 1) - num)));
                    }
                    num2 = 0x5f5e100 - num2;
                    long num3 = Convert.ToInt64(this.textBox7.Text.Substring(2)) % ((long) num2);
                    if (this.textBox5.Text == num3.ToString().PadLeft(8, '0'))
                    {
                        FlatMessageBox.Show(this, "注册成功！", "消息", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Information);
                        str = this.textBox7.Text + this.textBox6.Text + this.textBox5.Text;
                        str = Convert.ToBase64String(Encoding.ASCII.GetBytes(str));
                        System.IO.File.WriteAllText(Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini", str);
                        System.IO.File.SetAttributes(Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini", FileAttributes.Hidden);
                        base.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        FlatMessageBox.Show(this, "请输入正确的密码！", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                    }
                }
                else
                {
                    WebClient client = new WebClient();
                    string str2 = "";
                    try
                    {
                        str2 = client.DownloadString("http://www.keys-edu.com/register/register.asp?LISTCODE=" + this.textBox7.Text + "&MACHINECODE=" + this.textBox6.Text + "&REGISTERTYPE=0");
                    }
                    catch (WebException)
                    {
                        FlatMessageBox.Show(this, "网络连接错误！", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                        return;
                    }
                    catch (NotSupportedException)
                    {
                        FlatMessageBox.Show(this, "网络连接错误！", "错误", FlatMessageBox.KeysButtons.OK, FlatMessageBox.KeysIcon.Error);
                        return;
                    }
                    if (str2.StartsWith("0"))
                    {
                        FlatMessageBox.Show(this, str2.Substring(1));
                    }
                    else
                    {
                        MessageBox.Show("注册成功！");
                        str = this.textBox7.Text + this.textBox6.Text + str2.Substring(1);
                        str = Convert.ToBase64String(Encoding.ASCII.GetBytes(str));
                        System.IO.File.WriteAllText(Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini", str);
                        System.IO.File.SetAttributes(Environment.GetEnvironmentVariable("USERPROFILE") + @"\SHCD.ini", FileAttributes.Hidden);
                        base.DialogResult = DialogResult.OK;
                    }
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

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        private void InitializeComponent()
        {
            this.readOnlyRichTextBox1 = new ReadOnlyRichTextBox();
            this.textBox5 = new TextBox();
            this.textBox6 = new TextBox();
            this.textBox7 = new TextBox();
            this.label5 = new Label();
            this.label9 = new Label();
            this.label10 = new Label();
            this.label11 = new Label();
            this.label12 = new Label();
            this.button1 = new Button();
            this.radioButton1 = new RadioButton();
            this.radioButton2 = new RadioButton();
            base.SuspendLayout();
            this.readOnlyRichTextBox1.Cursor = Cursors.Arrow;
            this.readOnlyRichTextBox1.Location = new Point(12, 12);
            this.readOnlyRichTextBox1.Name = "readOnlyRichTextBox1";
            this.readOnlyRichTextBox1.ReadOnly = true;
            this.readOnlyRichTextBox1.Size = new Size(0x2e9, 250);
            this.readOnlyRichTextBox1.TabIndex = 0;
            this.readOnlyRichTextBox1.Text = "";
            this.textBox5.Location = new Point(0x4a, 0x169);
            this.textBox5.MaxLength = 8;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Size(0x11e, 0x15);
            this.textBox5.TabIndex = 0x1a;
            this.textBox6.Location = new Point(0x5b, 0x14b);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new Size(0x10d, 0x15);
            this.textBox6.TabIndex = 0x17;
            this.textBox7.Location = new Point(0x3d, 0x12d);
            this.textBox7.MaxLength = 0x12;
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Size(0x12b, 0x15);
            this.textBox7.TabIndex = 0x15;
            this.label5.AutoSize = true;
            this.label5.BackColor = Color.Transparent;
            this.label5.Font = new Font("黑体", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label5.Location = new Point(0x184, 0x16a);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0xac, 14);
            this.label5.TabIndex = 0x19;
            this.label5.Text = "（客服电话返回注册码）";
            this.label9.AutoSize = true;
            this.label9.BackColor = Color.Transparent;
            this.label9.Font = new Font("黑体", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label9.Location = new Point(11, 0x16a);
            this.label9.Name = "label9";
            this.label9.Size = new Size(0x43, 14);
            this.label9.TabIndex = 0x18;
            this.label9.Text = "注册码：";
            this.label10.AutoSize = true;
            this.label10.BackColor = Color.Transparent;
            this.label10.Font = new Font("黑体", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label10.Location = new Point(11, 0x14c);
            this.label10.Name = "label10";
            this.label10.Size = new Size(0x52, 14);
            this.label10.TabIndex = 0x16;
            this.label10.Text = "基础信息：";
            this.label10.Click += new EventHandler(this.label10_Click);
            this.label11.AutoSize = true;
            this.label11.BackColor = Color.Transparent;
            this.label11.Font = new Font("黑体", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label11.Location = new Point(0x184, 0x12e);
            this.label11.Name = "label11";
            this.label11.Size = new Size(0x142, 14);
            this.label11.TabIndex = 20;
            this.label11.Text = "（输入光盘包装内侧印刷的密码（需要刮开））";
            this.label12.AutoSize = true;
            this.label12.BackColor = Color.Transparent;
            this.label12.Font = new Font("黑体", 10.5f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label12.Location = new Point(12, 0x12e);
            this.label12.Name = "label12";
            this.label12.Size = new Size(0x34, 14);
            this.label12.TabIndex = 0x13;
            this.label12.Text = "密码：";
            this.button1.Cursor = Cursors.Hand;
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.Location = new Point(0x151, 0x192);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x4b, 0x17);
            this.button1.TabIndex = 0x1b;
            this.button1.Text = "注册";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = Color.Transparent;
            this.radioButton1.Cursor = Cursors.Hand;
            this.radioButton1.Font = new Font("黑体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.radioButton1.Location = new Point(15, 0x10c);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new Size(0x51, 0x12);
            this.radioButton1.TabIndex = 0x1d;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "网络注册";
            this.radioButton1.UseVisualStyleBackColor = false;
            this.radioButton1.CheckedChanged += new EventHandler(this.radioButton1_CheckedChanged);
            this.radioButton2.AutoSize = true;
            this.radioButton2.BackColor = Color.Transparent;
            this.radioButton2.Cursor = Cursors.Hand;
            this.radioButton2.Font = new Font("黑体", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.radioButton2.Location = new Point(0x85, 0x10c);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new Size(0x51, 0x12);
            this.radioButton2.TabIndex = 30;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "电话注册";
            this.radioButton2.UseVisualStyleBackColor = false;
            this.radioButton2.CheckedChanged += new EventHandler(this.radioButton2_CheckedChanged);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(0x45, 0x89, 0x94);
            base.ClientSize = new Size(0x301, 0x1bc);
            base.Controls.Add(this.radioButton2);
            base.Controls.Add(this.radioButton1);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.textBox5);
            base.Controls.Add(this.textBox6);
            base.Controls.Add(this.textBox7);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.label9);
            base.Controls.Add(this.label10);
            base.Controls.Add(this.label11);
            base.Controls.Add(this.label12);
            base.Controls.Add(this.readOnlyRichTextBox1);
            this.DoubleBuffered = true;
            base.FormBorderStyle = FormBorderStyle.FixedSingle;
            base.Icon = Resources.Main;
            base.MaximizeBox = false;
            base.Name = "FormReg";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "软件注册";
            base.SizeChanged += new EventHandler(this.Form1_SizeChanged);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void label10_Click(object sender, EventArgs e)
        {
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                this.label10.Visible = false;
                this.label9.Visible = false;
                this.label5.Visible = false;
                this.textBox5.Visible = false;
                this.textBox6.Visible = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton2.Checked)
            {
                this.label10.Visible = true;
                this.label9.Visible = true;
                this.label5.Visible = true;
                this.textBox5.Visible = true;
                this.textBox6.Visible = true;
            }
        }
    }
}

