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
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Search.Common.Enums;
using GoogleApi.Entities.Places.Search.Text.Request;
using GoogleApi.Entities.Places.Search.Text.Response;

namespace CommandWeather
{
    public class Weather
    {
        private string darkSkyApiKey;
        private string googleApiKey;
        private DarkSkyService client;
        private double[] currentLocation;
        public Forecast result;

        public Weather(string darkapi, string googlapi)
        {
            darkSkyApiKey = darkapi;
            googleApiKey = googlapi;
            client = new DarkSkyService(darkSkyApiKey);
        }

        private void typeWrite(string input, int speed, bool newLine = true)
        {
            char[] chars = input.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                Console.Write(chars[i]);
                Thread.Sleep(speed);
            }
            if (newLine)
            {
                Console.WriteLine("");
            }
        }

        public async Task<Forecast> getWeather(double latitude, double longitude)
        {
            SpinAnimation.Start(100);
            Forecast res = await client.GetWeatherDataAsync(latitude, longitude, DarkSkyApi.Unit.Auto);
            SpinAnimation.Stop();
            result = res;
            return res;
        }

        public double[] getLocation()
        {
            var json = new WebClient().DownloadString("http://freegeoip.net/json/");
            //Info res = JsonConvert.DeserializeObject<string[,]>(json);
            Info res = JsonConvert.DeserializeObject<Info>(json);
            double[] ret = new double[] { res.latitude, res.longitude };
            return ret;
        }

        public void displayWeekWeather()
        {
            string[] days = currentDayOrder();
            for (int i = 0; i < days.Length; i++)
            {
                if (i == 0)
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
            for (int i = 0; i < temps.Length; i++)
            {
                int temp = Convert.ToInt32(localR.Result.Daily.Days[i].MaxTemperature);
                temps[i] = temp.ToString();
            }
            List<String> tempsList = temps.ToList();
            tempsList.Insert(0, "Temps:       ");
            temps = tempsList.ToArray();
            foreach (string t in temps)
            {
                //typeWrite(t.PadRight(10), 30, false);
                Console.Write(t.PadRight(10));
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

        public void manageArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-l":
                        string[] location = args[i + 1].Split(',');
                        double[] newLocation = new double[] { Convert.ToDouble(location[0]), Convert.ToDouble(location[1]) };
                        currentLocation = newLocation;
                        break;
                    case "search":
                        //do something with search
                        locationSearch(args[i + 1]);
                        break;
                    case "":

                        break;
                    case "help":
                        Console.WriteLine("Here is some help.");
                    default:
                        //incorrect args??

                        break;
                }
            }
        }

        private string[] currentDayOrder()
        {
            string[] dayOrder = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            int dayInt = (int)DateTime.Today.DayOfWeek;
            string[] res = new string[7];
            for (int i = 0; i < res.Length; i++)
            {
                int dayOrderPos = dayInt + i;
                if (dayOrderPos > 6)
                {
                    dayOrderPos = dayOrderPos - 7;
                }
                res[i] = dayOrder[dayOrderPos];
            }
            return res;
        }

        private void locationSearch(string query)
        {
            PlacesTextSearchRequest request = new PlacesTextSearchRequest
            {
                Key = googleApiKey,
                Sensor = true,
                Language = "en",
                Query = query
            };

            PlacesTextSearchResponse res = GoogleApi.GooglePlaces.TextSearch.Query(request);
            if (res.Results.ToArray().Length == 1)
            {
                string location = res.Results.ToArray()[0].FormattedAddress;
                Console.WriteLine("You have chosen '" + location + "' as your location for the weather.");
                double[] coords = new double[] { res.Results.ToArray()[0].Geometry.Location.Latitude, res.Results.ToArray()[0].Geometry.Location.Longitude };
                currentLocation = coords;
            }
            else
            {
                Console.WriteLine("Multiple locations recieved, choose one below by typing the number and hitting enter.");
                string[] order = new string[res.Results.ToArray().Length];
                for (int i = 0; i < res.Results.ToArray().Length; i++)
                {
                    int n = i + 1;
                    string s = n.ToString() + ". ";
                    order[i] = s;
                }

                for (int i = 0; i < res.Results.ToArray().Length; i++)
                {
                    Console.Write(order[i]);
                    string location = res.Results.ToArray()[i].FormattedAddress;
                    Console.WriteLine(location);
                }

                string choice = Console.ReadLine().Trim(new char[] { ' ', '.' });
                int c = Convert.ToInt32(choice) - 1;
                double[] coords = new double[] { res.Results.ToArray()[c].Geometry.Location.Latitude, res.Results.ToArray()[c].Geometry.Location.Longitude };
                Console.Clear();
                Console.WriteLine("You have chosen '" + res.Results.ToArray()[c].FormattedAddress + "' as your location for the weather.");
                currentLocation = coords;
            }
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