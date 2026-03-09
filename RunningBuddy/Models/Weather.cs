using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningBuddy.Models
{
    internal class Weather
    {
        public double Humidity { get; set;}
        public double currentTemp { get; set;}
        public double windSpeed {get; set;}
        public string harshConditions {get; set;} = "";
        public string ConditionText { get; set; } = "";
        public string ConditionIconUrl { get; set; } = "";
        public double VisibilityMiles { get; set; }
    }
}
