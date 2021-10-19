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
        private Configuration configuracion;
        private Process.Schedule procesador;

        public ScheduleTest()
        {
            this.configuracion = new Configuration();
        }


        [Fact]
        public void Execute_once_diary_should_exit_date_step()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateFrom = new DateTime(2021, 1, 1);
            this.configuracion.TimeType = TypeStep.Once;
            this.configuracion.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.configuracion.TypeStep = TypeDayStep.Daily;
            DateTime LaFecha = new DateTime(2021 , 1 , 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.Equal(this.configuracion.DateStep, LaSalidas[0].OutputDate);
        }

        [Fact]
        public void Execute_once_diary_should_fill_date_step()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.TimeType = TypeStep.Once;
            this.configuracion.DateStep = null;
            this.configuracion.TypeStep = TypeDayStep.Daily;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            var caughtException =
                    Assert.Throws<Exception>(() => this.procesador.ExecuteDateStep(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateHourStep, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_WeeklyStep_should_be_higher_o_equal_to_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2021, 1, 1);
            this.configuracion.DateTo = new DateTime(2021, 1, 8);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 1;
            this.configuracion.HourStep = 1;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekMonday = true;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(this.configuracion.WeekStep > 0);
        }

        [Fact]
        public void Execute_recurring_WeeklyStep_not_should_be_lower_than_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2021, 1, 1);
            this.configuracion.DateTo = new DateTime(2021, 1, 8);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 1;
            this.configuracion.HourStep = 1;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekMonday = true;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(this.configuracion.WeekStep > 0);
        }

        [Fact]
        public void Execute_once_only_occurs_Diary()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateFrom = new DateTime(2021, 1, 1);
            this.configuracion.TimeType = TypeStep.Once;
            this.configuracion.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.configuracion.TypeStep = TypeDayStep.Daily;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.Equal(this.configuracion.DateStep, LaSalidas[0].OutputDate);
        }

        [Fact]
        public void Execute_once_not_occurs_Weekly()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateFrom = new DateTime(2021, 1, 1);
            this.configuracion.TimeType = TypeStep.Once;
            this.configuracion.DateStep = new DateTime(2021, 8, 1).AddHours(14);
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.NotEqual(TypeDayStep.Daily, this.configuracion.TypeStep);
        }

        [Fact]
        public void Execute_recurring_HourStep_should_be_higher_than_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateFrom = new DateTime(2021, 1, 1);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 1;
            this.configuracion.HourStep = 1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(this.configuracion.HourStep > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_only_Monday()
        { 
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekMonday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }


        [Fact]
        public void Execute_recurring_Weekly_without_hour_from_and_hour_to()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = null;
            this.configuracion.HourTo = null;
            this.configuracion.WeekMonday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_only_Tuesday()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekTuesday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_only_Wednesday()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekWednesday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_only_Thursday()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekThursday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_only_Friday()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekFriday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_only_Saturday()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekSaturday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_only_Sunday()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekSunday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_multiple_days()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 2;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekMonday = true;
            this.configuracion.WeekThursday = true;
            this.configuracion.WeekSaturday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly_not_avaliable_week_step()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = -1;
            this.configuracion.HourStep = 2;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekMonday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            var caughtException = 
                    Assert.Throws<Exception>(() => this.procesador.ExecuteDateStep(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateWeeklyStep, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_Weekly_not_avaliable_huor_step()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = new DateTime(2020, 1, 4);
            this.configuracion.DateFrom = new DateTime(2020, 1, 1);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 1;
            this.configuracion.HourStep = -1;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekMonday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            var caughtException =
                    Assert.Throws<Exception>(() => this.procesador.ExecuteDateStep(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateHourStep, caughtException.Message);
        }

        [Fact]
        public void Execute_recurring_Weekly_should_fill_date_step()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateStep = null;
            this.configuracion.DateFrom = new DateTime(2020, 1, 3);
            this.configuracion.DateTo = new DateTime(2020, 1, 27);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 1;
            this.configuracion.HourStep = -1;
            this.configuracion.HourFrom = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HourTo = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekMonday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            var caughtException =
                    Assert.Throws<Exception>(() => this.procesador.ExecuteDateStep(LaFecha));

            //Assert
            Assert.Equal(Global.ValidateDateConfiguration, caughtException.Message);
        }
    }
}
