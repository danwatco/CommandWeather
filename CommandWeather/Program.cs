﻿using System;
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
        public static double[] currentLocation = getLocation();

        static void Main(string[] args)
        {
            /*string[] days = new string[] { "Mon".PadLeft(10), "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            
            foreach (string d in days)
            {
                typeWrite(d.PadRight(7), 30, false);
                
            }*/
            displayWeekWeather();
            //Console.WriteLine("");
            typeWrite("Hello!", 100);
            Task<Forecast> localRes = getWeather(currentLocation[0], currentLocation[1]);
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

        static void displayWeekWeather()
        {
            string[] days = currentDayOrder();
            for(int i = 0; i < days.Length; i++)
            {
                if(i == 0)
                {
                    typeWrite(days[i].PadLeft(15), 30, false);
                }
                else
                {
                    typeWrite(days[i].PadLeft(10), 30, false);
                }
            }
            Console.WriteLine("");
            Task<Forecast> localR = getWeather(currentLocation[0], currentLocation[1]);
            string[] temps = new string[7];
            for(int i = 0; i < temps.Length; i++)
            {
                int temp = Convert.ToInt32(localR.Result.Daily.Days[i].MaxTemperature);
                temps[i] = temp.ToString();
            }
            List<String> tempsList = temps.ToList();
            tempsList.Insert(0, "Temps:       ");
            temps = tempsList.ToArray();
            foreach(string t in temps)
            {
                typeWrite(t.PadRight(10), 30, false);
            }
            //typeWrite("Temp:", 30);
            Console.WriteLine("");
            /*string[] summarys = new string[7];
            for (int i = 0; i < summarys.Length; i++)
            {
                string summary = localR.Result.Daily.Days[i].Summary;
                summarys[i] = summary;
            }
            List<String> summarysList = summarys.ToList();
            summarysList.Insert(0, "Summary:");
            summarys = summarysList.ToArray();
            foreach (string s in summarysList)
            {
                typeWrite(s.PadRight(10), 30, false);
            } */ 

            typeWrite("Summary: " + localR.Result.Daily.Summary, 30);
            

        }

        public static string PadBoth(string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);

        }

        public static string[] currentDayOrder()
        {
            string[] dayOrder = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            int dayInt = (int)DateTime.Today.DayOfWeek;
            string[] res = new string[7];
            for(int i = 0; i < res.Length; i++)
            {
                int dayOrderPos = dayInt + i;
                if(dayOrderPos > 6)
                {
                    dayOrderPos = dayOrderPos - 7;
                }
                res[i] = dayOrder[dayOrderPos];
            }
            return res;
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
        public string region_name { get; set; }
        public string city { get; set; }
        public string zip_code { get; set; }
        public string time_zone { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

}
