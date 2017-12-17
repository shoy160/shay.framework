using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Shay.Core.Cache;
using Shay.Core.Extensions;

namespace Shay.Dapper
{
    /// <summary> Dapper自定义扩展 </summary>
    public static class DapperExtension
    {
        private static readonly ICache DapperCache = CacheManager.GetCacher(nameof(DapperExtension));
        /// <summary> 查询到DataSet </summary>
        /// <param name="cnn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="adapter"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet QueryDataSet(this IDbConnection cnn, string sql,
            object param = null, IDbDataAdapter adapter = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            var ds = new DataSet();
            var wasClosed = cnn.State == ConnectionState.Closed;
            if (wasClosed) cnn.Open();
            var command = cnn.CreateCommand();
            if (commandType.HasValue)
                command.CommandType = commandType.Value;
            if (commandTimeout.HasValue)
                command.CommandTimeout = commandTimeout.Value;
            command.CommandText = sql;
            if (param != null)
            {
                var ps = param.GetType().GetProperties();
                foreach (var propertyInfo in ps)
                {
                    var propType = propertyInfo.PropertyType;
                    var value = propertyInfo.GetValue(param);
                    if (propType.IsNullableType() && value == null)
                        continue;
                    var p = command.CreateParameter();
                    p.ParameterName = $"@{propertyInfo.Name}";
                    p.Value = value;
                    command.Parameters.Add(p);
                }
            }
            adapter = adapter ?? new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(ds);
            if (wasClosed) cnn.Close();
            return ds;
        }

        /// <summary> 字段列表 </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static string[] Fields(this Type modelType)
        {
            var key = $"fields_{modelType.FullName}";
            var fields = DapperCache.Get<string[]>(key);
            if (fields != null && fields.Any()) return fields;
            var props = modelType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            fields = props.Select(t => t.Name).ToArray();
            DapperCache.Set(key, fields, TimeSpan.FromHours(2));
            return fields;
        }
        /// <summary> 生成insert语句 </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string InsertSql(this Type modelType, string[] excepts = null)
        {
            var tableName = modelType.Name;
            var key = $"insert_{modelType.FullName}";
            var sql = DapperCache.Get<string>(key);
            if (!string.IsNullOrWhiteSpace(sql))
                return sql;
            var sb = new StringBuilder();
            sb.Append($"INSERT INTO [{tableName}]");

            var fields = Fields(modelType);
            if (excepts != null && excepts.Any())
                fields = fields.Except(excepts).ToArray();
            var fieldSql = string.Join(",", fields.Select(t => $"[{t.ToLower()}]"));
            var paramSql = string.Join(",", fields.Select(t => $"@{t}"));
            sb.Append($" ({fieldSql}) VALUES ({paramSql})");
            sql = sb.ToString();
            DapperCache.Set(key, sql, TimeSpan.FromHours(1));
            return sql;
        }

        /// <summary> 设置对象属性 </summary>
        /// <param name="model"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public static void SetPropValue(this object model, string propName, object value)
        {
            var type = model.GetType();
            var prop = type.GetProperty(propName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null)
                prop.SetValue(model, value);
        }

        /// <summary> 获取对象属性 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static T PropValue<T>(this object model, string propName)
        {
            var type = model.GetType();
            var prop = type.GetProperty(propName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return prop == null ? default(T) : prop.GetValue(model).CastTo<T>();
        }

        /// <summary> 获取对象属性 </summary>
        /// <param name="model"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static string PropValue(this object model, string propName)
        {
            return PropValue<string>(model, propName);
        }
    }
}
