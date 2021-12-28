using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Schedule.Process;
using Schedule.Config;

namespace Schedule.Test
{
    public class ScheduleTest
    {
        private Process.Schedule process;

        public ScheduleTest()
        {
        }

        [Fact]
        public void Execute_without_configuration_enabled_sholud_return_date()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = false;
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.process = new Process.Schedule(CurrentConfiguration);

            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value,  CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 1);
            Assert.Equal(TheOutput[0].OutputDate, CurrentConfiguration.DateStep.Value);
        }

        [Fact]
        public void Execute_should_fill_date_from()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set the Date From and Step in configuration", caughtException.Message);
        }

        [Fact]
        public void Execute_should_fill_configuration()
        {
            // Arrange
            Configuration CurrentConfiguration = null;

            var caughtException =
                     Assert.Throws<ScheduleException>(() =>
                     this.process = new Process.Schedule(CurrentConfiguration));

            Assert.Equal("Need to fill the configuration", caughtException.Message);
        }

        [Fact]
        public void Execute_once_daily_should_exit_date_step()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.TimeType = TypeStep.Once;
            CurrentConfiguration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(LaFecha, CurrentConfiguration);

            //Assert
            Assert.Equal(CurrentConfiguration.DateStep, TheOutput[0].OutputDate);
        }

        [Fact]
        public void Execute_once_daily_should_fill_date_from()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Once;
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 8);
            CurrentConfiguration.Language = Languages.en_GB;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(new DateTime(2021, 1, 1), CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set the Date From and Step in configuration", caughtException.Message);
        }

        [Fact]
        public void Execute_once_daily_should_fill_date_step()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Once;
            CurrentConfiguration.DateStep = null;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha,CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set the Date From and Step in configuration", caughtException.Message);
        }


        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_once(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Once;
            CurrentConfiguration.DateStep = new DateTime(2021,1, 4);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 4);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 5);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act

            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 4, 0, 0, 0));            
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs once. Schedule will be used on 04/01/2021 starting on 04/01/2021 00:00");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre una vez. Schedule se usará en 04/01/2021 empezando en 04/01/2021 00:00");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs once. Schedule will be used on 1/4/2021 starting on 1/4/2021 00:00");
                    break;
            };
        }

        [Fact]
        public void Execute_recurring_daily_should_set_frequency_when_occurs()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            CurrentConfiguration.HourStep = 1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                  Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set frequency", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_daily_HourStep_should_be_higher_than_0()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Daily;
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            CurrentConfiguration.HourStep = 1;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(CurrentConfiguration.HourStep > 0);
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_daily_each_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Daily;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1, 14, 0, 0);
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1, 14, 0, 0);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 4, 14, 0, 0);
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Daily;
            CurrentConfiguration.DailyStep = 1;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 2, 14, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every day. Schedule will be used on 02/01/2021 at 14:00 starting on 01/01/2021 14:00");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada dia. Schedule se usará en 02/01/2021 a las 14:00 empezando en 01/01/2021 14:00");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every day. Schedule will be used on 1/2/2021 at 14:00 starting on 1/1/2021 14:00");
                    break;
            };
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 3, 14, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[1].Description == "Occurs every day. Schedule will be used on 03/01/2021 at 14:00 starting on 01/01/2021 14:00");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[1].Description == "Ocurre cada dia. Schedule se usará en 03/01/2021 a las 14:00 empezando en 01/01/2021 14:00");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[1].Description == "Occurs every day. Schedule will be used on 1/3/2021 at 14:00 starting on 1/1/2021 14:00");
                    break;
            };
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 4, 14, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[2].Description == "Occurs every day. Schedule will be used on 04/01/2021 at 14:00 starting on 01/01/2021 14:00");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[2].Description == "Ocurre cada dia. Schedule se usará en 04/01/2021 a las 14:00 empezando en 01/01/2021 14:00");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[2].Description == "Occurs every day. Schedule will be used on 1/4/2021 at 14:00 starting on 1/1/2021 14:00");
                    break;
            };
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_daily_more_one_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Daily;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1, 14, 0, 0);
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1, 14, 0, 0);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 4, 14, 0, 0);
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Daily;
            CurrentConfiguration.DailyStep = 2;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 2, 14, 0, 0));            
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 days. Schedule will be used on 02/01/2021 at 14:00 starting on 01/01/2021 14:00");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 dias. Schedule se usará en 02/01/2021 a las 14:00 empezando en 01/01/2021 14:00");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 days. Schedule will be used on 1/2/2021 at 14:00 starting on 1/1/2021 14:00");
                    break;
            };
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 4, 14, 0, 0));            
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[1].Description == "Occurs every 2 days. Schedule will be used on 04/01/2021 at 14:00 starting on 01/01/2021 14:00");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[1].Description == "Ocurre cada 2 dias. Schedule se usará en 04/01/2021 a las 14:00 empezando en 01/01/2021 14:00");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[1].Description == "Occurs every 2 days. Schedule will be used on 1/4/2021 at 14:00 starting on 1/1/2021 14:00");
                    break;
            };
        }

        [Fact]
        public void Execute_recurring_WeeklyStep_should_be_bigger_than_0()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 4);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 8);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.WeekStep = -1;
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                   Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set week step bigger than 0", caughtException.Message);
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_per_one_week(Languages Language)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Monday });

            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 1;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.WeeklyMonday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 4, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every week on monday every 2 hours between 08:00 and 08:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada semana el lunes cada 2 horas entre 8:00 y 8:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every week on monday every 2 hours between 8:00 am and 8:00 am starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 11, 8, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_per_one_hour(Languages Language)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Monday });

            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 1;
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.WeeklyMonday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 4, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every week on monday every hour between 08:00 and 08:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada semana el lunes cada hora entre 8:00 y 8:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every week on monday every hour between 8:00 am and 8:00 am starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 11, 8, 0, 0));
        }

        [Theory]
        [InlineData(8, 12, Languages.en_GB)]
        [InlineData(10, 14, Languages.es_ES)]
        [InlineData(15, 19, Languages.en_US)]
        [InlineData(8, 12, Languages.es_ES)]
        [InlineData(10, 14, Languages.en_US)]
        [InlineData(15, 19, Languages.en_GB)]
        [InlineData(8, 12, Languages.en_US)]
        [InlineData(10, 14, Languages.en_GB)]
        [InlineData(15, 19, Languages.es_ES)]
        public void Execute_recurring_Weekly_per_days_and_hours(int HourFrom, int HourTo, Languages Language)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday });

            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 2);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, HourFrom, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, HourTo, 0, 0);
            CurrentConfiguration.WeeklyMonday = TheWeek.Contains(DayOfWeek.Monday);
            CurrentConfiguration.WeeklyTuesday = TheWeek.Contains(DayOfWeek.Tuesday);
            CurrentConfiguration.WeeklyWednesday = TheWeek.Contains(DayOfWeek.Wednesday);
            CurrentConfiguration.WeeklyThursday = TheWeek.Contains(DayOfWeek.Thursday);
            CurrentConfiguration.WeeklyFriday = TheWeek.Contains(DayOfWeek.Friday);
            CurrentConfiguration.WeeklySaturday = TheWeek.Contains(DayOfWeek.Saturday);
            CurrentConfiguration.WeeklySunday = TheWeek.Contains(DayOfWeek.Sunday);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 6);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 1, HourFrom, 0, 0));
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, HourFrom, 0, 0);
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on monday, tuesday, wednesday, thursday, friday, saturday and sunday every 2 hours between " + HourFrom.ToString("00") + ":00 and " + HourTo.ToString("00") + ":00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el lunes, martes, miercoles, jueves, viernes, sabado y domingo cada 2 horas entre " + HourFrom.ToString("0") + ":00 y " + HourTo.ToString("00") + 
                        ":00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on monday, tuesday, wednesday, thursday, friday, saturday and sunday every 2 hours between " + (HourFrom > 12 ? (HourFrom - 12).ToString("0") + ":00 pm ": HourFrom.ToString("0") + ":00 am ") + "and " + (HourTo>12?(HourTo -12).ToString("0"):HourTo.ToString("0")) +  ":00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 1, HourFrom + 2, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 1, HourFrom + 2 + 2, 0, 0));
            Assert.True(TheOutput[3].OutputDate.Value == new DateTime(2021, 1, 2, HourFrom, 0, 0));
            Assert.True(TheOutput[4].OutputDate.Value == new DateTime(2021, 1, 2, HourFrom + 2, 0, 0));
            Assert.True(TheOutput[5].OutputDate.Value == new DateTime(2021, 1, 2, HourFrom + 2 + 2, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_monday_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.WeeklyMonday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 11, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on monday every 2 hours between 08:00 and 12:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el lunes cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on monday every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 11, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 11, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_tuesday_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.WeeklyTuesday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 12, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on tuesday every 2 hours between 08:00 and 12:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el martes cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on tuesday every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 12, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 12, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_wednesday_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.WeeklyWednesday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 13, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on wednesday every 2 hours between 08:00 and 12:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el miercoles cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on wednesday every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 13, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 13, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_thursday_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.WeeklyThursday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 14, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on thursday every 2 hours between 08:00 and 12:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el jueves cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on thursday every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 14, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 14, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_friday_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.WeeklyFriday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 1, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on friday every 2 hours between 08:00 and 12:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el viernes cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on friday every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 1, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 1, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_saturday_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.WeeklySaturday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 2, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on saturday every 2 hours between 08:00 and 12:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el sabado cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on saturday every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 2, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 2, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_sunday_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 14);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.WeeklySunday = true;
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 3, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on sunday every 2 hours between 08:00 and 12:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el domingo cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on sunday every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 3, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 3, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_saturady_begin(Languages Language)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Saturday });

            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 2);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 2);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 30);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.WeeklySaturday = TheWeek.Contains(DayOfWeek.Saturday);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 2, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on saturday every hour between 08:00 and 08:00 starting on 02/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el sabado cada hora entre 8:00 y 8:00 empezando en 02/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on saturday every hour between 8:00 am and 8:00 am starting on 1/2/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 16, 8, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 30, 8, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_sunday_begin(Languages Language)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Sunday });

            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 3);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 3);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 31);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.WeeklySunday = TheWeek.Contains(DayOfWeek.Sunday);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 3, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on sunday every hour between 08:00 and 08:00 starting on 03/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el domingo cada hora entre 8:00 y 8:00 empezando en 03/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on sunday every hour between 8:00 am and 8:00 am starting on 1/3/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 17, 8, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 31, 8, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_thursday_begin(Languages Language)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Thursday });

            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 7);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 7);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 31);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.WeeklyThursday = TheWeek.Contains(DayOfWeek.Thursday);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 7, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on thursday every hour between 08:00 and 08:00 starting on 07/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el jueves cada hora entre 8:00 y 8:00 empezando en 07/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on thursday every hour between 8:00 am and 8:00 am starting on 1/7/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 21, 8, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_wednesday_begin(Languages Language)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Wednesday });

            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 6);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 6);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 31);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.WeeklyWednesday = TheWeek.Contains(DayOfWeek.Wednesday);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 6, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on wednesday every hour between 08:00 and 08:00 starting on 06/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el miercoles cada hora entre 8:00 y 8:00 empezando en 06/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on wednesday every hour between 8:00 am and 8:00 am starting on 1/6/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 20, 8, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_Weekly_tuesday_begin(Languages Language)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Tuesday });

            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 5);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 5);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 31);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Weekly;
            CurrentConfiguration.WeekStep = 2;
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.WeeklyTuesday = TheWeek.Contains(DayOfWeek.Tuesday);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 5, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on tuesday every hour between 08:00 and 08:00 starting on 05/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre cada 2 semanas el martes cada hora entre 8:00 y 8:00 empezando en 05/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs every 2 weeks on tuesday every hour between 8:00 am and 8:00 am starting on 1/5/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 19, 8, 0, 0));
        }

        [Fact]
        public void Recurring_MonthlyMore_WeekStep_monthlyWeekVar_should_not_be_null()
        {
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Day;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(CurrentConfiguration.MonthlyMoreMonthSteps != null);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_monthly_configuration()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnceDay = 8;
            CurrentConfiguration.MonthlyOnceMonthSteps = 2;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set one of the checks in Monthly Configuration (day, the ..)", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_month_step_bigger_than_0()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = 8;
            CurrentConfiguration.MonthlyOnceMonthSteps = 0;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set month(s) bigger than 0", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_day_bigger_than_0_in_day_configuration()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = -1;
            CurrentConfiguration.MonthlyOnceMonthSteps = 1;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Day must be bigger than 0", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_month_bigger_than_0_in_day_configuration()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = 1;
            CurrentConfiguration.MonthlyOnceMonthSteps = -1;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set month(s) bigger than 0", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_hour_step_daily_frequency_bigger_than_0()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = 8;
            CurrentConfiguration.MonthlyOnceMonthSteps = 1;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 0;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set hour step in daily frequency bigger than 0", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_hour_from_should_be_smaller_than_hour_to()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = 8;
            CurrentConfiguration.MonthlyOnceMonthSteps = 1;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeDailyFrequency = DailyFrequency.Every;
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 10, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.Equal("Hour From not should be bigger than Hour To", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_month_frequency()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = 8;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 1);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                  Assert.Throws<ScheduleException>(() => this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration));

            //Assert
            Assert.True("Need to set one of the checks in Monthly Configuration (day, the ..)" == caughtException.Message);
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_once_per_months(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = 8;
            CurrentConfiguration.MonthlyOnceMonthSteps = 2;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 8);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 6);
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 8, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs day 8 of every 2 months every 2 hours between 08:00 and 12:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre dia 8 de cada 2 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs day 8 of every 2 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 1, 8, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2021, 1, 8, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2021, 3, 8, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2021, 3, 8, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2021, 3, 8, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_once_per_one_month(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = 8;
            CurrentConfiguration.MonthlyOnceMonthSteps = 1;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 2, 8);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 8, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs day 8 of every month every 2 hours between 08:00 and 08:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre dia 8 de cada mes cada 2 horas entre 8:00 y 8:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs day 8 of every month every 2 hours between 8:00 am and 8:00 am starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 2, 8, 8, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_once_per_day_31(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyOnce = true;
            CurrentConfiguration.MonthlyOnceDay = 31;
            CurrentConfiguration.MonthlyOnceMonthSteps = 1;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 4, 8);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.Language = Language;

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 31, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs day 31 of every month every 2 hours between 08:00 and 08:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre dia 31 de cada mes cada 2 horas entre 8:00 y 8:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs day 31 of every month every 2 hours between 8:00 am and 8:00 am starting on 1/1/2021");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 3, 31, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_without_hour_from_and_hour_to()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreMonthSteps = 1;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 4);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 1, 27);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = null;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set the hour from and hour to", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_should_set_MonthlyConfiguration()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 4);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 1, 27);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set one of the checks in Monthly Configuration (day, the ..)", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_not_avaliable_hour_step()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 4);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 1, 27);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = -1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha, CurrentConfiguration));

            //Assert
            Assert.Equal("Hour step must be bigger than 0", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_should_fill_date_step()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.DateStep = null;
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 3);
            CurrentConfiguration.DateTo = new DateTime(2020, 1, 27);
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = -1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set the Date From and Step in configuration", caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_without_days()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 2);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 1, 27);
            CurrentConfiguration.MonthlyMoreMonthSteps = 1;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.HourStep = 1;            
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            DateTime LaFecha = new DateTime(2020, 1, 1);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha, CurrentConfiguration));

            //Assert
            Assert.Equal("Need to set the day frequency", caughtException.Message);
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_monday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Monday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 6, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first monday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer lunes de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first monday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 6, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 6, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, 6, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, 6, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, 6, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_tuesday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Tuesday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 7, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first tuesday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer martes de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first tuesday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 7, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 7, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, 7, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, 7, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, 7, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_wednesday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Wednesday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 1, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first wednesday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer miercoles de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first wednesday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }

            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 1, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 1, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, 1, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, 1, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, 1, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_thursday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 2, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first thursday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer jueves de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first thursday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 2, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 2, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, 2, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, 2, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, 2, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_Friday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Friday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 3, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first friday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer viernes de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first friday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 3, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 3, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, 3, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, 3, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, 3, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_saturday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Saturday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 4, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first saturday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer sabado de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first saturday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 4, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 4, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, 4, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, 4, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, 4, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_sunday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Sunday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 5, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first sunday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer domingo de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first sunday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 5, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 5, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, 5, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, 5, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, 5, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_weekendday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 4, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first weekend of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer fin de semana de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first weekend of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 4, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 4, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 1, 5, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 1, 5, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 1, 5, 12, 0, 0));
            Assert.True(TheOutput[6].OutputDate == new DateTime(2020, 4, 4, 8, 0, 0));
            Assert.True(TheOutput[7].OutputDate == new DateTime(2020, 4, 4, 10, 0, 0));
            Assert.True(TheOutput[8].OutputDate == new DateTime(2020, 4, 4, 12, 0, 0));
            Assert.True(TheOutput[9].OutputDate == new DateTime(2020, 4, 5, 8, 0, 0));
            Assert.True(TheOutput[10].OutputDate == new DateTime(2020, 4, 5, 10, 0, 0));
            Assert.True(TheOutput[11].OutputDate == new DateTime(2020, 4, 5, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_weekday(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 1, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first weekday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer dia laboral de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first weekday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 1, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 1, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 1, 2, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 1, 2, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 1, 2, 12, 0, 0));
            Assert.True(TheOutput[6].OutputDate == new DateTime(2020, 1, 3, 8, 0, 0));
            Assert.True(TheOutput[7].OutputDate == new DateTime(2020, 1, 3, 10, 0, 0));
            Assert.True(TheOutput[8].OutputDate == new DateTime(2020, 1, 3, 12, 0, 0));
            Assert.True(TheOutput[9].OutputDate == new DateTime(2020, 1, 4, 8, 0, 0));
            Assert.True(TheOutput[10].OutputDate == new DateTime(2020, 1, 4, 10, 0, 0));
            Assert.True(TheOutput[11].OutputDate == new DateTime(2020, 1, 4, 12, 0, 0));
            Assert.True(TheOutput[12].OutputDate == new DateTime(2020, 1, 5, 8, 0, 0));
            Assert.True(TheOutput[13].OutputDate == new DateTime(2020, 1, 5, 10, 0, 0));
            Assert.True(TheOutput[14].OutputDate == new DateTime(2020, 1, 5, 12, 0, 0));
            Assert.True(TheOutput[15].OutputDate == new DateTime(2020, 4, 1, 8, 0, 0));
            Assert.True(TheOutput[16].OutputDate == new DateTime(2020, 4, 1, 10, 0, 0));
            Assert.True(TheOutput[17].OutputDate == new DateTime(2020, 4, 1, 12, 0, 0));
            Assert.True(TheOutput[18].OutputDate == new DateTime(2020, 4, 2, 8, 0, 0));
            Assert.True(TheOutput[19].OutputDate == new DateTime(2020, 4, 2, 10, 0, 0));
            Assert.True(TheOutput[20].OutputDate == new DateTime(2020, 4, 2, 12, 0, 0));
            Assert.True(TheOutput[21].OutputDate == new DateTime(2020, 4, 3, 8, 0, 0));
            Assert.True(TheOutput[22].OutputDate == new DateTime(2020, 4, 3, 10, 0, 0));
            Assert.True(TheOutput[23].OutputDate == new DateTime(2020, 4, 3, 12, 0, 0));
            Assert.True(TheOutput[24].OutputDate == new DateTime(2020, 4, 4, 8, 0, 0));
            Assert.True(TheOutput[25].OutputDate == new DateTime(2020, 4, 4, 10, 0, 0));
            Assert.True(TheOutput[26].OutputDate == new DateTime(2020, 4, 4, 12, 0, 0));
            Assert.True(TheOutput[27].OutputDate == new DateTime(2020, 4, 5, 8, 0, 0));
            Assert.True(TheOutput[28].OutputDate == new DateTime(2020, 4, 5, 10, 0, 0));
            Assert.True(TheOutput[29].OutputDate == new DateTime(2020, 4, 5, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_day(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Day;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 1, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first day of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer dia de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first day of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 1, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 1, 12, 0, 0));
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, 1, 8, 0, 0));
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, 1, 10, 0, 0));
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, 1, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_one_hour(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 2, 20);
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 2, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first thursday of every 3 months every hour between 08:00 and 08:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer jueves de cada 3 meses cada hora entre 8:00 y 8:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first thursday of every 3 months every hour between 8:00 am and 8:00 am starting on 1/1/2020");
                    break;
            }
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_one_month(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 1;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 1, 20);
            CurrentConfiguration.HourStep = 1;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 2, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first thursday of every month every hour between 08:00 and 08:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer jueves de cada mes cada hora entre 8:00 y 8:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first thursday of every month every hour between 8:00 am and 8:00 am starting on 1/1/2020");
                    break;
            }
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_may()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 5, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 5, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 5, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 5, 1, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 5, 2, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days2_january()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 1, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 2, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 1, 3, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_february()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 2, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 2, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 2, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 2, 6, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 2, 7, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_march()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 3, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 3, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 3, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 3, 6, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 3, 7, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_april()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 4, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 4, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 5, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 4, 3, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 4, 4, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_june()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 6, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 6, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 6, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 6, 5, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 6, 6, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_july()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 7, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 7, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 7, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 7, 3, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 7, 4, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_august()
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 8, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 8, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 8, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 8, 1, 8, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_first_week(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 2, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first thursday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer jueves de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first thursday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 2, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 2, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_second_week(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.Second;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 9, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the second thursday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el segundo jueves de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the second thursday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 9, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 9, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_third_week(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.Third;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 16, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the third thursday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el tercer jueves de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the third thursday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 16, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 16, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_fourth_week(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.Fourth;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 23, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the fourth thursday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el cuarto jueves de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the fourth thursday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 23, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 23, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_per_last_week(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.Last;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2020, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2020, 4, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 30, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the last thursday of every 3 months every 2 hours between 08:00 and 12:00 starting on 01/01/2020");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el último jueves de cada 3 meses cada 2 horas entre 8:00 y 12:00 empezando en 01/01/2020");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the last thursday of every 3 months every 2 hours between 8:00 am and 12:00 pm starting on 1/1/2020");
                    break;
            }
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, 30, 10, 0, 0));
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, 30, 12, 0, 0));
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_monday_day_1(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Monday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 2, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 2, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 2, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 2, 1, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first monday of every 3 months every 2 hours between 08:00 and 08:00 starting on 01/02/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer lunes de cada 3 meses cada 2 horas entre 8:00 y 8:00 empezando en 01/02/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first monday of every 3 months every 2 hours between 8:00 am and 8:00 am starting on 2/1/2021");
                    break;
            }
        }


        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_tuesday_day_1(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Tuesday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 6, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 6, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 7, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 6, 1, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first tuesday of every 3 months every 2 hours between 08:00 and 08:00 starting on 01/06/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer martes de cada 3 meses cada 2 horas entre 8:00 y 8:00 empezando en 01/06/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first tuesday of every 3 months every 2 hours between 8:00 am and 8:00 am starting on 6/1/2021");
                    break;
            }
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_friday_day_1(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Friday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 1, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 2, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 1, 8, 0, 0));            
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first friday of every 3 months every 2 hours between 08:00 and 08:00 starting on 01/01/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer viernes de cada 3 meses cada 2 horas entre 8:00 y 8:00 empezando en 01/01/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first friday of every 3 months every 2 hours between 8:00 am and 8:00 am starting on 1/1/2021");
                    break;
            }
        }

        [Theory]
        [InlineData(Languages.en_GB)]
        [InlineData(Languages.es_ES)]
        [InlineData(Languages.en_US)]
        public void Execute_recurring_MonthlyWeekly_more_sunday_day_1(Languages Language)
        {
            // Arrange
            Configuration CurrentConfiguration = new Configuration();
            CurrentConfiguration.Enabled = true;
            CurrentConfiguration.TimeType = TypeStep.Recurring;
            CurrentConfiguration.TypeRecurring = TypeTimeStep.Monthly;
            CurrentConfiguration.MonthlyMore = true;
            CurrentConfiguration.MonthlyMoreWeekStep = TypeWeekStep.First;
            CurrentConfiguration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Sunday;
            CurrentConfiguration.MonthlyMoreMonthSteps = 3;
            CurrentConfiguration.DateStep = new DateTime(2021, 8, 1);
            CurrentConfiguration.DateFrom = new DateTime(2021, 8, 1);
            CurrentConfiguration.DateTo = new DateTime(2021, 9, 20);
            CurrentConfiguration.HourStep = 2;
            CurrentConfiguration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            CurrentConfiguration.Language = Language;
            this.process = new Process.Schedule(CurrentConfiguration);

            //Act
            Output[] TheOutput = this.process.Execute(CurrentConfiguration.DateStep.Value, CurrentConfiguration);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 8, 1, 8, 0, 0));
            switch (CurrentConfiguration.Language)
            {
                case Languages.en_GB:
                    Assert.True(TheOutput[0].Description == "Occurs the first sunday of every 3 months every 2 hours between 08:00 and 08:00 starting on 01/08/2021");
                    break;
                case Languages.es_ES:
                    Assert.True(TheOutput[0].Description == "Ocurre el primer domingo de cada 3 meses cada 2 horas entre 8:00 y 8:00 empezando en 01/08/2021");
                    break;
                case Languages.en_US:
                    Assert.True(TheOutput[0].Description == "Occurs the first sunday of every 3 months every 2 hours between 8:00 am and 8:00 am starting on 8/1/2021");
                    break;
            }
        }
    }
}

