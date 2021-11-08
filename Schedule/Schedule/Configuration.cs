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
        public DateTime? DateFrom;
        public DateTime? DateTo;

        public TypeTimeStep? TypeRecurring;

        #region recurring
        public int? HourStep;
        public bool Enabled;
        public DateTime? HourFrom;
        public DateTime? HourTo;

        public int DailyStep;

        public int WeekStep;
        public bool WeeklyMonday;
        public bool WeeklyTuesday;
        public bool WeeklyWednesday;
        public bool WeeklyThursday;
        public bool WeeklyFriday;
        public bool WeeklySaturday;
        public bool WeeklySunday;

        public bool? MonthlyOnce;
        public int? MonthlyOnceDay;
        public int? MonthlyOnceMonthSteps;
        public DailyFrequency? TypeDailyFrequency;

        public bool? MonthlyMore;
        public TypeWeekStep? MonthlyMoreWeekStep;
        public TypeDayWeekStep? MonthlyMoreOrderDayWeekStep;
        public int? MonthlyMoreMonthSteps;

        #endregion
    }
}
