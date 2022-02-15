using System;
using System.Drawing;
using System.Windows.Forms;
namespace IRISA.Windows.Components
{
	public class IrisaDataGrid : DataGridView
	{
		public IrisaDataGrid()
		{
			base.CellContentDoubleClick += new DataGridViewCellEventHandler(this.IrisaDataGrid_CellContentDoubleClick);
			base.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
			base.BackgroundColor = Color.WhiteSmoke;
			base.ReadOnly = true;
			base.AutoGenerateColumns = false;
			base.AllowUserToAddRows = false;
			base.AllowUserToOrderColumns = true;
			base.MultiSelect = false;
		}
		private void IrisaDataGrid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
			{
				string text = base.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				int num = 1000;
				if (text.Length > num)
				{
					text = text.Remove(num);
					text += "...";
				}
				HelperMethods.ShowInformationMessage(text, new object[0]);
			}
		}
	}
}
