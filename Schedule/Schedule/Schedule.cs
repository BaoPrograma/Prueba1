using Schedule.RecursosTextos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    public class Schedule
    {
        private Configuracion configuracion;
        private DateTime fechaProcesada;
        private List<Output> salidasRecursivas;

        public Schedule(Configuracion Laconfiguracion)
        {
            this.configuracion = Laconfiguracion;
            this.salidasRecursivas = new List<Output>();
        }

        public Output[] Procesar(DateTime LaFecha)
        {
            if (this.configuracion.Enabled)
            {
                if (this.configuracion.Tipo == TipoFranja.Once)
                {
                    return new Output[] { this.ProcesarOnce(LaFecha) };
                }
                else if (this.configuracion.Tipo == TipoFranja.Recurring &&
                    this.configuracion.Periodicidad != null)
                {
                    this.fechaProcesada = LaFecha.AddDays(this.configuracion.DiasPeriodicidad.Value);

                    this.ProcesarDiasRecurring(new Output(), LaFecha);

                    return this.salidasRecursivas.ToArray();
                }

                return new Output[] {this.DevolverSalida("",
                   LaFecha, LaFecha, this.configuracion.FechaInicio) };
            }
            else
            {
                return new Output[] {this.DevolverSalida("",
                   LaFecha, LaFecha, this.configuracion.FechaInicio) };
            }
        }

        private Output ProcesarOnce(DateTime LaFecha)
        {
            Output LaSalida = new Output();

            if (this.configuracion.FechaPaso != null && this.configuracion.FechaPaso > LaFecha)
            {
                return this.DevolverSalida(this.configuracion.Tipo.ToString().ToLower(),
                   this.configuracion.FechaPaso.Value, this.configuracion.FechaPaso.Value, this.configuracion.FechaInicio);
            }
            else
            {
                return this.DevolverSalida(this.configuracion.Tipo.ToString().ToLower(),
                   LaFecha, LaFecha, this.configuracion.FechaInicio);
            }
        }

        private Output DevolverSalida(string ElTipo, DateTime LaFechaPaso, DateTime LaHoraPaso, DateTime LaFechaInicio)
        {
            string LaFechaHoraPasoStr = LaFechaPaso.ToString("dd/MM/yyyy");

            if (LaHoraPaso.TimeOfDay > new TimeSpan(00, 00, 00))
            {
                LaFechaHoraPasoStr += " at " + LaHoraPaso.ToString("HH:mm");
            }

            Output LaSalida = new Output();
            LaSalida.FechaSalida = LaFechaPaso;
            LaSalida.Descripcion = string.Format(Global.Salida,ElTipo, LaFechaHoraPasoStr
                , LaFechaInicio.ToString("dd/MM/yyyy"));

            return LaSalida;
        }

        private Output ProcesarDiasRecurring(Output LaSalida, DateTime LaFecha)
        {
            string ElTipoStr = "every day";
            if (LaSalida != null && LaSalida.FechaSalida != null)
            {
                this.salidasRecursivas.Add(LaSalida);
            }

            if (LaFecha < this.fechaProcesada)
            {                
                LaFecha = LaFecha.AddDays(1);
                LaSalida = this.DevolverSalida(ElTipoStr, LaFecha, LaFecha, this.configuracion.FechaInicio);
                
                return this.ProcesarDiasRecurring(LaSalida, LaFecha);
            }

            return null;
        }
    }
}
