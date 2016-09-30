using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CommandWeather
{
    class Program
    {
        static void Main(string[] args)
        {
            string ascii1 = @"__          __    _                                _            _____                           _     __          __           _    _";                
            string ascii2 = @"\ \        / /   | |                              | |          / ____|                         | |    \ \        / /          | |  | |";               
            string ascii3 = @" \ \  /\  / /___ | |  ___  ___   _ __ ___    ___  | |_  ___   | |      ___   _ __   ___   ___  | |  ___\ \  /\  / /___   __ _ | |_ | |__    ___  _ __";
            string ascii4 = @"  \ \/  \/ // _ \| | / __|/ _ \ | '_ ` _ \  / _ \ | __|/ _ \  | |     / _ \ | '_ \ / __| / _ \ | | / _ \\ \/  \/ // _ \ / _` || __|| '_ \  / _ \| '__|";
            string ascii5 = @"   \  /\  /|  __/| || (__| (_) || | | | | ||  __/ | |_| (_) | | |____| (_) || | | |\__ \| (_) || ||  __/ \  /\  /|  __/| (_| || |_ | | | ||  __/| |";   
            string ascii6 = @"    \/  \/  \___||_| \___|\___/ |_| |_| |_| \___|  \__|\___/   \_____|\___/ |_| |_||___/ \___/ |_| \___|  \/  \/  \___| \__,_| \__||_| |_| \___||_|";




            typeWrite(ascii1);
            typeWrite(ascii2);
            typeWrite(ascii3);
            typeWrite(ascii4);
            typeWrite(ascii5);
            typeWrite(ascii6);
            


            Console.ReadLine();
        }

        static void typeWrite(string input)
        {
            char[] chars = input.ToCharArray();

            for (int i = 0; i < chars.Length ; i++)
            {
                Console.Write(chars[i]);
                Thread.Sleep(5);
            }
            Console.WriteLine("");
        }
    }
}
