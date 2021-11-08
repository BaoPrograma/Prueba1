using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    public class ScheduleException : ApplicationException
    {
        public ScheduleException(string Mensaje)
            : base(Mensaje)
        {
        }
        public ScheduleException(string Mensaje, Exception innerException)
            : base(Mensaje, innerException)
        {

        }
    }
}
