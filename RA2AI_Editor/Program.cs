using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RA2AI_Editor
{
    public static class Program
    {
        public static string FileToOpen = null;

        [STAThread]
        public static void Main(string[] Args)
        {
            if (Args != null && Args.Length > 0)
                FileToOpen = Args[0];

            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
