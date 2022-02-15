using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IRISA.CommunicationCenter
{
   public enum IccEventStatus
    {
       [Description("هشدار")]
       Warning=0,

       [Description("اطلاعات")]
       Information=1,

       [Description("خطا")]
       Error=2

    }
}
