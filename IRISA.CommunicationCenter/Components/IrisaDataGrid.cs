using IRISA.CommunicationCenter.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Components
{
    public class IrisaDataGrid : DataGridView
    {
        public IrisaDataGrid()
        {
            CellContentDoubleClick += new DataGridViewCellEventHandler(IrisaDataGrid_CellContentDoubleClick);
            AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            BackgroundColor = Color.WhiteSmoke;
            ReadOnly = true;
            AutoGenerateColumns = false;
            AllowUserToAddRows = false;
            AllowUserToOrderColumns = true;
            MultiSelect = false;
        }
        private void IrisaDataGrid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string text = Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                int num = 1000;
                if (text.Length > num)
                {
                    text = text.Remove(num);
                    text += "...";
                }
                MessageForm.ShowInformationMessage(text);
            }
        }
    }
}
