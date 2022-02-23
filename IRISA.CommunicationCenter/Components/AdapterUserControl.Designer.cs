using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Logging;
using System.ComponentModel;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Components
{
    public partial class AdapterUserControl
    {
        private IContainer components = null;
        public PictureBox disconnectPictureBox;
        private FlowLayoutPanel flowLayoutPanel;
        public PictureBox connectPictureBox;
        private Panel panel1;
        private Label captionLabel;
        private ILogger eventLogger;
        public IIccAdapter Adapter
        {
            get;
            set;
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
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.disconnectPictureBox = new System.Windows.Forms.PictureBox();
            this.connectPictureBox = new System.Windows.Forms.PictureBox();
            this.captionLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.disconnectPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel.Controls.Add(this.panel1);
            this.flowLayoutPanel.Controls.Add(this.captionLabel);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(105, 41);
            this.flowLayoutPanel.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.disconnectPictureBox);
            this.panel1.Controls.Add(this.connectPictureBox);
            this.panel1.Location = new System.Drawing.Point(62, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(40, 35);
            this.panel1.TabIndex = 4;
            // 
            // disconnectPictureBox
            // 
            this.disconnectPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.disconnectPictureBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.disconnectPictureBox.Image = global::IRISA.CommunicationCenter.Properties.Resources.disconnected;
            this.disconnectPictureBox.Location = new System.Drawing.Point(0, 0);
            this.disconnectPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.disconnectPictureBox.Name = "disconnectPictureBox";
            this.disconnectPictureBox.Size = new System.Drawing.Size(35, 35);
            this.disconnectPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.disconnectPictureBox.TabIndex = 2;
            this.disconnectPictureBox.TabStop = false;
            // 
            // connectPictureBox
            // 
            this.connectPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.connectPictureBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.connectPictureBox.Image = global::IRISA.CommunicationCenter.Properties.Resources.connected;
            this.connectPictureBox.Location = new System.Drawing.Point(0, 0);
            this.connectPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.connectPictureBox.Name = "connectPictureBox";
            this.connectPictureBox.Size = new System.Drawing.Size(35, 35);
            this.connectPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.connectPictureBox.TabIndex = 3;
            this.connectPictureBox.TabStop = false;
            this.connectPictureBox.Visible = false;
            // 
            // captionLabel
            // 
            this.captionLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.captionLabel.AutoSize = true;
            this.captionLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.captionLabel.Location = new System.Drawing.Point(3, 14);
            this.captionLabel.Name = "captionLabel";
            this.captionLabel.Size = new System.Drawing.Size(53, 13);
            this.captionLabel.TabIndex = 4;
            this.captionLabel.Text = "نام کلاینت";
            this.captionLabel.Click += new System.EventHandler(this.Adapter_Click);
            // 
            // AdapterUserControl
            // 
            this.AccessibleName = "";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "AdapterUserControl";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(105, 41);
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.disconnectPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.connectPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
