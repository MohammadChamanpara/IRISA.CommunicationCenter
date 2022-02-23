using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Logging;
using System;
using System.Windows.Forms;
namespace IRISA.CommunicationCenter
{
    public partial class PluginUserControl : UserControl
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

		public PluginUserControl(IIccAdapter plugin, ILogger eventLogger)
		{
			this.InitializeComponent();
			this.eventLogger = eventLogger;
			this.Plugin = plugin;
			this.Caption = plugin.Name + " - " + plugin.PersianDescription;
			this.connectPictureBox.Click += new EventHandler(this.Plugin_Click);
			this.disconnectPictureBox.Click += new EventHandler(this.Plugin_Click);
            plugin.ConnectionChanged += plugin_ConnectionChanged;
			this.RefreshConnection();
		}

        private void Plugin_Click(object sender, EventArgs e)
		{
			this.RefreshConnection();
		}
		private void plugin_ConnectionChanged(object sender, AdapterConnectionChangedEventArgs e)
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
					if (this.Plugin.Connected)
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
				this.eventLogger.LogException(ex);
			}
		}
	}
}
