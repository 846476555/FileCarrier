using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using System.Security.AccessControl;
using NLog.Targets;

namespace FileCarrier.Src.View
{
    public enum RunningState
    {
        Stop,
        Running,
        Ziping
    }
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        

        public static MainWindowViewModel Instance { get; private set; }
        public MainWindowViewModel()
        {
            Instance = this;
            Init();
        }

        #region Properties
        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath == value)
                    return;
                _filePath = value;
                OnPropertyChanged();
            }
        }

        private string _zipPath;
        public string ZipPath
        {
            get => _zipPath;
            set
            {
                if (_zipPath == value)
                    return;
                _zipPath = value;
                OnPropertyChanged();
            }
        }
        private int _timeInterval = 60;
        public int TimeInterval
        {
            get => _timeInterval;
            set
            {
                if (_timeInterval == value)
                    return;
                _timeInterval = value;
                OnPropertyChanged();
            }
        }

        private RunningState _keepState = RunningState.Stop;
        public RunningState KeepState
        {
            get => _keepState;
            set
            {
                if (_keepState == value)
                    return;
                _keepState = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Method

        public void Init()
        {
            FilePath = AppConfig.Instance.FilePath;
            ZipPath = AppConfig.Instance.ZipPath;
        }
        public void CarryFiles()
        {
            DateTime date = DateTime.Now;

            //FilePath
            //    ZipPath

            //string filePath = desktop + @"\Origin";
            //string zipPath = desktop + @"\Target\" + date.ToString("yyyy-MM-dd-HH-mm-ss") + @".zip";

            //temp
            //DirectoryInfo di = Directory.CreateDirectory(ZipPath + @"\temp");
            //di.Attributes = FileAttributes.Hidden;

            //转移文件(不转移)

            KeepState = RunningState.Ziping;

            FilesToZip(FilePath, ZipPath + @"\" + date.ToString("yyyy-MM-dd-HH-mm-ss") + @".zip");

            KeepState = RunningState.Stop;
        }
        public bool MoveFile()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(FilePath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                //获取目录下（不包含子目录）的文件和子目录
                //foreach (FileSystemInfo i in fileinfo)
                //{
                //    if (i is DirectoryInfo)     //判断是否文件夹
                //    {
                //        if (!Directory.Exists(destPath + "\\" + i.Name))
                //        {
                //            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                //        }
                //        CopyDir(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                //    }
                //    else
                //    {
                //        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        public void FilesToZip(string origin, string target)
        {

            //压缩文件(弃用)
            //00:00:04.4753096

            //DateTime start = DateTime.Now;
            //ZipFile.CreateFromDirectory(origin, target);
            //DateTime end = DateTime.Now;
            //Sub(start, end);
            //return;
            //FileStream zipToOpen = File.Create(target);
            //ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            //FindAllFiles(origin,"/");
            //ZipArchiveEntry readmeEntry1 = archive.CreateEntry(@"file1/123.txt");
            //throw new NotImplementedException();
            //ZipArchiveEntry readmeEntry2 = archive.CreateEntry(@"file2/123.txt");

            //00:00:04.2261766
            DateTime start = DateTime.Now;
            using (FileStream zipToOpen = File.Create(target))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    FindAllFiles(origin, "", archive);
                }
            }


            DateTime end = DateTime.Now;
            Sub(start, end);


            void FindAllFiles(string originPath, string zipPath, ZipArchive archive)
            {
                DirectoryInfo dir = new DirectoryInfo(originPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();

                foreach (FileSystemInfo file in fileinfo)
                {
                    if (file.Attributes is FileAttributes.Directory)
                    {
                        archive.CreateEntry(zipPath + file.Name + "/");
                        FindAllFiles(file.FullName, zipPath + file.Name + "/", archive);
                    }
                    else
                    {
                        archive.CreateEntryFromFile(file.FullName, zipPath + file.Name);
                        //ZipArchiveEntry fileEntry = archive.CreateEntry(zipPath + file.Name);
                    }
                }
            }

            void Sub(DateTime t1, DateTime t2){
                Console.WriteLine(t1.Subtract(t2).Duration().ToString());
            }
        }

        public void ZipToFiles(string origin, string target)
        {
            //解压文件
            ZipFile.ExtractToDirectory(origin, target);
            //ZipFile.ExtractToDirectory(filePath, zipPath, Encoding.UTF8);
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
