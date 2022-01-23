using ScheduleModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    public class Translation
    {
        private DataTable tableTranslation;
        private DataView dwTranslation;

        public Translation()
        {
        }

        private DataTable TableTranslation
        {
            get
            {
                if (this.tableTranslation == null)
                {
                    this.tableTranslation = new DataTable();
                    this.tableTranslation.Columns.Add("id");
                    this.tableTranslation.Columns.Add("spanish");
                    this.tableTranslation.Columns.Add("englishGB");
                    this.tableTranslation.Columns.Add("englishUS");

                    this.AddTranslationWordsRows();
                    this.AddTranslationSentenceRows();

                }

                return this.tableTranslation;
            }
        }

        public DataView DwTranslation
        {
            get
            {
                if (this.dwTranslation == null)
                {
                    this.dwTranslation = new DataView(this.TableTranslation);
                    this.dwTranslation.Sort = "id";
                }

                return this.dwTranslation;
            }
        }

        public string GetTranslation(string Id, Languages Language)
        {
            DataRowView LaFila = this.DwTranslation.FindRows(Id)[0];

            switch (Language)
            {
                case Languages.es_ES:
                    return LaFila["spanish"].ToString().Trim();
                case Languages.en_GB:
                    return LaFila["englishGB"].ToString().Trim();
                case Languages.en_US:
                    return LaFila["englishUS"].ToString().Trim();
            }

            return string.Empty;
        }

        private void AddTranslationWordsRows()
        {
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("and", "y", "and", "and"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("at", "a las", "at", "at"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("between", "entre", "between", "between"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("day", "dia", "day", "day"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("days", "dias", "days", "days"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("every", "cada", "every", "every"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("First", "Primer", "First", "First"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Fourth", "Cuarto", "Fourth", "Fourth"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Friday", "Viernes", "Friday", "Friday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("hour", "hora", "hour", "hour"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("hours", "horas", "hours", "hours"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Last", "Último", "Last", "Last"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Monday", "Lunes", "Monday", "Monday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("month", "mes", "month", "month"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("months", "meses", "months", "months"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("of", "de", "of", "of"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("on", "el", "on", "on"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("once", "una vez", "once", "once"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Saturday", "Sabado", "Saturday", "Saturday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Second", "Segundo", "Second", "Second"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("the", "el", "the", "the"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Third", "Tercer", "Third", "Third"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Sunday", "Domingo", "Sunday", "Sunday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Thursday", "Jueves", "Thursday", "Thursday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Tuesday", "Martes", "Tuesday", "Tuesday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("Wednesday", "Miercoles", "Wednesday", "Wednesday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("week", "semana", "week", "week"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("weekday", "dia laboral", "weekday", "weekday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("weekend", "fin de semana", "weekend", "weekend"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("weekendday", "fin de semana", "weekendday", "weekendday"));
            this.tableTranslation.Rows.Add(
                this.AddTranslationRow("weeks", "semanas", "weeks", "weeks"));
        }

        private void AddTranslationSentenceRows()
        {
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ExitRecurring", "Ocurre {0} {1} entre {2} empezando en {3}", 
                  "Occurs {0} {1} between {2} starting on {3}", 
                  "Occurs {0} {1} between {2} starting on {3}"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("Output",
                  "Ocurre {0}. Schedule se usará en {1}",
                  "Occurs {0}. Schedule will be used on {1}",
                  "Occurs {0}. Schedule will be used on {1}"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("StartingOn",
                  "empezando en {0}",
                  "starting on {0}",
                  "starting on {0}"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateConfiguration",
                  "Debe rellenar la configuración",
                  "Need to fill the configuration",
                  "Need to fill the configuration"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateDailyFrequency",
                  "Debe rellenar la frecuencia diaria",
                  "Need to set Daily Frequency",
                  "Need to set Daily Frequency"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateDateConfiguration",
                  "Debe rellenar la fecha desde y hasta en la configuración",
                  "Need to set the Date From and Step in configuration",
                  "Need to set the Date From and Step in configuration"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateDayWeekSelected",
                  "Debe rellenar una fecha en la configuración semanal",
                  "Need to set any day in weekly configuration",
                  "Need to set any day in weekly configuration"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateHourFromBigggerHourTo",
                  "La hora desde no puede ser mayor que hora hasta",
                  "Hour From not should be bigger than Hour To",
                  "Hour From not should be bigger than Hour To"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateHourStep",
                  "El paso horario debe mayor que 0",
                  "Hour step must be bigger than 0",
                  "Hour step must be bigger than 0"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateHourStepOfDailyFrequency",
                  "Debe rellenar el paso horario en frecuencia diraria",
                  "Need to set hour step in daily frequency bigger than 0",
                  "Need to set hour step in daily frequency bigger than 0"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateMonthlyConfiguration",
                  "Debe rellenar una opción en la configuración mensual(dia, la..)",
                  "Need to set one of the checks in Monthly Configuration (day, the ..)",
                  "Need to set one of the checks in Monthly Configuration (day, the ..)"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateMonthlyMonths",
                  "Debe indicar un mes / meses mayor que 0",
                  "Need to set month(s) bigger than 0",
                  "Need to set month(s) bigger than 0"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateMonthlyMoreHourFromTo",
                  "Debe rellenar la hora desde y hasta",
                  "Need to set the hour from and hour to",
                  "Need to set the hour from and hour to"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateMonthlyMoreWeekStep",
                  "Debe rellenar la frecuencia diaria",
                  "Need to set the day frequency",
                  "Need to set the day frequency"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateMonthlyOnceDayFrequency",
                  "La frecuencia diaria debe ser mayor que 0",
                  "Day must be bigger than 0",
                  "Day must be bigger than 0"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateMonthlyOnceMonthFrequency",
                  "La frecuencia mensual debe ser mayor que 0",
                  "Month must be bigger than 0",
                  "Month must be bigger than 0"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateWeeklyStep",
                  "El paso semanal debe ser mayor que 0",
                  "Need to set week step bigger than 0",
                  "Need to set week step bigger than 0"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateHourFromBigggerHourTo",
                  "La hora desde no puede ser mayor que hora hasta",
                  "Hour From not should be bigger than Hour To",
                  "Hour From not should be bigger than Hour To"));
            this.tableTranslation.Rows.Add(
                  this.AddTranslationRow("ValidateRecurringFrequency",
                  "Debe rellenar la frecuencia",
                  "Need to set frequency",
                  "Need to set frequency"));
        }

        private DataRow AddTranslationRow(string Id, string Spanish, string EnglishGB, string EnglishUS)
        {
            DataRow NewRow = this.tableTranslation.NewRow();
            NewRow["id"] = Id;
            NewRow["spanish"] = Spanish;
            NewRow["englishGB"] = EnglishGB;
            NewRow["englishUS"] = EnglishUS;

            return NewRow;
        }
    }
}
