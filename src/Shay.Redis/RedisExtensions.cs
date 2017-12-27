using Shay.Core.Extensions;
using StackExchange.Redis;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shay.Redis
{
    public static class RedisExtensions
    {
        #region 私有方法
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }
            var binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                return binaryFormatter.Deserialize(memoryStream).CastTo<T>();
            }
        }
        #endregion

        public static T Get<T>(this IDatabase database, string key)
        {
            var value = database.StringGet(key);
            return Deserialize<T>(value);
        }

        public static object Get(this IDatabase database, string key)
        {
            return database.Get<object>(key);
        }

        public static void Set<T>(this IDatabase database, string key, T value, TimeSpan? expired = null)
        {
            database.StringSet(key, Serialize(value), expired);
        }
    }
}
