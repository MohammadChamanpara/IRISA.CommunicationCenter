using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace IRISA.Windows.Forms
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
			this.InitializeComponent();
		}
		private void okButton_Click(object sender, EventArgs e)
		{
			base.Close();
		}
		private void passwordTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				this.okButton_Click(null, null);
			}
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordDialogForm));
			this.passwordTextBox = new MaskedTextBox();
			this.okButton = new Button();
			this.label1 = new Label();
			this.splitContainer1 = new SplitContainer();
			((ISupportInitialize)this.splitContainer1).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			base.SuspendLayout();
			this.passwordTextBox.Location = new Point(22, 32);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new Size(151, 21);
			this.passwordTextBox.TabIndex = 0;
			this.passwordTextBox.KeyDown += new KeyEventHandler(this.passwordTextBox_KeyDown);
			this.okButton.Location = new Point(93, 9);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(75, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "تایید";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new EventHandler(this.okButton_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(179, 36);
			this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new Size(59, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "کلمه عبور :";
			this.splitContainer1.Dock = DockStyle.Fill;
			this.splitContainer1.Location = new Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = Orientation.Horizontal;
			this.splitContainer1.Panel1.BackColor = Color.White;
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.passwordTextBox);
			this.splitContainer1.Panel2.BackColor = SystemColors.Control;
			this.splitContainer1.Panel2.Controls.Add(this.okButton);
			this.splitContainer1.Size = new Size(261, 129);
			this.splitContainer1.SplitterDistance = 84;
			this.splitContainer1.SplitterWidth = 1;
			this.splitContainer1.TabIndex = 3;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new Size(261, 129);
			base.Controls.Add(this.splitContainer1);
			this.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 178);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.KeyPreview = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PasswordDialogForm";
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.TopMost = true;
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer1).EndInit();
			this.splitContainer1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
