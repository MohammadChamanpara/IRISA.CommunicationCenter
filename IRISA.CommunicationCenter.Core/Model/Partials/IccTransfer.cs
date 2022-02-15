using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace IRISA.CommunicationCenter.Model
{
    public partial class IccTransfer
    {
        public string PERSIAN_SEND_TIME
        {
            get
            {
                var date = this.SEND_TIME;
                string pDate = "";
                if (date != null)
                {
                    var Pc = new PersianCalendar();
                    int year = Pc.GetYear(date);
                    int month = Pc.GetMonth(date);
                    int day = Pc.GetDayOfMonth(date);
                    pDate = string.Format("{0:0000}/{1:00}/{2:00}-{3:00}:{4:00}:{5:00}", year, month, day, date.Hour, date.Minute, date.Second);
                }
                return pDate;
            }
        }

        public string PERSIAN_RECEIVE_TIME
        {
            get
            {
                 var date = this.RECEIVE_TIME;
                string pDate = "";
                if (date != null)
                {
                    var Pc = new PersianCalendar();
                    int year = Pc.GetYear(date.Value);
                    int month = Pc.GetMonth(date.Value);
                    int day = Pc.GetDayOfMonth(date.Value);
                    pDate = string.Format("{0:0000}/{1:00}/{2:00}-{3:00}:{4:00}:{5:00}", year, month, day, date.Value.Hour, date.Value.Minute, date.Value.Second);
                }
                return pDate;
            }
        }
    }
}
