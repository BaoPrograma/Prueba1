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
            configuration = Laconfiguracion;

            this.PrepareWeek();
        }

        private void PrepareWeek()
        {
            List<DayOfWeek> WeekList = new List<DayOfWeek>();

            if (configuration.WeekMonday)
                WeekList.Add(DayOfWeek.Monday);
            if (configuration.WeekTuesday)
                WeekList.Add(DayOfWeek.Tuesday);
            if (configuration.WeekWednesday)
                WeekList.Add(DayOfWeek.Wednesday);
            if (configuration.WeekThursday)
                WeekList.Add(DayOfWeek.Thursday);
            if (configuration.WeekFriday)
                WeekList.Add(DayOfWeek.Friday);
            if (configuration.WeekSaturday)
                WeekList.Add(DayOfWeek.Saturday);
            if (configuration.WeekSunday)
                WeekList.Add(DayOfWeek.Sunday);

            weekVar = WeekList.ToArray();
        }

        public Output[] ExecuteDateStep(DateTime TheDate)
        {
            if (configuration.Enabled)
            {
                return Execute(TheDate);
            }
            else
            {
                return new Output[] {ReturnOuput("",
                   TheDate, TheDate, configuration.DateFrom, null)};
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
            if (configuration.TimeType == TypeStep.Once)
            {
                TheStepStr = Global.once;
            }
            else
            {
                if (configuration.TypeStep == TypeDayStep.Daily)
                {
                    TheStepStr = Global.every + Global.day;
                }
                else
                {
                    TheStepStr = Global.every + " " + configuration.WeekStep + " " + Global.weeks;
                }
            }

            if (configuration.TimeType == TypeStep.Once)
            {
                return ExecuteOnce(TheStepStr);
            }
            else
            {
                return ExecuteRecurring(TheDate, TheStepStr);
            }
        }

        private Output[] ExecuteRecurring(DateTime TheDate, string TheTypeStepStr)
        {
            List<Output> TheExistList = new List<Output>();

            if (configuration.TypeStep == TypeDayStep.Daily)
            {
                for (DateTime EachDate = TheDate.AddDays(1); EachDate <= TheDate; EachDate = EachDate.AddDays(1))
                {
                    TheExistList.Add(ReturnOuput(TheTypeStepStr, EachDate, EachDate, configuration.DateFrom, null));
                }
            }
            else
            {
                DateTime TheDateTo = configuration.DateTo != null ? configuration.DateTo.Value : DateTime.MaxValue;
                int WeekStep = configuration.WeekStep > 0 ? configuration.WeekStep : 0;

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
            if (configuration.WeekStep > 0)
            {
                if (TheDate.DayOfWeek == DayOfWeek.Monday)
                    return TheDate.AddDays(7 * configuration.WeekStep);
                if (TheDate.DayOfWeek == DayOfWeek.Tuesday)
                    return TheDate.AddDays(7 * configuration.WeekStep - 1);
                if (TheDate.DayOfWeek == DayOfWeek.Wednesday)
                    return TheDate.AddDays(7 * configuration.WeekStep - 2);
                if (TheDate.DayOfWeek == DayOfWeek.Thursday)
                    return TheDate.AddDays(7 * configuration.WeekStep - 3);
                if (TheDate.DayOfWeek == DayOfWeek.Friday)
                    return TheDate.AddDays(7 * configuration.WeekStep - 4);
                if (TheDate.DayOfWeek == DayOfWeek.Saturday)
                    return TheDate.AddDays(7 * configuration.WeekStep - 5);
                if (TheDate.DayOfWeek == DayOfWeek.Sunday)
                    return TheDate.AddDays(7 * configuration.WeekStep - 6);
            }
            else
            {
                throw new Exception(Global.ValidateWeeklyStep);
            }

            return TheDate;
        }

        private Output ReturnExitRecurringWeekly(string TheTypeStepStr, DateTime TheDateStep, DateTime? TheDateFrom, DateTime? TheDate)
        {
            string TheDateStepStr = TheDateStep.ToString("dd/MM/yyyy");

            string WeekDaysStr = " on ";
            weekVar.ToList().ForEach(
                S => WeekDaysStr = WeekDaysStr + S.ToString().ToLower() + ", ");
            WeekDaysStr = WeekDaysStr.Trim().TrimEnd(',');

            string HorasDiasStr = (configuration.HourFrom != null ? configuration.HourFrom.Value.ToShortTimeString() :
                new DateTime(1900, 1, 1, 0, 0, 0).ToShortTimeString()) + " and " +
                (configuration.HourTo != null ? configuration.HourTo.Value.ToShortTimeString() :
                new DateTime(1900, 1, 1, 23, 59, 0).ToShortTimeString());

            string TheHourStepStr = Global.every + " " + configuration.HourStep.ToString() +
                " " + Global.hours;

            if (TheDate != null)
            {
                TheDateStep = TheDateStep.AddHours(TheDate.Value.Hour).AddMinutes(TheDate.Value.Minute);
            }

            Output TheExit = new Output();
            TheExit.OutputDate = TheDateStep;
            TheExit.Description =
                string.Format(Global.ExitRecurringWeekly, TheTypeStepStr, WeekDaysStr, TheHourStepStr, HorasDiasStr,
                TheDateFrom != null ? TheDateFrom.Value.ToString("dd/MM/yyyy") : "");

            return TheExit;
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

            int LaHoraPaso = configuration.HourStep != null ?
                configuration.HourStep.Value : 1;

            for (DateTime TheHour = LaHoraDesde; TheHour <= LaHoraHasta;
                TheHour = TheHour.AddHours(LaHoraPaso))
            {
                TheList.Add(ReturnExitRecurringWeekly(TheTypeStr, TheDate, configuration.DateFrom, TheHour));
            }

            return TheList.ToArray();
        }

        private DateTime ReturnHourTo(DateTime TheDate)
        {
            DateTime TheDateTo = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, 23, 59, 59);

            if (configuration.HourTo != null)
            {
                TheDateTo = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, configuration.HourTo.Value.Hour, configuration.HourTo.Value.Minute,
                    configuration.HourTo.Value.Second);
            }

            return TheDateTo;
        }

        private DateTime ReturnHourFrom(DateTime TheDate)
        {
            DateTime TheHourFrom = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, 0, 0, 0);

            if (configuration.HourFrom != null)
            {
                TheHourFrom = new DateTime(TheDate.Year, TheDate.Month, TheDate.Day, configuration.HourFrom.Value.Hour, configuration.HourFrom.Value.Minute,
                    configuration.HourFrom.Value.Second);
            }

            return TheHourFrom;
        }

        private Output[] ExecuteOnce(string ElTipoStr)
        {
            if (configuration.DateStep == null)
            {
                throw new Exception(Global.ValidateDateConfiguration);
            }

            if (configuration.DateFrom != null &&
                configuration.DateStep > configuration.DateFrom ||
                configuration.DateFrom == null)
            {
                return new Output[]{ReturnOuput(ElTipoStr,
                        configuration.DateStep.Value,
                        configuration.DateStep.Value, configuration.DateFrom, null) };
            }

            return null;
        }
    }
}
