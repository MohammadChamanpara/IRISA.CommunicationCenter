using IRISA.Windows.Forms;
namespace IRISA
{
    public static class HelperMethods
	{
		public static string ShowPasswordDialog(string caption)
		{
			PasswordDialogForm passwordDialogForm = new PasswordDialogForm();
			passwordDialogForm.Text = caption;
			passwordDialogForm.ShowDialog();
			return passwordDialogForm.passwordTextBox.Text;
		}
		public static void ShowErrorMessage(string messageFormat, params object[] messageArguments)
		{
			new MessageForm("خطا", messageFormat, messageArguments, CommunicationCenter.Properties.Resources.error).ShowDialog();
		}
		public static void ShowWarningMessage(string messageFormat, params object[] messageArguments)
		{
            new MessageForm("هشدار", messageFormat, messageArguments, CommunicationCenter.Properties.Resources.warning).ShowDialog();
		}
		public static void ShowInformationMessage(string messageFormat, params object[] messageArguments)
		{
            new MessageForm("اطلاعات", messageFormat, messageArguments, CommunicationCenter.Properties.Resources.info).ShowDialog();
		}
	}
}
