using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MT4bot
{
    class img
    {

        int topThreshold = 0;
        int botThreshold = 0;
        String state = "";
        public img(int topThreshold,int botThreshold)
        {
            this.topThreshold = topThreshold;
            this.botThreshold = botThreshold;
        }
        public void screenshot()
        {
            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

            bmpScreenshot.Save("screen.bmp", ImageFormat.Bmp);

            bmpScreenshot.Dispose();
            gfxScreenshot.Dispose();

        }

        public void saveScreenshot(Bitmap bmpScreenshot, string path , ImageFormat Format)
        {
            bmpScreenshot.Save(path, Format);
            bmpScreenshot.Dispose();
        }

        public String scanning(Bitmap bmpScreenshot )
        {
            //Console.WriteLine("*******begin scanning**********");
            bool found = false;
            //scan screen in threshold from right top to left top
            for (int x = Screen.PrimaryScreen.Bounds.Width - 1; x >= 0; x--)
            {   if (found){ break; }

                for (int y = topThreshold; y <= botThreshold; y++)
                {

                        //Console.WriteLine("Current position :{0},{1}", x, y);
                        var col = bmpScreenshot.GetPixel(x, y);
                        bmpScreenshot.SetPixel(x, y, Color.Red);

                        int white_threshold = col.R + col.G + col.B;
                        if (white_threshold < 500)
                        {
                           if (col.G == 255 && col.B == 0 && col.R == 0)
                            {
                            state = "G";
                            //Console.WriteLine("*******Found blue dot at {0},{1}", x, y);
                            //Console.WriteLine("Color value (RGB) {0},{1},{2}", col.R, col.G, col.B);
                            //Console.Beep();
                            found = true;
                            break;
                            }
                            if (col.R == 0 && col.B ==100 && col.G == 100)
                            {
                            state = "P";
                            //Console.WriteLine("******Found red dot at {0},{1}", x, y);
                            //Console.WriteLine("Color value (RGB) {0},{1},{2}", col.R, col.G, col.B);
                            //Console.Beep();
                            found = true;
                            break;
                        }
                        
                    }
                }
            }
            return state;
        }
#region testing function
        /*
         find working area              
         */
        public Bitmap Threshold (Bitmap bmpScreenshot,int topThreshold,int botThreshold)
        {
            for (int x = 0; x < Screen.PrimaryScreen.Bounds.Width; x++)
                bmpScreenshot.SetPixel(x, 110, Color.Green);

            for (int x = 0; x < Screen.PrimaryScreen.Bounds.Width; x++)
                bmpScreenshot.SetPixel(x, 855, Color.Green);

            return bmpScreenshot;
        }

        /*
         find color value               
         
         1079,110 
         */
        public Bitmap colorPosition(Bitmap bmpScreenshot)
        {
            for (int x = 0; x < Screen.PrimaryScreen.Bounds.Width; x++)
                bmpScreenshot.SetPixel(x, 103, Color.Red);

            for (int y = 0; y < Screen.PrimaryScreen.Bounds.Height; y++)
                bmpScreenshot.SetPixel(1901, y, Color.Red);                         
            return bmpScreenshot;
        }

        /*
         find color threshold               
         */
        public void colorfromPosition(Bitmap bmpScreenshot)
        {
            var col = bmpScreenshot.GetPixel(1426, 720);
            int a = col.A;
            int r = col.R;
            int g = col.G;
            int b = col.B; 
        }
        #endregion


    }
}
