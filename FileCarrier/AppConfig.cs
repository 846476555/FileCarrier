using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace FileCarrier
{
    public class AppConfig : Config
    {
        public static AppConfig Instance;
        public AppConfig() : base("./config.json")
        {
            Instance = this;
        }

        #region Config

        public string FilePath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Origin";

        public string ZipPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Target";

        public int TimeInterval { get; set; } = 60;
        #endregion

        #region Keep
        //public void Load()
        //{
        //    if (File.Exists(ConfigPath))
        //    {
        //        var json = File.ReadAllText(ConfigPath);
        //        var config = JsonConvert.DeserializeObject<AppConfig>(json);
        //        if (config != null)
        //        {
        //            var properties = GetType().GetProperties();
        //            foreach (var propertyInfo in properties)
        //            {
        //                if (propertyInfo.CanWrite)
        //                    propertyInfo.SetValue(this, propertyInfo.GetValue(config));
        //            }
        //        }
        //    }
        //    Save();
        //}

        //public void Save()
        //{
        //    var json = JsonConvert.SerializeObject(this);
        //    File.WriteAllText(ConfigPath, json);
        //}
        #endregion
    }
    public abstract class Config
    {
        [JsonIgnore]
        public EventHandler OnConfigChanged;

        [JsonIgnore]
        public readonly string ConfigPath;

        public Config(string filePath)
        {
            ConfigPath = filePath;
        }

        public virtual void Load()
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                var config = JsonConvert.DeserializeObject(json, GetType());
                if (config != null)
                {
                    var properties = GetType().GetProperties();
                    foreach (var propertyInfo in properties)
                    {
                        if (propertyInfo.CanWrite)
                            propertyInfo.SetValue(this, propertyInfo.GetValue(config));
                    }
                }
            }

            Save();
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(ConfigPath, json);
            OnConfigChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
