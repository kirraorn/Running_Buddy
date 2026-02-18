using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningBuddy.Models
{
    internal class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double ColdPreference { get; set; }
        public double TotalMilage { get; set; }

    }
}
