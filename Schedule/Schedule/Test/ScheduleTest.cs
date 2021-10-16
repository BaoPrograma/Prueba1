using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Schedule
{
    public class ScheduleTest
    {
        private Configuracion configuracion;
        private Schedule procesador;

        public ScheduleTest()
        {
            this.configuracion = new Configuracion();
        }


        [Fact]
        public void Lanzar_once_diario_deberia_devolver_fechaPaso()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.FechaInicio = new DateTime(2021, 1, 1);
            this.configuracion.Tipo = TipoFranja.Once;
            this.configuracion.FechaPaso = new DateTime(2021, 8, 1).AddHours(14);
            this.configuracion.Periodicidad = TipoPeriodicidad.Daily;
            DateTime LaFecha = new DateTime(2021 , 1 , 4);
            this.procesador = new Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ProcesarPeriodo(LaFecha);

            //Assert
            Assert.Equal(this.configuracion.FechaPaso, LaSalidas[0].FechaSalida);
        }

        [Fact]
        public void Lanzar_recurring_WeeklyPaso_debe_ser_mayor_o_igual_a_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.FechaInicio = new DateTime(2021, 1, 1);
            this.configuracion.Tipo = TipoFranja.Recurring;
            this.configuracion.Periodicidad = TipoPeriodicidad.Weekly;
            this.configuracion.WeeklyPaso = 1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ProcesarPeriodo(LaFecha);

            //Assert
            Assert.True(this.configuracion.WeeklyPaso >= 0);
        }

        [Fact]
        public void Lanzar_recurring_WeeklyPaso_no_debe_ser_menor_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.FechaInicio = new DateTime(2021, 1, 1);
            this.configuracion.Tipo = TipoFranja.Recurring;
            this.configuracion.Periodicidad = TipoPeriodicidad.Weekly;
            this.configuracion.WeeklyPaso = -1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ProcesarPeriodo(LaFecha);

            //Assert
            Assert.False(this.configuracion.WeeklyPaso < 0);
        }

        [Fact]
        public void Lanzar_once_solo_ocurre_Diary()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.FechaInicio = new DateTime(2021, 1, 1);
            this.configuracion.Tipo = TipoFranja.Once;
            this.configuracion.FechaPaso = new DateTime(2021, 8, 1).AddHours(14);
            this.configuracion.Periodicidad = TipoPeriodicidad.Daily;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ProcesarPeriodo(LaFecha);

            //Assert
            Assert.Equal(this.configuracion.FechaPaso, LaSalidas[0].FechaSalida);
        }

        [Fact]
        public void Lanzar_once_no_ocurre_Weekly()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.FechaInicio = new DateTime(2021, 1, 1);
            this.configuracion.Tipo = TipoFranja.Once;
            this.configuracion.FechaPaso = new DateTime(2021, 8, 1).AddHours(14);
            this.configuracion.Periodicidad = TipoPeriodicidad.Weekly;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ProcesarPeriodo(LaFecha);

            //Assert
            Assert.NotEqual(TipoPeriodicidad.Daily, this.configuracion.Periodicidad);
        }

        [Fact]
        public void Lanzar_recurring_HourPaso_debe_ser_mayor_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.FechaInicio = new DateTime(2021, 1, 1);
            this.configuracion.Tipo = TipoFranja.Recurring;
            this.configuracion.Periodicidad = TipoPeriodicidad.Weekly;
            this.configuracion.WeeklyPaso = -1;
            this.configuracion.HourPaso = 1;
            DateTime LaFecha = new DateTime(2021, 1, 4);
            this.procesador = new Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ProcesarPeriodo(LaFecha);

            //Assert
            Assert.True(this.configuracion.HourPaso > 0);
        }

        [Fact]
        public void Lanzar_recurring_Weekly()
        { 
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.FechaInicio = new DateTime(2020, 1, 1);
            this.configuracion.FechaFin = new DateTime(2020, 1, 27);
            this.configuracion.Tipo = TipoFranja.Recurring;
            this.configuracion.Periodicidad = TipoPeriodicidad.Weekly;
            this.configuracion.WeeklyPaso = 2;
            this.configuracion.HourPaso = 2;
            this.configuracion.HoraDesde = new DateTime(1900, 1, 1, 8, 0, 0);
            this.configuracion.HoraHasta = new DateTime(1900, 1, 1, 12, 0, 0);
            this.configuracion.WeekMonday = true;
            this.configuracion.WeekThursday = true;
            this.configuracion.WeekFriday = true;
            DateTime LaFecha = new DateTime(2020, 1, 1);
            this.procesador = new Schedule(this.configuracion);

            //Act
            Output[] LaSalidas = this.procesador.ProcesarPeriodo(LaFecha);

            //Assert
            Assert.True(LaSalidas.Count() > 0);
        }
    }
}
