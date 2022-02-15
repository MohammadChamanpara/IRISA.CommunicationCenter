using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace IRISA.CommunicationCenter
{
    /// <summary>
    /// Summary description for IccWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class IccWebService : System.Web.Services.WebService
    {

        public class TelegramsWithinRangeObject
        {
            public string PCS_Standard_Telegram_String_Form;
            public DateTime TelegramArrivalTime;
            public bool isTransferred;
            public DateTime TelegramTransferTime;
            public DateTime TelegramSenderSendTime;
            public string TelegramID;
            public TelegramsWithinRangeObject(string tel_body, DateTime sender_send_time, DateTime arrival_time, bool is_transferred, DateTime tel_trans_time, string telegram_id)
            {
                this.PCS_Standard_Telegram_String_Form = tel_body;
                this.TelegramArrivalTime = arrival_time;
                this.isTransferred = is_transferred;
                this.TelegramTransferTime = tel_trans_time;
                this.TelegramSenderSendTime = sender_send_time;
                this.TelegramID = telegram_id;
            }
            public TelegramsWithinRangeObject()
            {
                this.PCS_Standard_Telegram_String_Form = "";
                this.TelegramArrivalTime = DateTime.MinValue;
                this.isTransferred = false;
                this.TelegramTransferTime = DateTime.MinValue;
                this.TelegramSenderSendTime = DateTime.MinValue;
            }
        }
        private WebServiceAdapter adapter = new WebServiceAdapter();
        [WebMethod]
        public bool CheckForDataAvailability(string password)
        {
            bool result;
            try
            {
                this.adapter.CheckPassword(password);
                result = this.adapter.DataIsAvailableForWebService();
            }
            catch (Exception ex)
            {
                this.adapter.EventLogger.LogException(ex);
                if (Settings.Default.ThrowExceptionToCaller)
                {
                    throw ex;
                }
                result = false;
            }
            return result;
        }
        [WebMethod]
        public DateTime GetDateTime(string password)
        {
            DateTime result;
            try
            {
                this.adapter.CheckPassword(password);
                result = DateTime.Now;
            }
            catch (Exception ex)
            {
                this.adapter.EventLogger.LogException(ex);
                if (Settings.Default.ThrowExceptionToCaller)
                {
                    throw ex;
                }
                result = DateTime.MinValue;
            }
            return result;
        }
        [WebMethod]
        public string GetData(string password)
        {
            string result;
            try
            {
                this.adapter.CheckPassword(password);
                result = this.adapter.SendDataToWebService();
            }
            catch (Exception ex)
            {
                this.adapter.EventLogger.LogException(ex);
                if (Settings.Default.ThrowExceptionToCaller)
                {
                    throw ex;
                }
                result = null;
            }
            return result;
        }
        [WebMethod]
        public bool SendPcsData(string password, string telegram)
        {
            bool result;
            try
            {
                this.adapter.CheckPassword(password);
                result = this.adapter.ReceiveDataFromWebService(telegram);
            }
            catch (Exception ex)
            {
                this.adapter.EventLogger.LogException(ex);
                if (Settings.Default.ThrowExceptionToCaller)
                {
                    throw ex;
                }
                result = false;
            }
            return result;
        }
        [WebMethod]
        public IccWebService.TelegramsWithinRangeObject[] GetTelegramWithinRange(DateTime beg_date, DateTime end_date, bool SortOrder)
        {
            IccWebService.TelegramsWithinRangeObject[] result;
            try
            {
                result = this.adapter.GetHistory(beg_date, end_date, null, SortOrder);
            }
            catch (Exception ex)
            {
                this.adapter.EventLogger.LogException(ex);
                if (Settings.Default.ThrowExceptionToCaller)
                {
                    throw ex;
                }
                result = null;
            }
            return result;
        }
        [WebMethod]
        public IccWebService.TelegramsWithinRangeObject[] GetGivenTelegramIDWithinRange(DateTime beg_date, DateTime end_date, int tel_id, bool SortOrder)
        {
            IccWebService.TelegramsWithinRangeObject[] result;
            try
            {
                result = this.adapter.GetHistory(beg_date, end_date, new int?(tel_id), SortOrder);
            }
            catch (Exception ex)
            {
                this.adapter.EventLogger.LogException(ex);
                if (Settings.Default.ThrowExceptionToCaller)
                {
                    throw ex;
                }
                result = null;
            }
            return result;
        }
    }
}
