using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCapture
{
    public class UserArgs
    {
        public static bool IsLoading { get; set; }
        public static bool IsDataUpdated { get; set; }
        public static int ProgramId { get; set; }
        public static bool SupportsOverrides { get; set; }

        public static string DatabasePath { get; set; }

        public static DefaultConfigurations Configurations
        {
            get;
            set;
        }

        internal static void LoadConfigurations()
        {
            try
            {
                UserArgs.SupportsOverrides = true;
                UserArgs.Configurations = DefaultConfigurations.GetConiguration();
            }
            catch { }
        }
    }

    public class DefaultConfigurations
    {
        public bool IsGraphic { get; set; }
        public bool IsPDF { get; set; }
        public bool IsPDFFile { get; set; }

        public string ImageFormat { get; set; }

        public bool IsConstrained { get; set; }

        public InputParameters PhotoParameters { get; set; }

        public bool IsWaterMarkused { get; set; }
        public WaterMarking PhotoWatermarking { get; set; }

        internal static DefaultConfigurations GetConiguration()
        {
            DefaultConfigurations dconfigurations = new DefaultConfigurations();
            dconfigurations.LoadConfigurations();
            return dconfigurations;
        }

        private void LoadConfigurations()
        {
            try
            {
                this.IsGraphic = true;
                this.IsPDF = true;
                this.IsPDFFile = false;

                this.ImageFormat = string.Format("{0}.jpg", Guid.NewGuid());
                this.IsConstrained = true;
                this.IsWaterMarkused = true;

                this.PhotoParameters = InputParameters.GetParameters();
                this.PhotoWatermarking = WaterMarking.GetWatermarkParameters();
            }
            catch { }
        }
    }

    public class InputParameters
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public string DestinationFilePath { get; set; }
        public string BaseFileName { get; set; }

        public string SourceFileName { get; set; }

        internal static InputParameters GetParameters()
        {
            InputParameters iparameters = new InputParameters();
            iparameters.LoadParameters();
            return iparameters;
        }

        private void LoadParameters()
        {
            this.Width = 300;
            this.Height = 400;

            this.DestinationFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.BaseFileName = string.Format("{0}", Guid.NewGuid());
            this.SourceFileName = string.Empty;
        }
    }

    public class WaterMarking
    {
        public WaterMarking()
        {
            this.Opacity = 100;
        }
        public string Text { get; set; }
        public string FontName { get; set; }
        public Font Font
        {
            get
            {
                Font f = null;
                try
                {
                    FontFamily family = new FontFamily(this.FontName);
                    FontStyle style = FontStyle.Regular;
                    if (this.IsBold && family.IsStyleAvailable(FontStyle.Bold))
                        style = style | FontStyle.Bold;
                    if (this.IsItalic && family.IsStyleAvailable(FontStyle.Italic))
                        style = style | FontStyle.Italic;
                    if (this.IsUnderLine && family.IsStyleAvailable(FontStyle.Underline))
                        style = style | FontStyle.Underline;

                    if (this.FontSize <= 0)
                        this.FontSize = 8;
                    try
                    {
                        f = new Font(this.FontName, this.FontSize, style);
                    }
                    catch { }
                    if (f == null)
                    {
                        try
                        {
                            f = new Font(this.FontName, this.FontSize);
                        }
                        catch { }
                    }
                }
                catch { }
                if (f == null)
                    f = new Font("MS Sans Serif", 8);

                return f;
            }
        }

        public float FontSize { get; set; }
        public string Color { get; set; }

        public Color FontColor
        {
            get
            {
                Color thecolor = System.Drawing.Color.Black;
                if (!string.IsNullOrEmpty(this.Color))
                {
                    thecolor = ColorTranslator.FromHtml(this.Color);
                    thecolor = System.Drawing.Color.FromArgb(this.GetOpacity(), thecolor);
                }
                return thecolor;
            }
        }

        public float Opacity { get; set; }

        public int GetOpacity()
        {

            try
            {
                float opacitypercent = this.Opacity;
                if (opacitypercent > 100)
                    opacitypercent = 100;
                if (opacitypercent < 0)
                    opacitypercent = 0;
                return Convert.ToInt32(255 * (opacitypercent / 100));
            }
            catch { }
            return 255;
        }

        public bool IsItalic { get; set; }
        public bool IsBold { get; set; }
        public bool IsUnderLine { get; set; }

        public int Left { get; set; }
        public int Top { get; set; }

        internal static WaterMarking GetWatermarkParameters()
        {
            WaterMarking wmarking = new WaterMarking();
            wmarking.LoadParameters();
            return wmarking;
        }

        private void LoadParameters()
        {
            this.Text = "Test Watermark";
            this.FontName = "Arial";
            this.FontSize = 20f;
            this.Color = "#ffffff";
            this.Opacity = 0.70f;
            this.IsItalic = false;
            this.IsBold = true;
            this.IsUnderLine = true;
            this.Left = 50;
            this.Top = 50;
        }
    }

    public static class DataUtils
    {
        public static bool GetBoolData(DataRow drow, string columnName)
        {
            bool retval = false;
            try
            {
                retval = Convert.ToBoolean(drow[columnName]);
            }
            catch { }
            return retval;
        }
        public static string GetStringData(DataRow drow, string columnName)
        {
            string retval = string.Empty;
            try
            {
                retval = Convert.ToString(drow[columnName]);
            }
            catch { }
            return retval;
        }

        internal static int GetIntData(DataRow drow, string columnName)
        {
            int retval = 0;
            try
            {
                retval = Convert.ToInt32(drow[columnName]);
            }
            catch { }
            return retval;
        }

        internal static float GetPercentData(DataRow drow, string columnName)
        {
            float retval = 0;
            try
            {
                retval = Convert.ToSingle(drow[columnName]);
            }
            catch { }
            return retval;
        }

        internal static float GetSingleData(DataRow drow, string columnName)
        {
            float retval = 0;
            try
            {
                retval = Convert.ToSingle(drow[columnName]);
            }
            catch { }
            return retval;
        }
    }

}
