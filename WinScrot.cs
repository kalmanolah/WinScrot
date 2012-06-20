using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WinScrot
{
    /*
     * WinScrot (.NET edition), by Kalman Olah
     * 
     * Usage: WinScrot [-d DELAY] [-c] [-q QUALITY] [-t THUMBSIZE] [FILENAME.EXT]
     * 
     *     -d INT       Delay before the screenshot is taken, in seconds. Default: 0. Min: 0. Optional.
     *     -c             Displays a countdown when used with delay. Default: False. Optional.
     *     -q INT       Quality of the screenshot, in percentages. Default: 75. Min: 1. Max: 100. Optional.
     *                     NOTE: Differs depending on image format.
     *     -t INT        Resolution of the screenshot, in percentages of full desktop size. Default: 100. Min: 1. Max: 100. Optional.
     *      file.ext      Filename of the screenshot, relative or absolute. Allowed extensions: png, jpg, jpeg, gif, bmp. Optional.
     *                     NOTE: DateTime substitutions can be used in the filename in the "$dt.<example>$" format.
     *                     e.g.: 'WinScrot-$dt.dd.MM.yyyy--HH.mm.ss$.png' -> 'WinScrot-01.07.2012--17.48.53.png'
     *                    
     * This work is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
     * To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/3.0/ .
     */
    class WinScrot
    {
        static int varDelay = 0;
        static bool varCountdown = false;
        static int varQuality = 0;
        static int varThumbsize = 100;
        static string varFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "WinScrot-" + DateTime.Now.ToString("dd.MM.yyyy--HH.mm.ss") + ".png");

        static void Main(string[] args)
        {
            handleArgs(args);
            handleScrot();
        }

        static void handleArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case ("-d"):
                        try
                        {
                            int tmp = int.Parse(args[i + 1]);
                            args[i + 1] = null;
                            if (tmp >= 0)
                            {
                                varDelay = tmp;
                            }
                        }
                        catch { }
                        break;
                    case ("-c"):
                        varCountdown = true;
                        try
                        {
                            int tmp = int.Parse(args[i + 1]);
                            args[i + 1] = null;
                            if (tmp >= 0)
                            {
                                varDelay = tmp;
                            }
                        }
                        catch { }
                        break;
                    case ("-q"):
                        try
                        {
                            int tmp = int.Parse(args[i + 1]);
                            args[i + 1] = null;
                            if (tmp > 0 && tmp <= 100)
                            {
                                varQuality = tmp;
                            }
                        }
                        catch { }
                        break;
                    case ("-t"):
                        try
                        {
                            int tmp = int.Parse(args[i + 1]);
                            args[i + 1] = null;
                            if (tmp > 0 && tmp <= 100)
                            {
                                varThumbsize = tmp;
                            }
                        }
                        catch { }
                        break;
                    default:
                        try
                        {
                            string tmp = args[i];
                            while (Regex.IsMatch(tmp, @"\$dt\.(.*)\$"))
                            {
                                Match tmp2 = Regex.Match(tmp, @"\$dt\.(.*)\$");
                                if (tmp2.Success)
                                {
                                    string tmp3 = tmp2.Groups[1].Value;
                                    tmp = tmp.Replace("$dt." + tmp3 + "$", DateTime.Now.ToString(tmp3));
                                }
                            }
                            FileInfo tmp4 = new FileInfo(tmp);
                            if (tmp4.Directory.Exists)
                            {
                                varFilename = tmp;
                            }
                        }
                        catch { }
                        break;
                }
            }
        }

        static void handleScrot()
        {
            if (varDelay > 0)
            {
                Console.Out.Write("Taking screenshot in " + varDelay + "...");
                System.Timers.Timer tmp = new System.Timers.Timer(1000);
                tmp.Elapsed += new ElapsedEventHandler(timerTick);
                tmp.Enabled = true;
                while (true)
                {
                    Console.ReadLine();
                }
            }
            else
            {
                takeScrot();
            }
        }

        static void takeScrot()
        {
            Rectangle tmpRectangle = Rectangle.Empty;
            foreach (Screen tmpScreen in Screen.AllScreens)
            {
                tmpRectangle = Rectangle.Union(tmpRectangle, tmpScreen.Bounds);
            }
            Bitmap tmpBitmap = new Bitmap(tmpRectangle.Width, tmpRectangle.Height,PixelFormat.Format32bppArgb);
            using (Graphics tmpGraphics = Graphics.FromImage(tmpBitmap))
            {
                tmpGraphics.CopyFromScreen(tmpRectangle.X, tmpRectangle.Y, 0, 0, tmpRectangle.Size, CopyPixelOperation.SourceCopy);
            }
            if (varThumbsize < 100)
            {
                int tmpWidth = (tmpRectangle.Width * varThumbsize) / 100;
                int tmpHeight = (tmpRectangle.Height * varThumbsize) / 100;
                Bitmap tmpBitmap2 = new Bitmap(tmpWidth, tmpHeight, PixelFormat.Format32bppArgb);
                Graphics tmpGraphics2 = Graphics.FromImage(tmpBitmap2);
                tmpGraphics2.DrawImage(tmpBitmap2, 0, 0, tmpWidth, tmpHeight);
                tmpBitmap = tmpBitmap2;
                tmpGraphics2.Dispose();
            }
            ImageFormat tmpFormat = ImageFormat.Png;
            if (varFilename.ToLower().EndsWith("jpg") || varFilename.ToLower().EndsWith("jpeg"))
            {
                tmpFormat = ImageFormat.Jpeg;
            }
            else if (varFilename.ToLower().EndsWith("bmp"))
            {
                tmpFormat = ImageFormat.Bmp;
            }
            else if (varFilename.ToLower().EndsWith("gif"))
            {
                tmpFormat = ImageFormat.Gif;
            }
            if (varQuality > 0)
            {
                ImageCodecInfo tmpCodec = getEncoder(tmpFormat);
                System.Drawing.Imaging.Encoder tmpEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters tmpParams = new EncoderParameters(1);
                tmpParams.Param[0] = new EncoderParameter(tmpEncoder, (long)varQuality);
                tmpBitmap.Save(varFilename, tmpCodec, tmpParams);
            }
            else
            {
                tmpBitmap.Save(varFilename, tmpFormat);
            }
            tmpBitmap.Dispose();
            Console.Write(Environment.NewLine + "Screenshot saved to \"" + varFilename + "\"");
            Environment.Exit(0);
        }

        static void timerTick(object sender, ElapsedEventArgs e)
       {
           varDelay--;
           if (varDelay == 0)
           {
               ((System.Timers.Timer)sender).Enabled = false;
               handleScrot();
           }
           else if(varCountdown)
           {
               Console.Write(" " + varDelay + "...");
           }
       }
        
        static ImageCodecInfo getEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
