using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace RA2AI_Editor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //System.Threading.Mutex mutex;
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary> 
        /// <param name="hWnd">窗口句柄</param> /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分</param> 
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary> 
        ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。
        ///  系统给创建前台窗口的线程分配的权限稍高于其他线程。 
        /// </summary> 
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param> 
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>          
        [DllImport("User32.dll")]

        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            Process process = RuningInstance();
            if (process != null)
            {
                HandleRunningInstance(process);
                Environment.Exit(0);
            }

#if DEBUG
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException_Debug);
#else
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException_Release);
#endif

            LoadLanguage();
        }

        private static void CurrentDomain_UnhandledException_Debug(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                "UnhandledException: " + e.ToString() +
                "\r\nExceptionObject: " + e.ExceptionObject.ToString());
        }

        private static void CurrentDomain_UnhandledException_Release(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(Local.Dictionary("EXC_EXITNOW"));
        }

        private const int SW_SHOWNOMAL = 1;
        private static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, SW_SHOWNOMAL);
            SetForegroundWindow(instance.MainWindowHandle);
        }

        private static Process RuningInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] Processes = Process.GetProcessesByName(currentProcess.ProcessName);
            foreach (Process process in Processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == currentProcess.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        private static readonly string[] SupportedLanguage = new string[] { "en", "zh-CN" };
        public static string LanguageCurrent;
        private void LoadLanguage()
        {
            IniClass lang = new IniClass(AppDomain.CurrentDomain.BaseDirectory + @"\language.ini");
            LanguageCurrent = lang.ReadValueWithoutNotes("Language", "Current");
            if (!SupportedLanguage.Contains(LanguageCurrent))
            {
                LanguageCurrent = CultureInfo.CurrentUICulture.Name;
                if (!SupportedLanguage.Contains(LanguageCurrent))
                    LanguageCurrent = SupportedLanguage[0];
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(LanguageCurrent);

            lang.WriteValue("Language", "Current", LanguageCurrent);
            lang.Save();
            //CultureInfo currentCultureInfo = CultureInfo.CurrentCulture;
            ResourceDictionary langRd = null;
            try
            {
                langRd =
                Application.LoadComponent(
                new Uri(LanguageCurrent + ".xaml", UriKind.Relative)) as ResourceDictionary;
            }
            catch
            {
            }
            if (langRd != null)
            {
                this.Resources.MergedDictionaries.Remove(langRd);
                this.Resources.MergedDictionaries.Insert(0, langRd);
            }
        }
    }
}
