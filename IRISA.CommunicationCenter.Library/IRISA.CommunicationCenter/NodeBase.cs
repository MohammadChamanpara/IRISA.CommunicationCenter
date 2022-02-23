using System;
using System.Xml;
namespace IRISA.CommunicationCenter
{
	public class NodeBase
	{
		public XmlNode Node
		{
			get;
			set;
		}
		public string Name
		{
			get
			{
				string result;
				try
				{
					result = this.Node.Attributes["name"].InnerText.Trim();
				}
				catch
				{
					throw HelperMethods.CreateException("نام برای تعریف فیلد مشخص نشده است.", new object[0]);
				}
				return result;
			}
		}
		public string Description
		{
			get
			{
				string result;
				try
				{
					result = this.Node.Attributes["description"].InnerText.Trim();
				}
				catch
				{
					throw HelperMethods.CreateException("شرح برای تعریف فیلد {0} مشخص نشده است.", new object[]
					{
						this.Name
					});
				}
				return result;
			}
		}
		public NodeBase(XmlNode node)
		{
			this.Node = node;
		}
	}
}
