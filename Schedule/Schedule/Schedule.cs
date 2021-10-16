using Schedule.RecursosTextos;
using Semicrol.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Schedule
{
    public class Schedule
    {
        private Configuracion configuracion;
        private DayOfWeek[] week;

        public Schedule(Configuracion Laconfiguracion)
        {
            this.configuracion = Laconfiguracion;

            this.InicializarWeek();
        }

        private void InicializarWeek()
        {
            List<DayOfWeek> LaLista = new List<DayOfWeek>();

            if (this.configuracion.WeekMonday)
                LaLista.Add(DayOfWeek.Monday);
            if (this.configuracion.WeekTuesday)
                LaLista.Add(DayOfWeek.Tuesday);
            if (this.configuracion.WeekWednesday)
                LaLista.Add(DayOfWeek.Wednesday);
            if (this.configuracion.WeekThursday)
                LaLista.Add(DayOfWeek.Thursday);
            if (this.configuracion.WeekFriday)
                LaLista.Add(DayOfWeek.Friday);
            if (this.configuracion.WeekSaturday)
                LaLista.Add(DayOfWeek.Saturday);
            if (this.configuracion.WeekSunday)
                LaLista.Add(DayOfWeek.Sunday);

            this.week = LaLista.ToArray();
        }

        public Output[] ProcesarPeriodo(DateTime LaFecha)
        {
            if (this.configuracion.Enabled)
            {
               return this.Procesar(LaFecha);
            }
            else
            {
                return new Output[] {this.DevolverSalida("",
                   LaFecha, LaFecha, this.configuracion.FechaInicio, null)};
            }
        }

        private Output DevolverSalida(string ElTipo, DateTime LaFechaPaso, DateTime LaHoraPaso, DateTime? LaFechaInicio, DateTime? LaHora)
        {
            string LaFechaHoraPasoStr = LaFechaPaso.ToString("dd/MM/yyyy");

            if (LaHoraPaso.TimeOfDay > new TimeSpan(00, 00, 00))
            {
                LaFechaHoraPasoStr += Global.at + " " + LaHoraPaso.ToString("HH:mm");
            }

            Output LaSalida = new Output();
            LaSalida.FechaSalida = LaFechaPaso;
            LaSalida.Descripcion = 
                string.Format(Global.Salida,ElTipo, LaFechaHoraPasoStr) +
                (LaFechaInicio != null?" " + string.Format(Global.StartingOn
                , LaFechaInicio.Value.ToString("dd/MM/yyyy")):"") + 
                " " + LaHora != null?LaHora.Value.ToString("HH:mm"): "00:00";

            return LaSalida;
        }

        private Output[] Procesar(DateTime LaFecha)
        {
            string ElTipoPeriodoDiaStr = "";
            if (this.configuracion.Tipo == TipoFranja.Once)
            {
                ElTipoPeriodoDiaStr = Global.once;
            }
            else
            {
                if (this.configuracion.Periodicidad == TipoPeriodicidad.Daily)
                {
                    ElTipoPeriodoDiaStr = Global.every + Global.day;
                }
                else
                {
                    ElTipoPeriodoDiaStr = Global.every + " " + this.configuracion.WeeklyPaso + " " + Global.weeks;
                }
            }

            if (this.configuracion.Tipo == TipoFranja.Once)
            {
                return ProcesarOnce(ElTipoPeriodoDiaStr);
            }
            else
            {
                return ProcesarRecurring(LaFecha, ElTipoPeriodoDiaStr);
            }
        }

        private Output[] ProcesarRecurring(DateTime LaFecha, string ElTipoPeriodoDiaStr)
        {
            List<Output> LasSalidas = new List<Output>();

            if (this.configuracion.Periodicidad == TipoPeriodicidad.Daily)
            {
                for (DateTime CadaFecha = LaFecha.AddDays(1); CadaFecha <= LaFecha; CadaFecha = CadaFecha.AddDays(1))
                {
                    LasSalidas.Add(this.DevolverSalida(ElTipoPeriodoDiaStr, CadaFecha, CadaFecha, this.configuracion.FechaInicio, null));
                }
            }
            else
            {
                DateTime LaFechafin = this.configuracion.FechaFin != null ? this.configuracion.FechaFin.Value : DateTime.MaxValue;
                int WeekPaso = this.configuracion.WeeklyPaso > 0? this.configuracion.WeeklyPaso: 0;

                DateTime LaFechaSemana = LaFecha;

                for (DateTime CadaFechaSup = LaFecha; CadaFechaSup <= LaFechafin;
                    CadaFechaSup = this.GetFechaSiguiente(CadaFechaSup))
                {
                    LasSalidas.AddRange(
                        this.ProcesarSemanario
                        (LaFecha, ElTipoPeriodoDiaStr, LaFechafin, CadaFechaSup));
                }
            }

            return LasSalidas.ToArray();
        }

        private DateTime GetFechaSiguiente(DateTime LaFecha)
        {
            if (this.configuracion.WeeklyPaso > 0)
            {
                if (LaFecha.DayOfWeek == DayOfWeek.Monday)
                    return LaFecha.AddDays(7 * this.configuracion.WeeklyPaso);
                if (LaFecha.DayOfWeek == DayOfWeek.Tuesday)
                    return LaFecha.AddDays((7 * this.configuracion.WeeklyPaso) - 1);
                if (LaFecha.DayOfWeek == DayOfWeek.Wednesday)
                    return LaFecha.AddDays((7 * this.configuracion.WeeklyPaso) - 2);
                if (LaFecha.DayOfWeek == DayOfWeek.Thursday)
                    return LaFecha.AddDays((7 * this.configuracion.WeeklyPaso) - 3);
                if (LaFecha.DayOfWeek == DayOfWeek.Friday)
                    return LaFecha.AddDays((7 * this.configuracion.WeeklyPaso) - 4);
                if (LaFecha.DayOfWeek == DayOfWeek.Saturday)
                    return LaFecha.AddDays((7 * this.configuracion.WeeklyPaso) - 5);
                if (LaFecha.DayOfWeek == DayOfWeek.Sunday)
                    return LaFecha.AddDays((7 * this.configuracion.WeeklyPaso) - 6);
            }
            else
            {
                throw new Exception(Global.ValidarWeeklyPaso);
            }

            return LaFecha;
        }

        private Output DevolverSalidaRecurringWeekly(string ElTipoPeriodoDiaStr, DateTime LaFechaPaso, DateTime? LaFechaInicio, DateTime? LaHora)
        {
            string LaFechaPasoStr = LaFechaPaso.ToString("dd/MM/yyyy");

            string DiasSemanaStr = " on ";
            this.week.ToList().ForEach(
                S => DiasSemanaStr = DiasSemanaStr + S.ToString().ToLower() + ", ");
            DiasSemanaStr = DiasSemanaStr.Trim().TrimEnd(',');

            string HorasDiasStr = (this.configuracion.HoraDesde != null ? this.configuracion.HoraDesde.Value.ToShortTimeString() :
                new DateTime(1900, 1, 1, 0, 0, 0).ToShortTimeString()) + " and " +
                (this.configuracion.HoraHasta != null ? this.configuracion.HoraHasta.Value.ToShortTimeString() :
                new DateTime(1900, 1, 1, 23, 59, 0).ToShortTimeString());

            string ElTipoPeriodoHora = Global.every + " " + this.configuracion.HourPaso.ToString() + 
                " " + Global.hours;

            if (LaHora != null)
            {
                LaFechaPaso = LaFechaPaso.AddHours(LaHora.Value.Hour).AddMinutes(LaHora.Value.Minute);
            }

            Output LaSalida = new Output();
            LaSalida.FechaSalida = LaFechaPaso;
            LaSalida.Descripcion =
                string.Format(Global.SalidaRecurringWeekly, ElTipoPeriodoDiaStr, DiasSemanaStr, ElTipoPeriodoHora, HorasDiasStr,
                (LaFechaInicio != null ? LaFechaInicio.Value.ToString("dd/MM/yyyy") : ""));

            return LaSalida;
        }

        private Output[] ProcesarSemanario(DateTime LaFecha, string ElTipoStr, 
            DateTime LaFechafin, DateTime CadaFechaSup)
        {
            List<Output> LasSalidas = new List<Output>();

            for (DateTime CadaFecha = CadaFechaSup; CadaFecha <= LaFechafin;
                CadaFecha = CadaFecha.AddDays(1))
            {
                if (this.week.Any(W => W.Equals(CadaFecha.DayOfWeek)))
                {
                    LasSalidas.AddRange(this.ProcesarHoras(ElTipoStr, CadaFecha));
                }

                if (CadaFecha.DayOfWeek == DayOfWeek.Sunday)
                {
                    break;
                }
            }

            return LasSalidas.ToArray();
        }

        private Output[] ProcesarHoras(string ElTipoStr, DateTime LaFecha)
        {
            List<Output> LaLista = new List<Output>();

            DateTime LaHoraDesde = this.RecuperarHoraDesde(LaFecha);

            DateTime LaHoraHasta = this.RecuperarHoraHasta(LaFecha);

            int LaHoraPaso = this.configuracion.HourPaso != null ?
                this.configuracion.HourPaso.Value : 1;

            for (DateTime LaHora = LaHoraDesde; LaHora <= LaHoraHasta;
                LaHora = LaHora.AddHours(LaHoraPaso))
            {
                LaLista.Add(this.DevolverSalidaRecurringWeekly(ElTipoStr, LaFecha, this.configuracion.FechaInicio, LaHora));
            }

            return LaLista.ToArray();
        }

        private DateTime RecuperarHoraHasta(DateTime LaFecha)
        {
            DateTime LaHoraHasta = new DateTime(LaFecha.Year, LaFecha.Month, LaFecha.Day, 23, 59, 59);

            if (this.configuracion.HoraHasta != null)
            {
                LaHoraHasta = new DateTime(LaFecha.Year, LaFecha.Month, LaFecha.Day, this.configuracion.HoraHasta.Value.Hour, this.configuracion.HoraHasta.Value.Minute,
                    this.configuracion.HoraHasta.Value.Second);
            }

            return LaHoraHasta;
        }

        private DateTime RecuperarHoraDesde(DateTime LaFecha)
        {
            DateTime LaHoraDesde = new DateTime(LaFecha.Year, LaFecha.Month, LaFecha.Day, 0, 0, 0);

            if (this.configuracion.HoraDesde != null)
            {
                LaHoraDesde = new DateTime(LaFecha.Year, LaFecha.Month, LaFecha.Day, this.configuracion.HoraDesde.Value.Hour, this.configuracion.HoraDesde.Value.Minute,
                    this.configuracion.HoraDesde.Value.Second);
            }

            return LaHoraDesde;
        }

        private Output[] ProcesarOnce(string ElTipoStr)
        {
            if (this.configuracion.FechaPaso == null)
            {
                throw new Exception(Global.ValidarFechaConfiguracion);
            }

            if ((this.configuracion.FechaInicio != null &&
                this.configuracion.FechaPaso > this.configuracion.FechaInicio) ||
                (this.configuracion.FechaInicio == null))
            {
                return new Output[]{this.DevolverSalida(ElTipoStr,
                        this.configuracion.FechaPaso.Value,
                        this.configuracion.FechaPaso.Value, this.configuracion.FechaInicio, null) };
            }

            return null;
        }
    }
}
