using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    public class Spanish
    {
        public static Hashtable Translations
        {
            get
            {
                Hashtable TableTranslations = new Hashtable();

                TableTranslations.Add("and", "y");
                TableTranslations.Add("at", "a las");
                TableTranslations.Add("between", "entre");
                TableTranslations.Add("day", "dia");
                TableTranslations.Add("days", "dias");
                TableTranslations.Add("every", "cada");
                TableTranslations.Add("ExitRecurring", "Ocurre {0} {1} entre {2} empezando en {3}");
                TableTranslations.Add("First", "Primer");
                TableTranslations.Add("Fourth", "Cuarto");
                TableTranslations.Add("Friday", "Viernes");
                TableTranslations.Add("hour", "hora");
                TableTranslations.Add("hours", "horas");
                TableTranslations.Add("Last", "Último");
                TableTranslations.Add("Monday", "lunes");
                TableTranslations.Add("month", "mes");
                TableTranslations.Add("months", "meses");
                TableTranslations.Add("of", "de");
                TableTranslations.Add("on", "el");
                TableTranslations.Add("once", "una vez");
                TableTranslations.Add("Output", "Ocurre {0}. Schedule se usará en {1}");
                TableTranslations.Add("Saturday", "Sabado");
                TableTranslations.Add("Second", "Segundo");
                TableTranslations.Add("StartingOn", "empezando en {0}");
                TableTranslations.Add("Sunday", "Domingo");
                TableTranslations.Add("the", "el");
                TableTranslations.Add("Third", "Tercer");
                TableTranslations.Add("Thursday", "Jueves");
                TableTranslations.Add("Tuesday", "Martes");
                TableTranslations.Add("ValidateConfiguration", "Debe rellenar la configuración");
                TableTranslations.Add("ValidateDailyFrequency", "Debe rellenar la frecuencia diaria");
                TableTranslations.Add("ValidateDateConfiguration", "Debe rellenar la fecha desde y hasta en la configuración");
                TableTranslations.Add("ValidateDayWeekSelected", "Debe rellenar una fecha en la configuración semanal");
                TableTranslations.Add("ValidateHourFromBigggerHourTo", "La hora desde no puede ser mayor que hora hasta");
                TableTranslations.Add("ValidateHourStep", "El paso horario debe mayor que 0");
                TableTranslations.Add("ValidateHourStepOfDailyFrequency", "Debe rellenar el paso horario en frecuencia diraria");
                TableTranslations.Add("ValidateMonthlyConfiguration", "Debe rellenar una opción en la configuración mensual(dia, la..)");
                TableTranslations.Add("ValidateMonthlyMonths", "Debe indicar un mes / meses mayor que 0");
                TableTranslations.Add("ValidateMonthlyMoreHourFromTo", "Debe rellenar la hora desde y hasta");
                TableTranslations.Add("ValidateMonthlyMoreWeekStep", "Debe rellenar la frecuencia diaria");
                TableTranslations.Add("ValidateMonthlyOnceDayFrequency", "La frecuencia diaria debe ser mayor que 0");
                TableTranslations.Add("ValidateMonthlyOnceMonthFrequency", "La frecuencia mensual debe ser mayor que 0");
                TableTranslations.Add("ValidateRecurringFrequency", "Debe rellenar la frecuencia");
                TableTranslations.Add("ValidateWeeklyStep", "El paso semanal debe ser mayor que 0");
                TableTranslations.Add("Wednesday", "Miercoles");
                TableTranslations.Add("week", "semana");
                TableTranslations.Add("weekday", "dia laboral");
                TableTranslations.Add("weekend", "fin de semana");
                TableTranslations.Add("weekendday", "fin de semana");
                TableTranslations.Add("weeks", "semanas");

                return TableTranslations;
            }
        }
    }
}
