using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WinScrot
{
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
                            varDelay = tmp < 0 ? 0 : tmp;
                            args[i + 1] = null;
                        }
                        catch { }
                        break;
                    case ("-c"):
                        varCountdown = true;
                        try
                        {
                            int tmp = int.Parse(args[i + 1]);
                            varDelay = tmp < 0 ? 0 : tmp;
                            args[i + 1] = null;
                        }
                        catch { }
                        break;
                    case ("-q"):
                        try
                        {
                            int tmp = int.Parse(args[i + 1]);
                            varQuality = (tmp > 0 && tmp <= 100) ? tmp : 0;
                            args[i + 1] = null;
                        }
                        catch { }
                        break;
                    case ("-t"):
                        try
                        {
                            int tmp = int.Parse(args[i + 1]);
                            varThumbsize = (tmp > 0 && tmp <= 100) ? tmp : 100;
                            args[i + 1] = null;
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
                            if (new FileInfo(tmp).Directory.Exists) varFilename = tmp;
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
                Console.Write("Taking screenshot in " + varDelay + "...");
                System.Timers.Timer tmp = new System.Timers.Timer(1000);
                tmp.Elapsed += new ElapsedEventHandler(timerTick);
                tmp.Enabled = true;
                while (tmp.Enabled) { }
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
            Bitmap tmpBitmap = new Bitmap(tmpRectangle.Width, tmpRectangle.Height, PixelFormat.Format32bppArgb);
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
                tmpGraphics2.DrawImage(tmpBitmap, 0, 0, tmpWidth, tmpHeight);
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
            Console.Write("Screenshot saved to \"" + varFilename + "\"");
        }

        static void timerTick(object sender, ElapsedEventArgs e)
        {
            varDelay--;
            if (varCountdown) Console.Write(" " + varDelay + "...");
            if (varDelay == 0)
            {
                Console.Write(Environment.NewLine);
                handleScrot();
                ((System.Timers.Timer)sender).Enabled = false;
            }
        }

        static ImageCodecInfo getEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid) return codec;
            }
            return null;
        }
    }
}
