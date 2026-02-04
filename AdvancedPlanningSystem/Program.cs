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
            // [Security] 啟用 TLS 1.2 並保持向下相容 (Bitwise OR)
            // 解決 .NET 4.5.2 預設只開啟 SSL3/TLS1.0 的問題
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12; // | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // [Logging] 初始化 log4net
            log4net.Config.XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
