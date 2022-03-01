using IRISA.CommunicationCenter.Library.Definitions;
using IRISA.CommunicationCenter.Library.Models;
using System.Linq;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Forms
{
    public partial class TelegramViewerForm : Form
    {
        public TelegramViewerForm(IccTelegram iccTelegram, ITelegramDefinitions telegramDefinitions)
        {
            InitializeComponent();
            ShowTelegram(iccTelegram, telegramDefinitions);
        }
        private void ShowTelegram(IccTelegram iccTelegram, ITelegramDefinitions telegramDefinitions)
        {
            var telegramDefinition = telegramDefinitions.Find(iccTelegram);
            TreeNode treeNode = treeView.Nodes.Add("Telegram");
            treeNode.Nodes.Add("id : " + telegramDefinition.Id);
            treeNode.Nodes.Add("Name : " + telegramDefinition.Name);
            treeNode.Nodes.Add("Description : " + telegramDefinition.Description);
            treeNode.Nodes.Add("Source : " + telegramDefinition.Source);
            treeNode.Nodes.Add("Destination : " + telegramDefinition.Destination);
            TreeNode parentTreeNode = treeNode.Nodes.Add("Body");
            string[] array = iccTelegram.Body.ToArray();
            MakeTree(parentTreeNode, telegramDefinition, ref array);
            treeNode.Expand();
        }
        private void MakeTree(TreeNode parentTreeNode, ITelegramDefinition xmlNode, ref string[] values)
        {
            int num = 0;
            foreach (var fieldDefinition in xmlNode.Fields)
            {
                num++;
                if (!fieldDefinition.IsArray)
                {
                    string text = (values.Length == 0) ? "<Empty>" : values[0];
                    string text2 = string.Format("{0}. {1} ( {2} - {3} )  :  {4}", new object[]
                    {
                        num,
                        fieldDefinition.Name,
                        fieldDefinition.Type,
                        fieldDefinition.Size,
                        text
                    });
                    parentTreeNode.Nodes.Add(text2);
                    values = values.Skip(1).ToArray<string>();
                }
                else
                {
                    string text2 = string.Format("{0} ( {1} - {2} )", fieldDefinition.Name, fieldDefinition.Type, fieldDefinition.Size);
                    TreeNode treeNode = parentTreeNode.Nodes.Add(text2);
                    for (int i = 0; i < fieldDefinition.Size; i++)
                    {
                        _ = treeNode.Nodes.Add(fieldDefinition.Name + "[" + (i + 1).ToString() + "]");
                    }
                }
            }
        }

    }
}
