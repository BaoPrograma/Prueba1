using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Schedule.Process;
using Schedule.Config;
using Schedule.RecursosTextos;

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
        public void Execute_without_configuration_enabled_sholud_return_date()
        {
            // Arrange
            this.configuration.Enabled = false;
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.process = new Process.Schedule(this.configuration);

            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.Equal(TheOutput.Length, 1);
            Assert.Equal(TheOutput[0].OutputDate, this.configuration.DateStep.Value);
        }

        [Fact]
        public void Execute_solud_fill_date_from()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.process = new Process.Schedule(this.configuration);

            //Act
            var caughtException =
                    Assert.Throws<ScheduleException>(() => this.process.Execute(this.configuration.DateStep.Value));

            //Assert
            Assert.Equal(Global.ValidateDateConfiguration, caughtException.Message);
        }

        [Fact]
        public void Execute_should_fill_configuration()
        {
            // Arrange
            this.configuration = null;

            var caughtException =
                     Assert.Throws<ScheduleException>(() => 
                     this.process = new Process.Schedule(this.configuration));

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
        public void Execute_recurring_daily_should_set_frequency_when_occurs()
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

        [Fact]
        public void Execute_recurring_daily_more_one_day()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Daily;
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
            Assert.True(TheOutput[0].Description ==
                this.GenerateOutputDailyDescription(new DateTime(2021, 1, 2)));
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 4, 14, 0, 0));
            Assert.True(TheOutput[1].Description ==
                this.GenerateOutputDailyDescription(new DateTime(2021, 1, 4)));
        }

        private string GenerateOutputDailyDescription(DateTime LaFecha)
        {
            if (this.configuration.DailyStep > 1)
            {
                return
                    (string.Format(Global.Output, Global.every + " " + this.configuration.DailyStep.ToString() + " " + Global.days,
                    (LaFecha.ToShortDateString()) + " " + Global.at + " " + this.configuration.DateFrom.Value.ToShortTimeString())) + " " +
                    string.Format(Global.StartingOn, this.configuration.DateFrom.Value.ToShortDateString() + " " +
                    this.configuration.DateFrom.Value.ToShortTimeString());
            }
            else
            {
                return
                    (string.Format(Global.Output, Global.every + " " + Global.day,
                    (LaFecha.ToShortDateString()) + " " + Global.at + " " + this.configuration.DateFrom.Value.ToShortTimeString())) + " " +
                    string.Format(Global.StartingOn, this.configuration.DateFrom.Value.ToShortDateString() + " " +
                    this.configuration.DateFrom.Value.ToShortTimeString());
            }
        }

        [Fact]
        public void Execute_recurring_WeeklyStep_should_be_bigger_than_0()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
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
            Assert.True(Global.ValidateWeeklyStep == caughtException.Message);
        }
                
        [Fact]
        public void Execute_recurring_Weekly_per_one_week()
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Monday });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 1, 14);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 1;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.WeeklyMonday = true;

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            string TheOutputDescruipction = string.Format(Global.ExitRecurring, Global.every + " " + Global.week + " " + Global.on + " " + Global.Monday, Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours, this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                  this.configuration.HourTo.Value.ToShortTimeString(), this.configuration.DateFrom.Value.ToShortDateString());

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 4, 8, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description && 
                TheOutput[0].Description == TheOutputDescruipction);
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 11, 8, 0, 0));
        }


        [Fact]
        public void Execute_recurring_Weekly_per_one_hour()
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Monday });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 1, 14);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 1;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.WeeklyMonday = true;

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            string TheOutputDescruipction = string.Format(Global.ExitRecurring, Global.every + " " + Global.week + " " + Global.on + " " + Global.Monday, Global.every + " " + Global.hour, this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                  this.configuration.HourTo.Value.ToShortTimeString(), this.configuration.DateFrom.Value.ToShortDateString());

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 4, 8, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description &&
                TheOutput[0].Description == TheOutputDescruipction);
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 11, 8, 0, 0));
        }

        [Theory]
        [InlineData(8, 12)]
        [InlineData(10, 14)]
        [InlineData(15, 19)]
        [InlineData(19, 23)]
        public void Execute_recurring_Weekly_per_days_and_hours(int HourFrom, int HourTo)
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
            this.GenerateWeeklyRecurringOutput(TheWeek, TheOutput);
        }

        private void GenerateWeeklyRecurringOutput(List<DayOfWeek> TheWeek, Output[] TheOutput)
        {
            string TheOutputDescription = this.GenerateWeeklyOutput(TheWeek);

            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 1,
                this.configuration.HourFrom.Value.Hour, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description &&
                TheOutput[1].Description == TheOutput[2].Description &&
                TheOutput[2].Description == TheOutput[3].Description &&
                TheOutput[4].Description == TheOutput[5].Description && TheOutput[0].Description == TheOutputDescription);
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 1,
                this.configuration.HourFrom.Value.Hour + this.configuration.HourStep.Value, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 1,
                this.configuration.HourFrom.Value.Hour + this.configuration.HourStep.Value + 2, 0, 0));
            Assert.True(TheOutput[3].OutputDate.Value == new DateTime(2021, 1, 2,
                this.configuration.HourFrom.Value.Hour, 0, 0));
            Assert.True(TheOutput[4].OutputDate.Value == new DateTime(2021, 1, 2,
                this.configuration.HourFrom.Value.Hour + this.configuration.HourStep.Value, 0, 0));
            Assert.True(TheOutput[5].OutputDate.Value == new DateTime(2021, 1, 2,
                this.configuration.HourFrom.Value.Hour + this.configuration.HourStep.Value + 2, 0, 0));
        }

        [Theory]
        [InlineData(DayOfWeek.Monday)]
        [InlineData(DayOfWeek.Tuesday)]
        [InlineData(DayOfWeek.Wednesday)]
        [InlineData(DayOfWeek.Thursday)]
        [InlineData(DayOfWeek.Friday)]
        [InlineData(DayOfWeek.Saturday)]
        [InlineData(DayOfWeek.Sunday)]
        public void Execute_recurring_Weekly_per_each_day(DayOfWeek TheDay)
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
            this.CheckOutputWeekly(TheOutput, TheDay);
        }

        [Fact]       
        public void Execute_recurring_Weekly_saturady_begin()
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Saturday });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 2);
            this.configuration.DateFrom = new DateTime(2021, 1, 2);
            this.configuration.DateTo = new DateTime(2021, 1, 30);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 2;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.WeeklySaturday = TheWeek.Contains(DayOfWeek.Saturday);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            string TheOutputDescription = this.GenerateWeeklyOutputPerDay(DayOfWeek.Saturday);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 2, 8, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description &&
                TheOutput[1].Description == TheOutput[2].Description && TheOutput[0].Description == TheOutputDescription);
            Assert.True(TheOutput[0].Description == TheOutputDescription);
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 16, 8, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 30, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_Weekly_sunday_begin()
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Sunday });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 3);
            this.configuration.DateFrom = new DateTime(2021, 1, 3);
            this.configuration.DateTo = new DateTime(2021, 1, 31);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 2;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.WeeklySunday = TheWeek.Contains(DayOfWeek.Sunday);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            string TheOutputDescription = this.GenerateWeeklyOutputPerDay(DayOfWeek.Sunday);

            //Assert
            Assert.True(TheOutput.Length == 3);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 3, 8, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description &&
                TheOutput[1].Description == TheOutput[2].Description && TheOutput[0].Description == TheOutputDescription);
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 17, 8, 0, 0));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, 31, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_Weekly_thursday_begin()
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Thursday });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 7);
            this.configuration.DateFrom = new DateTime(2021, 1, 7);
            this.configuration.DateTo = new DateTime(2021, 1, 31);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 2;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.WeeklyThursday = TheWeek.Contains(DayOfWeek.Thursday);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            string TheOutputDescription = this.GenerateWeeklyOutputPerDay(DayOfWeek.Thursday);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 7, 8, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description && TheOutput[0].Description == TheOutputDescription);
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 21, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_Weekly_wednesday_begin()
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Wednesday });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 6);
            this.configuration.DateFrom = new DateTime(2021, 1, 6);
            this.configuration.DateTo = new DateTime(2021, 1, 31);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 2;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.WeeklyWednesday = TheWeek.Contains(DayOfWeek.Wednesday);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            string TheOutputDescription = this.GenerateWeeklyOutputPerDay(DayOfWeek.Wednesday);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 6, 8, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description && TheOutput[0].Description == TheOutputDescription);
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 20, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_Weekly_tuesday_begin()
        {
            List<DayOfWeek> TheWeek = new List<DayOfWeek>();
            TheWeek.AddRange(new DayOfWeek[] { DayOfWeek.Tuesday });

            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2021, 1, 5);
            this.configuration.DateFrom = new DateTime(2021, 1, 5);
            this.configuration.DateTo = new DateTime(2021, 1, 31);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Weekly;
            this.configuration.WeekStep = 2;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.WeeklyTuesday = TheWeek.Contains(DayOfWeek.Tuesday);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            string TheOutputDescription = this.GenerateWeeklyOutputPerDay(DayOfWeek.Tuesday);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, 5, 8, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description && TheOutput[0].Description == TheOutputDescription);
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, 19, 8, 0, 0));
        }

        private void CheckOutputWeekly(Output[] TheOutput, DayOfWeek TheDay)
        {
            int Index = 1;

            switch (TheDay)
            {
                case DayOfWeek.Monday:
                    Index = Index + 10;
                    break;
                case DayOfWeek.Tuesday:
                    Index = Index + 11;
                    break;
                case DayOfWeek.Wednesday:
                    Index = Index + 12;
                    break;
                case DayOfWeek.Thursday:
                    Index = Index + 13;
                    break;
                case DayOfWeek.Friday:
                    Index = 1;
                    break;
                case DayOfWeek.Saturday:
                    Index = 2;
                    break;
                case DayOfWeek.Sunday:
                    Index = 3;
                    break;

            }

            Assert.True(TheOutput[0].OutputDate.Value == new DateTime(2021, 1, Index, 8, 0, 0));
            Assert.True(TheOutput[0].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            Assert.True(TheOutput[1].OutputDate.Value == new DateTime(2021, 1, Index, 10, 0, 0));
            Assert.True(TheOutput[1].Description == this.GenerateWeeklyOutputPerDay(TheDay));
            Assert.True(TheOutput[2].OutputDate.Value == new DateTime(2021, 1, Index, 12, 0, 0));
            Assert.True(TheOutput[2].Description == this.GenerateWeeklyOutputPerDay(TheDay));
        }

        private string GenerateWeeklyOutputPerDay(DayOfWeek TheDay)
        {
            string TheHoursStr = "";
            if (this.configuration.HourStep > 1)
            {
                TheHoursStr = this.configuration.HourStep.ToString() + " " + Global.hours;
            }
            else
            {
                TheHoursStr = Global.hour;
            }

            return
               string.Format(Global.ExitRecurring, Global.every + " " + this.configuration.WeekStep.ToString() +
               " " + Global.weeks + " " + Global.on + " " + TheDay.ToString(), Global.every + " " + TheHoursStr, this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
               this.configuration.HourTo.Value.ToShortTimeString(),
               this.configuration.DateFrom.Value.ToShortDateString());
        }

        [Fact]
        public void Recurring_MonthlyMore_WeekStep_monthlyWeekVar_should_not_be_null()
        {
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Day;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2020, 1, 1);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 4, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            //Act
            this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.Equal(true, (this.process.MonthlyWeekVar != null));
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
            Assert.True(Global.ValidateMonthlyConfiguration == caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_per_months()
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

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_per_one_month()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 8;
            this.configuration.MonthlyOnceMonthSteps = 1;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 2, 8);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 8, 8, 0, 0));
            Assert.True(TheOutput[0].Description == TheOutput[1].Description  && 
                TheOutput[0].Description == this.GenerateMonthlyOnceOutputDescription());
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 2, 8, 8, 0, 0));
        }


        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_per_day_31()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 31;
            this.configuration.MonthlyOnceMonthSteps = 1;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 4, 8);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput.Length == 2);
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 31, 8, 0, 0));
            Assert.True(TheOutput[0].Description == this.GenerateMonthlyOnceOutputDescription());
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 3, 31, 8, 0, 0));
            Assert.True(TheOutput[1].Description == this.GenerateMonthlyOnceOutputDescription());
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_once_datestep_bigger_than_MonthlyOnceDay()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyOnce = true;
            this.configuration.MonthlyOnceDay = 8;
            this.configuration.MonthlyOnceMonthSteps = 2;
            this.configuration.DateStep = new DateTime(2021, 1, 10);
            this.configuration.DateFrom = new DateTime(2021, 1, 10);
            this.configuration.DateTo = new DateTime(2021, 3, 10);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);

            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput.Length == 1);
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 3, 8, 8, 0, 0));
            Assert.True(TheOutput[0].Description == this.GenerateMonthlyOnceOutputDescription());
        }

        private string GenerateMonthlyOnceOutputDescription()
        {
            string TheHoursStr = "";
            if (this.configuration.HourStep.Value > 1)
            {
                TheHoursStr = this.configuration.HourStep.Value.ToString() + " " + Global.hours;
            }
            else
            {
                TheHoursStr = Global.hour;
            }

            if (this.configuration.MonthlyOnceMonthSteps > 1)
            {
                return string.Format(Global.ExitRecurring,
                 Global.day + " " +
                 this.configuration.MonthlyOnceDay.ToString(), Global.of + " " + Global.every + " " +
                 this.configuration.MonthlyOnceMonthSteps.ToString() + " " + Global.months + " " +
                 Global.every + " " + TheHoursStr,
                 this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                 this.configuration.HourTo.Value.ToShortTimeString(), this.configuration.DateFrom.Value.ToShortDateString());
            }
            else
            {
                return string.Format(Global.ExitRecurring,
                Global.day + " " + this.configuration.MonthlyOnceDay.ToString(), Global.of + " " + Global.every + " " +
                Global.month + " " + Global.every + " " + TheHoursStr,
                this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                this.configuration.HourTo.Value.ToShortTimeString(), this.configuration.DateFrom.Value.ToShortDateString());
            }
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
            this.configuration.MonthlyMoreMonthSteps = 1;
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
        public void Execute_recurring_MonthlyWeekly_more_should_set_MonthlyConfiguration()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.DateStep = new DateTime(2020, 1, 4);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 1, 27);
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.HourStep = 2;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
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
            this.configuration.MonthlyMoreMonthSteps = 1;
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
        [InlineData(TypeDayWeekStep.Tuesday)]
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
                this.CheckRecurringWeekendayOutput(TheOutput);
            }
            else
            {
                this.CheckRecurringOutput(Index, TheOutput);
            }
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_one_hour()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2020, 1, 1);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 2, 20);
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.Saturday, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 2, 8, 0, 0));
            Assert.True(TheOutput[0].Description ==
                string.Format(Global.ExitRecurring, Global.the + " " + this.configuration.MonthlyMoreWeekStep.ToString().ToLower() + " " + this.configuration.MonthlyMoreOrderDayWeekStep.ToString().ToLower() + " " + Global.of + " " + Global.every + " " + this.configuration.MonthlyMoreMonthSteps.ToString() + " " + Global.months,
                            Global.every + " " + Global.hour,
                            this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                            this.configuration.HourTo.Value.ToShortTimeString(),
                            this.configuration.DateFrom.Value.ToShortDateString()));
        }


        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_one_month()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Thursday;
            this.configuration.MonthlyMoreMonthSteps = 1;
            this.configuration.DateStep = new DateTime(2020, 1, 1);
            this.configuration.DateFrom = new DateTime(2020, 1, 1);
            this.configuration.DateTo = new DateTime(2020, 1, 20);
            this.configuration.HourStep = 1;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.Saturday, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2020, 1, 2, 8, 0, 0));
            Assert.True(TheOutput[0].Description ==
                string.Format(Global.ExitRecurring, Global.the + " " + this.configuration.MonthlyMoreWeekStep.ToString().ToLower() + " " + this.configuration.MonthlyMoreOrderDayWeekStep.ToString().ToLower() + " " + Global.of + " " + Global.every + " "  + Global.month,
                            Global.every + " " + Global.hour,
                            this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                            this.configuration.HourTo.Value.ToShortTimeString(),
                            this.configuration.DateFrom.Value.ToShortDateString()));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_may()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 5, 1);
            this.configuration.DateFrom = new DateTime(2021, 5, 1);
            this.configuration.DateTo = new DateTime(2021, 5, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.WeekendDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 5, 1, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 5, 2, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days2_january()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 1, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.WeekendDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 2, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 1, 3, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_february()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 2, 1);
            this.configuration.DateFrom = new DateTime(2021, 2, 1);
            this.configuration.DateTo = new DateTime(2021, 2, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.WeekendDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 2, 6, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 2, 7, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_march()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 3, 1);
            this.configuration.DateFrom = new DateTime(2021, 3, 1);
            this.configuration.DateTo = new DateTime(2021, 3, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.WeekendDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 3, 6, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 3, 7, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_april()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 4, 1);
            this.configuration.DateFrom = new DateTime(2021, 4, 1);
            this.configuration.DateTo = new DateTime(2021, 5, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.WeekendDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 4, 3, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 4, 4, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_june()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 6, 1);
            this.configuration.DateFrom = new DateTime(2021, 6, 1);
            this.configuration.DateTo = new DateTime(2021, 6, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.WeekendDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 6, 5, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 6, 6, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_july()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 7, 1);
            this.configuration.DateFrom = new DateTime(2021, 7, 1);
            this.configuration.DateTo = new DateTime(2021, 7, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.WeekendDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 7, 3, 8, 0, 0));
            Assert.True(TheOutput[1].OutputDate == new DateTime(2021, 7, 4, 8, 0, 0));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_weekend_days_august()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.WeekendDay;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 8, 1);
            this.configuration.DateFrom = new DateTime(2021, 8, 1);
            this.configuration.DateTo = new DateTime(2021, 8, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            int Index = this.GetIndexDay(TypeDayWeekStep.WeekendDay, this.configuration.DateFrom.Value);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 8, 1, 8, 0, 0));
        }

        private void CheckRecurringWeekendayOutput(Output[] TheOutput)
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

            if (TheDay == TypeDayWeekStep.Monday || TheDay == TypeDayWeekStep.Tuesday)
            {
                Index = Convert.ToInt32(TheDay) + 6;
            }
            if (TheDay == TypeDayWeekStep.Day)
            {
                Index = (new DateTime(TheDayFrom.Year, TheDayFrom.Month, 1)).Day;
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
                            this.configuration.DateFrom.Value.ToShortDateString());
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

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_monday_day_1()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Monday;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 2, 1);
            this.configuration.DateFrom = new DateTime(2021, 2, 1);
            this.configuration.DateTo = new DateTime(2021, 2, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 2, 1, 8, 0, 0));
            Assert.True(TheOutput[0].Description ==
                string.Format(Global.ExitRecurring, Global.the + " " + this.configuration.MonthlyMoreWeekStep.ToString().ToLower() + " " + this.configuration.MonthlyMoreOrderDayWeekStep.ToString().ToLower() + " " + Global.of + " " + Global.every + " " +
                            this.configuration.MonthlyMoreMonthSteps.ToString() + " " + Global.months,
                            Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours,
                            this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                            this.configuration.HourTo.Value.ToShortTimeString(),
                            this.configuration.DateFrom.Value.ToShortDateString()));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_tuesday_day_1()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Tuesday;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 6, 1);
            this.configuration.DateFrom = new DateTime(2021, 6, 1);
            this.configuration.DateTo = new DateTime(2021, 7, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 6, 1, 8, 0, 0));
            Assert.True(TheOutput[0].Description ==
                string.Format(Global.ExitRecurring, Global.the + " " + this.configuration.MonthlyMoreWeekStep.ToString().ToLower() + " " + this.configuration.MonthlyMoreOrderDayWeekStep.ToString().ToLower() + " " + Global.of + " " + Global.every + " " +
                            this.configuration.MonthlyMoreMonthSteps.ToString() + " " + Global.months,
                            Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours,
                            this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                            this.configuration.HourTo.Value.ToShortTimeString(),
                            this.configuration.DateFrom.Value.ToShortDateString()));
        }


        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_friday_day_1()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Friday;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021,1, 1);
            this.configuration.DateFrom = new DateTime(2021, 1, 1);
            this.configuration.DateTo = new DateTime(2021, 2, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 1, 1, 8, 0, 0));
            Assert.True(TheOutput[0].Description ==
                string.Format(Global.ExitRecurring, Global.the + " " + this.configuration.MonthlyMoreWeekStep.ToString().ToLower() + " " + this.configuration.MonthlyMoreOrderDayWeekStep.ToString().ToLower() + " " + Global.of + " " + Global.every + " " +
                            this.configuration.MonthlyMoreMonthSteps.ToString() + " " + Global.months,
                            Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours,
                            this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                            this.configuration.HourTo.Value.ToShortTimeString(),
                            this.configuration.DateFrom.Value.ToShortDateString()));
        }

        [Fact]
        public void Execute_recurring_MonthlyWeekly_more_sunday_day_1()
        {
            // Arrange
            this.configuration.Enabled = true;
            this.configuration.TimeType = TypeStep.Recurring;
            this.configuration.TypeRecurring = TypeTimeStep.Monthly;
            this.configuration.MonthlyMore = true;
            this.configuration.MonthlyMoreWeekStep = TypeWeekStep.First;
            this.configuration.MonthlyMoreOrderDayWeekStep = TypeDayWeekStep.Sunday;
            this.configuration.MonthlyMoreMonthSteps = 3;
            this.configuration.DateStep = new DateTime(2021, 8, 1);
            this.configuration.DateFrom = new DateTime(2021, 8, 1);
            this.configuration.DateTo = new DateTime(2021, 9, 20);
            this.configuration.HourStep = 2;
            this.configuration.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuration.HourTo = new DateTime(1900, 1, 1, 8, 0, 0);
            this.process = new Process.Schedule(this.configuration);

            //Act
            Output[] TheOutput = this.process.Execute(this.configuration.DateStep.Value);

            //Assert
            Assert.True(TheOutput[0].OutputDate == new DateTime(2021, 8, 1, 8, 0, 0));
            Assert.True(TheOutput[0].Description ==
                string.Format(Global.ExitRecurring, Global.the + " " + this.configuration.MonthlyMoreWeekStep.ToString().ToLower() + " " + this.configuration.MonthlyMoreOrderDayWeekStep.ToString().ToLower() + " " + Global.of + " " + Global.every + " " +
                            this.configuration.MonthlyMoreMonthSteps.ToString() + " " + Global.months,
                            Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours,
                            this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                            this.configuration.HourTo.Value.ToShortTimeString(),
                            this.configuration.DateFrom.Value.ToShortDateString()));
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

            if (this.configuration.WeekStep > 1)
            {
                return string.Format(Global.ExitRecurring, Global.every + " " + this.configuration.WeekStep.ToString() +
                    " " + Global.weeks + " " + Global.on + " " + DaysString, Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours, this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                    this.configuration.HourTo.Value.ToShortTimeString(), this.configuration.DateFrom.Value.ToShortDateString());
            }
            else
            {
                return
                   string.Format(Global.ExitRecurring, Global.every + " " +
                   " " + Global.week + " " + Global.on + " " + DaysString, Global.every + " " + this.configuration.HourStep.ToString() + " " + Global.hours, this.configuration.HourFrom.Value.ToShortTimeString() + " and " +
                   this.configuration.HourTo.Value.ToShortTimeString(), this.configuration.DateFrom.Value.ToShortDateString());
            }
        }
    }
}

