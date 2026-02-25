using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningBuddy.Models
{
    internal class Clothing
    {
        public String hat_gloves {  get; set; }
        public String top { get; set; }
        public String bottom { get; set; }


        //algorithm to recoment clothing based on weather and user preference

        //needs to be somehow tied to the creation of the weather data
        //I would recommend this is created as a single variable right when the weather is created
        //or maybe make this a variable inside the weather class?
        public void setClothing(Weather weather, double cold_preference)
        {
            double userTemp = weather.currentTemp + (cold_preference * 2) - weather.windSpeed;
            //the base case is the user gets cold easily
            //user sensitivity simply offsets the temperature range for each clothing category
            //this assumes the max cold_preference value is 5.0

            //my expericence has shown that wind over 10 mph feels like 10 degrees colder, so every 1 mph of wind = 1 degree colder -Alex
            // simpler option: -10 if the wind is over 10 mph

            //note: humidity is more complicated, it can make it feel hotter when hot and colder when cold
            //assummes humidity is a double 0-100
            if (weather.Humidity > 50 && weather.currentTemp < 55)
            {
                userTemp -= 10;
            }


            //main algorithm
            if (userTemp >= 70)
            {
                hat_gloves = "None";
                top = "tank top";
                bottom = "shorts";
            }
            else if (userTemp >= 60)
            {
                hat_gloves = "None";
                top = "short sleeves";
                bottom = "shorts";
            }
            else if (userTemp >= 50)
            {
                hat_gloves = "Hat + Gloves";
                top = "long sleeves";
                bottom = "jacket";
            }
            else if (userTemp >= 40)
            {
                hat_gloves = "Hat + Glooves";
                top = "long sleeves";
                bottom = "shorts";
            }
            else //temp is 30s or colder
            {
                hat_gloves = "Hat and Mittens";
                top = "long sleeves + jacket";
                bottom = "shorts + pants";
            }
        }
    }
}
