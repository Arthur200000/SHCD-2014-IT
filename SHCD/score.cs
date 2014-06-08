namespace SHCD
{
    using ExamClientControlsLibrary;
    using Qisi.General;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class score : Form
    {
        private Button button1;
        private Button button2;
        private IContainer components = null;
        private Label Label1;
        private Label Label10;
        private Label Label11;
        private Label Label12;
        private Label Label13;
        private Label Label14;
        private Label Label2;
        private Label Label3;
        private Label Label4;
        private Label Label5;
        private Label Label6;
        private Label Label7;
        private Label Label8;
        private Label Label9;
        private Form myowner;
        private string mypaper;

        public score(string paper)
        {
            this.InitializeComponent();
            this.mypaper = paper;
            MemoryIniFile file = new MemoryIniFile();
            file.LoadFromEncodedFile(Path.Combine(Program.tempAnswerDir, "Answer.ini"));
            this.Label1.Text = "<" + paper + ">  " + file.ReadValue("ANSWER", "SUBJECT", "");
            int num = file.ReadValue("ANSWER", "NUM", 0);
            int num2 = 0;
            float num3 = 0f;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            float num7 = 0f;
            MemoryIniFile file2 = new MemoryIniFile();
            file2.LoadFromFile(Path.Combine(Application.StartupPath, @"STD\" + paper + @"\stds.ini"));
            for (int i = 0; i < num; i++)
            {
                string sectionName = file.ReadValue(i.ToString(), "ID", "");
                string str2 = file2.ReadValue(sectionName, "std", "");
                if (str2 != "")
                {
                    num2++;
                    float num9 = file2.ReadValue(sectionName, "SCORE", (float) 0f);
                    num3 += num9;
                    string str3 = file.ReadValue(i.ToString(), "Answer", "");
                    if ((str3 == "") || (str3 == "[]"))
                    {
                        num4++;
                    }
                    else if (str3 == ("[" + str2 + "]"))
                    {
                        num5++;
                        num7 += num9;
                    }
                    else
                    {
                        num6++;
                    }
                }
            }
            this.Label3.Text = "题数";
            this.Label4.Text = num2.ToString();
            this.Label5.Text = "满分";
            this.Label6.Text = num3.ToString();
            this.Label7.Text = "未作答";
            this.Label8.Text = num4.ToString();
            this.Label9.Text = "答对";
            this.Label10.Text = num5.ToString();
            this.Label11.Text = "答错";
            this.Label12.Text = num6.ToString();
            this.Label13.Text = "得分";
            this.Label14.Text = num7.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormTemplate template = new FormTemplate(TestPaperPlayer.RunMode.CDAnalysis, this.mypaper);
            this.myowner = base.Owner;
            base.Owner.Hide();
            base.Hide();
            template.Disposed += new EventHandler(this.t_Disposed);
            template.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new upload(this.mypaper).ShowDialog();
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
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
            ComponentResourceManager manager = new ComponentResourceManager(typeof(score));
            this.Label1 = new Label();
            this.Label2 = new Label();
            this.button1 = new Button();
            this.button2 = new Button();
            this.Label3 = new Label();
            this.Label4 = new Label();
            this.Label5 = new Label();
            this.Label6 = new Label();
            this.Label7 = new Label();
            this.Label8 = new Label();
            this.Label9 = new Label();
            this.Label10 = new Label();
            this.Label11 = new Label();
            this.Label12 = new Label();
            this.Label13 = new Label();
            this.Label14 = new Label();
            base.SuspendLayout();
            this.Label1.AutoSize = true;
            this.Label1.BackColor = Color.Transparent;
            this.Label1.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label1.ForeColor = Color.White;
            this.Label1.Location = new Point(1, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new Size(0x38, 0x10);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Label1";
            this.Label2.AutoSize = true;
            this.Label2.BackColor = Color.Transparent;
            this.Label2.ForeColor = Color.White;
            this.Label2.Location = new Point(1, 0x20);
            this.Label2.Name = "Label2";
            this.Label2.Size = new Size(0x29, 12);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "选择题";
            this.button1.Cursor = Cursors.Hand;
            this.button1.FlatStyle = FlatStyle.Flat;
            this.button1.Location = new Point(0x45, 0xe3);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x4b, 0x17);
            this.button1.TabIndex = 3;
            this.button1.Text = "分析";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.Cursor = Cursors.Hand;
            this.button2.FlatStyle = FlatStyle.Flat;
            this.button2.Location = new Point(0xc7, 0xe3);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x4b, 0x17);
            this.button2.TabIndex = 4;
            this.button2.Text = "退出";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.Label3.AutoSize = true;
            this.Label3.BackColor = Color.Transparent;
            this.Label3.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label3.ForeColor = Color.White;
            this.Label3.Location = new Point(12, 0x36);
            this.Label3.Name = "Label3";
            this.Label3.Size = new Size(0x38, 0x10);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Label3";
            this.Label4.AutoSize = true;
            this.Label4.BackColor = Color.Transparent;
            this.Label4.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label4.ForeColor = Color.White;
            this.Label4.Location = new Point(0xc4, 0x36);
            this.Label4.Name = "Label4";
            this.Label4.Size = new Size(0x38, 0x10);
            this.Label4.TabIndex = 6;
            this.Label4.Text = "Label4";
            this.Label5.AutoSize = true;
            this.Label5.BackColor = Color.Transparent;
            this.Label5.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label5.ForeColor = Color.White;
            this.Label5.Location = new Point(12, 80);
            this.Label5.Name = "Label5";
            this.Label5.Size = new Size(0x38, 0x10);
            this.Label5.TabIndex = 7;
            this.Label5.Text = "Label5";
            this.Label6.AutoSize = true;
            this.Label6.BackColor = Color.Transparent;
            this.Label6.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label6.ForeColor = Color.White;
            this.Label6.Location = new Point(0xc4, 80);
            this.Label6.Name = "Label6";
            this.Label6.Size = new Size(0x38, 0x10);
            this.Label6.TabIndex = 8;
            this.Label6.Text = "Label6";
            this.Label7.AutoSize = true;
            this.Label7.BackColor = Color.Transparent;
            this.Label7.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label7.ForeColor = Color.White;
            this.Label7.Location = new Point(12, 0x6a);
            this.Label7.Name = "Label7";
            this.Label7.Size = new Size(0x38, 0x10);
            this.Label7.TabIndex = 9;
            this.Label7.Text = "Label7";
            this.Label8.AutoSize = true;
            this.Label8.BackColor = Color.Transparent;
            this.Label8.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label8.ForeColor = Color.White;
            this.Label8.Location = new Point(0xc4, 0x6a);
            this.Label8.Name = "Label8";
            this.Label8.Size = new Size(0x38, 0x10);
            this.Label8.TabIndex = 10;
            this.Label8.Text = "Label8";
            this.Label9.AutoSize = true;
            this.Label9.BackColor = Color.Transparent;
            this.Label9.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label9.ForeColor = Color.White;
            this.Label9.Location = new Point(12, 0x84);
            this.Label9.Name = "Label9";
            this.Label9.Size = new Size(0x38, 0x10);
            this.Label9.TabIndex = 11;
            this.Label9.Text = "Label9";
            this.Label10.AutoSize = true;
            this.Label10.BackColor = Color.Transparent;
            this.Label10.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label10.ForeColor = Color.White;
            this.Label10.Location = new Point(0xc4, 0x84);
            this.Label10.Name = "Label10";
            this.Label10.Size = new Size(0x40, 0x10);
            this.Label10.TabIndex = 12;
            this.Label10.Text = "Label10";
            this.Label11.AutoSize = true;
            this.Label11.BackColor = Color.Transparent;
            this.Label11.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label11.ForeColor = Color.White;
            this.Label11.Location = new Point(12, 0x9e);
            this.Label11.Name = "Label11";
            this.Label11.Size = new Size(0x40, 0x10);
            this.Label11.TabIndex = 13;
            this.Label11.Text = "Label11";
            this.Label12.AutoSize = true;
            this.Label12.BackColor = Color.Transparent;
            this.Label12.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label12.ForeColor = Color.White;
            this.Label12.Location = new Point(0xc4, 0x9e);
            this.Label12.Name = "Label12";
            this.Label12.Size = new Size(0x40, 0x10);
            this.Label12.TabIndex = 14;
            this.Label12.Text = "Label12";
            this.Label13.AutoSize = true;
            this.Label13.BackColor = Color.Transparent;
            this.Label13.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label13.ForeColor = Color.White;
            this.Label13.Location = new Point(12, 0xb8);
            this.Label13.Name = "Label13";
            this.Label13.Size = new Size(0x40, 0x10);
            this.Label13.TabIndex = 15;
            this.Label13.Text = "Label13";
            this.Label14.AutoSize = true;
            this.Label14.BackColor = Color.Transparent;
            this.Label14.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.Label14.ForeColor = Color.White;
            this.Label14.Location = new Point(0xc4, 0xb8);
            this.Label14.Name = "Label14";
            this.Label14.Size = new Size(0x40, 0x10);
            this.Label14.TabIndex = 0x10;
            this.Label14.Text = "Label14";
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(0x45, 0x89, 0x94);
            base.ClientSize = new Size(0x173, 0x121);
            base.Controls.Add(this.Label14);
            base.Controls.Add(this.Label13);
            base.Controls.Add(this.Label12);
            base.Controls.Add(this.Label11);
            base.Controls.Add(this.Label10);
            base.Controls.Add(this.Label9);
            base.Controls.Add(this.Label8);
            base.Controls.Add(this.Label7);
            base.Controls.Add(this.Label6);
            base.Controls.Add(this.Label5);
            base.Controls.Add(this.Label4);
            base.Controls.Add(this.Label3);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.Label2);
            base.Controls.Add(this.Label1);
            this.DoubleBuffered = true;
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "score";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "分数";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public void t_Disposed(object sender, EventArgs e)
        {
            if (this.myowner != null)
            {
                this.myowner.Show();
                base.Owner = this.myowner;
                base.ShowDialog(this.myowner);
            }
        }
    }
}

