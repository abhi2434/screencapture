using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoCapture
{
    /// <summary>
    /// Interaction logic for OptionsBox.xaml
    /// </summary>
    public partial class OptionsBox : Window
    {
        DefaultConfigurations Config { get; set; }
        int ProgramId { get; set; }

        public OptionsBox(DefaultConfigurations configuration, int programId)
        {
            InitializeComponent();
            this.Config = configuration;
            this.ProgramId = programId;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Save 
            this.Config.IsWaterMarkused = this.chkWatermarking.IsChecked.Value;
            this.Config.IsGraphic = this.chkGraphics.IsChecked.Value;
            this.Config.IsPDF = this.chkPDF.IsChecked.Value;
            this.Config.IsPDFFile = this.chkPDFFile.IsChecked.Value;
            this.Config.IsConstrained = this.chkConstrained.IsChecked.Value;

            if (this.Config.IsGraphic)
            {
                this.Config.ImageFormat = this.cmbGType.SelectedItem.ToString();
                this.Config.PhotoParameters.Width = this.GetInt32(this.tbWidth.Text);
                this.Config.PhotoParameters.Height = this.GetInt32(this.tbHeight.Text);
                this.Config.PhotoParameters.BaseFileName = this.tbDestinationFile.Text;
                this.Config.PhotoParameters.DestinationFilePath = this.tbDestinationPath.Text;

                if (string.IsNullOrEmpty(this.tbDestinationFile.Text))
                    this.Config.PhotoParameters.BaseFileName = string.Format("{0}.{1}", this.ProgramId, this.Config.ImageFormat);

                if (this.Config.IsPDFFile)
                {
                    this.Config.PhotoParameters.SourceFileName = this.tbSourceFile.Text;
                }
            }
            if (this.Config.IsWaterMarkused)
            {
                this.Config.PhotoWatermarking.Text = this.tbWMText.Text;
                this.Config.PhotoWatermarking.Left = this.GetInt32(this.tbWMLeft.Text);
                this.Config.PhotoWatermarking.Top = this.GetInt32(this.tbWMTop.Text);
                this.Config.PhotoWatermarking.FontName = this.tbFont.Text;
                this.Config.PhotoWatermarking.Color = this.tbFontColor.Text;
                this.Config.PhotoWatermarking.FontSize = this.GetInt32(this.tbFontSize.Text);

                this.Config.PhotoWatermarking.IsBold = this.chkBold.IsChecked.Value;
                this.Config.PhotoWatermarking.IsItalic = this.chkItalic.IsChecked.Value;
                this.Config.PhotoWatermarking.IsUnderLine = this.chkUnderline.IsChecked.Value;
                this.Config.PhotoWatermarking.Opacity = this.GetInt32(this.tbOpacity.Text);
            }
        }

        private int GetInt32(string text)
        {
            int retVal = 0;

            try
            {
                retVal = Convert.ToInt32(text);
            }
            catch { }
            return retVal;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        //{
        //    //Constrained
        //    bool isconstrained = this.chkConstraint.IsChecked.Value;
        //    this.imgWidth.IsEnabled = this.imgHeight.IsEnabled = this.chkProportional.IsEnabled = isconstrained;
        //    UserArgs.DoConstrained = isconstrained;
        //}

        //private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        //{
        //    //Proportional
        //    UserArgs.DoProportional = this.chkProportional.IsChecked.Value;
        //}

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.cmbGType.ItemsSource = new string[] { "bmp", "jpg", "jpeg", "gif", "png", "tiff" };
            this.chkWatermarking.IsChecked = this.Config.IsWaterMarkused;
            this.chkGraphics.IsChecked = this.Config.IsGraphic;
            this.chkPDF.IsChecked = this.Config.IsPDF;
            this.chkPDFFile.IsChecked = this.Config.IsPDFFile;
            this.chkConstrained.IsChecked = this.Config.IsConstrained;

            if (this.Config.IsGraphic)
            {
                this.cmbGType.SelectedItem = this.Config.ImageFormat;
                this.tbWidth.Text = this.Config.PhotoParameters.Width.ToString();
                this.tbHeight.Text = this.Config.PhotoParameters.Height.ToString();
                this.tbDestinationFile.Text = this.Config.PhotoParameters.BaseFileName;
                this.tbDestinationPath.Text = this.Config.PhotoParameters.DestinationFilePath;

                if (string.IsNullOrEmpty(this.Config.PhotoParameters.BaseFileName))
                    this.tbDestinationFile.Text = string.Format("{0}.{1}", this.ProgramId, this.Config.ImageFormat);

                if (this.Config.IsPDFFile)
                {
                    this.tbSourceFile.Text = this.Config.PhotoParameters.SourceFileName;
                }
            }
            if (this.Config.IsWaterMarkused)
            {
                this.tbWMText.Text = this.Config.PhotoWatermarking.Text;
                this.tbWMLeft.Text = this.Config.PhotoWatermarking.Left.ToString();
                this.tbWMTop.Text = this.Config.PhotoWatermarking.Top.ToString();
                this.tbFont.Text = this.Config.PhotoWatermarking.FontName;
                this.tbFontColor.Text = this.Config.PhotoWatermarking.Color;
                this.tbFontSize.Text = this.Config.PhotoWatermarking.FontSize.ToString();
                this.tbOpacity.Text = this.Config.PhotoWatermarking.Opacity.ToString();
                this.chkBold.IsChecked = this.Config.PhotoWatermarking.IsBold;
                this.chkItalic.IsChecked = this.Config.PhotoWatermarking.IsItalic;
                this.chkUnderline.IsChecked = this.Config.PhotoWatermarking.IsUnderLine;
            }
        }

        private void chkWatermarking_Checked_1(object sender, RoutedEventArgs e)
        {
            if (this.chkWatermarking.IsChecked.Value)
                this.Height = 600;
            else
                this.Height = 434;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //Image Path
            System.Windows.Forms.FolderBrowserDialog folderdialog = new System.Windows.Forms.FolderBrowserDialog();
            folderdialog.Description = "Choose image path where image need to be created";
            folderdialog.SelectedPath = this.tbDestinationPath.Text;
            folderdialog.ShowNewFolderButton = true;
            if (folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tbDestinationPath.Text = folderdialog.SelectedPath;
            }
        }

        private void tbNumeric_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }


        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);

        }

        //private void Button_Click_4(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog dlg = new OpenFileDialog();
        //    dlg.InitialDirectory = UserArgs.DatabasePath;
        //    dlg.FileName = UserArgs.DatabasePath;
        //    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        UserArgs.DatabasePath = dlg.FileName;
        //}
    }
}
