using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Media.Imaging;

namespace TestServer.Services
{
    internal class CaptchaImage
    {
        private readonly Random _random = new();

        public byte[] GenerateCaptcha(string text)
        {
            using Bitmap bitmap = new(200, 60, PixelFormat.Format32bppArgb);
            using Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;


            Rectangle rect = new(0, 0, bitmap.Width, bitmap.Height);
            g.FillRectangle(Brushes.White, rect);

            int i, r, x, y;
            Pen pen = new(Color.Yellow);
            for (i = 1; i < 10 + text.Length / 2; i++)
            {
                pen.Color = Color.FromArgb(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255));

                r = _random.Next(0, bitmap.Width / 3);
                x = _random.Next(0, bitmap.Width);
                y = _random.Next(0, bitmap.Height);

                g.DrawEllipse(pen, x - r, y - r, r, r);
            }

            Font font = new("Tahoma", 22, FontStyle.Bold);
            RectangleF rectF = new(0, 0, bitmap.Width, bitmap.Height);
            StringFormat sf = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(text, font, Brushes.Black, rectF, sf);

            return BitmapToBytes(bitmap);
        }

        private static byte[] BitmapToBytes(Bitmap bitmap)
        {
            BitmapImage bitmapImage;
            using (MemoryStream ms = new())
            {
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return ImageToByteArray(bitmapImage);
        }

        private static byte[] ImageToByteArray(BitmapImage bitmapImage)
        {
            using var stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(stream);
            return stream.ToArray();
        }
    }
}
