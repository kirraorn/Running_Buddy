using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningBuddy.Models
{
    internal class Shoe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string ShoeType {get; set; }
        public double CurrentMilage { get; set; }
        public double MaxMilage { get; set; }

    }
}
