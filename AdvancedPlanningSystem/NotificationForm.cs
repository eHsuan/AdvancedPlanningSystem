using System;
using System.Drawing;
using System.Windows.Forms;

namespace AdvancedPlanningSystem
{
    public enum NotificationLevel
    {
        Info,
        Warning,
        Critical
    }

    public class NotificationForm : Form
    {
        private Label lblMessage;
        private Label lblTitle;
        private Button btnClose;
        private Timer autoCloseTimer;

        public NotificationForm(string title, string message, NotificationLevel level, int autoCloseSec = 0)
        {
            InitializeComponent();
            
            lblTitle.Text = title;
            lblMessage.Text = message;

            switch (level)
            {
                case NotificationLevel.Critical:
                    this.BackColor = Color.Firebrick;
                    lblTitle.ForeColor = Color.White;
                    lblMessage.ForeColor = Color.White;
                    break;
                case NotificationLevel.Warning:
                    this.BackColor = Color.Goldenrod;
                    lblTitle.ForeColor = Color.Black;
                    lblMessage.ForeColor = Color.Black;
                    break;
                default:
                    this.BackColor = Color.SteelBlue;
                    lblTitle.ForeColor = Color.White;
                    lblMessage.ForeColor = Color.White;
                    break;
            }

            if (autoCloseSec > 0)
            {
                autoCloseTimer = new Timer();
                autoCloseTimer.Interval = autoCloseSec * 1000;
                autoCloseTimer.Tick += (s, e) => { autoCloseTimer.Stop(); this.Close(); };
                autoCloseTimer.Start();
            }
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblMessage = new Label();
            this.btnClose = new Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Font = new Font("Arial", 12F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 9);
            this.lblTitle.Size = new Size(360, 25);
            this.lblTitle.Text = "Notification";

            // lblMessage
            this.lblMessage.Font = new Font("Arial", 10F);
            this.lblMessage.Location = new Point(12, 40);
            this.lblMessage.Size = new Size(360, 60);
            this.lblMessage.Text = "Message content goes here.";

            // btnClose
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.ForeColor = Color.White;
            this.btnClose.Location = new Point(297, 105);
            this.btnClose.Size = new Size(75, 25);
            this.btnClose.Text = "Dismiss";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += (s, e) => this.Close();

            // NotificationForm
            this.ClientSize = new Size(384, 141);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            
            this.ResumeLayout(false);
        }

        public static void ShowAsync(string title, string message, NotificationLevel level, int autoCloseSec = 0)
        {
            // Ensure thread safety for UI
            if (Application.OpenForms.Count > 0)
            {
                var mainForm = Application.OpenForms[0];
                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(new Action(() => ShowAsync(title, message, level, autoCloseSec)));
                    return;
                }
            }

            var form = new NotificationForm(title, message, level, autoCloseSec);
            form.Show();
        }
    }
}
