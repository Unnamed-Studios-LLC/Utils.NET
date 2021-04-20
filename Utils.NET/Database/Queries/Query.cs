using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Utils.NET.Database.Queries.Methods;
using Utils.NET.Database.Queries.Results;

namespace Utils.NET.Database.Queries
{
    public static class Query
    {
        private static readonly ConcurrentQueue<DbConnection> connections = new ConcurrentQueue<DbConnection>();

        private static IDbConnectionFactory connectionFactory;

        /// <summary>
        /// Sets the database connection factory
        /// </summary>
        public static void SetConnectionFactory(IDbConnectionFactory factory)
        {
            connectionFactory = factory;
        }

        /// <summary>
        /// Trys to get a database from the queue, creates a new database if none are available
        /// </summary>
        internal async static Task<DbConnection> GetConnection()
        {
            if (connections.TryDequeue(out var connection) && connection.State == ConnectionState.Open)
                return connection;
            return await Create();
        }

        /// <summary>
        /// Returns a connection to the queue
        /// </summary>
        internal static void ReturnConnection(DbConnection connection)
        {
            connections.Enqueue(connection);
        }

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <returns></returns>
        private async static Task<DbConnection> Create()
        {
            return await connectionFactory?.CreateDbConnection();
        }

        public static QueryConditionSelector<T, ModelResult<T>, List<T>> Select<T>(Func<T, DbFieldValue[]> fieldGetter = null) where T : DbModel, new()
        {
            var builder = new QueryBuilder<T>();
            builder.SetMethod(new SelectMethod<T>(fieldGetter));
            return new QueryConditionSelector<T, ModelResult<T>, List<T>>(builder);
        }

        public static QueryConditionSelector<T, OperationResult<T>, int> Update<T>(T model) where T : DbModel, new()
        {
            var builder = new QueryBuilder<T>();
            builder.SetMethod(new UpdateMethod<T>(model));
            return new QueryConditionSelector<T, OperationResult<T>, int>(builder);
        }

        public static Task<int> Insert<T>(T model) where T : DbModel, new()
        {
            var builder = new QueryBuilder<T>();
            builder.SetMethod(new InsertMethod<T>(model));

            var result = new OperationResult<T>();
            result.SetQuery(builder);
            return result.GetResult();
        }

        public static QueryConditionSelector<T, OperationResult<T>, int> Delete<T>() where T : DbModel, new()
        {
            var builder = new QueryBuilder<T>();
            builder.SetMethod(new DeleteMethod<T>());
            return new QueryConditionSelector<T, OperationResult<T>, int>(builder);
        }
    }
}
