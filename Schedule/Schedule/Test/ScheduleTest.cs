using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Schedule.Process;
using Schedule.Config;
using Schedule.RecursosTextos;
using Semicrol.Utilidades;

namespace Schedule.Test
{
    public class ScheduleTest
    {
        private Configuration configuration;
        private Process.Schedule process;

        public ScheduleTest()
        {
            this.configuration = new Configuration();
        }

        [Fact]
        public void Execute_should_fill_configuration()
        {
            // Arrange
            this.configuration = null;

            var caughtException =
                     Assert.Throws<ScheduleException>(() => this.process = new Process.Schedule(this.configuration));

            //Assert
            Assert.Equal(Global.ValidateConfiguration, caughtException.Message);
        }


        [Fact]
        public void Execute_once_daily_should_exit_date_step()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.TimeType = TypeStep.Once;
            this.configuration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(LaFecha);

            //Assert
            Assert.Equal(this.configuration.DateStep, TheOutput[0].OutputDate);
        }

        [Fact]
        public void Execute_once_daily_should_fill_date_from()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Once;
            this.configuration.DateTo = new DateTime(2021, 1, 8);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(new DateTime(2021, 1, 1)));

            //Assert
            Assert.Equal(Global.ValidateDateConfiguration, caughtException.Message);
        }

        [Fact]
        public void Execute_once_daily_should_fill_date_step()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Once;
            this.configuration.DateStep = null;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateDateConfiguration, caughtException.Message);
        }


        [Fact]
        public void Execute_recurring_should_set_when_occurs()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.configuration.HourStep = 1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                  Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.True(Global.ValidateRecurringFrequency == caughtException.Message);
        }


        [Fact]
        public void Execute_recurring_daily_HourStep_should_be_higher_than_0()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Daily;
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.configuration.HourStep = 1;
            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(this.configuration.HourStep > 0);
        }

        [Fact]
        public void Execute_recurring_daily_each_day()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Daily;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.DateFrom = new DateTime(2021, 1, 1, 14, 0, 0);
            this.configuration.DateStep = new DateTime(2021, 1, 1, 14, 0, 0);
            this.configuration.DateTo = new DateTime(2021, 1, 4, 14, 0, 0);
            this.configuration.TypeRecurring = TypeTimeStep.Daily;
            this.configuration.DailyStep = 1;

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 2, 14, 0, 0));
            Assert.True(TheOutput[0].Description ==
                this.GenerateOutputDailyDescription(new DateTime(2021, 1, 2)));
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 3, 14, 0, 0));
            Assert.True(TheOutput[1].Description ==
                this.GenerateOutputDailyDescription(new DateTime(2021, 1, 3)));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 4, 14, 0, 0));
            Assert.True(TheOutput[2].Description ==
                this.GenerateOutputDailyDescription(new DateTime(2021, 1, 4)));
        }

        private string GenerateOutputDailyDescription(DateTime LaFecha)
        {
            return
                (string.Format(Global.Output, Global.every + " " + Global.day,
                (LaFecha.ToString("dd/MM/yyyy")) + " " + Global.at + " " + this.configuration.DateFrom.Value.ToString("HH:mm"))) + " " +
                string.Format(Global.StartingOn, this.configuration.DateFrom.Value.ToString("dd/MM/yyyy") + " " +
                this.configuration.DateFrom.Value.ToString("HH:mm"));
        }

        [Fact]
        public void Execute_recurring_daily_more_1_day()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.DateFrom = new DateTime(2021, 1, 1, 14, 0, 0);
            this.configuration.DateStep = new DateTime(2021, 1, 1, 14, 0, 0);
            this.configuration.DateTo = new DateTime(2021, 1, 4, 14, 0, 0);
            this.configuration.TypeRecurring = TypeTimeStep.Daily;
            this.configuration.DailyStep = 2;

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 2, 14, 0, 0));
            Assert.True(TheOutput[0].Description == this.GenerateOutputDailyMore1dayDescription(new DateTime(2021, 1, 2)));
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 4, 14, 0, 0));
            Assert.True(TheOutput[1].Description == this.GenerateOutputDailyMore1dayDescription(new DateTime(2021, 1, 4)));
        }


        private string GenerateOutputDailyMore1dayDescription(DateTime LaFecha)
        {
            return
                (string.Format(Global.Output, Global.every + " " + this.configuration.DailyStep.ToString() + " " + Global.days,
                (LaFecha.ToString("dd/MM/yyyy")) + " " + Global.at + " " + this.configuration.DateFrom.Value.ToString("HH:mm"))) + " " +
                string.Format(Global.StartingOn, this.configuration.DateFrom.Value.ToString("dd/MM/yyyy") + " " +
                this.configuration.DateFrom.Value.ToString("HH:mm"));
        }

        [Fact]
        public void Execute_recurring_WeeklyStep_should_be_bigger_than_0()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2020, 1, 4);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 1, 8);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.WeekStep = -1;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                   Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.True(Global.ValidateRecurringFrequency == caughtException.Message);
        }

        [Theory]
        [InlineData(8, 12)]
        [InlineData(10, 14)]
        public void Execute_recurring_Weekly(int HourFrom, int HourTo)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 1, 2);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 2;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, HourFrom, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, HourTo, 0, 0);
            this.configuration.WeeklyMonday = TheWeek.Contains(DayOfWeek.Monday);
            this.configuration.WeeklyTuesday = TheWeek.Contains(DayOfWeek.Tuesday);
            this.configuration.WeeklyWednesday = TheWeek.Contains(DayOfWeek.Wednesday);
            this.configuration.WeeklyThursday = TheWeek.Contains(DayOfWeek.Thursday);
            this.configuration.WeeklyFriday = TheWeek.Contains(DayOfWeek.Friday);
            this.configuration.WeeklySaturday = TheWeek.Contains(DayOfWeek.Saturday);
            this.configuration.WeeklySunday = TheWeek.Contains(DayOfWeek.Sunday);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput.Length == 6);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 1,
                this.configuration.HourFrom.Value.Hour, 0, 0));
            Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutput(TheWeek));
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 1,
                this.configuration.HourFrom.Value.Hour + this.configuration.HourStep.Value, 0, 0));
            Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutput(TheWeek));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 1,
                this.configuration.HourFrom.Value.Hour + this.configuration.HourStep.Value + 2, 0, 0));
            Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutput(TheWeek));
            Assert.True(TheOutput[3].OutputDate.Value == new DateTime(2021, 1, 2,
                this.configuration.HourFrom.Value.Hour, 0, 0));
            Assert.True(TheOutput[4].Description == this.GenerateWeeklyOutput(TheWeek));
            Assert.True(TheOutput[4].OutputDate.Value == new DateTime(2021, 1, 2,
                this.configuration.HourFrom.Value.Hour + this.configuration.HourStep.Value, 0, 0));
            Assert.True(TheOutput[4].Description == this.GenerateWeeklyOutput(TheWeek));
            Assert.True(TheOutput[5].OutputDate.Value == new DateTime(2021, 1, 2,
                this.configuration.HourFrom.Value.Hour + this.configuration.HourStep.Value + 2, 0, 0));
            Assert.True(TheOutput[5].Description == this.GenerateWeeklyOutput(TheWeek));
        }

        private string GenerateWeeklyOutput(List<DayOfWeek> TheWeek)
        {
            string DaysString = "";

            for (int Index = 0; Index < TheWeek.Count; Index++)
            {
                if (Index != TheWeek.Count - 2 && Index != TheWeek.Count - 1)
                    DaysString = DaysString + TheWeek[Index].ToString() + ", ";
                else if (Index == TheWeek.Count - 1)
                    DaysString = DaysString + TheWeek[Index].ToString();
                else if (Index == TheWeek.Count - 2)
                    DaysString = DaysString + TheWeek[Index].ToString() + " and ";
            }

            return
                string.Format(Global.ExitRecurring, Global.every + " " + this.configuration.WeekStep.ToString() +
                " " + Global.weeks + " " + Global.on + " " + DaysString, Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours, this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                this.configuration.HourTo.Value.ToShortTimeString(),
                this.configuration.DateFrom.Value.ToString("dd/MM/yyy"));
        }

        [Theory]
        [InlineData(DayOfWeek.Monday)]
        [InlineData(DayOfWeek.Tuesday)]
        [InlineData(DayOfWeek.Wednesday)]
        [InlineData(DayOfWeek.Thursday)]
        [InlineData(DayOfWeek.Friday)]
        [InlineData(DayOfWeek.Saturday)]
        [InlineData(DayOfWeek.Sunday)]
        public void Execute_recurring_Weekly_per_day(DayOfWeek TheDay)
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { TheDay });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 1, 14);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 2;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuration.WeeklyMonday = TheWeek.Contains(DayOfWeek.Monday);
            this.configuration.WeeklyTuesday = TheWeek.Contains(DayOfWeek.Tuesday);
            this.configuration.WeeklyWednesday = TheWeek.Contains(DayOfWeek.Wednesday);
            this.configuration.WeeklyThursday = TheWeek.Contains(DayOfWeek.Thursday);
            this.configuration.WeeklyFriday = TheWeek.Contains(DayOfWeek.Friday);
            this.configuration.WeeklySaturday = TheWeek.Contains(DayOfWeek.Saturday);
            this.configuration.WeeklySunday = TheWeek.Contains(DayOfWeek.Sunday);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput.Length == 3);
            if (TheWeek.Contains(DayOfWeek.Monday))
            {
                Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 11, 8, 0, 0));
                Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 11, 10, 0, 0));
                Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 11, 12, 0, 0));
                Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            }
            if (TheWeek.Contains(DayOfWeek.Tuesday))
            {
                Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 12, 8, 0, 0));
                Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 12, 10, 0, 0));
                Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 12, 12, 0, 0));
                Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            }
            if (TheWeek.Contains(DayOfWeek.Wednesday))
            {
                Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 13, 8, 0, 0));
                Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 13, 10, 0, 0));
                Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 13, 12, 0, 0));
                Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            }
            if (TheWeek.Contains(DayOfWeek.Thursday))
            {
                Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 14, 8, 0, 0));
                Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 14, 10, 0, 0));
                Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 14, 12, 0, 0));
                Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            }
            if (TheWeek.Contains(DayOfWeek.Friday))
            {
                Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 1, 8, 0, 0));
                Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 1, 10, 0, 0));
                Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 1, 12, 0, 0));
                Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            }
            if (TheWeek.Contains(DayOfWeek.Saturday))
            {
                Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 2, 8, 0, 0));
                Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 2, 10, 0, 0));
                Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 2, 12, 0, 0));
                Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            }
            if (TheWeek.Contains(DayOfWeek.Sunday))
            {
                Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 3, 8, 0, 0));
                Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 3, 10, 0, 0));
                Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutputPerDay(TheDay));
                Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 3, 12, 0, 0));
                Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            }
        }

        private string GenerateWeeklyOutputPerDay(DayOfWeek TheDay)
        {
            return
               string.Format(Global.ExitRecurring, Global.every + " " + this.configuration.WeekStep.ToString() +
               " " + Global.weeks + " " + Global.on + " " + TheDay.ToString(), Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours, this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
               this.configuration.HourTo.Value.ToShortTimeString(),
               this.configuration.DateFrom.Value.ToString("dd/MM/yyy"));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_monthly_configuration()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnceDay = 8;
            this.configuration.MonthlyOnceMonthSteps = 2;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.Equal(Global.ValidateMonthlyConfiguration, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_month_step_bigger_than_0()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 8;
            this.configuration.MonthlyOnceMonthSteps = 0;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.Equal(Global.ValidateMonthlyMonths, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_day_bigger_than_0_in_day_configuration()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = -1;
            this.configuration.MonthlyOnceMonthSteps = 1;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.Equal(Global.ValidateMonthlyOnceDayFrequency, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_month_bigger_than_0_in_day_configuration()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 1;
            this.configuration.MonthlyOnceMonthSteps = -1;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.Equal(Global.ValidateMonthlyMonths, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_hour_step_daily_frequency_bigger_than_0()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 8;
            this.configuration.MonthlyOnceMonthSteps = 1;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 0;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.Equal(Global.ValidateHourStepOfDailyFrequency, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_hour_from_should_be_smaller_than_hour_to()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 8;
            this.configuration.MonthlyOnceMonthSteps = 1;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeDailyFrequency = DailyFrequency.Every;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 10, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.Equal(Global.ValidateHourFromBigggerHourTo, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_should_set_month_frequency()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 8;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 1);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                  Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.True(Global.ValidateMonthlyOnceMonthFrequency == caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 8;
            this.configuration.MonthlyOnceMonthSteps = 2;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 8);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput.Length == 6);
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 8, 8, 0, 0));
            Assert.True(TheOutput[0].Description == this.GenerateMonthlyOnceOutputDescription());
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 1, 8, 10, 0, 0));
            Assert.True(TheOutput[1].Description == this.GenerateMonthlyOnceOutputDescription());
            Assert.True(TheOutput[2].OutputDate == new DateTime(2021, 1, 8, 12, 0, 0));
            Assert.True(TheOutput[2].Description == this.GenerateMonthlyOnceOutputDescription());
            Assert.True(TheOutput[3].OutputDate == new DateTime(2021, 3, 8, 8, 0, 0));
            Assert.True(TheOutput[3].Description == this.GenerateMonthlyOnceOutputDescription());
            Assert.True(TheOutput[4].OutputDate == new DateTime(2021, 3, 8, 10, 0, 0));
            Assert.True(TheOutput[4].Description == this.GenerateMonthlyOnceOutputDescription());
            Assert.True(TheOutput[5].OutputDate == new DateTime(2021, 3, 8, 12, 0, 0));
            Assert.True(TheOutput[5].Description == this.GenerateMonthlyOnceOutputDescription());
        }

        private string GenerateMonthlyOnceOutputDescription()
        {
            return string.Format(Global.ExitRecurring,
                 Global.day + " " +
                 this.configuration.MonthlyOnceDay.ToString(), Global.of + " " + Global.every + " " +
                 this.configuration.MonthlyOnceMonthSteps.ToString() + " " + Global.months + " " +
                 Global.every + " " + this.configuration.HourStep.Value.ToString() + " " + Global.hours,
                 this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                 this.configuration.HourTo.Value.ToShortTimeString(),
                 this.configuration.DateFrom.Value.ToString("dd/MM/yyy"));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_without_hour_from_and_hour_to()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            this.configuration.DateStep = new DateTime(2020, 1, 4);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 1, 27);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = null;
            this.configuration.HourTo = null;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateMonthlyMoreHourFromTo, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_not_avaliable_configuration()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2020, 1, 4);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 1, 27);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateMonthlyConfiguration, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_not_avaliable_huor_step()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2020, 1, 4);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 1, 27);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = -1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateHourStep, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_should_fill_date_step()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = null;
            this.configuration.DateFrom = new DateTime(2020, 1, 3);
            this.configuration.DateTo = new DateTime(2020, 1, 27);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = -1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateDateConfiguration, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_without_days()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.DateStep = new DateTime(2020, 1, 2);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 1, 27);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateMonthlyMoreWeekStep, caughtException.Message);
        }

        [Theory]
        [InlineData(TypeDayWeekStep.Thursday)]
        [InlineData(TypeDayWeekStep.Wednesday)]
        [InlineData(TypeDayWeekStep.Friday)]
        [InlineData(TypeDayWeekStep.Saturday)]
        [InlineData(TypeDayWeekStep.Sunday)]
        [InlineData(TypeDayWeekStep.Monday)]
        [InlineData(TypeDayWeekStep.Day)]
        [InlineData(TypeDayWeekStep.WeekDay)]
        [InlineData(TypeDayWeekStep.WeekendDay)]
        public void Execute_recurring_MonthlyWeekly_more_per_days(TypeDayWeekStep TheDay)
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TheDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2020, 1, 1);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 4, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TheDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            if (TheDay == TypeDayWeekStep.WeekDay)
            {
                this.CheckRecurringWeekdayOutput(TheOutput);
            }
            else if (TheDay == TypeDayWeekStep.WeekendDay)
            {
                this.CheckRecurringWeekenddayOutput(TheOutput);
            }
            else
            {
                this.CheckRecurringOutput(Index, TheOutput);
            }
        }

        private void CheckRecurringWeekenddayOutput(Output[] TheOutput)
        {
            int IndexDay = 4;
            int IndexMonth = 1;
            int IndexAdd = 0;

            for (int Index = 1; Index <= 4; Index++)
            {
                if (Index == 3)
                {
                    IndexDay = 4;
                    IndexMonth = IndexMonth + 3;
                }

                Assert.True(TheOutput[Index - 1 + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 8, 0, 0));
                Assert.True(TheOutput[Index - 1 + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());
                Assert.True(TheOutput[Index + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 10, 0, 0));
                Assert.True(TheOutput[Index + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());
                Assert.True(TheOutput[Index + 1 + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 12, 0, 0));
                Assert.True(TheOutput[Index + 1 + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());

                IndexDay = IndexDay + 1;
                IndexAdd = IndexAdd + 2;
            }
        }

        private void CheckRecurringWeekdayOutput(Output[] TheOutput)
        {
            int IndexAdd = 0;
            int IndexMonth = 1;
            int IndexDay = 0;

            for (int Index = 1; Index <= 10; Index++)
            {
                if (Index == 6)
                {
                    IndexMonth = IndexMonth  + 3;
                    IndexDay = 1;
                }
                else
                {
                    IndexDay = IndexDay + 1;
                }

                Assert.True(TheOutput[Index - 1 + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 8, 0, 0));
                Assert.True(TheOutput[Index - 1 + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());
                Assert.True(TheOutput[Index + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 10, 0, 0));
                Assert.True(TheOutput[Index + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());
                Assert.True(TheOutput[Index + 1 + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 12, 0, 0));
                Assert.True(TheOutput[Index + 1 + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());

                IndexAdd = IndexAdd + 2;
            }
        }

        private void CheckRecurringOutput(int Index, Output[] TheOutput)
        {
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, Index, 8, 0, 0));
            Assert.True(TheOutput[0].Description ==
                this.GetMonthlyMoreRecurringOutputDescription());
            Assert.True(TheOutput[1].OutputDate == new DateTime(2020, 1, Index, 10, 0, 0));
            Assert.True(TheOutput[1].Description ==
                this.GetMonthlyMoreRecurringOutputDescription());
            Assert.True(TheOutput[2].OutputDate == new DateTime(2020, 1, Index, 12, 0, 0));
            Assert.True(TheOutput[2].Description ==
                this.GetMonthlyMoreRecurringOutputDescription());
            Assert.True(TheOutput[3].OutputDate == new DateTime(2020, 4, Index, 8, 0, 0));
            Assert.True(TheOutput[3].Description ==
                this.GetMonthlyMoreRecurringOutputDescription());
            Assert.True(TheOutput[4].OutputDate == new DateTime(2020, 4, Index, 10, 0, 0));
            Assert.True(TheOutput[4].Description ==
                this.GetMonthlyMoreRecurringOutputDescription());
            Assert.True(TheOutput[5].OutputDate == new DateTime(2020, 4, Index, 12, 0, 0));
            Assert.True(TheOutput[5].Description ==
                this.GetMonthlyMoreRecurringOutputDescription());
        }

        private int GetIndexDay(TypeDayWeekStep TheDay, DateTime TheDayFrom)
        {
            int Index = Convert.ToInt32(TheDay) - 1;

            if (TheDay == TypeDayWeekStep.Monday)
            {
                Index = Convert.ToInt32(TheDay) + 6;
            }
            if (TheDay == TypeDayWeekStep.Tuesday)
            {
                Index = Convert.ToInt32(TheDay) + 7;
            }
            if (TheDay == TypeDayWeekStep.Day)
            {
                Index = new Mes(TheDayFrom).PrimerDia.Day;
            }

            return Index;
        }

        private string GetMonthlyMoreRecurringOutputDescription()
        {
            return string.Format(Global.ExitRecurring, Global.the + " " + this.configuration.MonthlyMoreWeekStep.ToString().ToLower() + " " +
                            this.configuration.MonthlyMoreOrderDayWeekStep.ToString().ToLower() + " " + Global.of + " " + Global.every + " " +
                            this.configuration.MonthlyMoreMonthSteps.ToString() + " " + Global.months,
                            Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours,
                            this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                            this.configuration.HourTo.Value.ToShortTimeString(),
                            this.configuration.DateFrom.Value.ToString("dd/MM/yyyy"));
        }

        [Theory]
        [InlineData(TypeWeekStep.First)]
        [InlineData(TypeWeekStep.Second)]
        [InlineData(TypeWeekStep.Third)]
        [InlineData(TypeWeekStep.Fourth)]
        [InlineData(TypeWeekStep.Last)]
        public void Execute_recurring_MonthlyWeekly_more_per_week(TypeWeekStep TheWeek)
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TheWeek;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2020, 1, 1);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 4, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            int IndexAdd = 0;
            int IndexDay = 2;
            int IndexMonth = 1;

            if (this.configuration.MonthlyMoreWeekStep != TypeWeekStep.First)
            {
                IndexDay = IndexDay - 7 + (7 * (Convert.ToInt32(this.configuration.MonthlyMoreWeekStep)));
            }

            for (int Index = 1; Index <= 2; Index++)
            {
                if (Index == 2)
                {
                    IndexMonth = IndexMonth + 3;
                }

                if (Index - 1 + IndexAdd == TheOutput.Length)
                {
                    break;
                }

                //Assert
                Assert.True(TheOutput[Index - 1 + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 8, 0, 0));
                Assert.True(TheOutput[Index - 1 + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());
                Assert.True(TheOutput[Index + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 10, 0, 0));
                Assert.True(TheOutput[Index + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());
                Assert.True(TheOutput[Index + 1 + IndexAdd].OutputDate == new DateTime(2020, IndexMonth, IndexDay, 12, 0, 0));
                Assert.True(TheOutput[Index + 1 + IndexAdd].Description ==
                    this.GetMonthlyMoreRecurringOutputDescription());

                IndexAdd = IndexAdd + 2;
            }
        }
    }
}

