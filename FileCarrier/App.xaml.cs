using FileCarrier.Src.View;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileCarrier
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private const string AppName = @"文件搬运器";

        public MainWindowViewModel ViewModel = null;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            //是否已启动

            //程序初始化
            StartApp();
        }


        private void StartApp()
        {
            //加载用户配置
            var appConfig = new AppConfig();
            appConfig.Load();

            ViewModel = new MainWindowViewModel();
            //是否打开页面 | 后台挂载
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        private void App_OnSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            ReasonSessionEnding reason = e.ReasonSessionEnding;
        }
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            ///如果正在拷贝等待拷贝完成或终止拷贝
        }
    }
}
