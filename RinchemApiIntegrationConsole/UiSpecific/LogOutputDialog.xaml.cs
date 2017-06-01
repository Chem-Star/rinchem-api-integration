using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace asnIntegratorConsole.UiSpecific
{
    /// <summary>
    /// Interaction logic for LogOutputDialog.xaml
    /// </summary>
    public partial class LogOutputDialog : Window
    {
        public LogOutputDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Application curApp = Application.Current;
            //Window mainWindow = curApp.MainWindow;
            //this.Left = mainWindow.Left;
            //this.Top = mainWindow.Top + (mainWindow.Height - 30);
        }


        public void updateOutput(String message)
        {
            try
            {
                LogOutput.Text = message;
                LogScroller.ScrollToEnd();
            }
            catch
            {

            }
        }

    }
}
