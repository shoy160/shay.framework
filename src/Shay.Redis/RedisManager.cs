using Shay.Core;
using Shay.Core.Exceptions;
using Shay.Core.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace Shay.Redis
{
    /// <summary> Redis管理器 </summary>
    public class RedisManager : IDisposable
    {
        private string DefaultName;
        private ConcurrentDictionary<string, ConnectionMultiplexer> _connections;
        private RedisManager()
        {
            DefaultName = "redisConfigName".Config("default");
            _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        public static RedisManager Instance = Singleton<RedisManager>.Instance = (Singleton<RedisManager>.Instance = new RedisManager());

        private string GetConfigName(string configName)
        {
            if (string.IsNullOrWhiteSpace(configName))
                return DefaultName;
            return configName;
        }

        private string GetConnectionString(string configName)
        {
            var connectionString = $"redis:{configName}".Config(string.Empty);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new BusiException($"config:redis:{configName}配置异常");
            return connectionString;
        }

        private ConnectionMultiplexer GetConnection(string configName)
        {
            configName = GetConfigName(configName);
            var connectionString = GetConnectionString(configName);
            return _connections.GetOrAdd(configName, p => ConnectionMultiplexer.Connect(connectionString));
        }

        private ConnectionMultiplexer GetConnection(string configName, ConfigurationOptions configOpts)
        {
            configName = GetConfigName(configName);
            return _connections.GetOrAdd(configName, p => ConnectionMultiplexer.Connect(configOpts));
        }

        public IDatabase GetDatabase(string configName = null, int defaultDb = -1)
        {
            var conn = GetConnection(configName);
            return conn.GetDatabase(defaultDb);
        }

        public IServer GetServer(string configName = null, int endPointsIndex = 0)
        {
            configName = GetConfigName(configName);

            var connectionString = GetConnectionString(configName);
            var confOption = ConfigurationOptions.Parse(connectionString);

            return GetConnection(configName, confOption).GetServer(confOption.EndPoints[endPointsIndex]);
        }

        public IServer GetServer(string host, int port, string configName = null)
        {
            var conn = GetConnection(configName);
            return conn.GetServer(host, port);
        }

        public ISubscriber GetSubscriber(string configName = null)
        {
            return GetConnection(configName).GetSubscriber();
        }

        public void Dispose()
        {
            if (_connections == null || _connections.Count == 0)
                return;
            _connections.Values.Foreach(t => t.Close());
        }
    }
}
