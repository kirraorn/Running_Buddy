using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningBuddy.Models
{
    internal class Goal
    {
        public int Id { get; set; }
        public double Distance { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime Date { get; set;}
        public int? TrainingPlanId { get; set; }
        public string PlanType { get; set; } = ""; 

    }
}
