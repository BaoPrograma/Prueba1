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
        public int? DiasPeriodicidad;
        public DateTime FechaInicio;
        public DateTime? FechaFin;
        public bool Enabled;
    }
}
