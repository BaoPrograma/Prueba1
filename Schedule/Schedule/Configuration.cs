using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Config
{
    public class Configuration
    {
        public Configuration()
        {
        }

        public TypeStep TimeType;
        public DateTime? DateStep;
        public TypeDayStep? TypeStep;
        public DateTime? DateFrom;
        public DateTime? DateTo;
        public int? HourStep;
        public int WeekStep;
        public bool Enabled;
        public bool WeekMonday;
        public bool WeekTuesday;
        public bool WeekWednesday;
        public bool WeekThursday;
        public bool WeekFriday;
        public bool WeekSaturday;
        public bool WeekSunday;
        public DateTime? HourFrom;
        public DateTime? HourTo;
    }
}
