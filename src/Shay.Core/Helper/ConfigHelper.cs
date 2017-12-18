using Microsoft.Extensions.Configuration;
using System.IO;
using System.Runtime.CompilerServices;

namespace Shay.Core.Helper
{
    public class ConfigHelper
    {
        private IConfigurationRoot _config;

        private ConfigHelper()
        {
            _config = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .Build();
        }

        public static ConfigHelper Instance = Singleton<ConfigHelper>.Instance ?? (Singleton<ConfigHelper>.Instance = new ConfigHelper());

        /// <summary>
        /// 得到AppSettings中的配置字符串信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfigString(string key)
        {
            return GetAppSetting(string.Empty, supressKey: key);
        }

        /// <summary> 配置文件读取 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseFunc">类型转换方法</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="key">配置名</param>
        /// <param name="supressKey">配置别名</param>
        /// <returns></returns>
        public T GetAppSetting<T>(T defaultValue = default(T), [CallerMemberName] string key = null,
              string supressKey = null)
        {
            if (!string.IsNullOrWhiteSpace(supressKey))
                key = supressKey;
            key = $"config:{key}";
            return _config.GetValue(key, defaultValue);
        }
    }
}
