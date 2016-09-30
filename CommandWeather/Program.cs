using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DarkSkyApi;
using DarkSkyApi.Models;


namespace CommandWeather
{
    class Program
    {
        public static DarkSkyService client = new DarkSkyService("d96970bf601538b3a43b1eb547d865ba");
        public static Forecast result;

        static void Main(string[] args)
        {


            typeWrite("Hello!");

            Task<Forecast> localRes = getWeather(51.4201050, -0.7242300);
            typeWrite(localRes.Result.Currently.Summary);

            Console.ReadLine();
        }

        static void typeWrite(string input)
        {
            char[] chars = input.ToCharArray();

            for (int i = 0; i < chars.Length ; i++)
            {
                Console.Write(chars[i]);
                Thread.Sleep(100);
            }
            Console.WriteLine("");
        }

        static async Task<Forecast> getWeather(double latitude, double longitude)
        {
            Forecast res = await client.GetWeatherDataAsync(latitude, longitude);
            result = res;
            return res;
        }
    }
}
