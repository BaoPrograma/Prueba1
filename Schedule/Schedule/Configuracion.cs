using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    public class Configuracion
    {
        public Configuracion()
        {
        }

        public TipoFranja Tipo;
        public DateTime? FechaPaso;
        public TipoPeriodicidad? Periodicidad;
        public DateTime? FechaInicio;
        public DateTime? FechaFin;
        public int? HourPaso;
        public int WeeklyPaso;
        public bool Enabled;
        public bool WeekMonday;
        public bool WeekTuesday;
        public bool WeekWednesday;
        public bool WeekThursday;
        public bool WeekFriday;
        public bool WeekSaturday;
        public bool WeekSunday;
        public DateTime? HoraDesde;
        public DateTime? HoraHasta;
    }
}
