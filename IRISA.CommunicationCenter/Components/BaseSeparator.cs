using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Components
{
    public abstract class BaseSeparator : Label
    {
        public BaseSeparator()
        {
            BorderStyle = BorderStyle.Fixed3D;
            AutoSize = false;
            Text = "";
        }
    }
}
