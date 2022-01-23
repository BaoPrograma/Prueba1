using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleModel
{
    public class ScheduleContext : DbContext
    {
        public DbSet<ScheduleConfiguration> Configuration { get; set; }

        public ScheduleContext(DbContextOptions<ScheduleContext> options)
        : base(options)
        {
        }
    }

    public class ScheduleConfiguration
    {
        [Key]
        public int SchedulerId { get; set; }

        public TypeStep TimeType { get; set; }
        public DateTime? DateStep { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public TypeTimeStep? TypeRecurring { get; set; }

        #region recurring
        public int? HourStep { get; set; }
        public bool Enabled { get; set; }
        public DateTime? HourFrom { get; set; }
        public DateTime? HourTo { get; set; }

        public int DailyStep { get; set; }

        public int WeekStep { get; set; }
        public bool WeeklyMonday { get; set; }
        public bool WeeklyTuesday { get; set; }
        public bool WeeklyWednesday { get; set; }
        public bool WeeklyThursday { get; set; }
        public bool WeeklyFriday { get; set; }
        public bool WeeklySaturday { get; set; }
        public bool WeeklySunday { get; set; }

        public bool? MonthlyOnce { get; set; }
        public int? MonthlyOnceDay { get; set; }
        public int? MonthlyOnceMonthSteps { get; set; }
        public DailyFrequency? TypeDailyFrequency { get; set; }

        public bool? MonthlyMore { get; set; }
        public TypeWeekStep? MonthlyMoreWeekStep { get; set; }
        public TypeDayWeekStep? MonthlyMoreOrderDayWeekStep { get; set; }
        public int? MonthlyMoreMonthSteps { get; set; }

        public Languages Language { get; set; }

        #endregion
    }
}