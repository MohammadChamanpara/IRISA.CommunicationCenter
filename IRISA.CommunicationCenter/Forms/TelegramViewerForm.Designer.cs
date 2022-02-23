using System.ComponentModel;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter
{
    public partial class TelegramViewerForm
    {
        private IContainer components = null;
        private TreeView treeView;
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
            this.treeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(473, 431);
            this.treeView.TabIndex = 0;
            // 
            // TelegramViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 431);
            this.Controls.Add(this.treeView);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Name = "TelegramViewerForm";
            this.Text = "مشاهده محتوای تلگرام";
            this.ResumeLayout(false);

        }
    }
}
