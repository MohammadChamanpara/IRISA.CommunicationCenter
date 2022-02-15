using System;
using System.Windows.Forms;
namespace IRISA.Windows.Components
{
	public abstract class BaseSeparator : Label
	{
		public BaseSeparator()
		{
			this.BorderStyle = BorderStyle.Fixed3D;
			this.AutoSize = false;
			this.Text = "";
		}
	}
}
