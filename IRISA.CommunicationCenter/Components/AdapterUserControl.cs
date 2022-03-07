using IRISA.CommunicationCenter.Library.Adapters;
using System;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Components
{
    public partial class AdapterUserControl : UserControl
    {
        public string Caption
        {
            get
            {
                return captionLabel.Text;
            }
            set
            {
                captionLabel.Text = value;
            }
        }

        public AdapterUserControl(IIccAdapter adapter)
        {
            InitializeComponent();
            Adapter = adapter;
            Caption = adapter.Name + " - " + adapter.PersianDescription;
            connectPictureBox.Click += new EventHandler(Adapter_Click);
            disconnectPictureBox.Click += new EventHandler(Adapter_Click);
            RefreshConnection();
        }

        private void Adapter_Click(object sender, EventArgs e)
        {
            RefreshConnection();
        }

        public void RefreshConnection()
        {
            if (!IsHandleCreated)
                CreateControl();

            if (!IsHandleCreated)
                return;

            _ = Invoke(new Action(() =>
              {
                  var connected = Adapter.Connected;
                  connectPictureBox.Visible = connected;
                  disconnectPictureBox.Visible = !connected;
              }));
        }
    }
}
