using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Forms
{
    public class PasswordDialogForm : Form
    {
        private IContainer components = null;
        private Button okButton;
        private Label label1;
        private SplitContainer splitContainer1;
        internal MaskedTextBox passwordTextBox;
        public PasswordDialogForm()
        {
            InitializeComponent();
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                okButton_Click(null, null);
            }
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PasswordDialogForm));
            passwordTextBox = new MaskedTextBox();
            okButton = new Button();
            label1 = new Label();
            splitContainer1 = new SplitContainer();
            ((ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            passwordTextBox.Location = new Point(22, 32);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new Size(151, 21);
            passwordTextBox.TabIndex = 0;
            passwordTextBox.KeyDown += new KeyEventHandler(passwordTextBox_KeyDown);
            okButton.Location = new Point(93, 9);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 1;
            okButton.Text = "تایید";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += new EventHandler(okButton_Click);
            label1.AutoSize = true;
            label1.Location = new Point(179, 36);
            label1.Name = "label1";
            label1.RightToLeft = RightToLeft.Yes;
            label1.Size = new Size(59, 13);
            label1.TabIndex = 2;
            label1.Text = "کلمه عبور :";
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            splitContainer1.Panel1.BackColor = Color.White;
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(passwordTextBox);
            splitContainer1.Panel2.BackColor = SystemColors.Control;
            splitContainer1.Panel2.Controls.Add(okButton);
            splitContainer1.Size = new Size(261, 129);
            splitContainer1.SplitterDistance = 84;
            splitContainer1.SplitterWidth = 1;
            splitContainer1.TabIndex = 3;
            AutoScaleDimensions = new SizeF(6f, 13f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(261, 129);
            Controls.Add(splitContainer1);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 178);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PasswordDialogForm";
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
