using System;
using System.Windows.Forms;

namespace APSSimulator
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // 初始化資料庫
            DB.DatabaseHelper.InitializeDatabase();

            Application.Run(new FormMain());
        }
    }
}
