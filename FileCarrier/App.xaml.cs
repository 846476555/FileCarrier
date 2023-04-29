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
using IWshRuntimeLibrary;
using System.IO;
using File = System.IO.File;

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

            //开机自启初始化
            AutoStartInit();

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

        #region 开机自启
        private void AutoStartInit()
        {
            if (AppConfig.Instance.AutoInit)
            {
                //生成程序开机自启快捷方式
                CreatedAutoStartLink();
            }
            else
            {
                //从开机自启目录清除程序快捷方式
                ClearAutoStartLink();
            }
        }
        private void ClearAutoStartLink()
        {
            // 获取启动文件夹路径
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (File.Exists(startupPath + "\\FileCarrier.lnk"))
            {
                File.Delete(startupPath + "\\FileCarrier.lnk");
            }
        }
        private void CreatedAutoStartLink()
        {
            //不操作注册表
            //位于用户\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup下

            // 获取启动文件夹路径
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            //程序绝对路径
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //程序目录绝对路径
            var workingDirectory = Path.GetFullPath(Environment.CurrentDirectory);

            // 创建快捷方式对象
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Path.Combine(startupPath, "FileCarrier.lnk"));

            // 设置快捷方式的属性
            shortcut.TargetPath = path;
            shortcut.WorkingDirectory = workingDirectory;
            shortcut.Description = "FileCarrier";
            //shortcut.IconLocation = "icon绝对路径";//默认未程序的图标
            shortcut.Save();

            //桌面路径
            //Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        #endregion
    }
}
