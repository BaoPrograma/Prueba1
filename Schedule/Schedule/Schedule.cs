using Schedule.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Schedule.Process
{
    public class Schedule
    {
        private CultureInfo culture;

        public Schedule(Configuration TheConfiguration)
        {
            this.ValidateConfiguration(TheConfiguration);

            this.culture = CultureInfo.GetCultureInfo(
                TheConfiguration.Language.ToString());

            Thread.CurrentThread.CurrentUICulture = this.culture;
        }

        private void ValidateConfiguration(Configuration TheConfiguration)
        {
            if (TheConfiguration == null)
                throw new ScheduleException(
                    this.GetTranslation(new Configuration(), "ValidateConfiguration"));
        }

        private DayOfWeek[] PrepareWeeklyVar(Configuration TheConfiguration)
        {
            List<DayOfWeek> WeekList = new List<DayOfWeek>();

            if (TheConfiguration.WeeklyMonday)
                WeekList.Add(DayOfWeek.Monday);
            if (TheConfiguration.WeeklyTuesday)
                WeekList.Add(DayOfWeek.Tuesday);
            if (TheConfiguration.WeeklyWednesday)
                WeekList.Add(DayOfWeek.Wednesday);
            if (TheConfiguration.WeeklyThursday)
                WeekList.Add(DayOfWeek.Thursday);
            if (TheConfiguration.WeeklyFriday)
                WeekList.Add(DayOfWeek.Friday);
            if (TheConfiguration.WeeklySaturday)
                WeekList.Add(DayOfWeek.Saturday);
            if (TheConfiguration.WeeklySunday)
                WeekList.Add(DayOfWeek.Sunday);

            return WeekList.ToArray();
        }

        public Output[] Execute(DateTime TheDate, Configuration TheConfiguration)
        {
            if (TheConfiguration.Enabled)
            {
                this.ValidateDatesConfiguration(TheConfiguration);

                DateTime TheDateAux = TheConfiguration.DateFrom != null &&
                    TheDate > TheConfiguration.DateFrom ? TheDate : TheConfiguration.DateFrom.Value;

                return this.ExecuteDateStep(TheDateAux, TheConfiguration);
            }
            else
            {
                return new Output[] {ReturnOuput(TheConfiguration, "",
                   TheDate, TheDate, TheConfiguration.DateFrom, null)};
            }
        }

        private void ValidateDatesConfiguration(Configuration TheConfiguration)
        {
            if (TheConfiguration.DateStep == null)
            {
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateDateConfiguration"));
            }
            if (TheConfiguration.DateFrom == null)
            {
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateDateConfiguration"));
            }
        }

        private Output ReturnOuput(Configuration TheConfiguration,
            string TheWeeStepStr, DateTime TheDateStep, DateTime TheHourStep, DateTime? TheDateFrom, DateTime? TheHour)
        {
            string TheDateStepStr = TheDateStep.ToString("d", this.culture);

            if (TheHourStep.TimeOfDay > new TimeSpan(00, 00, 00))
            {
                TheDateStepStr += " " +
                    this.GetTranslation(TheConfiguration, "at")
                    + " " + TheHourStep.ToShortTimeString();
            }

            Output TheExit = new Output();
            TheExit.OutputDate = TheDateStep;
            TheExit.Description =
                string.Format(
                this.GetTranslation(TheConfiguration, "Output"),
                TheWeeStepStr, TheDateStepStr) +
                (TheDateFrom != null ? " " + string.Format(
                    this.GetTranslation(TheConfiguration, "StartingOn")
                , TheDateFrom.Value.ToString("d", this.culture)) : "") +
                " " + (TheHour != null ? TheHour.Value.ToShortTimeString() : "00:00");

            return TheExit;
        }

        private Output[] ExecuteDateStep(DateTime TheDate, Configuration TheConfiguration)
        {
            string TheStepStr = this.GetStepDescription(TheConfiguration);

            if (TheConfiguration.TimeType == TypeStep.Once)
            {
                return ExecuteOnce(TheStepStr, TheConfiguration);
            }
            else
            {
                return ExecuteRecurring(TheDate, TheStepStr, TheConfiguration);
            }
        }

        private void ValidateRecurringConfiguration(Configuration TheConfiguration)
        {
            if (TheConfiguration.HourStep < 0)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateHourStep"));

            if (TheConfiguration.TypeRecurring == null)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateRecurringFrequency")); 

            if (TheConfiguration.MonthlyOnce == true && TheConfiguration.MonthlyOnceMonthSteps == null)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyConfiguration")); 

            if (TheConfiguration.MonthlyMore == true && TheConfiguration.MonthlyMoreMonthSteps == null)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyConfiguration")); 

            if (TheConfiguration.MonthlyMore == true && TheConfiguration.MonthlyMoreOrderDayWeekStep == null)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyMoreWeekStep")); 
        }

        private string GetStepDescription(Configuration TheConfiguration)
        {
            string TheStepStr = "";
            if (TheConfiguration.TimeType == TypeStep.Once)
            {
                TheStepStr = this.GetTranslation(TheConfiguration, "once");
            }
            else
            {
                this.ValidateRecurringConfiguration(TheConfiguration);

                TheStepStr = this.GetStepRecurringDescription(TheStepStr, TheConfiguration);
            }

            return TheStepStr;
        }

        private string GetTranslation(Configuration TheConfiguration, string Id)
        {
            string Text = "";
            if (TheConfiguration != null)
            {
                switch (TheConfiguration.Language)
                {
                    case Languages.en_GB:
                        Text = English.Translations[Id].ToString();
                        break;
                    case Languages.es_ES:
                        Text = Spanish.Translations[Id].ToString();
                        break;
                    case Languages.en_US:
                        Text = EnglishUS.Translations[Id].ToString();
                        break;
                }
            }

            return Text;
        }

        private string GetStepRecurringDescription(string TheStepStr, Configuration TheConfiguration)
        {
            if (TheConfiguration.TypeRecurring == TypeTimeStep.Daily)
            {
                TheStepStr = this.GetTranslation(TheConfiguration, "every")
                     + " " + (TheConfiguration.DailyStep == 1 ? "" + this.GetTranslation(TheConfiguration, "day") : TheConfiguration.DailyStep.ToString() + " " +
                     this.GetTranslation(TheConfiguration, "days"));
            }
            else if (TheConfiguration.TypeRecurring == TypeTimeStep.Weekly)
            {
                TheStepStr = this.GetStepRecurringWeeklyDescription(TheConfiguration);
            }
            else
            {
                string MonthStr = "";
               
                if (TheConfiguration.MonthlyOnce == true)
                {
                    if (TheConfiguration.MonthlyOnceMonthSteps > 1)
                    {
                        MonthStr = TheConfiguration.MonthlyOnceMonthSteps.Value.ToString() + " " +
                            this.GetTranslation(TheConfiguration, "months");
                    }
                    else
                    {
                        MonthStr = this.GetTranslation(TheConfiguration, "month");
                    }

                    TheStepStr = this.GetTranslation(TheConfiguration, "day") + " " + TheConfiguration.MonthlyOnceDay.Value.ToString() +
                        " " + this.GetTranslation(TheConfiguration, "of") + " " +
                        this.GetTranslation(TheConfiguration, "every") + " " + MonthStr;
                }
                else if (TheConfiguration.MonthlyMore == true)
                {
                    if (TheConfiguration.MonthlyMoreMonthSteps > 1)
                    {
                        MonthStr = TheConfiguration.MonthlyMoreMonthSteps.Value.ToString() + " " +
                            this.GetTranslation(TheConfiguration, "months");
                    }
                    else
                    {
                        MonthStr = this.GetTranslation(TheConfiguration, "month");
                    }


                    if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.WeekendDay)
                    {
                        TheStepStr = this.GetTranslation(TheConfiguration, "the") + " " + this.GetMonthlyOrderString(TheConfiguration) + " " +
                            this.GetDayStepDescription(TheConfiguration,
                               TheConfiguration.MonthlyMoreOrderDayWeekStep.Value) + " " +
                               this.GetTranslation(TheConfiguration, "of") +
                               " " + this.GetTranslation(TheConfiguration, "every") + " " + MonthStr;
                    }
                    else
                    {
                        TheStepStr = this.GetTranslation(TheConfiguration, "the")  
                            + " " + this.GetMonthlyOrderString(TheConfiguration) + " " +
                           this.GetDayStepDescription(TheConfiguration, 
                               TheConfiguration.MonthlyMoreOrderDayWeekStep.Value).ToLower() + " " + this.GetTranslation(TheConfiguration, "of") + " " +
                               this.GetTranslation(TheConfiguration, "every") + " " + MonthStr;
                    }
                }
            }

            return TheStepStr;
        }

        private string GetDayStepDescription(Configuration TheConfiguration, TypeDayWeekStep Step)
        {
            switch (Step)
            {
                case TypeDayWeekStep.Monday:
                    return this.GetTranslation(TheConfiguration, "Monday");
                case TypeDayWeekStep.Tuesday:
                    return this.GetTranslation(TheConfiguration, "Tuesday");
                case TypeDayWeekStep.Wednesday:
                    return this.GetTranslation(TheConfiguration, "Wednesday");
                case TypeDayWeekStep.Thursday:
                    return this.GetTranslation(TheConfiguration, "Thursday");
                case TypeDayWeekStep.Friday:
                    return this.GetTranslation(TheConfiguration, "Friday");
                case TypeDayWeekStep.Saturday:
                    return this.GetTranslation(TheConfiguration, "Saturday");
                case TypeDayWeekStep.Sunday:
                    return this.GetTranslation(TheConfiguration, "Sunday");
                case TypeDayWeekStep.Day:
                    return this.GetTranslation(TheConfiguration, "day");
                case TypeDayWeekStep.WeekDay:
                    return this.GetTranslation(TheConfiguration, "weekday");
                case TypeDayWeekStep.WeekendDay:
                    return this.GetTranslation(TheConfiguration, "weekend");
            }

            return string.Empty;
        }

        private string GetDayDescription(Configuration TheConfiguration, DayOfWeek Day)
        {
            switch(Day)
            {
                case DayOfWeek.Monday:
                    return this.GetTranslation(TheConfiguration, "Monday");
                case DayOfWeek.Tuesday:
                    return this.GetTranslation(TheConfiguration, "Tuesday");
                case DayOfWeek.Wednesday:
                    return this.GetTranslation(TheConfiguration, "Wednesday");
                case DayOfWeek.Thursday:
                    return this.GetTranslation(TheConfiguration, "Thursday");
                case DayOfWeek.Friday:
                    return this.GetTranslation(TheConfiguration, "Friday");
                case DayOfWeek.Saturday:
                    return this.GetTranslation(TheConfiguration, "Saturday");
                case DayOfWeek.Sunday:
                    return this.GetTranslation(TheConfiguration, "Sunday");
            }

            return string.Empty;
        }

        private string GetMonthlyOrderString(Configuration TheConfiguration)
        {
            switch(TheConfiguration.MonthlyMoreWeekStep)
            {
                case TypeWeekStep.First:
                    return this.GetTranslation(TheConfiguration, "First").ToLower();
                case TypeWeekStep.Second:
                    return this.GetTranslation(TheConfiguration, "Second").ToLower();
                case TypeWeekStep.Third:
                    return this.GetTranslation(TheConfiguration, "Third").ToLower();
                case TypeWeekStep.Fourth:
                    return this.GetTranslation(TheConfiguration, "Fourth").ToLower();
                case TypeWeekStep.Last:
                    return this.GetTranslation(TheConfiguration, "Last").ToLower();
            }

            return string.Empty;
        }

        private string GetStepRecurringWeeklyDescription(Configuration TheConfiguration)
        {
            string TheStepStr;
            String DaysString = "";

            DayOfWeek[] TheDays = this.PrepareWeeklyVar(TheConfiguration);

            for (int Index = 0; Index < TheDays.Length; Index++)
            {
                if (Index != TheDays.Length - 2 && Index != TheDays.Length - 1)
                    DaysString = DaysString + this.GetDayDescription(TheConfiguration,TheDays[Index]).ToLower() + ", ";
                else if (Index == TheDays.Length - 1)
                    DaysString = DaysString + this.GetDayDescription(TheConfiguration, TheDays[Index]).ToLower();
                else if (Index == TheDays.Length - 2)
                    DaysString = DaysString + this.GetDayDescription(TheConfiguration, TheDays[Index]).ToLower() + " " + 
                        this.GetTranslation(TheConfiguration, "and") + " ";
            }

            if (TheConfiguration.WeekStep > 1)
            {
                TheStepStr = this.GetTranslation(TheConfiguration, "every") + " " + TheConfiguration.WeekStep.ToString() + " " + 
                    this.GetTranslation(TheConfiguration, "weeks") + " " +
                    this.GetTranslation(TheConfiguration, "on") + " " + DaysString;
            }
            else
            {
                TheStepStr = this.GetTranslation(TheConfiguration, "every")
                    + " " + this.GetTranslation(TheConfiguration, "week") + " " +
                    this.GetTranslation(TheConfiguration, "on") + " " + DaysString;
            }

            return TheStepStr;
        }

        #region Once
        private Output[] ExecuteOnce(string TheTypeStr, Configuration TheConfiguration)
        {
            return new Output[]{this.ReturnOuput(TheConfiguration, TheTypeStr,
                        TheConfiguration.DateStep.Value,
                        TheConfiguration.DateStep.Value, TheConfiguration.DateFrom, null) };
        }
        #endregion

        #region Recurring

        private Output[] ExecuteRecurring(DateTime TheDate, string TheTypeStepStr, Configuration TheConfiguration)
        {
            List<Output> TheExistList = new List<Output>();

            if (TheConfiguration.TypeRecurring == TypeTimeStep.Daily)
            {
                this.ExecuteRecurringDaily(TheDate, TheTypeStepStr, TheExistList, TheConfiguration);
            }
            else if (TheConfiguration.TypeRecurring == TypeTimeStep.Weekly)
            {
                this.ExecuteRecurringWeekly(TheDate, TheTypeStepStr, TheExistList, TheConfiguration);
            }
            else if (TheConfiguration.TypeRecurring == TypeTimeStep.Monthly)
            {
                this.ValidateMonthlyConfiguration(TheConfiguration);

                this.ExecuteRecurringMonthly(TheDate, TheTypeStepStr, TheExistList, TheConfiguration);
            }

            return TheExistList.ToArray();
        }

        private void ValidateMonthlyConfiguration(Configuration TheConfiguration)
        {
            if (TheConfiguration.MonthlyOnce == null && TheConfiguration.MonthlyMore == null)
            {
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyConfiguration"));
            }
            if (TheConfiguration.MonthlyOnceMonthSteps <= 0)
            {
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyMonths"));
            }
            if (TheConfiguration.HourStep <= 0)
            {
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateHourStepOfDailyFrequency")); 
            }
            if (TheConfiguration.HourFrom > TheConfiguration.HourTo)
            {
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateHourFromBigggerHourTo")); 
            }
        }

        private void ExecuteRecurringMonthly(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList, Configuration TheConfiguration)
        {
            DateTime TheDateTo = TheConfiguration.DateTo != null ? TheConfiguration.DateTo.Value : DateTime.MaxValue;

            if (TheConfiguration.MonthlyOnce == true)
            {
                this.ExecuteRecurringMonthlyOnce(TheDate, TheTypeStepStr, TheExistList, TheDateTo,
                    TheConfiguration);
            }
            else
            {
                this.ExecuteRecurringMonthlyMore(TheDate, TheTypeStepStr, TheExistList, TheConfiguration);
            }
        }

        private void ExecuteRecurringMonthlyMore(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList, Configuration TheConfiguration)
        {
            this.ValidateMonthlyMoreRecurring(TheConfiguration);

            DateTime TheDateTo = TheConfiguration.DateTo != null ? TheConfiguration.DateTo.Value : DateTime.MaxValue;
                        
            int IndexDayWeek = 1;

            DateTime? TheDay = this.GetDayInMonth(TheDate, IndexDayWeek, TheConfiguration);

            TheDay = TheDay == null ? TheDate : TheDay; 

            for (DateTime EachWeekDate = TheDay.Value; EachWeekDate <= TheDateTo;
                EachWeekDate = this.GetNexDate(EachWeekDate, TheConfiguration))
            {
                TheExistList.AddRange(
                    this.ExecuteRecurringMonthlyWeekHours(TheTypeStepStr, TheDateTo, EachWeekDate,
                    TheConfiguration));
            }
        }

        private void ValidateMonthlyMoreRecurring(Configuration TheConfiguration)
        {
            if (TheConfiguration.MonthlyMoreWeekStep == null)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyMoreWeekStep"));

            if (TheConfiguration.HourFrom == null || TheConfiguration.HourTo == null)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyMoreHourFromTo"));
        }

        private Output[] ExecuteRecurringMonthlyWeekHours(string TheTypeStepStr, DateTime TheDateTo, 
            DateTime TheFirstDayWeek, Configuration TheConfiguration)
        {
            List<Output> TheList = new List<Output>();

            DateTime TheWeekDay = this.GetWeekDay(TheFirstDayWeek, TheConfiguration);

            for (int IndexWeek = 1; IndexWeek <= 5; IndexWeek++)
            {
                if (IndexWeek == Convert.ToInt32(TheConfiguration.MonthlyMoreWeekStep))
                {
                    if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Day ||
                        TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Monday ||
                        TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Tuesday ||
                        TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Wednesday ||
                        TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Thursday || TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Friday ||
                        TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Saturday ||
                        TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Sunday)
                    {
                        TheList.AddRange(this.ExecuteRecurringMonthlyWeekHoursPerDay(TheTypeStepStr, TheDateTo, TheFirstDayWeek, TheWeekDay, TheConfiguration));
                    }
                    else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.WeekDay)
                    {
                        TheList.AddRange(this.ExecuteRecurringMonthlyWeekHoursPerWeekDay(TheTypeStepStr, TheDateTo, TheFirstDayWeek, TheWeekDay, TheConfiguration));
                    }
                    else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.WeekendDay)
                    {
                        TheList.AddRange(this.ExecuteRecurringMonthlyWeekHoursPerWeekendDay(TheTypeStepStr, TheDateTo, TheWeekDay, TheConfiguration));
                    }
                    break;
                }
            }

            return TheList.ToArray();
        }

        private Output[] ExecuteRecurringMonthlyWeekHoursPerWeekendDay(string TheTypeStepStr, DateTime TheDateTo,  DateTime TheWeekDay, Configuration TheConfiguration)
        {
            List<Output> TheList = new List<Output>();

            DateTime TheDayfromWeek = this.GetWeekend(
                new DateTime(TheWeekDay.Year, TheWeekDay.Month, 1));

            if (TheConfiguration.DateFrom > TheDayfromWeek)
            {
                TheDayfromWeek = TheConfiguration.DateFrom.Value;
            }

            DateTime TheDayToWeek = this.GetEndWeek(TheWeekDay);

            for (DateTime EachDay = TheDayfromWeek; EachDay <= TheDayToWeek;
                EachDay = EachDay.AddDays(1))
            {
                TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay, TheConfiguration));
            }

            return TheList.ToArray();
        }

        private Output[] ExecuteRecurringMonthlyWeekHoursPerWeekDay(string TheTypeStepStr, DateTime TheDateTo, DateTime TheFirstDayWeek, DateTime TheWeekDay, Configuration TheConfiguration)
        {
            List<Output> TheList = new List<Output>();

            DateTime TheEndWeek = this.GetEndWeek(TheFirstDayWeek);

            for (DateTime EachDay = TheWeekDay;
                    EachDay <= TheEndWeek; EachDay =
                    EachDay.AddDays(1))
            {
                TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay,
                    TheConfiguration));
            }

            return TheList.ToArray();
        }

        private Output[] ExecuteRecurringMonthlyWeekHoursPerDay(string TheTypeStepStr, DateTime TheDateTo, DateTime TheFirstDayWeek, DateTime TheWeekDay, Configuration TheConfiguration)
        {
            List<Output> TheList = new List<Output>();

            for (DateTime EachDay = TheWeekDay;
                   EachDay <= TheWeekDay.AddDays(6); EachDay =
                   EachDay.AddDays(1))
            {
                if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Day &&
                    EachDay.Date == TheFirstDayWeek.Date)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay, TheConfiguration));
                    break;
                }
                else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Monday &&
                    EachDay.DayOfWeek == DayOfWeek.Monday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay,
                        TheConfiguration));
                    break;
                }
                else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Tuesday &&
                    EachDay.DayOfWeek == DayOfWeek.Tuesday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay,
                        TheConfiguration));
                    break;
                }
                else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Wednesday &&
                    EachDay.DayOfWeek == DayOfWeek.Wednesday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay,
                        TheConfiguration));
                    break;
                }
                else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Thursday &&
                    EachDay.DayOfWeek == DayOfWeek.Thursday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay, TheConfiguration));
                    break;
                }
                else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Friday &&
                    EachDay.DayOfWeek == DayOfWeek.Friday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay,
                        TheConfiguration));
                    break;
                }
                else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Saturday &&
                    EachDay.DayOfWeek == DayOfWeek.Saturday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay,
                        TheConfiguration));
                    break;
                }
                else if (TheConfiguration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Sunday &&
                    EachDay.DayOfWeek == DayOfWeek.Sunday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay,
                        TheConfiguration));
                    break;
                }
            }

            return TheList.ToArray();
        }

        private DateTime GetWeekDay(DateTime TheFirstDayWeek, Configuration TheConfiguration)
        {
            switch (TheConfiguration.MonthlyMoreWeekStep)
            {
                case TypeWeekStep.First:
                    return TheFirstDayWeek;
                case TypeWeekStep.Second:
                    return this.GetFirstDayWeek(
                        (new DateTime(TheFirstDayWeek.Year, TheFirstDayWeek.Month, 1)).AddDays(7));
                case TypeWeekStep.Third:
                    return this.GetFirstDayWeek(
                        (new DateTime(TheFirstDayWeek.Year, TheFirstDayWeek.Month, 1)).AddDays(14));
                case TypeWeekStep.Fourth:
                    return this.GetFirstDayWeek(
                        (new DateTime(TheFirstDayWeek.Year, TheFirstDayWeek.Month, 1)).AddDays(21));
                case TypeWeekStep.Last:
                    return this.GetFirstDayWeek(
                        (new DateTime(TheFirstDayWeek.Year, TheFirstDayWeek.Month, 1)).AddDays(28));
            }

            return TheFirstDayWeek;
        }

        private DateTime GetWeekend(DateTime TheDay)
        {
            switch (TheDay.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return TheDay.AddDays(5);
                case DayOfWeek.Tuesday:
                    return TheDay.AddDays(4);
                case DayOfWeek.Wednesday:
                    return TheDay.AddDays(3);
                case DayOfWeek.Thursday:
                    return TheDay.AddDays(2);
                case DayOfWeek.Friday:
                    return TheDay.AddDays(1);
                case DayOfWeek.Saturday:
                    return TheDay.AddDays(0);
                case DayOfWeek.Sunday:
                    return TheDay.AddDays(-1);
            }

            return TheDay;
        }

        private DateTime GetEndWeek(DateTime TheDay)
        {
            switch (TheDay.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return TheDay.AddDays(6);
                case DayOfWeek.Tuesday:
                    return TheDay.AddDays(5);
                case DayOfWeek.Wednesday:
                    return TheDay.AddDays(4);
                case DayOfWeek.Thursday:
                    return TheDay.AddDays(3);
                case DayOfWeek.Friday:
                    return TheDay.AddDays(2);
                case DayOfWeek.Saturday:
                    return TheDay.AddDays(1);
                case DayOfWeek.Sunday:
                    return TheDay;
            }

            return TheDay;
        }

        private DateTime? GetDayInMonth(DateTime TheDay, int IndexDayWeek, Configuration TheConfiguration)
        {
            DateTime TheFirstDay = new DateTime(TheDay.Year, TheDay.Month, 1);
            DateTime? TheAuxDay = null;

            for (int IndexWeek = 1; IndexWeek < 5; IndexWeek++)
            {
                if (IndexWeek == Convert.ToInt32(TheConfiguration.MonthlyMoreWeekStep))
                {
                    TheFirstDay = this.GetFirstDayWeek(TheFirstDay);

                    for (DateTime IndexDay = TheFirstDay; IndexDay <= TheFirstDay.AddDays(6);
                  IndexDay = IndexDay.AddDays(1))
                    {
                        if (IndexDay.Month != TheFirstDay.Month)
                        {
                            continue;
                        }

                        TheAuxDay = this.GetDayWeek(TheFirstDay, TheConfiguration);

                        if (TheAuxDay != null)
                        {
                            TheAuxDay = IndexDay;
                            break;
                        }
                        IndexDayWeek++;
                    }
                    if (TheAuxDay != null)
                    {
                        break;
                    }
                }
            }
            if (TheAuxDay == null)
            {
                TheFirstDay = this.GetFirstDayWeek(
                    TheFirstDay.AddDays(DateTime.DaysInMonth(TheFirstDay.Year, TheFirstDay.Month)));

                TheAuxDay = this.GetDayWeek(TheFirstDay, TheConfiguration);
            }

            return TheDay;
        }

        private void ExecuteRecurringMonthlyOnce(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList, DateTime TheDateTo, Configuration TheConfiguration)
        {
            this.ValidateMonthlyOnceRecurring(TheConfiguration);

            DateTime TheNewDate;
            if (TheDate.Day > TheConfiguration.MonthlyOnceDay)
            {
                TheNewDate = new DateTime(TheDate.Year, TheDate.AddMonths(
                    TheConfiguration.MonthlyOnceMonthSteps.Value).Month, TheConfiguration.MonthlyOnceDay.Value, TheDate.Hour, TheDate.Minute, TheDate.Second);
            }
            else
            {
                TheNewDate = new DateTime(TheDate.Year, TheDate.Month, TheConfiguration.MonthlyOnceDay.Value, TheDate.Hour, TheDate.Minute, TheDate.Second);
            }

            for (DateTime EachDate = TheNewDate; EachDate <= TheDateTo;
                EachDate = this.GetNexDate(EachDate, TheConfiguration))
            {
                if (TheConfiguration.MonthlyOnceDay.Value == EachDate.Day)
                {
                    TheExistList.AddRange(
                        this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDate, TheConfiguration));
                }
            }
        }

        private void ValidateMonthlyOnceRecurring(Configuration TheConfiguration)
        {
            if (TheConfiguration.MonthlyOnceDay <= 0)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyOnceDayFrequency"));

            if (TheConfiguration.MonthlyOnceMonthSteps == null || TheConfiguration.MonthlyOnceMonthSteps <= 0)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateMonthlyOnceMonthFrequency"));
        }

        private void ExecuteRecurringWeekly(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList, Configuration TheConfiguration)
        {
            this.ValidateWeeklyRecurring(TheConfiguration);

            DateTime TheDateTo = TheConfiguration.DateTo != null ? TheConfiguration.DateTo.Value : DateTime.MaxValue;
            int WeekStep = TheConfiguration.WeekStep > 0 ? TheConfiguration.WeekStep : 0;

            for (DateTime EachWeekDate = TheDate; EachWeekDate <= TheDateTo;
                EachWeekDate = this.GetNexDate(EachWeekDate, TheConfiguration))
            {
                TheExistList.AddRange(
                    this.ExecuteWeekly(TheTypeStepStr, TheDateTo, EachWeekDate, TheConfiguration));
            }
        }

        private void ValidateWeeklyRecurring(Configuration TheConfiguration)
        {
            if (TheConfiguration.WeekStep <= 0)
                throw new ScheduleException(
                    this.GetTranslation(TheConfiguration, "ValidateWeeklyStep"));
        }

        private void ExecuteRecurringDaily(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList, Configuration TheConfiguration)
        {
            for (DateTime EachDate = TheDate.AddDays(1); EachDate.Date <= TheConfiguration.DateTo; EachDate = EachDate.AddDays(TheConfiguration.DailyStep))
            {
                TheExistList.Add(this.ReturnOuput(TheConfiguration, TheTypeStepStr, EachDate, EachDate, TheConfiguration.DateFrom, TheConfiguration.DateStep.Value));
            }
        }

        private Output[] ExecuteWeekly(string TheTypeStr,
           DateTime TheDateTo, DateTime EachDateWeek, Configuration TheConfiguration)
        {
            List<Output> TheExits = new List<Output>();

            DayOfWeek[] TheDays = this.PrepareWeeklyVar(TheConfiguration);

            for (DateTime EachDate = EachDateWeek; EachDate <= TheDateTo;
                EachDate = EachDate.AddDays(1))
            {
                if (TheDays.Any(W => W.Equals(EachDate.DayOfWeek)))
                {
                    TheExits.AddRange(this.ExecuteHours(TheTypeStr, EachDate, TheConfiguration));
                }

                if (EachDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    break;
                }
            }

            return TheExits.ToArray();
        }

        private DateTime GetFirstDayWeek(DateTime TheDate)
        {
            if (TheDate.DayOfWeek == DayOfWeek.Monday)
                return TheDate;
            if (TheDate.DayOfWeek == DayOfWeek.Tuesday)
                return TheDate.AddDays(-1);
            if (TheDate.DayOfWeek == DayOfWeek.Wednesday)
                return TheDate.AddDays(-2);
            if (TheDate.DayOfWeek == DayOfWeek.Thursday)
                return TheDate.AddDays(-3);
            if (TheDate.DayOfWeek == DayOfWeek.Friday)
                return TheDate.AddDays(-4);
            if (TheDate.DayOfWeek == DayOfWeek.Saturday)
                return TheDate.AddDays(-5);
            if (TheDate.DayOfWeek == DayOfWeek.Sunday)
                return TheDate.AddDays(-6);

            return TheDate;
        }

        private DateTime? GetDayWeek(DateTime TheFirstDayWeek, Configuration TheConfiguration)
        {
            switch (TheConfiguration.MonthlyMoreOrderDayWeekStep)
            {
                case TypeDayWeekStep.Day:
                case TypeDayWeekStep.Monday:
                    return TheFirstDayWeek;
                case TypeDayWeekStep.Tuesday:
                    return TheFirstDayWeek.AddDays(1);
                case TypeDayWeekStep.Wednesday:
                    return TheFirstDayWeek.AddDays(2);
                case TypeDayWeekStep.Thursday:
                    return TheFirstDayWeek.AddDays(3);
                case TypeDayWeekStep.Friday:
                    return TheFirstDayWeek.AddDays(4);
                case TypeDayWeekStep.Saturday:
                    return TheFirstDayWeek.AddDays(5);
                case TypeDayWeekStep.Sunday:
                    return TheFirstDayWeek.AddDays(6);
            }

            return null;
        }

        private DateTime GetNexDate(DateTime TheDate, Configuration TheConfiguration)
        {
            if (TheConfiguration.TypeRecurring == TypeTimeStep.Weekly)
            {
                if (TheConfiguration.WeekStep > 0)
                {
                    switch (TheDate.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            TheDate = TheDate.AddDays(TheConfiguration.WeekStep * 7);
                            break;
                        case DayOfWeek.Tuesday:
                            TheDate = TheDate.AddDays(-1 + TheConfiguration.WeekStep * 7);
                            break;
                        case DayOfWeek.Wednesday:
                            TheDate = TheDate.AddDays(-2 + TheConfiguration.WeekStep * 7);
                            break;
                        case DayOfWeek.Thursday:
                            TheDate = TheDate.AddDays(-3 + TheConfiguration.WeekStep * 7);
                            break;
                        case DayOfWeek.Friday:
                            TheDate = TheDate.AddDays(-4 + TheConfiguration.WeekStep * 7);
                            break;
                        case DayOfWeek.Saturday:
                            TheDate = TheDate.AddDays(-5 + TheConfiguration.WeekStep * 7);
                            break;
                        case DayOfWeek.Sunday:
                            TheDate = TheDate.AddDays(-6 + TheConfiguration.WeekStep * 7);
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (TheConfiguration.MonthlyOnce == true)
            {
                int CurrentDay = TheConfiguration.MonthlyOnceDay.Value;

                TheDate = TheDate.AddMonths(TheConfiguration.MonthlyOnceMonthSteps.Value);

                while (TheConfiguration.MonthlyOnceDay.Value == 31 && 
                    DateTime.DaysInMonth(TheDate.Year, TheDate.Month) != 31)
                {
                    TheDate = TheDate.AddMonths(TheConfiguration.MonthlyOnceMonthSteps.Value);
                }

                TheDate = new DateTime(TheDate.Year, TheDate.Month, CurrentDay);
            }
            else if (TheConfiguration.MonthlyMore == true)
            {
                TheDate = TheDate.AddMonths(TheConfiguration.MonthlyMoreMonthSteps.Value);
            }

            return TheDate;
        }

        private Output ReturnExitRecurringMonthtlyWeekly(string TheTypeStepStr, DateTime TheDateStep, DateTime? TheDateFrom, DateTime? TheDate, Configuration TheConfiguration)
        {
            string HourDayStr = this.GetHourDayString(TheConfiguration).ToLower();

            string TheHourStepStr = "";

            if (TheConfiguration.HourStep.Value > 1)
            {
                TheHourStepStr = 
                    this.GetTranslation(TheConfiguration, "every") + " " + 
                    TheConfiguration.HourStep.ToString() + " " +
                    this.GetTranslation(TheConfiguration, "hours");
            }
            else
            {
                TheHourStepStr = this.GetTranslation(TheConfiguration, "every")
                    + " " + this.GetTranslation(TheConfiguration, "hour");
            }

            if (TheDate != null)
            {
                TheDateStep = TheDateStep.AddHours(TheDate.Value.Hour).AddMinutes(TheDate.Value.Minute);
            }

            Output TheExit = new Output();
            if (TheConfiguration.TypeRecurring == TypeTimeStep.Weekly)
            {
                TheExit.OutputDate = TheDateStep;
                TheExit.Description =
                    string.Format(
                    this.GetTranslation(TheConfiguration, "ExitRecurring"),
                    TheTypeStepStr, TheHourStepStr, HourDayStr,
                    TheDateFrom != null ? TheDateFrom.Value.ToString("d", this.culture) : "");
            }
            else
            {
                if (TheConfiguration.MonthlyOnce == true)
                {
                    TheExit.OutputDate = TheDateStep;
                    TheExit.Description =
                        string.Format(
                        this.GetTranslation(TheConfiguration, "ExitRecurring"), 
                        TheTypeStepStr, TheHourStepStr, HourDayStr,
                        TheDateFrom != null ? TheDateFrom.Value.ToString("d", this.culture) : "");
                }
                else if (TheConfiguration.MonthlyMore == true)
                {
                    TheExit.OutputDate = TheDateStep;
                    TheExit.Description =
                        string.Format(
                        this.GetTranslation(TheConfiguration, "ExitRecurring"), TheTypeStepStr, TheHourStepStr, HourDayStr,
                        TheDateFrom != null ? TheDateFrom.Value.ToString("d", this.culture) : "");
                }
            }

            return TheExit;
        }

        private string GetHourDayString(Configuration TheConfiguration)
        {
            return (TheConfiguration.HourFrom != null ? TheConfiguration.HourFrom.Value.ToString("t", this.culture) :
                            new DateTime(1900, 1, 1, 0, 0, 0).ToString("t", this.culture)) + " " + 
                            this.GetTranslation(TheConfiguration, "and") + " " +
                            (TheConfiguration.HourTo != null ? TheConfiguration.HourTo.Value.ToString("t", this.culture) :
                            new DateTime(1900, 1, 1, 23, 59, 0).ToString("t", this.culture));
        }

        private Output[] ExecuteMonthlyWeeklyHours(string TheTypeStepStr,
            DateTime TheDateTo, DateTime EachDateMonthly, Configuration TheConfiguration)
        {
            List<Output> TheExists = new List<Output>();

            TheExists.AddRange(this.ExecuteHours(TheTypeStepStr, EachDateMonthly, TheConfiguration));

            return TheExists.ToArray();
        }

        private Output[] ExecuteHours(string TheTypeStr, DateTime TheDate, Configuration TheConfiguration)
        {
            List<Output> TheList = new List<Output>();

            DateTime TheHourFrom = this.ReturnHourFrom(TheDate, TheConfiguration);

            DateTime TheHourTo = this.ReturnHourTo(TheDate, TheConfiguration);

            int TheHourStep = TheConfiguration.HourStep != null ?
                TheConfiguration.HourStep.Value : 1;

            for (DateTime TheHour = TheHourFrom; TheHour <= TheHourTo;
                TheHour = TheHour.AddHours(TheHourStep))
            {
                TheList.Add(this.ReturnExitRecurringMonthtlyWeekly(TheTypeStr, TheDate, 
                    TheConfiguration.DateFrom, TheHour, TheConfiguration));
            }

            return TheList.ToArray();
        }

        private DateTime ReturnHourTo(DateTime TheDate, Configuration TheConfiguration)
        {
            DateTime TheDateTo = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, 23, 59, 59);

            if (TheConfiguration.HourTo != null)
            {
                TheDateTo = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, TheConfiguration.HourTo.Value.Hour, TheConfiguration.HourTo.Value.Minute,
                    TheConfiguration.HourTo.Value.Second);
            }

            return TheDateTo;
        }

        private DateTime ReturnHourFrom(DateTime TheDate, Configuration TheConfiguration)
        {
            DateTime TheHourFrom = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, 0, 0, 0);

            if (TheConfiguration.HourFrom != null)
            {
                TheHourFrom = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, TheConfiguration.HourFrom.Value.Hour, TheConfiguration.HourFrom.Value.Minute,
                    TheConfiguration.HourFrom.Value.Second);
            }

            return TheHourFrom;
        }

        #endregion
    }
}
