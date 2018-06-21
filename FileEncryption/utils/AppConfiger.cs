using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace FileEncryption
{
    class AppConfiger
    {
        private const String outPathKey = "outputPath";
        private const String serverPathKey = "serverPath";
        //读取配置文件
        public static String getOutputPath()
        {
            String outPutPath = ConfigurationManager.AppSettings[outPathKey].ToString();
            return outPutPath;
        }

        public static String getServertPath()
        {
            String outPutPath = ConfigurationManager.AppSettings[serverPathKey].ToString();
            return outPutPath;
        }

        //写入配置文件
        public static bool saveOutputPath(String path)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[outPathKey].Value = path;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            return true;
        }

        public static bool saveServerPath(String path)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[serverPathKey].Value = path;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            return true;
        }
    }
}
