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
        public static double[] currentLocation;

        static void Main(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
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
                        break;
                    case "":
                        SpinAnimation.Start(100);
                        currentLocation = getLocation();
                        SpinAnimation.Stop();
                        break;
                    default:
                        //incorrect args??
                        
                        break;
                }
            }
            Console.WriteLine("Welcome to CommandWeather, use 'commandweather help' to see usage.");
            Console.WriteLine("Meanwhile, your current weather based on your IP is below.");
            Console.WriteLine("");

            

            displayWeekWeather();

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
            SpinAnimation.Start(100);
            Forecast res = await client.GetWeatherDataAsync(latitude, longitude, DarkSkyApi.Unit.Auto);
            SpinAnimation.Stop();
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

    public static class SpinAnimation
    {

        //spinner background thread
        private static System.ComponentModel.BackgroundWorker spinner = initialiseBackgroundWorker();
        //starting position of spinner changes to current position on start
        private static int spinnerPosition = 25;
        //pause time in milliseconds between each character in the spin animation
        private static int spinWait = 25;
        //field and property to inform client if spinner is currently running
        private static bool isRunning;

        public static bool IsRunning { get { return isRunning; } }

        /// <summary>
        /// Worker thread factory
        /// </summary>
        /// <returns>background worker thread</returns>

        private static System.ComponentModel.BackgroundWorker initialiseBackgroundWorker()
        {

            System.ComponentModel.BackgroundWorker obj = new System.ComponentModel.BackgroundWorker();
            //allow cancellation to be able to stop the spinner
            obj.WorkerSupportsCancellation = true;
            //anonymous method for background thread's DoWork event
            obj.DoWork += delegate
            {
                //set the spinner position to the current console position
                spinnerPosition = Console.CursorLeft;
                //run animation unless a cancellation is pending
                while (!obj.CancellationPending)
                {
                    //characters to iterate through during animation
                    char[] spinChars = new char[] { '|', '/', '-', '\\' };
                    //iterate through animation character array
                    foreach (char spinChar in spinChars)
                    {
                        //reset the cursor position to the spinner position
                        Console.CursorLeft = spinnerPosition;
                        //write the current character to the console
                        Console.Write(spinChar);
                        //pause for smooth animation - set by the start method
                        System.Threading.Thread.Sleep(spinWait);
                    }
                }
            };
            return obj;
        }

        /// <summary>
        /// Start the animation
        /// </summary>
        /// <param name="spinWait">wait time between spin steps in milliseconds</param>
        public static void Start(int spinWait)
        {
            //Set the running flag
            isRunning = true;
            //process spinwait value
            SpinAnimation.spinWait = spinWait;
            //start the animation unless already started
            if (!spinner.IsBusy)
                spinner.RunWorkerAsync();
            else throw new InvalidOperationException("Cannot start spinner whilst spinner is already running");
        }

        /// <summary>
        /// Overloaded Start method with default wait value
        /// </summary>
        public static void Start() { Start(25); }
        /// <summary>
        /// Stop the spin animation
        /// </summary>

        public static void Stop()
        {
            //Stop the animation
            spinner.CancelAsync();
            //wait for cancellation to complete
            while (spinner.IsBusy) System.Threading.Thread.Sleep(100);
            //reset the cursor position
            Console.CursorLeft = spinnerPosition;
            //set the running flag
            isRunning = false;
        }
    }

}
