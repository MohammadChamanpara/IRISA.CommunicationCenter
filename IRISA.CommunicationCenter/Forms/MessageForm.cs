using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Forms
{
    public class MessageForm : Form
    {
        private IContainer components = null;
        private Panel panel1;
        private Button okButton;
        private Panel panel2;
        internal PictureBox pictureBox;
        internal Label messageLabel;
        public MessageForm(string caption, string messageFormat, object[] messageArguments, Bitmap icon)
        {
            InitializeComponent();
            Text = caption;
            messageLabel.Text = string.Format(messageFormat.Trim(), messageArguments);
            pictureBox.Image = icon;
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void MessageForm_Load(object sender, EventArgs e)
        {
            Width = messageLabel.Width + pictureBox.Width + 30;
            Height = messageLabel.Height + 130;
            pictureBox.Left = Width - pictureBox.Width - 20;
            messageLabel.Left = pictureBox.Left - messageLabel.Width;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            panel1 = new Panel();
            okButton = new Button();
            messageLabel = new Label();
            pictureBox = new PictureBox();
            panel2 = new Panel();
            panel1.SuspendLayout();
            ((ISupportInitialize)pictureBox).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(okButton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 79);
            panel1.Name = "panel1";
            panel1.Size = new Size(144, 43);
            panel1.TabIndex = 0;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.None;
            okButton.Location = new Point(35, 10);
            okButton.Margin = new Padding(10);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 0;
            okButton.Text = "تایید";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += new EventHandler(okButton_Click);
            // 
            // messageLabel
            // 
            messageLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            messageLabel.AutoSize = true;
            messageLabel.Location = new Point(68, 33);
            messageLabel.MaximumSize = new Size(600, 0);
            messageLabel.MinimumSize = new Size(10, 10);
            messageLabel.Name = "messageLabel";
            messageLabel.RightToLeft = RightToLeft.Yes;
            messageLabel.Size = new Size(24, 13);
            messageLabel.TabIndex = 1;
            messageLabel.Text = "پیام";
            messageLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pictureBox
            // 
            pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                        | AnchorStyles.Right;
            pictureBox.Location = new Point(100, 23);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(32, 32);
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox.TabIndex = 2;
            pictureBox.TabStop = false;
            // 
            // panel2
            // 
            panel2.AutoSize = true;
            panel2.BackColor = Color.White;
            panel2.Controls.Add(pictureBox);
            panel2.Controls.Add(messageLabel);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(144, 79);
            panel2.TabIndex = 3;
            // 
            // MessageForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(144, 122);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 178);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(150, 150);
            Name = "MessageForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MessageForm";
            Load += new EventHandler(MessageForm_Load);
            panel1.ResumeLayout(false);
            ((ISupportInitialize)pictureBox).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        public static void ShowErrorMessage(string messageFormat, params object[] messageArguments)
        {
            new MessageForm("خطا", messageFormat, messageArguments, CommunicationCenter.Properties.Resources.error).ShowDialog();
        }
        public static void ShowWarningMessage(string messageFormat, params object[] messageArguments)
        {
            new MessageForm("هشدار", messageFormat, messageArguments, CommunicationCenter.Properties.Resources.warning).ShowDialog();
        }
        public static void ShowInformationMessage(string messageFormat, params object[] messageArguments)
        {
            new MessageForm("اطلاعات", messageFormat, messageArguments, CommunicationCenter.Properties.Resources.info).ShowDialog();
        }
    }
}
