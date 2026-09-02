using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvancedPlanningSystem
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // [Logging] 初始化 log4net (從 App.config 或執行目錄載入)
            string appConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.config");
            if (System.IO.File.Exists(appConfigPath))
            {
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(appConfigPath));
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure();
            }

            // [Configuration] 載入外部 appsettings.json 設定
            AppConfig.Load();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
