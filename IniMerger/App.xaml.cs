using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace IniMerger
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            args = Environment.GetCommandLineArgs();
            bool isSilent = false;
            int type = 0;
            string target = "";
            string source = "";
            string output = "";
            foreach (string arg in args)
            {
                if (arg == "-s")
                    isSilent = true;
                else if (arg.StartsWith("-t"))
                {
                    try
                    {
                        type = Convert.ToInt32(arg.Substring(2));
                    }
                    catch
                    {
                        type = 0;
                    }
                }
                else if (arg.StartsWith("t:"))
                    target = arg.Substring(2);
                else if (arg.StartsWith("s:"))
                    source = arg.Substring(2);
                else if (arg.StartsWith("o:"))
                    output = arg.Substring(2);
            }
            if (isSilent)
            {
                MainWindow.IniAnalyse result = new MainWindow.IniAnalyse(target, new List<string> { source });
                if (result.IsEnabled)
                    result.UseRules(output, type);
                Environment.Exit(0);
            }
        }

        private string[] args;
    }
}
