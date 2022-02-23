using IRISA.CommunicationCenter.Library.Loggers;
using System.Xml;

namespace IRISA.CommunicationCenter.Library.Definitions
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
                    result = Node.Attributes["name"].InnerText.Trim();
                }
                catch
                {
                    throw IrisaException.Create("نام برای تعریف فیلد مشخص نشده است.", new object[0]);
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
                    result = Node.Attributes["description"].InnerText.Trim();
                }
                catch
                {
                    throw IrisaException.Create("شرح برای تعریف فیلد {0} مشخص نشده است.", new object[]
                    {
                        Name
                    });
                }
                return result;
            }
        }
        public NodeBase(XmlNode node)
        {
            Node = node;
        }
    }
}
