using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace Barcode
{

    internal class Program
    {
        // the image we are generating
        // margin should always be 0 we will do margining with the size
        static int imageMargin = 0;
        static int imageWidth = 220;
        static int imageHeight = 75;


        // the actaul print canvas
        // how much the image should go to right
        static int printWidthMargin = 90;
        // how much the image should go up
        static int PrintHeightMargin = 490;
        static int printWidth = 225;
        static int printHeight = 100;

        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  Barcode.exe <barcode> <text> <printer-name> <copies>");
                Console.WriteLine();
                Console.WriteLine("Example:");
                Console.WriteLine("  Barcode.exe 1231312312313 \"زانست (٢.٠٠٠) دینار\" \"Xprinter XP-460B\" 2");
                return;
            }

            string barcode = args[0];
            string text = args[1];
            string printerName = args[2];

            if (!int.TryParse(args[3], out int copies) || copies < 1)
            {
                Console.WriteLine("Error: copies must be a positive number.");
                return;
            }

            if (text.Length > 20)
            {
                Console.Write("max length is 20 characters.");
                return;
            }

            if (text.Length >= 13)
            {
                imageWidth = 220;
            }
            else if (text.Length >= 12)
            {
                imageWidth = 200;
            }
            else if (text.Length >= 9)
            {
                imageWidth = 175;
            }
            else if (text.Length >= 7)
            {
                imageWidth = 150;
            }
            else if (text.Length >= 5)
            {
                imageWidth = 130;
            }
            else if (text.Length >= 3)
            {
                imageWidth = 110;
            }
            else
            {
                imageWidth = 90;
            }
            int letterCount = text.Count(char.IsLetter);
            imageWidth += letterCount * 5;


            Bitmap bitmap = GenerateBarcodeWithText(barcode, text, imageWidth, imageHeight, imageMargin);
            PrintBarcode(bitmap, printerName, copies);
        }

        //for testing uncomment this and comment the above
        //static void Main(string[] args)
        //{
        //    string text = Console.ReadLine();

        //    if (text.Length > 20)
        //    {
        //        Console.Write("max length is 20 characters.");
        //        return;
        //    }

        //    if (text.Length >= 13)
        //    {
        //        imageWidth = 220;
        //    }
        //    else if (text.Length >= 12)
        //    {
        //        imageWidth = 200;
        //    }
        //    else if (text.Length >= 9)
        //    {
        //        imageWidth = 175;
        //    }
        //    else if (text.Length >= 7)
        //    {
        //        imageWidth = 150;
        //    }
        //    else if (text.Length >= 5)
        //    {
        //        imageWidth = 130;
        //    }
        //    else if (text.Length >= 3)
        //    {
        //        imageWidth = 110;
        //    }
        //    else
        //    {
        //        imageWidth = 90;
        //    }
        //    int letterCount = text.Count(char.IsLetter);
        //    imageWidth += letterCount * 5;


        //    Bitmap bitmap = GenerateBarcodeWithText(text, $"زانست (2000) دینار", imageWidth, imageHeight, imageMargin);
        //    PrintBarcode(bitmap, "Xprinter XP-460B", 1);
        //}


        public static Bitmap GenerateBarcodeWithText(string barcodeText, string additionalText, int width, int height, int margin)
        {

            var barcodeWriter = new ZXing.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions { Width = width, Height = height, Margin = margin }
            };

            Bitmap barcodeBitmap = barcodeWriter.Write(barcodeText);

            int upperTextHeight = 25;
            Bitmap finalBitmap = new Bitmap(width, height + upperTextHeight);

            using (Graphics g = Graphics.FromImage(finalBitmap))
            {
                g.Clear(Color.White);
                Font font = new Font("_Auder SamikTimes", 15);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near
                };
                //-5 so that the text will goes to the top more
                g.DrawString(additionalText, font, Brushes.Black, new RectangleF(0, -5, width, upperTextHeight), format);
                g.DrawImage(barcodeBitmap, new Point(0, upperTextHeight));
            }

            return finalBitmap;
        }

        private static void PrintBarcode(Bitmap bitmap, string printerName, int copies)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = printerName;
            pd.PrinterSettings.Copies = (short)copies;

            pd.PrintPage += (sender, e) =>
            {
                e.Graphics.DrawImage(bitmap, new Rectangle(printWidthMargin, PrintHeightMargin, printWidth, printHeight));
            };

            pd.Print();
        }
    }
}
