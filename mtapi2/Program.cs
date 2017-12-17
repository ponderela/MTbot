using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using MT4bot;
using Newtonsoft;

namespace mtapi2
{
    class Program
    {
        static String previous = "";
        static int counter = 0;

        static void Main(string[] args)
        {
            //Connect con = new Connect(1);
            //Connect con2 = new Connect(2);

            while (true)
            {
                var time = DateTime.Now.Minute.ToString();
               

                if(counter == 0)
                {
                    Console.WriteLine("Program start at {0}", DateTime.Now.ToString());
                    Interval();
                }

                if (isTime(time))
                {
                    Interval();
                }
                Thread.Sleep(60*1000);
            }
        }
        public static bool isTime(String minutes)
        {
            bool flag =false;
            if ( minutes == "1")
            {
                Console.WriteLine("Hit time Condition at {0}", DateTime.Now.ToString());
                flag = true;
            }

            //Console.WriteLine("not match time Condition at {0}", DateTime.Now.ToString());
            return flag;
        }

        public static void Interval()
        {
            int topThreshold = 110;
            int botThreshold = 855;

            img bitmapClass = new img(topThreshold, botThreshold);
            bitmapClass.screenshot();
            using (Bitmap bmp = new Bitmap("screen.bmp"))
            {
                var scanning = bitmapClass.scanning(bmp);
                if (counter == 0)
                {
                    previous = scanning;
                    Console.WriteLine("First Initate initial state :{0}", previous);
                }
                else
                {
                    if (scanning == "B" && previous != "B")
                    {
                        Console.WriteLine("Statechange from {0} to {1} BUY Signal", previous, scanning);
                        Connect con = new Connect(1);
                        previous = scanning;
                    }
                    else if (scanning == "S" && previous != "S")
                    {
                        Console.WriteLine("Statechange from {0} to {1} SELL signal", previous, scanning);
                        Connect con = new Connect(2);
                        previous = scanning;
                    }
                }
            }
            counter++;
        }
    }
}
