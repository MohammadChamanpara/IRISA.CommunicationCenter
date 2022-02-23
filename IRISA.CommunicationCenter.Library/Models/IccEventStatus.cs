using System.ComponentModel;

namespace IRISA.CommunicationCenter.Library.Models
{
    public enum IccEventStatus
    {
        [Description("هشدار")]
        Warning = 0,

        [Description("اطلاعات")]
        Information = 1,

        [Description("خطا")]
        Error = 2

    }
}
