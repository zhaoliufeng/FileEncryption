using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileEncryption
{

    public partial class App : Application
    {
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            String activatedTime = DateTime.Now.ToString();
            Console.WriteLine("ApplicationActivated activatedTime = {0}", activatedTime);
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            int exitCode = 0;
            Console.WriteLine("ApplicationActivated exitCode = {0}", exitCode);
        }
    }
}
