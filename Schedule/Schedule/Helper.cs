using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    public enum TypeStep
    {
        Once = 0,
        Recurring = 1
    }

    public enum TypeTimeStep
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2
    }

    public enum TypeWeekStep
    {
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Last = 5
    }
    public enum TypeDayWeekStep
    {
        Monday = 0,
        Tuesday = 1,
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5,
        Sunday = 6,
        Day = 7,
        WeekDay = 8,
        WeekendDay = 9
    }

    public enum DailyFrequency
    {
        Once,
        Every
    }

    public enum Languages
    {
        en_GB = 0,
        es_ES = 1,
        en_US = 2
    }
}
