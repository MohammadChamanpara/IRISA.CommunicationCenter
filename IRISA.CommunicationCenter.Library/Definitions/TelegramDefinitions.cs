using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Loggers;
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
                throw IrisaException.Create("فایل تعریف تلگرام ها با نام {0} وجود ندارد.", new object[]
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
                throw IrisaException.Create("تلگرام دریافت شده در سیستم تعریف نشده است.");
            }
            list = (
                from x in list
                where x.Source.Contains(iccTelegram.Source)
                select x).ToList();
            if (list.Count() == 0)
            {
                throw IrisaException.Create("فرستنده تعیین شده برای تلگرام با فرستنده جاری متفاوت است.");
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
                    throw IrisaException.Create("گیرنده تعیین شده برای تلگرام با گیرنده جاری متفاوت است.");
                }
            }
            if (list.Count > 1)
            {
                throw IrisaException.Create("تلگرام  با مشخصات دریافت شده چند بار در سیستم تعریف شده است.");
            }
            return list.First();
        }
    }
}
