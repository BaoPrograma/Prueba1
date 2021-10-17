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
        public void Execute_recurring_WeeklyStep_should_be_higher_o_equal_to_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateFrom = new DateTime(2021, 1, 1);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = 1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(this.configuracion.WeekStep >= 0);
        }

        [Fact]
        public void Execute_recurring_WeeklyStep_not_should_be_lower_than_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.DateFrom = new DateTime(2021, 1, 1);
            this.configuracion.TimeType = TypeStep.Recurring;
            this.configuracion.TypeStep = TypeDayStep.Weekly;
            this.configuracion.WeekStep = -1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.False(this.configuracion.WeekStep < 0);
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
            this.configuracion.WeekStep = -1;
            this.configuracion.HourStep = 1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(this.configuracion.HourStep > 0);
        }

        [Fact]
        public void Execute_recurring_Weekly()
        { 
            // Arrange
            this.configuracion.Enabled = true;
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
            this.configuracion.WeekFriday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Process.Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ExecuteDateStep(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }
    }
}
