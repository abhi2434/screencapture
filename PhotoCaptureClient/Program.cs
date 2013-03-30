using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoCapture
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                MessageBox.Show("Welcome to Async");
                StartWindow startWindow = new StartWindow();
                UserArgs.LoadConfigurations();
                //if (args.Length == 2)
                //{
                //    //Load database path
                //    UserArgs.DatabasePath = args[0];

                //    //Load ProgramId
                //    UserArgs.ProgramId = Convert.ToInt32(args[1]);
                //    UserArgs.LoadConfigurations();

                //    if (UserArgs.Configurations.IsPDFFile)
                //    {
                //        string sourceFile = UserArgs.Configurations.PhotoParameters.SourceFileName;
                //        string destinationFilePath = Path.Combine(UserArgs.Configurations.PhotoParameters.DestinationFilePath, UserArgs.Configurations.PhotoParameters.BaseFileName) + ".pdf";
                //        if (!File.Exists(sourceFile))
                //        {
                //            OpenFileDialog dlg = new OpenFileDialog();
                //            dlg.DefaultExt = "rtf";
                //            dlg.Filter = "All Files|*.*|Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.tiff|Text Files|*.txt|Rich Text Files|*.rtf|Html Files|*.htm;*.html";
                //            DialogResult res = dlg.ShowDialog();
                //            if (res == System.Windows.Forms.DialogResult.OK)
                //            {
                //                sourceFile = dlg.FileName;
                //            }
                //        }
                //        ImageUtil.CreatePDF(sourceFile, destinationFilePath);
                //        ImageUtil.UpdateDatabaseInfo(UserArgs.DatabasePath, true, "PDF File succesfully created");
                //    }
                //    else
                startWindow.ShowDialog();

                //}
                //else
                //{
                //    MessageBox.Show("Number of argument is either less or too much");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //if (File.Exists(UserArgs.DatabasePath))
                //    ImageUtil.UpdateDatabaseInfo(UserArgs.DatabasePath, false, ex.Message);
            }
        }
    }
}
