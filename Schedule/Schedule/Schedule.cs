using Schedule.Config;
using Schedule.RecursosTextos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Schedule.Process
{
    public class Schedule
    {
        private Configuration configuration;
        private DayOfWeek[] weeklyVar;
        private List<DayOfWeek> monthlyWeekVar;

        public Schedule(Configuration TheConfiguration)
        {
            this.ValidateConfiguration(TheConfiguration);

            this.configuration = TheConfiguration;
            this.monthlyWeekVar = new List<DayOfWeek>();
            if (this.configuration.TypeRecurring == TypeTimeStep.Weekly)
            {
                this.PrepareWeeklyVar();
            }
            else if (this.configuration.TypeRecurring == TypeTimeStep.Monthly)
            {
                this.PrepareMonthlyWeekVar();
            }
        }

        public List<DayOfWeek> MonthlyWeekVar
        {
            get
            {
                return this.monthlyWeekVar;
            }
        }

        private void ValidateConfiguration(Configuration TheConfiguration)
        {
            if (TheConfiguration == null)
                throw new ScheduleException(Global.ValidateConfiguration);
        }

        private void PrepareWeeklyVar()
        {
            List<DayOfWeek> WeekList = new List<DayOfWeek>();

            if (this.configuration.WeeklyMonday)
                WeekList.Add(DayOfWeek.Monday);
            if (this.configuration.WeeklyTuesday)
                WeekList.Add(DayOfWeek.Tuesday);
            if (this.configuration.WeeklyWednesday)
                WeekList.Add(DayOfWeek.Wednesday);
            if (this.configuration.WeeklyThursday)
                WeekList.Add(DayOfWeek.Thursday);
            if (this.configuration.WeeklyFriday)
                WeekList.Add(DayOfWeek.Friday);
            if (this.configuration.WeeklySaturday)
                WeekList.Add(DayOfWeek.Saturday);
            if (this.configuration.WeeklySunday)
                WeekList.Add(DayOfWeek.Sunday);

            this.weeklyVar = WeekList.ToArray();
        }

        private void PrepareMonthlyWeekVar()
        {
            if (this.monthlyWeekVar != null)
            {
                switch (this.configuration.MonthlyMoreOrderDayWeekStep)
                {
                    case TypeDayWeekStep.Day:
                        this.monthlyWeekVar.AddRange(new DayOfWeek[] { DayOfWeek.Monday,
                    DayOfWeek.Tuesday, DayOfWeek.Wednesday,DayOfWeek.Thursday,
                    DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday});
                        break;
                    case TypeDayWeekStep.Monday:
                        this.monthlyWeekVar.Add(DayOfWeek.Monday);
                        break;
                    case TypeDayWeekStep.Tuesday:
                        this.monthlyWeekVar.Add(DayOfWeek.Tuesday);
                        break;
                    case TypeDayWeekStep.Wednesday:
                        this.monthlyWeekVar.Add(DayOfWeek.Wednesday);
                        break;
                    case TypeDayWeekStep.Thursday:
                        this.monthlyWeekVar.Add(DayOfWeek.Thursday);
                        break;
                    case TypeDayWeekStep.Friday:
                        this.monthlyWeekVar.Add(DayOfWeek.Friday);
                        break;
                    case TypeDayWeekStep.Saturday:
                        this.monthlyWeekVar.Add(DayOfWeek.Saturday);
                        break;
                    case TypeDayWeekStep.Sunday:
                        this.monthlyWeekVar.Add(DayOfWeek.Sunday);
                        break;
                    case TypeDayWeekStep.WeekDay:
                        this.monthlyWeekVar.AddRange(new DayOfWeek[] { DayOfWeek.Monday,
                    DayOfWeek.Tuesday, DayOfWeek.Wednesday,DayOfWeek.Thursday,
                    DayOfWeek.Friday });
                        break;
                    case TypeDayWeekStep.WeekendDay:
                        this.monthlyWeekVar.AddRange(new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday });
                        break;
                }
            }
        }

        public Output[] Execute(DateTime TheDate)
        {
            if (this.configuration.Enabled)
            {
                this.ValidateDatesConfiguration();

                DateTime TheDateAux = this.configuration.DateFrom != null &&
                    TheDate > this.configuration.DateFrom ? TheDate : this.configuration.DateFrom.Value;

                return this.ExecuteDateStep(TheDateAux);
            }
            else
            {
                return new Output[] {ReturnOuput("",
                   TheDate, TheDate, this.configuration.DateFrom, null)};
            }
        }

        private void ValidateDatesConfiguration()
        {
            if (this.configuration.DateStep == null)
            {
                throw new ScheduleException(Global.ValidateDateConfiguration);
            }
            if (this.configuration.DateFrom == null)
            {
                throw new ScheduleException(Global.ValidateDateConfiguration);
            }
        }

        private Output ReturnOuput(string TheWeeStepStr, DateTime TheDateStep, DateTime TheHourStep, DateTime? TheDateFrom, DateTime? TheHour)
        {
            string TheDateStepStr = TheDateStep.ToShortDateString();

            if (TheHourStep.TimeOfDay > new TimeSpan(00, 00, 00))
            {
                TheDateStepStr += " " + Global.at + " " + TheHourStep.ToShortTimeString();
            }

            Output TheExit = new Output();
            TheExit.OutputDate = TheDateStep;
            TheExit.Description =
                string.Format(Global.Output, TheWeeStepStr, TheDateStepStr) +
                (TheDateFrom != null ? " " + string.Format(Global.StartingOn
                , TheDateFrom.Value.ToShortDateString()) : "") +
                " " + (TheHour != null ? TheHour.Value.ToShortTimeString() : "00:00");

            return TheExit;
        }

        private Output[] ExecuteDateStep(DateTime TheDate)
        {
            string TheStepStr = this.GetStepDescription();

            if (this.configuration.TimeType == TypeStep.Once)
            {
                return ExecuteOnce(TheStepStr);
            }
            else
            {
                return ExecuteRecurring(TheDate, TheStepStr);
            }
        }

        private void ValidateRecurringConfiguration()
        {
            if (this.configuration.HourStep < 0)
                throw new ScheduleException(Global.ValidateHourStep);

            if (this.configuration.TypeRecurring == null)
                throw new ScheduleException(Global.ValidateRecurringFrequency);

            if (this.configuration.MonthlyOnce == true && this.configuration.MonthlyOnceMonthSteps == null)
                throw new ScheduleException(Global.ValidateMonthlyConfiguration);

            if (this.configuration.MonthlyMore == true && this.configuration.MonthlyMoreMonthSteps == null)
                throw new ScheduleException(Global.ValidateMonthlyConfiguration);
        }

        private string GetStepDescription()
        {
            string TheStepStr = "";
            if (this.configuration.TimeType == TypeStep.Once)
            {
                TheStepStr = Global.once;
            }
            else
            {
                this.ValidateRecurringConfiguration();

                TheStepStr = this.GetStepRecurringDescription(TheStepStr);
            }

            return TheStepStr;
        }

        private string GetStepRecurringDescription(string TheStepStr)
        {
            if (this.configuration.TypeRecurring == TypeTimeStep.Daily)
            {
                TheStepStr = Global.every + " " + (this.configuration.DailyStep == 1 ? "" + Global.day : this.configuration.DailyStep.ToString() + " " + Global.days);
            }
            else if (this.configuration.TypeRecurring == TypeTimeStep.Weekly)
            {
                TheStepStr = this.GetStepRecurringWeeklyDescription();
            }
            else
            {
                if (this.configuration.MonthlyOnce == true)
                {
                    TheStepStr = Global.day + " " + 
                        this.configuration.MonthlyOnceDay.Value.ToString() +
                        " " + Global.of + " " + Global.every + " " +
                        this.configuration.MonthlyOnceMonthSteps.Value.ToString() + " " + Global.months;
                }
                else if (this.configuration.MonthlyMore == true)
                {
                    TheStepStr = Global.the + " " +
                        this.configuration.MonthlyMoreWeekStep.ToString().ToLower() + " " +
                        this.configuration.MonthlyMoreOrderDayWeekStep.ToString().ToLower() + " " + Global.of + " " + Global.every + " " + 
                        this.configuration.MonthlyMoreMonthSteps + " " + Global.months;
                }
            }

            return TheStepStr;
        }

        private string GetStepRecurringWeeklyDescription()
        {
            string TheStepStr;
            String DaysString = "";

            for (int Index = 0; Index < this.weeklyVar.Length; Index++)
            {
                if (Index != this.weeklyVar.Length - 2 && Index != this.weeklyVar.Length - 1)
                    DaysString = DaysString + this.weeklyVar[Index].ToString() + ", ";
                else if (Index == this.weeklyVar.Length - 1)
                    DaysString = DaysString + this.weeklyVar[Index].ToString();
                else if (Index == this.weeklyVar.Length - 2)
                    DaysString = DaysString + this.weeklyVar[Index].ToString() + " and ";
            }

            TheStepStr = Global.every + " " + this.configuration.WeekStep + " " + Global.weeks +
                " " + Global.on + " " + DaysString;

            return TheStepStr;
        }

        #region Once
        private Output[] ExecuteOnce(string TheTypeStr)
        {
            return new Output[]{ReturnOuput(TheTypeStr,
                        this.configuration.DateStep.Value,
                        this.configuration.DateStep.Value, this.configuration.DateFrom, null) };
        }
        #endregion

        #region Recurring

        private Output[] ExecuteRecurring(DateTime TheDate, string TheTypeStepStr)
        {
            List<Output> TheExistList = new List<Output>();

            if (this.configuration.TypeRecurring == TypeTimeStep.Daily)
            {
                this.ExecuteRecurringDaily(TheDate, TheTypeStepStr, TheExistList);
            }
            else if (this.configuration.TypeRecurring == TypeTimeStep.Weekly)
            {
                this.ExecuteRecurringWeekly(TheDate, TheTypeStepStr, TheExistList);
            }
            else if (this.configuration.TypeRecurring == TypeTimeStep.Monthly)
            {
                this.ValidateMonthlyConfiguration();

                this.ExecuteRecurringMonthly(TheDate, TheTypeStepStr, TheExistList);
            }

            return TheExistList.ToArray();
        }

        private void ValidateMonthlyConfiguration()
        {
            if (this.configuration.MonthlyOnce == null && this.configuration.MonthlyMore == null)
            {
                throw new ScheduleException(Global.ValidateMonthlyConfiguration);
            }
            if (this.configuration.MonthlyOnceMonthSteps <= 0)
            {
                throw new ScheduleException(Global.ValidateMonthlyMonths);
            }
            if (this.configuration.HourStep <= 0)
            {
                throw new ScheduleException(Global.ValidateHourStepOfDailyFrequency);
            }
            if (this.configuration.HourFrom > this.configuration.HourTo)
            {
                throw new ScheduleException(Global.ValidateHourFromBigggerHourTo);
            }
        }

        private void ExecuteRecurringMonthly(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList)
        {
            DateTime TheDateTo = this.configuration.DateTo != null ? this.configuration.DateTo.Value : DateTime.MaxValue;

            if (this.configuration.MonthlyOnce == true)
            {
                this.ExecuteRecurringMonthlyOnce(TheDate, TheTypeStepStr, TheExistList, TheDateTo);
            }
            else
            {
                this.ExecuteRecurringMonthlyMore(TheDate, TheTypeStepStr, TheExistList);
            }
        }

        private void ExecuteRecurringMonthlyMore(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList)
        {
            this.ValidateMonthlyMoreRecurring();

            DateTime TheDateTo = this.configuration.DateTo != null ? this.configuration.DateTo.Value : DateTime.MaxValue;
                        
            int IndexDayWeek = 1;

            DateTime? TheDay = this.GetDayInMonth(TheDate, IndexDayWeek);

            TheDay = TheDay == null ? TheDate : TheDay; 

            for (DateTime EachWeekDate = TheDay.Value; EachWeekDate <= TheDateTo;
                EachWeekDate = this.GetNexDate(EachWeekDate))
            {
                TheExistList.AddRange(
                    this.ExecuteRecurringMonthlyWeekHours(TheTypeStepStr, TheDateTo, EachWeekDate));
            }
        }

        private void ValidateMonthlyMoreRecurring()
        {
            if (this.configuration.MonthlyMoreWeekStep == null)
                throw new ScheduleException(Global.ValidateMonthlyMoreWeekStep);

            if (this.configuration.HourFrom == null || this.configuration.HourTo == null)
                throw new ScheduleException(Global.ValidateMonthlyMoreHourFromTo);
        }

        private Output[] ExecuteRecurringMonthlyWeekHours(string TheTypeStepStr, DateTime TheDateTo, 
            DateTime TheFirstDayWeek)
        {
            List<Output> TheList = new List<Output>();

            DateTime TheWeekDay = this.GetWeekDay(TheFirstDayWeek);

            for (int IndexWeek = 1; IndexWeek <= 5; IndexWeek++)
            {
                if (IndexWeek == Convert.ToInt32(this.configuration.MonthlyMoreWeekStep))
                {
                    if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Day ||
                        this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Monday ||
                        this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Tuesday ||
                        this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Wednesday ||
                        this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Thursday || this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Friday ||
                        this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Saturday ||
                        this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Sunday)
                    {
                        TheList.AddRange(this.ExecuteRecurringMonthlyWeekHoursPerDay(TheTypeStepStr, TheDateTo, TheFirstDayWeek, TheWeekDay));
                    }
                    else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.WeekDay)
                    {
                        TheList.AddRange(this.ExecuteRecurringMonthlyWeekHoursPerWeekDay(TheTypeStepStr, TheDateTo, TheFirstDayWeek, TheWeekDay));
                    }
                    else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.WeekendDay)
                    {
                        TheList.AddRange(this.ExecuteRecurringMonthlyWeekHoursPerWeekendDay(TheTypeStepStr, TheDateTo, TheWeekDay));
                    }
                    break;
                }
            }

            return TheList.ToArray();
        }

        private Output[] ExecuteRecurringMonthlyWeekHoursPerWeekendDay(string TheTypeStepStr, DateTime TheDateTo,  DateTime TheWeekDay)
        {
            List<Output> TheList = new List<Output>();

            DateTime TheDayfromWeek = this.GetWeekend(
                new DateTime(TheWeekDay.Year, TheWeekDay.Month, 1));

            if (this.configuration.DateFrom > TheDayfromWeek)
            {
                TheDayfromWeek = this.configuration.DateFrom.Value;
            }

            DateTime TheDayToWeek = this.GetEndWeek(TheWeekDay);

            for (DateTime EachDay = TheDayfromWeek; EachDay <= TheDayToWeek;
                EachDay = EachDay.AddDays(1))
            {
                TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
            }

            return TheList.ToArray();
        }

        private Output[] ExecuteRecurringMonthlyWeekHoursPerWeekDay(string TheTypeStepStr, DateTime TheDateTo, DateTime TheFirstDayWeek, DateTime TheWeekDay)
        {
            List<Output> TheList = new List<Output>();

            DateTime TheEndWeek = this.GetEndWeek(TheFirstDayWeek);

            for (DateTime EachDay = TheWeekDay;
                    EachDay <= TheEndWeek; EachDay =
                    EachDay.AddDays(1))
            {
                TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
            }

            return TheList.ToArray();
        }

        private Output[] ExecuteRecurringMonthlyWeekHoursPerDay(string TheTypeStepStr, DateTime TheDateTo, DateTime TheFirstDayWeek, DateTime TheWeekDay)
        {
            List<Output> TheList = new List<Output>();

            for (DateTime EachDay = TheWeekDay;
                   EachDay <= TheWeekDay.AddDays(6); EachDay =
                   EachDay.AddDays(1))
            {
                if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Day &&
                    EachDay.Date == TheFirstDayWeek.Date)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
                    break;
                }
                else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Monday &&
                    EachDay.DayOfWeek == DayOfWeek.Monday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
                    break;
                }
                else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Tuesday &&
                    EachDay.DayOfWeek == DayOfWeek.Tuesday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
                    break;
                }
                else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Wednesday &&
                    EachDay.DayOfWeek == DayOfWeek.Wednesday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
                    break;
                }
                else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Thursday &&
                    EachDay.DayOfWeek == DayOfWeek.Thursday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
                    break;
                }
                else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Friday &&
                    EachDay.DayOfWeek == DayOfWeek.Friday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
                    break;
                }
                else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Saturday &&
                    EachDay.DayOfWeek == DayOfWeek.Saturday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
                    break;
                }
                else if (this.configuration.MonthlyMoreOrderDayWeekStep == TypeDayWeekStep.Sunday &&
                    EachDay.DayOfWeek == DayOfWeek.Sunday)
                {
                    TheList.AddRange(this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDay));
                    break;
                }
            }

            return TheList.ToArray();
        }

        private DateTime GetWeekDay(DateTime TheFirstDayWeek)
        {
            switch (this.configuration.MonthlyMoreWeekStep)
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

        private DateTime? GetDayInMonth(DateTime TheDay, int IndexDayWeek)
        {
            DateTime TheFirstDay = new DateTime(TheDay.Year, TheDay.Month, 1);
            DateTime? TheAuxDay = null;

            for (int IndexWeek = 1; IndexWeek < 5; IndexWeek++)
            {
                if (IndexWeek == Convert.ToInt32(this.configuration.MonthlyMoreWeekStep))
                {
                    TheFirstDay = this.GetFirstDayWeek(TheFirstDay);

                    for (DateTime IndexDay = TheFirstDay; IndexDay <= TheFirstDay.AddDays(6);
                  IndexDay = IndexDay.AddDays(1))
                    {
                        if (IndexDay.Month != TheFirstDay.Month)
                        {
                            continue;
                        }

                        TheAuxDay = this.GetDayWeek(TheFirstDay);

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

                TheAuxDay = this.GetDayWeek(TheFirstDay);
            }

            return TheDay;
        }

        private void ExecuteRecurringMonthlyOnce(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList, DateTime TheDateTo)
        {
            this.ValidateMonthlyOnceRecurring();

            DateTime TheNewDate;
            if (TheDate.Day > this.configuration.MonthlyOnceDay)
            {
                TheNewDate = new DateTime(TheDate.Year, TheDate.AddMonths(
                    this.configuration.MonthlyOnceMonthSteps.Value).Month, this.configuration.MonthlyOnceDay.Value, TheDate.Hour, TheDate.Minute, TheDate.Second);
            }
            else
            {
                TheNewDate = new DateTime(TheDate.Year, TheDate.Month, this.configuration.MonthlyOnceDay.Value, TheDate.Hour, TheDate.Minute, TheDate.Second);
            }

            for (DateTime EachDate = TheNewDate; EachDate <= TheDateTo;
                EachDate = this.GetNexDate(EachDate))
            {
                if (this.configuration.MonthlyOnceDay.Value == EachDate.Day)
                {
                    TheExistList.AddRange(
                        this.ExecuteMonthlyWeeklyHours(TheTypeStepStr, TheDateTo, EachDate));
                }
            }
        }

        private void ValidateMonthlyOnceRecurring()
        {
            if (this.configuration.MonthlyOnceDay <= 0)
                throw new ScheduleException(Global.ValidateMonthlyOnceDayFrequency);

            if (this.configuration.MonthlyOnceMonthSteps == null || this.configuration.MonthlyOnceMonthSteps <= 0)
                throw new ScheduleException(Global.ValidateMonthlyOnceMonthFrequency);
        }

        private void ExecuteRecurringWeekly(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList)
        {
            this.ValidateWeeklyRecurring();

            DateTime TheDateTo = this.configuration.DateTo != null ? this.configuration.DateTo.Value : DateTime.MaxValue;
            int WeekStep = this.configuration.WeekStep > 0 ? this.configuration.WeekStep : 0;

            for (DateTime EachWeekDate = TheDate; EachWeekDate <= TheDateTo;
                EachWeekDate = this.GetNexDate(EachWeekDate))
            {
                TheExistList.AddRange(
                    this.ExecuteWeekly(TheTypeStepStr, TheDateTo, EachWeekDate));
            }
        }

        private void ValidateWeeklyRecurring()
        {
            if (this.configuration.WeekStep <= 0)
                throw new ScheduleException(Global.ValidateWeeklyStep);
        }

        private void ExecuteRecurringDaily(DateTime TheDate, string TheTypeStepStr, List<Output> TheExistList)
        {
            for (DateTime EachDate = TheDate.AddDays(1); EachDate.Date <= this.configuration.DateTo; EachDate = EachDate.AddDays(this.configuration.DailyStep))
            {
                TheExistList.Add(this.ReturnOuput(TheTypeStepStr, EachDate, EachDate, this.configuration.DateFrom, this.configuration.DateStep.Value));
            }
        }

        private Output[] ExecuteWeekly(string TheTypeStr,
           DateTime TheDateTo, DateTime EachDateWeek)
        {
            List<Output> TheExits = new List<Output>();

            for (DateTime EachDate = EachDateWeek; EachDate <= TheDateTo;
                EachDate = EachDate.AddDays(1))
            {
                if (this.weeklyVar.Any(W => W.Equals(EachDate.DayOfWeek)))
                {
                    TheExits.AddRange(this.ExecuteHours(TheTypeStr, EachDate));
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

        private DateTime? GetDayWeek(DateTime TheFirstDayWeek)
        {
            switch (this.configuration.MonthlyMoreOrderDayWeekStep)
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

        private DateTime GetNexDate(DateTime TheDate)
        {
            if (this.configuration.TypeRecurring == TypeTimeStep.Weekly)
            {
                if (this.configuration.WeekStep > 0)
                {
                    switch (TheDate.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            TheDate = TheDate.AddDays(this.configuration.WeekStep * 7);
                            break;
                        case DayOfWeek.Tuesday:
                            TheDate = TheDate.AddDays(-1 + this.configuration.WeekStep * 7);
                            break;
                        case DayOfWeek.Wednesday:
                            TheDate = TheDate.AddDays(-2 + this.configuration.WeekStep * 7);
                            break;
                        case DayOfWeek.Thursday:
                            TheDate = TheDate.AddDays(-3 + this.configuration.WeekStep * 7);
                            break;
                        case DayOfWeek.Friday:
                            TheDate = TheDate.AddDays(-4 + this.configuration.WeekStep * 7);
                            break;
                        case DayOfWeek.Saturday:
                            TheDate = TheDate.AddDays(-5 + this.configuration.WeekStep * 7);
                            break;
                        case DayOfWeek.Sunday:
                            TheDate = TheDate.AddDays(-6 + this.configuration.WeekStep * 7);
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (this.configuration.MonthlyOnce == true)
            {
                TheDate = TheDate.AddMonths(this.configuration.MonthlyOnceMonthSteps.Value);
            }
            else if (this.configuration.MonthlyMore == true)
            {
                TheDate = TheDate.AddMonths(this.configuration.MonthlyMoreMonthSteps.Value);
            }

            return TheDate;
        }

        private Output ReturnExitRecurringMonthtlyWeekly(string TheTypeStepStr, DateTime TheDateStep, DateTime? TheDateFrom, DateTime? TheDate)
        {
            string HourDayStr = this.GetHourDayString();

            string TheHourStepStr = "";

            if (this.configuration.HourStep.Value > 1)
            {
                TheHourStepStr = Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours;
            }
            else
            {
                TheHourStepStr = Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hour;
            }

            if (TheDate != null)
            {
                TheDateStep = TheDateStep.AddHours(TheDate.Value.Hour).AddMinutes(TheDate.Value.Minute);
            }

            Output TheExit = new Output();
            if (this.configuration.TypeRecurring == TypeTimeStep.Weekly)
            {
                TheExit.OutputDate = TheDateStep;
                TheExit.Description =
                    string.Format(Global.ExitRecurring, TheTypeStepStr, TheHourStepStr, HourDayStr,
                    TheDateFrom != null ? TheDateFrom.Value.ToShortDateString() : "");
            }
            else
            {
                if (this.configuration.MonthlyOnce == true)
                {
                    TheExit.OutputDate = TheDateStep;
                    TheExit.Description =
                        string.Format(Global.ExitRecurring, TheTypeStepStr, TheHourStepStr, HourDayStr,
                        TheDateFrom != null ? TheDateFrom.Value.ToShortDateString() : "");
                }
                else if (this.configuration.MonthlyMore == true)
                {
                    TheExit.OutputDate = TheDateStep;
                    TheExit.Description =
                        string.Format(Global.ExitRecurring, TheTypeStepStr, TheHourStepStr, HourDayStr,
                        TheDateFrom != null ? TheDateFrom.Value.ToShortDateString() : "");
                }
            }

            return TheExit;
        }

        private string GetHourDayString()
        {
            return (this.configuration.HourFrom != null ? this.configuration.HourFrom.Value.ToShortTimeString() :
                            new DateTime(1900, 1, 1, 0, 0, 0).ToShortTimeString()) + " and " +
                            (this.configuration.HourTo != null ? this.configuration.HourTo.Value.ToShortTimeString() :
                            new DateTime(1900, 1, 1, 23, 59, 0).ToShortTimeString());
        }

        private Output[] ExecuteMonthlyWeeklyHours(string TheTypeStepStr,
            DateTime TheDateTo, DateTime EachDateMonthly)
        {
            List<Output> TheExists = new List<Output>();

            TheExists.AddRange(this.ExecuteHours(TheTypeStepStr, EachDateMonthly));

            return TheExists.ToArray();
        }

        private Output[] ExecuteHours(string TheTypeStr, DateTime TheDate)
        {
            List<Output> TheList = new List<Output>();

            DateTime TheHourFrom = ReturnHourFrom(TheDate);

            DateTime TheHourTo = ReturnHourTo(TheDate);

            int TheHourStep = this.configuration.HourStep != null ?
                this.configuration.HourStep.Value : 1;

            for (DateTime TheHour = TheHourFrom; TheHour <= TheHourTo;
                TheHour = TheHour.AddHours(TheHourStep))
            {
                TheList.Add(this.ReturnExitRecurringMonthtlyWeekly(TheTypeStr, TheDate, this.configuration.DateFrom, TheHour));
            }

            return TheList.ToArray();
        }

        private DateTime ReturnHourTo(DateTime TheDate)
        {
            DateTime TheDateTo = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, 23, 59, 59);

            if (this.configuration.HourTo != null)
            {
                TheDateTo = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, this.configuration.HourTo.Value.Hour, this.configuration.HourTo.Value.Minute,
                    this.configuration.HourTo.Value.Second);
            }

            return TheDateTo;
        }

        private DateTime ReturnHourFrom(DateTime TheDate)
        {
            DateTime TheHourFrom = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, 0, 0, 0);

            if (this.configuration.HourFrom != null)
            {
                TheHourFrom = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, this.configuration.HourFrom.Value.Hour, this.configuration.HourFrom.Value.Minute,
                    this.configuration.HourFrom.Value.Second);
            }

            return TheHourFrom;
        }

        #endregion
    }
}
