using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
namespace IRISA.CommunicationCenter
{
    public partial class TelegramViewerForm : Form
	{
		private UiSettings uiSettings = new UiSettings();
		public TelegramViewerForm(IccTelegram iccTelegram)
		{
			this.InitializeComponent();
			this.ShowTelegram(iccTelegram);
		}
		private void ShowTelegram(IccTelegram iccTelegram)
		{
			TelegramDefinitions telegramDefinitions = new TelegramDefinitions(this.uiSettings.TelegramDefinitionFile);
			TelegramDefinition telegramDefinition = telegramDefinitions.Find(iccTelegram);
			TreeNode treeNode = this.treeView.Nodes.Add("Telegram");
			treeNode.Nodes.Add("id : " + telegramDefinition.Id);
			treeNode.Nodes.Add("Name : " + telegramDefinition.Name);
			treeNode.Nodes.Add("Description : " + telegramDefinition.Description);
			treeNode.Nodes.Add("Source : " + telegramDefinition.Source);
			treeNode.Nodes.Add("Destination : " + telegramDefinition.Destination);
			TreeNode parentTreeNode = treeNode.Nodes.Add("Body");
			List<string> list = new List<string>();
			if (iccTelegram.Body != null)
			{
				list = iccTelegram.Body;
			}
			string[] array = iccTelegram.Body.ToArray();
			this.MakeTree(parentTreeNode, telegramDefinition.Node, ref array);
			treeNode.Expand();
		}
		private void MakeTree(TreeNode parentTreeNode, XmlNode xmlNode, ref string[] values)
		{
			int num = 0;
			foreach (XmlNode xmlNode2 in xmlNode)
			{
				num++;
				FieldDefinition fieldDefinition = new FieldDefinition(xmlNode2);
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
						TreeNode parentTreeNode2 = treeNode.Nodes.Add(fieldDefinition.Name + "[" + (i + 1).ToString() + "]");
						this.MakeTree(parentTreeNode2, xmlNode2, ref values);
					}
				}
			}
		}

	}
}
