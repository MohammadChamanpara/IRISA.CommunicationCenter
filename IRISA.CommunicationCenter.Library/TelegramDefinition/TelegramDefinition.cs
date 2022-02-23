using System.Collections.Generic;
using System.Xml;
namespace IRISA.CommunicationCenter
{
    public class TelegramDefinition : NodeBase
	{
		public int Id
		{
			get
			{
				int result;
				try
				{
					result = int.Parse(base.Node.Attributes["id"].InnerText.Trim());
				}
				catch
				{
					throw HelperMethods.CreateException("شناسه عددی در تعریف تلگرام {0} مشخص نشده است.", new object[]
					{
						base.Name
					});
				}
				return result;
			}
		}
		public string Source
		{
			get
			{
				string result;
				try
				{
					result = base.Node.Attributes["source"].InnerText.Trim();
				}
				catch
				{
					throw HelperMethods.CreateException("مبدا در تعریف تلگرام مشخص نشده است.", new object[0]);
				}
				return result;
			}
		}
		public string Destination
		{
			get
			{
				string result;
				try
				{
					result = base.Node.Attributes["destination"].InnerText.Trim();
				}
				catch
				{
					throw HelperMethods.CreateException("مقصد در تعریف تلگرام مشخص نشده است.", new object[0]);
				}
				return result;
			}
		}
		public List<FieldDefinition> Fields
		{
			get
			{
				return this.GetFields(base.Node);
			}
		}
		public TelegramDefinition(XmlNode node) : base(node)
		{
		}
		private List<FieldDefinition> GetFields(XmlNode node)
		{
			List<FieldDefinition> list = new List<FieldDefinition>();
			foreach (XmlNode node2 in node.ChildNodes)
			{
				FieldDefinition fieldDefinition = new FieldDefinition(node2);
				if (fieldDefinition.IsArray)
				{
					List<FieldDefinition> fields = this.GetFields(node2);
					for (int i = 0; i < fieldDefinition.Size; i++)
					{
						list.AddRange(fields);
					}
				}
				else
				{
					list.Add(fieldDefinition);
				}
			}
			return list;
		}
	}
}
