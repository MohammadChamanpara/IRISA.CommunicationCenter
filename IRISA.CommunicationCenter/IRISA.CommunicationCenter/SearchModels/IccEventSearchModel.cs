using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRISA.CommunicationCenter.SearchModels
{
    public class IccEventSearchModel
    {
        public string EventDateFrom { get; set; }

        public Nullable<int> HourFrom { get; set; }

        public Nullable<int> MinuteFrom { get; set; }

        public Nullable<int> SecondFrom { get; set; }

        public string EventDateTo { get; set; }

        public Nullable<int> HourTo { get; set; }

        public Nullable<int> MinuteTo { get; set; }

        public Nullable<int> SecondTo { get; set; }


        public string Type { get; set; }

    }
}
