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
using ProcessCapture.Log;


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
                string file = Logger.GetInstance().Log(e.Exception);
                MessageBox.Show("An error has occurred - it has been saved to " + file + " - please email this to millsy@openspan.com");
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error has occurred - but can not be saved to disk");
            }
        }
    }
}
