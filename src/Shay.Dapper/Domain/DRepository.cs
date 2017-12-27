using Dapper;
using Shay.Core;
using System;
using System.Data;

namespace Shay.Dapper.Domain
{
    public abstract class DRepository
    {
        private readonly string _defaultConnectionName;
        private const string DefaultName = "default";
        /// <summary> 获取默认连接 </summary>
        protected IDbConnection Connection => GetConnection(_defaultConnectionName);

        protected DRepository(string connectionName)
        {
            _defaultConnectionName = string.IsNullOrWhiteSpace(connectionName) ? DefaultName : connectionName;
        }

        public static TRepository Instance<TRepository>()
            where TRepository : DRepository, new()
        {
            return Singleton<TRepository>.Instance ?? (Singleton<TRepository>.Instance = new TRepository());
        }

        protected DRepository(Enum enumType) : this(enumType.ToString())
        {
        }

        /// <summary>
        /// 参数化前缀
        /// </summary>
        protected virtual string FormatVariable(string variable)
        {
            return $"@{variable}";
        }
        protected virtual string FormatColumn(string column)
        {
            return $"[{column}]";
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName"></param>
        /// <param name="threadCache"></param>
        /// <returns></returns>
        protected IDbConnection GetConnection(Enum connectionName, bool threadCache = true)
        {
            return ConnectionFactory.Instance.Connection(connectionName, threadCache);
        }

        /// <summary> 获取数据库连接 </summary>
        /// <param name="connectionName"></param>
        /// <param name="threadCache"></param>
        /// <returns></returns>
        protected IDbConnection GetConnection(string connectionName, bool threadCache = true)
        {
            return ConnectionFactory.Instance.Connection(connectionName, threadCache);
        }

        /// <summary> 执行数据库事务 </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="connectionName"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected TResult Transaction<TResult>(Func<IDbConnection, IDbTransaction, TResult> action, string connectionName = null,
            IsolationLevel? level = null)
        {
            var conn = GetConnection(connectionName ?? _defaultConnectionName, false);
            using (conn)
            {
                conn.Open();
                var transaction = level.HasValue ? conn.BeginTransaction(level.Value) : conn.BeginTransaction();
                using (transaction)
                {
                    try
                    {
                        var result = action.Invoke(conn, transaction);
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary> 执行数据库事务 </summary>
        /// <param name="action"></param>
        /// <param name="connectionName"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected DResult Transaction(Action<IDbConnection, IDbTransaction> action, string connectionName = null,
            IsolationLevel? level = null)
        {
            var result = Transaction((conn, trans) =>
            {
                action.Invoke(conn, trans);
                return DResult.Success;
            }, connectionName, level);
            return result ?? DResult.Error("事务执行失败");
        }

        /// <summary> 插入数据，返回自增的ID </summary>
        /// <typeparam name="T">Id类型:byte,int,long</typeparam>
        /// <param name="conn">数据库连接</param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        protected T InsertEntity<T>(IDbConnection conn, string sql, object param = null,
            IDbTransaction transaction = null)
            where T : struct
        {
            return string.IsNullOrWhiteSpace(sql)
                ? default(T)
                : new SQL(sql).InsertIdentity<T>(conn, param, transaction);
        }

        /// <summary> 更新数量 </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <param name="count"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        protected int UpdateCount(IDbConnection conn, string table, string column, object key, string keyColumn = "id",
            int count = 1, IDbTransaction trans = null)
        {
            var sql = $"UPDATE {FormatColumn(table)} SET {FormatColumn(column)}={FormatColumn(column)}+{FormatVariable("count")} WHERE {FormatColumn(keyColumn)}={FormatVariable("id")}";
            return conn.Execute(sql, new { id = key, count }, trans);
        }

        /// <summary> 异步更新数量 </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="key"></param>
        /// <param name="keyColumn"></param>
        /// <param name="count"></param>
        protected void UpdateCountAsync(string table, string column, object key, string keyColumn = "id", int count = 1)
        {
            Connection.ExecuteAsync($"UPDATE {FormatColumn(table)} SET {FormatColumn(column)}={FormatColumn(column)}+{FormatVariable("count")} WHERE {FormatColumn(keyColumn)}={FormatVariable("id")}",
                new { id = key, count });
        }
    }
}
