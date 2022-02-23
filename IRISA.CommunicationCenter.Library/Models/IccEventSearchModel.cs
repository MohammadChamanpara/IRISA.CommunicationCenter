
namespace IRISA.CommunicationCenter.Library.Models
{
    public class IccEventSearchModel
    {
        public string EventDateFrom { get; set; }

        public int? HourFrom { get; set; }

        public int? MinuteFrom { get; set; }

        public int? SecondFrom { get; set; }

        public string EventDateTo { get; set; }

        public int? HourTo { get; set; }

        public int? MinuteTo { get; set; }

        public int? SecondTo { get; set; }


        public string Type { get; set; }

    }
}
