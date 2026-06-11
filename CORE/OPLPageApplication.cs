using OPLAPI.OIEL.CORE.Browser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace OPLAPI.CORE
{
    public class OPLPageApplication : Application
    {
        //
        public PageBrowser? MainPage { get; protected set; }

        //
        public new void Run()
        {
            MainWindow = new()
            {
                Content = MainPage
            };
            base.Run(MainWindow);
        }
    }
}
