using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningBuddy.Models
{
    internal class Routes
    {
        public int Id { get; set; }
        public string Name  {get; set;}
        public double Length { get; set; }
        public DateTime Time { get; set; }
        public DateTime Date { get; set;}

        public double Elevation {get;set;}

        public enum Terrain {Track,Asphault,Gravel,Trail,Dirt,Grass}

    }
}