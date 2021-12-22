using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    public class English
    {
        public static Hashtable Translations
        {
            get
            {
                Hashtable TableTranslations = new Hashtable();

                TableTranslations.Add("and", "and");
                TableTranslations.Add("at", "at");
                TableTranslations.Add("between", "between");
                TableTranslations.Add("day", "day");
                TableTranslations.Add("days", "days");
                TableTranslations.Add("every", "every");
                TableTranslations.Add("ExitRecurring", "Occurs {0} {1} between {2} starting on {3}");
                TableTranslations.Add("First", "First");
                TableTranslations.Add("Fourth", "Fourth");
                TableTranslations.Add("Friday", "Friday");
                TableTranslations.Add("hour", "hour");
                TableTranslations.Add("hours", "hours");
                TableTranslations.Add("Last", "Last");
                TableTranslations.Add("Monday", "Monday");
                TableTranslations.Add("month", "month");
                TableTranslations.Add("months", "months");
                TableTranslations.Add("of", "of");
                TableTranslations.Add("on", "on");
                TableTranslations.Add("once", "once");
                TableTranslations.Add("Output", "Occurs {0}. Schedule will be used on {1}");
                TableTranslations.Add("Saturday", "Saturday");
                TableTranslations.Add("Second", "Second");
                TableTranslations.Add("StartingOn", "starting on {0}");
                TableTranslations.Add("Sunday", "Sunday");
                TableTranslations.Add("the", "the");
                TableTranslations.Add("Third", "Third");
                TableTranslations.Add("Thursday", "Thursday");
                TableTranslations.Add("Tuesday", "Tuesday");
                TableTranslations.Add("ValidateConfiguration", "Need to fill the configuration");
                TableTranslations.Add("ValidateDailyFrequency", "Need to set Daily Frequency");
                TableTranslations.Add("ValidateDateConfiguration", "Need to set the Date From and Step in configuration");
                TableTranslations.Add("ValidateDayWeekSelected", "Need to set any day in weekly configuration");
                TableTranslations.Add("ValidateHourFromBigggerHourTo", "Hour From not should be bigger than Hour To");
                TableTranslations.Add("ValidateHourStep", "Hour step must be bigger than 0");
                TableTranslations.Add("ValidateHourStepOfDailyFrequency", "Need to set hour step in daily frequency bigger than 0");
                TableTranslations.Add("ValidateMonthlyConfiguration", "Need to set one of the checks in Monthly Configuration (day, the ..)");
                TableTranslations.Add("ValidateMonthlyMonths", "Need to set month(s) bigger than 0");
                TableTranslations.Add("ValidateMonthlyMoreHourFromTo", "Need to set the hour from and hour to");
                TableTranslations.Add("ValidateMonthlyMoreWeekStep", "Need to set the day frequency");
                TableTranslations.Add("ValidateMonthlyOnceDayFrequency", "Day must be bigger than 0");
                TableTranslations.Add("ValidateMonthlyOnceMonthFrequency", "Month must be bigger than 0");
                TableTranslations.Add("ValidateRecurringFrequency", "Need to set frequency");
                TableTranslations.Add("ValidateWeeklyStep", "Need to set week step bigger than 0");
                TableTranslations.Add("Wednesday", "Wednesday");
                TableTranslations.Add("week", "week");
                TableTranslations.Add("weekday", "weekday");
                TableTranslations.Add("weekend", "weekend");
                TableTranslations.Add("weekendday", "weekendday");
                TableTranslations.Add("weeks", "weeks");

                return TableTranslations;
            }
        }
    }
}
