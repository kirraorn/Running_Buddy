using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningBuddy.Models
{
    public class TrainingPlan
    {
        public int Id { get; set; }
        public string PlanType { get; set; } = "";       // 5K", "10K", "Half Marathon and Full Marathon"
        public DateTime StartDate { get; set; }
        public int DurationWeeks { get; set; }            // Auto set based on plan type
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
