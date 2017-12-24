using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Shay.Core.Helper
{
    public class ConfigHelper
    {
        private IConfigurationRoot _config;
        private IConfigurationBuilder _builder;
        private const string ConfigPrefix = "config:";
        private const string ConfigName = "appsettings.json";

        private ConfigHelper()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigName);
            _builder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory());
            if (File.Exists(path))
            {
                _config = _builder.AddJsonFile(ConfigName, optional: false, reloadOnChange: true).Build();
            }
            else
            {
                _config = _builder.Build();
            }
        }

        public static ConfigHelper Instance =
            Singleton<ConfigHelper>.Instance ?? (Singleton<ConfigHelper>.Instance = new ConfigHelper());


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
            key = $"{ConfigPrefix}{key}";
            var type = typeof(T);
            if (type.IsValueType || type == typeof(string))
                return _config.GetValue(key, defaultValue);

            var obj = Activator.CreateInstance<T>();
            _config.GetSection(key).Bind(obj);
            return obj;
        }
    }
}
