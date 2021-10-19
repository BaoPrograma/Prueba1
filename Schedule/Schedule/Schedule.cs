using Schedule.Config;
using Schedule.RecursosTextos;
using Semicrol.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Schedule.Process
{
    public class Schedule
    {
        private Configuration configuration;
        private DayOfWeek[] weekVar;

        public Schedule(Configuration Laconfiguracion)
        {
            this.configuration = Laconfiguracion;

            this.PrepareWeek();
        }

        private void PrepareWeek()
        {
            List<DayOfWeek> WeekList = new List<DayOfWeek>();

            if (this.configuration.WeekMonday)
                WeekList.Add(DayOfWeek.Monday);
            if (this.configuration.WeekTuesday)
                WeekList.Add(DayOfWeek.Tuesday);
            if (this.configuration.WeekWednesday)
                WeekList.Add(DayOfWeek.Wednesday);
            if (this.configuration.WeekThursday)
                WeekList.Add(DayOfWeek.Thursday);
            if (this.configuration.WeekFriday)
                WeekList.Add(DayOfWeek.Friday);
            if (this.configuration.WeekSaturday)
                WeekList.Add(DayOfWeek.Saturday);
            if (this.configuration.WeekSunday)
                WeekList.Add(DayOfWeek.Sunday);

            weekVar = WeekList.ToArray();
        }

        public Output[] ExecuteDateStep(DateTime TheDate)
        {
            if (this.configuration.Enabled)
            {
                if (this.configuration.DateStep == null)
                {
                    throw new Exception(Global.ValidateDateConfiguration);
                }

                DateTime TheDateAux = this.configuration.DateFrom != null &&
                    TheDate > this.configuration.DateFrom ? TheDate : this.configuration.DateFrom.Value;

                return this.Execute(TheDateAux);
            }
            else
            {
                return new Output[] {ReturnOuput("",
                   TheDate, TheDate, this.configuration.DateFrom, null)};
            }
        }

        private Output ReturnOuput(string TheWeeStepStr, DateTime TheDateStep, DateTime TheHourStep, DateTime? TheDateFrom, DateTime? TheHour)
        {
            string TheDateStepStr = TheDateStep.ToString("dd/MM/yyyy");

            if (TheHourStep.TimeOfDay > new TimeSpan(00, 00, 00))
            {
                TheDateStepStr += Global.at + " " + TheHourStep.ToString("HH:mm");
            }

            Output TheExit = new Output();
            TheExit.OutputDate = TheDateStep;
            TheExit.Description =
                string.Format(Global.Output, TheWeeStepStr, TheDateStepStr) +
                (TheDateFrom != null ? " " + string.Format(Global.StartingOn
                , TheDateFrom.Value.ToString("dd/MM/yyyy")) : "") +
                " " + TheHour != null ? TheHour.Value.ToString("HH:mm") : "00:00";

            return TheExit;
        }

        private Output[] Execute(DateTime TheDate)
        {
            string TheStepStr = "";
            if (this.configuration.TimeType == TypeStep.Once)
            {
                TheStepStr = Global.once;
            }
            else
            {
                if (this.configuration.TypeStep == TypeDayStep.Daily)
                {
                    TheStepStr = Global.every + Global.day;
                }
                else
                {
                    TheStepStr = Global.every + " " + this.configuration.WeekStep + " " + Global.weeks;
                }
            }

            if (this.configuration.TimeType == TypeStep.Once)
            {
                return ExecuteOnce(TheStepStr);
            }
            else
            {
                if (this.configuration.WeekStep < 0)
                    throw new Exception(Global.ValidateWeeklyStep);

                if (this.configuration.HourStep < 0)
                    throw new Exception(Global.ValidateHourStep);

                return ExecuteRecurring(TheDate, TheStepStr);
            }
        }

        #region Once
        private Output[] ExecuteOnce(string ElTipoStr)
        {
            if ((this.configuration.DateFrom != null &&
                this.configuration.DateStep > this.configuration.DateFrom) ||
                this.configuration.DateFrom == null)
            {
                return new Output[]{ReturnOuput(ElTipoStr,
                        this.configuration.DateStep.Value,
                        this.configuration.DateStep.Value, this.configuration.DateFrom, null) };
            }
            else
            {
                if (this.configuration.DateFrom != null)
                {
                    return new Output[]{ReturnOuput(ElTipoStr,
                        this.configuration.DateFrom.Value,
                        this.configuration.DateFrom.Value, this.configuration.DateFrom, null) };
                }
            }

            return null;
        }
        #endregion

        #region Recurring
        private Output[] ExecuteRecurring(DateTime TheDate, string TheTypeStepStr)
        {
            List<Output> TheExistList = new List<Output>();

            if (this.configuration.TypeStep == TypeDayStep.Daily)
            {
                for (DateTime EachDate = TheDate.AddDays(1); EachDate <= TheDate; EachDate = EachDate.AddDays(1))
                {
                    TheExistList.Add(ReturnOuput(TheTypeStepStr, EachDate, EachDate, this.configuration.DateFrom, null));
                }
            }
            else
            {
                DateTime TheDateTo = this.configuration.DateTo != null ? this.configuration.DateTo.Value : DateTime.MaxValue;
                int WeekStep = this.configuration.WeekStep > 0 ? this.configuration.WeekStep : 0;

                for (DateTime EachWeekDate = TheDate; EachWeekDate <= TheDateTo;
                    EachWeekDate = this.GetNexDate(EachWeekDate))
                {
                    TheExistList.AddRange(
                        this.ExecuteWeekly(TheTypeStepStr, TheDateTo, EachWeekDate));
                }
            }

            return TheExistList.ToArray();
        }

        private DateTime GetNexDate(DateTime TheDate)
        {
            if (TheDate.DayOfWeek == DayOfWeek.Monday)
                return TheDate.AddDays(7 * this.configuration.WeekStep);
            if (TheDate.DayOfWeek == DayOfWeek.Tuesday)
                return TheDate.AddDays(7 * this.configuration.WeekStep - 1);
            if (TheDate.DayOfWeek == DayOfWeek.Wednesday)
                return TheDate.AddDays(7 * this.configuration.WeekStep - 2);
            if (TheDate.DayOfWeek == DayOfWeek.Thursday)
                return TheDate.AddDays(7 * this.configuration.WeekStep - 3);
            if (TheDate.DayOfWeek == DayOfWeek.Friday)
                return TheDate.AddDays(7 * this.configuration.WeekStep - 4);
            if (TheDate.DayOfWeek == DayOfWeek.Saturday)
                return TheDate.AddDays(7 * this.configuration.WeekStep - 5);
            if (TheDate.DayOfWeek == DayOfWeek.Sunday)
                return TheDate.AddDays(7 * this.configuration.WeekStep - 6);

            return TheDate;
        }

        private Output ReturnExitRecurringWeekly(string TheTypeStepStr, DateTime TheDateStep, DateTime? TheDateFrom, DateTime? TheDate)
        {
            string TheDateStepStr = TheDateStep.ToString("dd/MM/yyyy");

            string WeekDaysStr = this.GetDayString();

            string HourDayStr = this.GetHourDayString();

            string TheHourStepStr = Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours;

            if (TheDate != null)
            {
                TheDateStep = TheDateStep.AddHours(TheDate.Value.Hour).AddMinutes(TheDate.Value.Minute);
            }

            Output TheExit = new Output();
            TheExit.OutputDate = TheDateStep;
            TheExit.Description =
                string.Format(Global.ExitRecurringWeekly, TheTypeStepStr, WeekDaysStr, TheHourStepStr, TheHourStepStr,
                TheDateFrom != null ? TheDateFrom.Value.ToString("dd/MM/yyyy") : "");

            return TheExit;
        }

        private string GetHourDayString()
        {
            return (this.configuration.HourFrom != null ? this.configuration.HourFrom.Value.ToShortTimeString() :
                            new DateTime(1900, 1, 1, 0, 0, 0).ToShortTimeString()) + " and " +
                            (this.configuration.HourTo != null ? this.configuration.HourTo.Value.ToShortTimeString() :
                            new DateTime(1900, 1, 1, 23, 59, 0).ToShortTimeString());
        }

        private string GetDayString()
        {
            string WeekDaysStr = " on ";
            weekVar.ToList().ForEach(
                S => WeekDaysStr = WeekDaysStr + S.ToString().ToLower() + ", ");
            WeekDaysStr = WeekDaysStr.Trim().TrimEnd(',');
            return WeekDaysStr;
        }

        private Output[] ExecuteWeekly(string ElTipoStr,
            DateTime TheDateTo, DateTime EachDateWeek)
        {
            List<Output> TheExits = new List<Output>();

            for (DateTime EachDate = EachDateWeek; EachDate <= TheDateTo;
                EachDate = EachDate.AddDays(1))
            {
                if (weekVar.Any(W => W.Equals(EachDate.DayOfWeek)))
                {
                    TheExits.AddRange(ExecuteHours(ElTipoStr, EachDate));
                }

                if (EachDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    break;
                }
            }

            return TheExits.ToArray();
        }

        private Output[] ExecuteHours(string TheTypeStr, DateTime TheDate)
        {
            List<Output> TheList = new List<Output>();

            DateTime LaHoraDesde = ReturnHourFrom(TheDate);

            DateTime LaHoraHasta = ReturnHourTo(TheDate);

            int LaHoraPaso = this.configuration.HourStep != null ?
                this.configuration.HourStep.Value : 1;

            for (DateTime TheHour = LaHoraDesde; TheHour <= LaHoraHasta;
                TheHour = TheHour.AddHours(LaHoraPaso))
            {
                TheList.Add(ReturnExitRecurringWeekly(TheTypeStr, TheDate, this.configuration.DateFrom, TheHour));
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
