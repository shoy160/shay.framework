﻿using Dapper;
using Shay.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shay.Dapper.Domain
{
    /// <summary> 基础仓储 </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : DapperRepository
        where T : IEntity
    {
        private readonly string _tableName;
        private readonly string[] _fields;
        private readonly Type _modelType;
        private readonly ILogger _logger = LogManager.Logger("BaseRepository");

        /// <summary> 构造 </summary>
        /// <param name="connectionName"></param>
        public BaseRepository(string connectionName) : base(connectionName)
        {
            _modelType = typeof(T);
            _tableName = _modelType.Name;
            _fields = _modelType.Fields();
        }

        /// <summary> 构造 </summary>
        /// <param name="enumType"></param>
        public BaseRepository(Enum enumType) : this(enumType.ToString())
        {
        }

        /// <summary> 查询所有数据 </summary>
        public IEnumerable<T> Query()
        {
            var fields = string.Join(",", _fields.Select(t => $"[{t}]"));
            var sql = $"SELECT {fields} FROM [{_tableName}]";
            _logger.Debug(sql);
            return Connection.Query<T>(sql);
        }

        /// <summary> 根据主键查询单条 </summary>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public T QueryById(object key, string keyColumn = "id")
        {
            var fields = string.Join(",", _fields.Select(t => $"[{t}]"));
            var sql = $"SELECT {fields} FROM [{_tableName}] WHERE [{keyColumn}]=@id";
            _logger.Debug(sql);
            return Connection.QueryFirstOrDefault<T>(sql, new { id = key });
        }

        /// <summary> 插入单条数据,不支持有自增列 </summary>
        /// <param name="model"></param>
        /// <param name="excepts">过滤项(如：自增ID)</param>
        /// <returns></returns>
        public int Insert(T model, string[] excepts = null)
        {
            var sql = _modelType.InsertSql(excepts);
            _logger.Debug(sql);
            return Connection.Execute(sql, model);
        }

        /// <summary> 批量插入 </summary>
        /// <param name="models"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        public int BatchInsert(IEnumerable<T> models, string[] excepts = null)
        {
            var sql = _modelType.InsertSql(excepts);
            _logger.Debug(sql);
            return Connection.Execute(sql, models.ToArray());
        }

        ///// <summary> 未实现 </summary>
        ///// <param name="update"></param>
        ///// <param name="condition"></param>
        ///// <returns></returns>
        //public int Update(Expression<Func<T>> update, Expression<Func<T, bool>> condition)
        //{
        //    var set = ExpressionHelper.GetExpression(update);
        //    var where = ExpressionHelper.GetExpression(condition);
        //    var sql = $"UPDATE [{_tableName}] SET {set} WHERE {where}";
        //    return Connection.Execute(sql);
        //}

        /// <summary> 删除 </summary>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        public int Delete(object key, string keyColumn = "id")
        {
            var sql = $"DELETE FROM [{_tableName}] WHERE [{keyColumn}]=@key";
            _logger.Debug(sql);
            return Connection.Execute(sql, new { key });
        }
    }
}
