using IRISA.CommunicationCenter.Library.Models;

namespace IRISA.CommunicationCenter.Core.Model
{
    public static class IccTransferExtensions
    {
        public static IccTelegram ToIccTelegram(this IccTransfer iccTransfer)
        {
            var telegram = new IccTelegram()
            {
                Destination = iccTransfer.DESTINATION,
                TelegramId = iccTransfer.TELEGRAM_ID ?? 0,
                TransferId = iccTransfer.ID,
                SendTime = iccTransfer.SEND_TIME,
                Source = iccTransfer.SOURCE,
                Dropped = iccTransfer.DROPPED,
                Sent = iccTransfer.SENT,
                DropReason = iccTransfer.DROP_REASON,
                ReceiveTime = iccTransfer.RECEIVE_TIME
            };
            telegram.SetBodyFromString(iccTransfer.BODY,',');
            return telegram;
        }

        public static IccTransfer ToIccTransfer(this IccTelegram iccTelegram)
        {
            return new IccTransfer
            {
                ID = iccTelegram.TransferId,
                TELEGRAM_ID = iccTelegram.TelegramId,
                SOURCE = iccTelegram.Source,
                DESTINATION = iccTelegram.Destination,
                SEND_TIME = iccTelegram.SendTime,
                RECEIVE_TIME = iccTelegram.ReceiveTime,
                SENT = iccTelegram.Sent,
                DROPPED = iccTelegram.Dropped,
                DROP_REASON = iccTelegram.DropReason,
                BODY = iccTelegram.BodyString
            };
        }
    }
}
