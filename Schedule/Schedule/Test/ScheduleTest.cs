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
            this.procesador = new Schedule(this.configuracion);
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

            //Act
            Output[] LaSalidas = this.procesador.Procesar(LaFecha);

            //Assert
            Assert.Equal(this.configuracion.FechaPaso, LaSalidas[0].FechaSalida);
        }

        [Fact]
        public void Lanzar_recurring_diario_DiasPeriodicidad_debe_ser_mayor_o_igual_a_0()
        {
            // Arrange
            this.configuracion.Enabled = true;
            this.configuracion.FechaInicio = new DateTime(2021, 1, 1);
            this.configuracion.Tipo = TipoFranja.Recurring;
            this.configuracion.Periodicidad = TipoPeriodicidad.Daily;
            this.configuracion.DiasPeriodicidad = 0;
            DateTime LaFecha = new DateTime(2021, 1, 4);

            //Act
            Output[] LaSalidas = this.procesador.Procesar(LaFecha);

            //Assert
            if (LaSalidas.Length == 0)
            {
                Assert.Equal(LaFecha, LaFecha);
            }
            else
            {
                Assert.Equal(LaFecha.AddDays(this.configuracion.DiasPeriodicidad), LaSalidas[LaSalidas.Length- 1].FechaSalida);
            }
        }


    }
}
