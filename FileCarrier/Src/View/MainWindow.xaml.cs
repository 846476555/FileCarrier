using FileCarrier.Src.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace FileCarrier.Src.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainWindowViewModel.Instance;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            ((App)Application.Current).IsMainWindowShowing = false;
        }

        private void File_OnClick(object sender, RoutedEventArgs e)
        {
            //var ttt = new System.Windows.Forms.OpenFileDialog();
            //ttt.Filter = "|*";
            //ttt.ShowDialog();

            var dialog = new FolderBrowserDialog() { SelectedPath = MainWindowViewModel.Instance.FilePath };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                MainWindowViewModel.Instance.FilePath = dialog.SelectedPath;
        }

        private void Zip_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog() { SelectedPath = MainWindowViewModel.Instance.ZipPath };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                MainWindowViewModel.Instance.ZipPath = dialog.SelectedPath;
        }

        private void KeepConfig_OnClick(object sender, RoutedEventArgs e)
        {
            AppConfig config = AppConfig.Instance;
            config.FilePath = MainWindowViewModel.Instance.FilePath;
            config.ZipPath = MainWindowViewModel.Instance.ZipPath;
            AppConfig.Instance.Save();
            MessageBox.Show("配置保存成功");
        }
        private void StartKeep_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MainWindowViewModel.Instance.FilePath))
            {
                MessageBox.Show("请指定待压缩源文件文件夹");
                return;
            }
            if (string.IsNullOrEmpty(MainWindowViewModel.Instance.ZipPath))
            {
                MessageBox.Show("请指定压缩包存放文件夹");
                return;
            }

            bool res = int.TryParse(TimeInterval_TB.Text, out int result);
            if (res)
            {
                if (result == 0)
                {
                    MessageBox.Show("运行间隔时间太短");
                    return;
                }
                MainWindowViewModel.Instance.TimeInterval = result;
            }
            else
            {
                MessageBox.Show("时间间隔输入框，值格式不符");
            }
            Task.Run(() =>
            {
                MainWindowViewModel.Instance.CarryFiles();
            });
        }

    }
}
