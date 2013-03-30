using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private List<Process> _allOpenedProcesses;
        List<Process> AllOpenedProcesses
        {
            get
            {
                this._allOpenedProcesses = this._allOpenedProcesses ?? new List<Process>();
                return this._allOpenedProcesses;
            }
        }
        public StartWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            var width = this.GetWidth();
            double height = this.GetHeight();
            double top = this.GetTop();
            double left = this.GetLeft();
            this.WindowState = System.Windows.WindowState.Minimized;
            window.WindowState = System.Windows.WindowState.Normal;
            window.Left = left;
            window.Top = top;
            window.Width = width;
            window.Height = height;
            window.Topmost = true;

            window.Show();
            window.Closed += (o, ea) =>
            {
                this.WindowState = System.Windows.WindowState.Normal;
                if (UserArgs.IsDataUpdated)
                    this.Close();
            };
        }
        private double GetTop()
        {
            var top = 0;
            foreach (Screen screen in Screen.AllScreens)
                if (screen.Bounds.Top < top)
                    top = screen.Bounds.Top;

            return top;
        }
        private double GetLeft()
        {
            var left = 0;
            foreach (Screen screen in Screen.AllScreens)
                if (screen.Bounds.Left < left)
                    left = screen.Bounds.Left;

            return left;
        }
        private double GetHeight()
        {
            var height = Screen.PrimaryScreen.WorkingArea.Height;
            foreach (Screen screen in Screen.AllScreens)
                if (screen.Bounds.Height > height)
                    height = screen.WorkingArea.Height;

            return height;
        }

        private double GetWidth()
        {
            var width = 0;
            foreach (Screen screen in Screen.AllScreens)
                width += screen.WorkingArea.Width;

            return width;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opendlg = new OpenFileDialog();
            opendlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            opendlg.Filter = "Text Files|*.txt|Word files|*.doc,*.docx|All Files|*.*";
            opendlg.Multiselect = true;
            if (opendlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var fileName in opendlg.FileNames)
                {
                    Process newProcess = new Process();
                    newProcess.StartInfo.FileName = fileName;
                    newProcess.Start();

                    this.AllOpenedProcesses.Add(newProcess);
                    //newProcess.WaitForExit();
                }
                
            }
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            foreach (Process p in this.AllOpenedProcesses)
            {
                if(!p.HasExited)
                    p.CloseMainWindow();
            }

            //if(!UserArgs.IsDataUpdated)
            //    ImageUtil.UpdateDatabaseCancel(UserArgs.DatabasePath, "User closed the application");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OptionsBox box = new OptionsBox(UserArgs.Configurations, UserArgs.ProgramId);
            this.Topmost = false;
            box.ShowDialog();
            this.Topmost = true;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (!UserArgs.SupportsOverrides)
                this.btnOptions.Visibility = System.Windows.Visibility.Collapsed;
        }

    }
}
