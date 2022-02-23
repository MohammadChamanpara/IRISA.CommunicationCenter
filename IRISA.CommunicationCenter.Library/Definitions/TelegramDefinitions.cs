using IRISA.CommunicationCenter.Library.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace IRISA.CommunicationCenter.Library.Definitions
{
    public class TelegramDefinitions
    {
        public List<TelegramDefinition> List
        {
            get;
            set;
        }
        public TelegramDefinitions(string telegramDefinitionFilePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (!File.Exists(telegramDefinitionFilePath))
            {
                throw HelperMethods.CreateException("فایل تعریف تلگرام ها با نام {0} وجود ندارد.", new object[]
                {
                    telegramDefinitionFilePath
                });
            }
            xmlDocument.Load(telegramDefinitionFilePath);
            List = new List<TelegramDefinition>();
            foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
            {
                List.Add(new TelegramDefinition(node));
            }
        }
        public TelegramDefinition Find(IccTelegram iccTelegram)
        {
            List<TelegramDefinition> list = new List<TelegramDefinition>();
            foreach (TelegramDefinition current in List)
            {
                if (iccTelegram.TelegramId == current.Id)
                {
                    list.Add(current);
                }
            }
            if (list.Count == 0)
            {
                throw HelperMethods.CreateException("تلگرام دریافت شده در سیستم تعریف نشده است.", new object[0]);
            }
            list = (
                from x in list
                where x.Source.Contains(iccTelegram.Source)
                select x).ToList();
            if (list.Count() == 0)
            {
                throw HelperMethods.CreateException("فرستنده تعیین شده برای تلگرام با فرستنده جاری متفاوت است.", new object[0]);
            }
            if (iccTelegram.Destination.HasValue())
            {
                if (list.Count > 0)
                {
                    list = (
                        from x in list
                        where x.Destination.Contains(iccTelegram.Destination)
                        select x).ToList();
                }
                if (list.Count == 0)
                {
                    throw HelperMethods.CreateException("گیرنده تعیین شده برای تلگرام با گیرنده جاری متفاوت است.", new object[0]);
                }
            }
            if (list.Count > 1)
            {
                throw HelperMethods.CreateException("تلگرام  با مشخصات دریافت شده چند بار در سیستم تعریف شده است.", new object[0]);
            }
            return list.First();
        }
    }
}
