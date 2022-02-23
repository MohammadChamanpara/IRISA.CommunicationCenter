using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Logging;
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
				return this.captionLabel.Text;
			}
			set
			{
				this.captionLabel.Text = value;
			}
		}

		public AdapterUserControl(IIccAdapter adapter, ILogger eventLogger)
		{
			this.InitializeComponent();
			this.eventLogger = eventLogger;
			this.Adapter = adapter;
			this.Caption = adapter.Name + " - " + adapter.PersianDescription;
			this.connectPictureBox.Click += new EventHandler(this.Adapter_Click);
			this.disconnectPictureBox.Click += new EventHandler(this.Adapter_Click);
            adapter.ConnectionChanged += Adapter_ConnectionChanged;
			this.RefreshConnection();
		}

        private void Adapter_Click(object sender, EventArgs e)
		{
			this.RefreshConnection();
		}
		private void Adapter_ConnectionChanged(object sender, AdapterConnectionChangedEventArgs e)
		{
			this.RefreshConnection();
		}

		public void RefreshConnection()
		{
			try
			{
				if (!base.IsHandleCreated)
				{
					base.CreateControl();
				}
				if (base.IsHandleCreated)
				{
					if (this.Adapter.Connected)
					{
						base.Invoke(new Action(()=>
						{
							this.connectPictureBox.Visible = true;
						}));
						base.Invoke(new Action(()=>
						{
							this.disconnectPictureBox.Visible = false;
						}));
					}
					else
					{
						base.Invoke(new Action(()=>
						{
							this.disconnectPictureBox.Visible = true;
						}));
						base.Invoke(new Action(()=>
						{
							this.connectPictureBox.Visible = false;
						}));
					}
				}
			}
			catch (Exception ex)
			{
				if (this.eventLogger == null)
				{
					throw ex;
				}
				this.eventLogger.LogException(ex, "بروز خطا هنگام بررسی وضعیت اتصال کلاینت");
			}
		}
	}
}
