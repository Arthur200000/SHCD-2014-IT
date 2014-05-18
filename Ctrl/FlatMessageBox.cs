namespace Qisi.General.Controls
{
    using Qisi.General.Controls.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FlatMessageBox : Form
    {
        private static Button btnAbort;
        private static Button btnCancel;
        private static Button btnIgnore;
        private static Button btnNo;
        private static Button btnOK;
        private static Button btnRetry;
        private static Button btnYes;
        private IContainer components;
        private static FlowLayoutPanel flpButtons;
        private static Image frmIcon;
        private static Label frmMessage;
        private static DialogResult KeysReturnButton;
        private static FlatMessageBox newMessageBox;
        private static PictureBox pIcon;

        private static void btnAbort_Click(object sender, EventArgs e)
        {
            KeysReturnButton = DialogResult.Abort;
            newMessageBox.Close();
        }

        private static void btnCancel_Click(object sender, EventArgs e)
        {
            KeysReturnButton = DialogResult.Cancel;
            newMessageBox.Close();
        }

        private static void btnIgnore_Click(object sender, EventArgs e)
        {
            KeysReturnButton = DialogResult.Ignore;
            newMessageBox.Close();
        }

        private static void btnNo_Click(object sender, EventArgs e)
        {
            KeysReturnButton = DialogResult.No;
            newMessageBox.Close();
        }

        private static void btnOK_Click(object sender, EventArgs e)
        {
            KeysReturnButton = DialogResult.OK;
            newMessageBox.Close();
        }

        private static void btnRetry_Click(object sender, EventArgs e)
        {
            KeysReturnButton = DialogResult.Retry;
            newMessageBox.Close();
        }

        private static void btnYes_Click(object sender, EventArgs e)
        {
            KeysReturnButton = DialogResult.Yes;
            newMessageBox.Close();
        }

        private static void BuildMessageBox(string title)
        {
            newMessageBox = new FlatMessageBox();
            newMessageBox.DoubleBuffered = true;
            newMessageBox.Text = title;
            newMessageBox.Size = new Size(400, 200);
            newMessageBox.StartPosition = FormStartPosition.CenterScreen;
            newMessageBox.FormBorderStyle = FormBorderStyle.FixedDialog;
            newMessageBox.Visible = false;
            newMessageBox.BackColor = Color.White;
            newMessageBox.MaximizeBox = false;
            TableLayoutPanel panel = new TableLayoutPanel {
                RowCount = 3,
                ColumnCount = 0,
                Dock = DockStyle.Fill
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));
            panel.BackColor = Color.Transparent;
            panel.Padding = new Padding(2, 5, 2, 2);
            frmMessage = new Label();
            frmMessage.Dock = DockStyle.Fill;
            frmMessage.BackColor = Color.White;
            frmMessage.Font = new Font("Cambria", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            frmMessage.Text = "";
            pIcon = new PictureBox();
            pIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            flpButtons = new FlowLayoutPanel();
            flpButtons.FlowDirection = FlowDirection.RightToLeft;
            flpButtons.Padding = new Padding(0, 5, 5, 0);
            flpButtons.Dock = DockStyle.Fill;
            flpButtons.BackColor = Color.Transparent;
            TableLayoutPanel panel2 = new TableLayoutPanel {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 0,
                Padding = new Padding(4, 5, 4, 4)
            };
            panel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64f));
            panel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            panel2.Controls.Add(pIcon);
            panel2.Controls.Add(frmMessage);
            panel.Controls.Add(panel2);
            panel.Controls.Add(flpButtons);
            newMessageBox.Controls.Add(panel);
        }

        private static void ButtonStatements(KeysButtons MButtons)
        {
            if (MButtons == KeysButtons.AbortRetryIgnore)
            {
                ShowIgnoreButton();
                ShowRetryButton();
                ShowAbortButton();
            }
            if (MButtons == KeysButtons.OK)
            {
                ShowOKButton();
            }
            if (MButtons == KeysButtons.OKCancel)
            {
                ShowCancelButton();
                ShowOKButton();
            }
            if (MButtons == KeysButtons.RetryCancel)
            {
                ShowCancelButton();
                ShowRetryButton();
            }
            if (MButtons == KeysButtons.YesNo)
            {
                ShowNoButton();
                ShowYesButton();
            }
            if (MButtons == KeysButtons.YesNoCancel)
            {
                ShowCancelButton();
                ShowNoButton();
                ShowYesButton();
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

        private static void IconStatements(KeysIcon MIcon)
        {
            if (MIcon == KeysIcon.Error)
            {
                frmIcon = Resources.Error;
            }
            if (MIcon == KeysIcon.Explorer)
            {
                frmIcon = Resources.Explorer;
            }
            if (MIcon == KeysIcon.Find)
            {
                frmIcon = Resources.Find;
            }
            if (MIcon == KeysIcon.Information)
            {
                frmIcon = Resources.Information;
            }
            if (MIcon == KeysIcon.Mail)
            {
                frmIcon = Resources.Mail;
            }
            if (MIcon == KeysIcon.Media)
            {
                frmIcon = Resources.Media;
            }
            if (MIcon == KeysIcon.Print)
            {
                frmIcon = Resources.Print;
            }
            if (MIcon == KeysIcon.Question)
            {
                frmIcon = Resources.Question;
            }
            if (MIcon == KeysIcon.RecycleBinEmpty)
            {
                frmIcon = Resources.ReEmpty;
            }
            if (MIcon == KeysIcon.RecycleBinFull)
            {
                frmIcon = Resources.ReFull;
            }
            if (MIcon == KeysIcon.Stop)
            {
                frmIcon = Resources.Stop;
            }
            if (MIcon == KeysIcon.User)
            {
                frmIcon = Resources.User;
            }
            if (MIcon == KeysIcon.Warning)
            {
                frmIcon = Resources.Warning;
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            base.AutoScaleMode = AutoScaleMode.Font;
        }

        public static DialogResult Show(string Message)
        {
            BuildMessageBox("");
            frmMessage.Text = Message;
            ShowOKButton();
            newMessageBox.ShowDialog();
            return KeysReturnButton;
        }

        public static DialogResult Show(string Message, string Title)
        {
            BuildMessageBox(Title);
            frmMessage.Text = Message;
            ShowOKButton();
            newMessageBox.ShowDialog();
            return KeysReturnButton;
        }

        public static DialogResult Show(IWin32Window win32, string Message)
        {
            BuildMessageBox("");
            frmMessage.Text = Message;
            ShowOKButton();
            newMessageBox.ShowDialog(win32);
            return KeysReturnButton;
        }

        public static DialogResult Show(string Message, string Title, KeysButtons MButtons)
        {
            BuildMessageBox(Title);
            frmMessage.Text = Message;
            ButtonStatements(MButtons);
            newMessageBox.ShowDialog();
            return KeysReturnButton;
        }

        public static DialogResult Show(IWin32Window win32, string Message, string Title)
        {
            BuildMessageBox(Title);
            frmMessage.Text = Message;
            ShowOKButton();
            newMessageBox.ShowDialog(win32);
            return KeysReturnButton;
        }

        public static DialogResult Show(string Message, string Title, KeysButtons MButtons, KeysIcon MIcon)
        {
            BuildMessageBox(Title);
            frmMessage.Text = Message;
            ButtonStatements(MButtons);
            IconStatements(MIcon);
            pIcon.Image = frmIcon;
            newMessageBox.ShowDialog();
            return KeysReturnButton;
        }

        public static DialogResult Show(IWin32Window win32, string Message, string Title, KeysButtons MButtons)
        {
            BuildMessageBox(Title);
            frmMessage.Text = Message;
            ButtonStatements(MButtons);
            newMessageBox.ShowDialog(win32);
            return KeysReturnButton;
        }

        public static DialogResult Show(IWin32Window win32, string Message, string Title, KeysButtons MButtons, KeysIcon MIcon)
        {
            BuildMessageBox(Title);
            frmMessage.Text = Message;
            ButtonStatements(MButtons);
            IconStatements(MIcon);
            pIcon.Image = frmIcon;
            newMessageBox.ShowDialog(win32);
            return KeysReturnButton;
        }

        private static void ShowAbortButton()
        {
            btnAbort = new Button();
            btnAbort.Text = "终止";
            btnAbort.Cursor = Cursors.Hand;
            btnAbort.FlatStyle = FlatStyle.Flat;
            btnAbort.Size = new Size(80, 0x20);
            btnAbort.BackColor = Color.FromArgb(0xff, 0xff, 0xff);
            btnAbort.Font = new Font("Tahoma", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnAbort.Click += new EventHandler(FlatMessageBox.btnAbort_Click);
            flpButtons.Controls.Add(btnAbort);
        }

        private static void ShowCancelButton()
        {
            btnCancel = new Button();
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Text = "取消";
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(80, 0x20);
            btnCancel.BackColor = Color.FromArgb(0xff, 0xff, 0xff);
            btnCancel.Font = new Font("Tahoma", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnCancel.Click += new EventHandler(FlatMessageBox.btnCancel_Click);
            flpButtons.Controls.Add(btnCancel);
        }

        private static void ShowIgnoreButton()
        {
            btnIgnore = new Button();
            btnIgnore.Cursor = Cursors.Hand;
            btnIgnore.Text = "忽略";
            btnIgnore.FlatStyle = FlatStyle.Flat;
            btnIgnore.Size = new Size(80, 0x20);
            btnIgnore.BackColor = Color.FromArgb(0xff, 0xff, 0xff);
            btnIgnore.Font = new Font("Tahoma", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnIgnore.Click += new EventHandler(FlatMessageBox.btnIgnore_Click);
            flpButtons.Controls.Add(btnIgnore);
        }

        private static void ShowNoButton()
        {
            btnNo = new Button();
            btnNo.Cursor = Cursors.Hand;
            btnNo.Text = "否";
            btnNo.FlatStyle = FlatStyle.Flat;
            btnNo.Size = new Size(80, 0x20);
            btnNo.BackColor = Color.FromArgb(0xff, 0xff, 0xff);
            btnNo.Font = new Font("Tahoma", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnNo.Click += new EventHandler(FlatMessageBox.btnNo_Click);
            flpButtons.Controls.Add(btnNo);
        }

        private static void ShowOKButton()
        {
            btnOK = new Button();
            btnOK.Cursor = Cursors.Hand;
            btnOK.Text = "确定";
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Size = new Size(80, 0x20);
            btnOK.BackColor = Color.FromArgb(0xff, 0xff, 0xff);
            btnOK.Font = new Font("Tahoma", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnOK.Click += new EventHandler(FlatMessageBox.btnOK_Click);
            flpButtons.Controls.Add(btnOK);
        }

        private static void ShowRetryButton()
        {
            btnRetry = new Button();
            btnRetry.Cursor = Cursors.Hand;
            btnRetry.Text = "重试";
            btnRetry.FlatStyle = FlatStyle.Flat;
            btnRetry.Size = new Size(80, 0x20);
            btnRetry.BackColor = Color.FromArgb(0xff, 0xff, 0xff);
            btnRetry.Font = new Font("Tahoma", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnRetry.Click += new EventHandler(FlatMessageBox.btnRetry_Click);
            flpButtons.Controls.Add(btnRetry);
        }

        private static void ShowYesButton()
        {
            btnYes = new Button();
            btnYes.Cursor = Cursors.Hand;
            btnYes.Text = "是";
            btnYes.FlatStyle = FlatStyle.Flat;
            btnYes.Size = new Size(80, 0x20);
            btnYes.BackColor = Color.FromArgb(0xff, 0xff, 0xff);
            btnYes.Font = new Font("Tahoma", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
            btnYes.Click += new EventHandler(FlatMessageBox.btnYes_Click);
            flpButtons.Controls.Add(btnYes);
        }

        public enum KeysButtons
        {
            AbortRetryIgnore,
            OK,
            OKCancel,
            RetryCancel,
            YesNo,
            YesNoCancel
        }

        public enum KeysIcon
        {
            Error,
            Explorer,
            Find,
            Information,
            Mail,
            Media,
            Print,
            Question,
            RecycleBinEmpty,
            RecycleBinFull,
            Stop,
            User,
            Warning
        }
    }
}

