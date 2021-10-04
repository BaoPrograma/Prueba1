using Schedule.RecursosTextos;
using System;
using System.Collections.Generic;

namespace Schedule
{
    public class Schedule
    {
        private Configuracion configuracion;

        public Schedule(Configuracion Laconfiguracion)
        {
            this.configuracion = Laconfiguracion;
        }

        public Output[] Procesar(DateTime LaFecha)
        {
            if (this.configuracion.Enabled)
            {
               return this.ProcesarDias(LaFecha);
            }
            else
            {
                return new Output[] {this.DevolverSalida("",
                   LaFecha, LaFecha, this.configuracion.FechaInicio) };
            }
        }

        private Output DevolverSalida(string ElTipo, DateTime LaFechaPaso, DateTime LaHoraPaso, DateTime? LaFechaInicio)
        {
            string LaFechaHoraPasoStr = LaFechaPaso.ToString("dd/MM/yyyy");

            if (LaHoraPaso.TimeOfDay > new TimeSpan(00, 00, 00))
            {
                LaFechaHoraPasoStr += " at " + LaHoraPaso.ToString("HH:mm");
            }

            Output LaSalida = new Output();
            LaSalida.FechaSalida = LaFechaPaso;
            LaSalida.Descripcion = string.Format(Global.Salida,ElTipo, LaFechaHoraPasoStr) +
                (LaFechaInicio != null?" " + string.Format(Global.StartingOn
                , LaFechaInicio.Value.ToString("dd/MM/yyyy")):"");

            return LaSalida;
        }

        private Output[] ProcesarDias(DateTime LaFecha)
        {
            string ElTipoStr = "";
            if (this.configuracion.Tipo == TipoFranja.Once)
            {
                ElTipoStr = "once";
            }
            else
            {
                ElTipoStr = "every day";
            }

            if (this.configuracion.Tipo == TipoFranja.Once)
            {
                return ProcesarOnce(ElTipoStr);
            }
            else
            {
                return ProcesarRecurring(LaFecha, ElTipoStr);
            }
        }

        private Output[] ProcesarRecurring(DateTime LaFecha, string ElTipoStr)
        {
            List<Output> LasSalidas = new List<Output>();

            for (DateTime CadaFecha = LaFecha.AddDays(1); CadaFecha <= LaFecha.AddDays(this.configuracion.DiasPeriodicidad); CadaFecha = CadaFecha.AddDays(1))
            {
                LasSalidas.Add(this.DevolverSalida(ElTipoStr, CadaFecha, CadaFecha, this.configuracion.FechaInicio));
            }

            return LasSalidas.ToArray();
        }

        private Output[] ProcesarOnce(string ElTipoStr)
        {
            if (this.configuracion.FechaPaso == null)
            {
                throw new Exception("Debe indicar la Fecha de Configuración");
            }

            if ((this.configuracion.FechaInicio != null &&
                this.configuracion.FechaPaso > this.configuracion.FechaInicio) ||
                (this.configuracion.FechaInicio == null))
            {
                return new Output[]{this.DevolverSalida(ElTipoStr,
                        this.configuracion.FechaPaso.Value,
                        this.configuracion.FechaPaso.Value, this.configuracion.FechaInicio) };
            }

            return null;
        }
    }
}
