using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Net;
using System.Threading;
using System.Web;
using System.Collections.Specialized;


namespace OpenSpanWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);

        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                string tempPath = System.IO.Path.GetTempPath();
                System.IO.File.WriteAllText(tempPath + "millsy.txt", e.Exception.Message + Environment.NewLine + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine + Environment.NewLine + (e.Exception.InnerException != null ? e.Exception.InnerException.StackTrace : "No inner exception"));
                MessageBox.Show("An error has occurred - it has been saved to "+tempPath+"millsy.txt - please email this to millsy@openspan.com");
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error has occurred - but can not be saved to disk");
            }
        }
    }
}
