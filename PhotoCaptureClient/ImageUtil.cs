using Itenso.Rtf;
using Itenso.Rtf.Converter.Html;
using Itenso.Rtf.Support;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoCapture
{
    public class ImageUtil
    {
        public static Image GetResized(Image image, Size size, bool isProportionaltoAspect = true)
        {
            int newWidth;
            int newHeight;
            if (isProportionaltoAspect)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
        public static void AddWatermark(Image image, WaterMarking mark)
        {
            Graphics imgGraphics = Graphics.FromImage(image);
            //Font defaultFont = new Font("Verdana", 14, FontStyle.Bold);

            //calculates the fontsize of watermark
            //double width = image.Width * 0.5;
            //var size = imgGraphics.MeasureString(watermarktext, defaultFont);
            //var ratio = size.Width / defaultFont.SizeInPoints;
            //var requiredSize = width / ratio;

            //Creates transparency
            SolidBrush firstBrush = new SolidBrush(mark.FontColor);
            SolidBrush secondBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));

            //final font
            //SavedStates.FontName = "Verdana";
            //SavedStates.FontSize = requiredSize.ToString();
            //SavedStates.Left = 0;
            //SavedStates.Top = 0;
            //var font = new Font("Verdana", (float)requiredSize, FontStyle.Bold);

            //writes two text to create a drop shadow effect
            imgGraphics.DrawString(mark.Text, mark.Font, firstBrush, mark.Left + 2, mark.Top + 2);
            imgGraphics.DrawString(mark.Text, mark.Font, secondBrush, mark.Left, mark.Top);

        }
        public void SaveScreen(double x, double y, double width, double height)
        {
            int ix, iy, iw, ih;
            ix = Convert.ToInt32(x);
            iy = Convert.ToInt32(y);
            iw = Convert.ToInt32(width);
            ih = Convert.ToInt32(height);
            try
            {
                Bitmap myImage = new Bitmap(iw, ih);

                Graphics gr1 = Graphics.FromImage(myImage);
                IntPtr dc1 = gr1.GetHdc();
                IntPtr dc2 = NativeMethods.GetWindowDC(NativeMethods.GetForegroundWindow());
                NativeMethods.BitBlt(dc1, ix, iy, iw, ih, dc2, ix, iy, 13369376);
                gr1.ReleaseHdc(dc1);
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = "png";
                dlg.Filter = "Png Files|*.png";
                DialogResult res = dlg.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                    myImage.Save(dlg.FileName, ImageFormat.Png);
            }
            catch { }
        }

        public static void CaptureScreen(double x, double y,
                                         double width, double height,
                                         Size desiredSize, string watermarkText)
        {
            int ix, iy, iw, ih;
            ix = Convert.ToInt32(x);
            iy = Convert.ToInt32(y);
            iw = Convert.ToInt32(width);
            ih = Convert.ToInt32(height);
            Image image = new Bitmap(iw, ih, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(image);
            g.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih), CopyPixelOperation.SourceCopy);

            if (UserArgs.Configurations.IsConstrained)
                image = ImageUtil.GetResized(image, desiredSize, false);
            if (UserArgs.Configurations.PhotoParameters.Width > 0 && UserArgs.Configurations.PhotoParameters.Height > 0)
                image = ImageUtil.GetResized(image, desiredSize, false); ;

            if (UserArgs.Configurations.IsWaterMarkused)
                ImageUtil.AddWatermark(image, UserArgs.Configurations.PhotoWatermarking);

            ImageUtil.SaveFile(image);

            //ImageUtil.UpdateDatabaseInfo(UserArgs.DatabasePath, true, "Process completed successfully");
        }

        //public static void UpdateDatabaseInfo(string path, bool isSuccess, string message)
        //{
        //    if (!UserArgs.IsDataUpdated)
        //    {
        //        AccessDbManager dbManager = new AccessDbManager(path);
        //        if (isSuccess)
        //            dbManager.UpdateLog(message, UserArgs.SupportsOverrides);
        //        else
        //            dbManager.UpdateFailedLog(message);
        //        UserArgs.IsDataUpdated = true;
        //    }
        //}
        //public static void UpdateDatabaseCancel(string path, string message)
        //{
        //    if (!UserArgs.IsDataUpdated)
        //    {
        //        AccessDbManager dbManager = new AccessDbManager(path);
        //        dbManager.UpdateCancelledLog(message);
        //        UserArgs.IsDataUpdated = true;
        //    }
        //}
        public static void SaveFile(Image image)
        {
            string defaultfilePath = UserArgs.Configurations.PhotoParameters.DestinationFilePath;
            string filename = UserArgs.Configurations.PhotoParameters.BaseFileName;
            string imageFormat = UserArgs.Configurations.ImageFormat;

            if (!Directory.Exists(defaultfilePath))
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = imageFormat;
                dlg.Filter = "Png Files|*.png|Jpeg Files|*.jpg|Gif Files|*.gif|Bitmap Files|*.bmp|All Files|*.*";
                DialogResult res = dlg.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    defaultfilePath = Path.GetDirectoryName(dlg.FileName);
                    filename = Path.GetFileNameWithoutExtension(dlg.FileName);
                    imageFormat = Path.GetExtension(dlg.FileName);
                }
                UserArgs.Configurations.PhotoParameters.DestinationFilePath = defaultfilePath;
                UserArgs.Configurations.PhotoParameters.BaseFileName = filename;
                UserArgs.Configurations.ImageFormat = imageFormat;
            }

            ImageFormat format = GetImageFormat(imageFormat);
            if (UserArgs.Configurations.IsGraphic)
            {
                string filePath = System.IO.Path.Combine(defaultfilePath, filename) + "." + imageFormat;
                image.Save(filePath, format);
            }
            if (UserArgs.Configurations.IsPDF)
            {
                string pdfPath = System.IO.Path.Combine(defaultfilePath, filename) + ".pdf";
                ImageUtil.CreatePDF(image, pdfPath, format);
            }
        }
        public static void CreatePDF(Image image, string filePath, ImageFormat format)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            string filename = Path.GetFileNameWithoutExtension(filePath);
            iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);
            try
            {
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, new FileStream(Path.Combine(directoryPath, filename) + ".pdf", FileMode.Create));

                document.Open();
                iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(image, format);
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
                pic.Border = 1;
                pic.BorderColor = iTextSharp.text.BaseColor.BLACK;
                paragraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                paragraph.Add(pic);
                document.Add(paragraph);
                document.NewPage();
            }
            catch (iTextSharp.text.DocumentException de)
            {
                Console.Error.WriteLine(de.Message);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
            }
            document.Close();
        }
        public static void CreatePDF(string filePath, string destinationfilePath)
        {
            string directoryPath = Path.GetDirectoryName(destinationfilePath);
            string filename = Path.GetFileNameWithoutExtension(destinationfilePath);
            iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);
            try
            {
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, new FileStream(Path.Combine(directoryPath, filename) + ".pdf", FileMode.Create));

                document.Open();
                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
                paragraph.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                var extension = Path.GetExtension(filePath);
                switch (extension.ToLower())
                {
                    case ".jpg":
                    case ".bmp":
                    case ".jpeg":
                    case ".gif":
                    case ".png":
                    case ".tiff":
                        iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Image.FromFile(filePath), GetImageFormat(UserArgs.Configurations.ImageFormat));
                        pic.Border = 1;
                        pic.BorderColor = iTextSharp.text.BaseColor.BLACK;
                        paragraph.Add(pic);
                        document.Add(paragraph);
                        document.NewPage();
                        break;
                    case ".txt":
                        paragraph.Add(File.ReadAllText(filePath));
                        document.Add(paragraph);
                        document.NewPage();
                        break;
                    case ".htm":
                    case ".html":
                        TextReader reader = new StringReader(File.ReadAllText(filePath));
                        HTMLWorker worker = new HTMLWorker(document);
                        worker.StartDocument();
                        worker.Parse(reader);
                        worker.EndDocument();
                        worker.Close();
                        break;
                    case ".rtf":
                        string htmlDocument = GetHtmlFromRTF(File.ReadAllText(filePath));
                        reader = new StringReader(htmlDocument);
                        worker = new HTMLWorker(document);
                        worker.StartDocument();
                        worker.Parse(reader);
                        worker.EndDocument();
                        worker.Close();
                        break;
                    default:
                        paragraph.Add(File.ReadAllText(filePath));
                        document.Add(paragraph);
                        document.NewPage();
                        break;
                }
            }
            catch (iTextSharp.text.DocumentException de)
            {
                Console.Error.WriteLine(de.Message);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
            }
            document.Close();
        }
        private static string GetHtmlFromRTF(string rtfSelection)
        {
            IRtfDocument rtfDocument = RtfInterpreterTool.BuildDoc(rtfSelection);
            RtfHtmlConverter htmlConverter = new RtfHtmlConverter(rtfDocument);
            return htmlConverter.Convert();
        }
        private static ImageFormat GetImageFormat(string filename)
        {
            string extension = Path.GetExtension(filename);
            //SavedStates.FileExtension = extension;
            ImageFormat format = ImageFormat.Png;
            switch (extension.ToLower())
            {
                case "bmp":
                    format = ImageFormat.Bmp;
                    break;
                case "jpg":
                case "jpeg":
                    format = ImageFormat.Jpeg;
                    break;
                case "gif":
                    format = ImageFormat.Gif;
                    break;
                case "png":
                    format = ImageFormat.Png;
                    break;
                case "tiff":
                    format = ImageFormat.Tiff;
                    break;
            }
            return format;
        }
        internal class NativeMethods
        {

            [DllImport("user32.dll")]
            public extern static IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hwnd);
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetForegroundWindow();
            [DllImport("gdi32.dll")]
            public static extern UInt64 BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, System.Int32 dwRop);

        }
    }
}
