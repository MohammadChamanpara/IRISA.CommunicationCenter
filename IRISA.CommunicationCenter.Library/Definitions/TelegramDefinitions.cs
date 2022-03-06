using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Loggers;
using IRISA.CommunicationCenter.Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace IRISA.CommunicationCenter.Library.Definitions
{
    public class TelegramDefinitions : ITelegramDefinitions
    {
        private const string TelegramDefinitionFilePath = "TelegramDefinitions.xml";

        private readonly List<TelegramDefinition> _telegramDefinitions = new List<TelegramDefinition>();
        public TelegramDefinitions()
        {
            XmlDocument xmlDocument = new XmlDocument();

            if (!File.Exists(TelegramDefinitionFilePath))
            {
                throw IrisaException.Create($"فایل تعریف تلگرام ها وجود ندارد. موقعیت فایل: {Path.GetFullPath(TelegramDefinitionFilePath)}");
            }

            xmlDocument.Load(TelegramDefinitionFilePath);

            _telegramDefinitions.Clear();
            foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
            {
                _telegramDefinitions.Add(new TelegramDefinition(node));
            }
        }

        public ITelegramDefinition Find(IccTelegram iccTelegram)
        {
            var telegramDefinitions = _telegramDefinitions.Where(x => x.Id == iccTelegram.TelegramId);

            if (telegramDefinitions.Count() == 0)
                throw IrisaException.Create("تلگرام دریافت شده در سیستم تعریف نشده است.");

            if (!iccTelegram.Source.HasValue())
                throw IrisaException.Create("فرستنده تلگرام مشخص نشده است.");

            telegramDefinitions = telegramDefinitions
                .Where(definition => ("," + definition.Source?.ToLower() + ",")
                .Contains("," + iccTelegram.Source.ToLower() + ","))
                .ToList();

            if (telegramDefinitions.Count() == 0)
                throw IrisaException.Create("فرستنده تعیین شده برای تلگرام با فرستنده جاری متفاوت است.");

            if (telegramDefinitions.Count() > 1)
                throw IrisaException.Create("تلگرام با مشخصات دریافت شده چند بار در سیستم تعریف شده است.");

            return telegramDefinitions.Single();
        }

        public void ValidateTelegramExpiry(IccTelegram iccTelegram)
        {
            var definition = Find(iccTelegram);

            if (!definition.ExpiryInMinutes.HasValue)
                return;

            var expiry = definition.ExpiryInMinutes.Value;
            int passedMinutes = (int)(DateTime.Now - iccTelegram.SendTime).TotalMinutes;

            if (passedMinutes > expiry)
                throw IrisaException.Create($"تلگرام منقضی شده است. از زمان ارسال تلگرام {passedMinutes} دقیقه گذشته است. زمان معتبر بودن تلگرام {expiry} دقیقه است.");
        }
    }
}
