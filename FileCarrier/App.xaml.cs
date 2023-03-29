using FileCarrier.Src.View;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using Timer = System.Timers.Timer;

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
            if (CheckSingleInstance())
            {
                return;
            }
            //程序初始化
            StartApp();
        }

        private static bool CheckSingleInstance()
        {
            const string appName = "FileCarrier";
            Mutex mutex = new Mutex(true, appName, out var createdNew);

            if (!createdNew)
            {
                //MessageWindow.Show("多媒体管理已经启动");
                MessageBox.Show("软件已启动");
                return true;
            }

            return false;
        }
        private void StartApp()
        {
            //加载用户配置
            var appConfig = new AppConfig();
            appConfig.Load();

            ViewModel = new MainWindowViewModel();

            InitIcon();

            //是否打开页面 | 后台挂载
        }
        public bool IsMainWindowShowing = false;
        private void ShowWindow()
        {
            if (!IsMainWindowShowing)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                IsMainWindowShowing = true;
            }
        }
        private void App_OnSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            ReasonSessionEnding reason = e.ReasonSessionEnding;
        }
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            ///如果正在拷贝等待拷贝完成或终止拷贝
        }

        #region 托盘图标
        private bool _canClick = true;
        private void InitIcon()
        {
            NotifyIcon icon = new NotifyIcon()
            {
                BalloonTipText = AppName
            };
            icon.Text = AppName;
            icon.Visible = true;

            var uri = new Uri(@"/Resources/Image/" + "icon.ico", UriKind.RelativeOrAbsolute);
            var stream = GetResourceStream(uri)?.Stream;
            icon.Icon = new Icon(stream ?? throw new InvalidOperationException());

            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);

            icon.ShowBalloonTip(3000, "You Have A New Message", "Yoo ! Man What's Up ? ", ToolTipIcon.Info);

            icon.MouseDoubleClick += (sender, args) =>
            {
                //Logger.Info("Click icon");
                if (_canClick)
                {
                    //Logger.Info("can Click icon");
                    _canClick = false;
                    Timer t = new Timer(2000);
                    t.Elapsed += new System.Timers.ElapsedEventHandler(ShowNotifyIcon);
                    t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
                    t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                    t.Start();
                    ShowWindow();
                }
            };

            var exit = new MenuItem(@"退出");
            exit.Click += (o, s) =>
            {
                Shutdown();
            };
            MenuItem[] children = { exit };
            icon.ContextMenu = new ContextMenu(children);
        }
        private void ShowNotifyIcon(object source, System.Timers.ElapsedEventArgs e)
        {
            //Logger.Info("show icon");
            _canClick = true;
        }
        #endregion
    }
}
