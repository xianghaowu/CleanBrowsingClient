using CleanBrowsingClient.Models;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CleanBrowsingClient.Helper
{
    class ConfigureSettingManager
    {
        public string FileName => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configure.bin");
        public void Serialize(ConfigureSetting pConfSettings)
        {
            Stream ms = File.OpenWrite(FileName);

            var formatter = new BinaryFormatter();

            formatter.Serialize(ms, pConfSettings);
            ms.Flush();
            ms.Close();
            ms.Dispose();
        }
        public ConfigureSetting Deserialize()
        {
            var formatter = new BinaryFormatter();

            var fs = File.Open(FileName, FileMode.Open);

            var obj = formatter.Deserialize(fs);
            var confSettings = (ConfigureSetting)obj;
            fs.Flush();
            fs.Close();
            fs.Dispose();

            return confSettings;

        }
    }
}
