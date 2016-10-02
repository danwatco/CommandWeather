using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using DarkSkyApi;
using DarkSkyApi.Models;
using Newtonsoft.Json;


namespace CommandWeather
{
    class Program
    {
        public static DarkSkyService client = new DarkSkyService("d96970bf601538b3a43b1eb547d865ba");
        public static Forecast result;

        static void Main(string[] args)
        {
            string[] days = new string[] { "Mon".PadLeft(5), "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            
            foreach (string d in days)
            {
                typeWrite(d.PadRight(7), 30, false);
                
            }
            Console.WriteLine("");
            typeWrite("Hello!", 100);
            double[] location = getLocation();
            Task<Forecast> localRes = getWeather(location[0], location[1]);
            typeWrite(localRes.Result.Currently.Summary, 100);

            Console.ReadLine();
        }

        static void typeWrite(string input, int speed, bool newLine = true)
        {
            char[] chars = input.ToCharArray();

            for (int i = 0; i < chars.Length ; i++)
            {
                Console.Write(chars[i]);
                Thread.Sleep(speed);
            }
            if (newLine)
            {
                Console.WriteLine("");
            }
        }

        static async Task<Forecast> getWeather(double latitude, double longitude)
        {
            Forecast res = await client.GetWeatherDataAsync(latitude, longitude);
            result = res;
            return res;
        }

        static double[] getLocation()
        {
            var json = new WebClient().DownloadString("http://freegeoip.net/json/");
            //Info res = JsonConvert.DeserializeObject<string[,]>(json);
            Info res = JsonConvert.DeserializeObject<Info>(json);
            double[] ret = new double[]{ res.latitude, res.longitude };
            return ret;
        }
    }

    public class Info
    {
        /*@"{"ip":"86.179.111.58",
                "country_code":"GB","country_name":"United Kingdom",
                "region_code":"ENG","region_name":"England","city":"Bracknell",
                "zip_code":"RG12","time_zone":"Europe/London","latitude":51.4167,"longitude":-0.75,"metro_code":0}";*/
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string region_code { get; set; }
        public string region_name { get; set; }
        public string city { get; set; }
        public string zip_code { get; set; }
        public string time_zone { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int metro_code { get; set; }

    }

}
