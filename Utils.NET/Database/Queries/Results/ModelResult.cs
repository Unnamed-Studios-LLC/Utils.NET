using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Utils.NET.Logging;

namespace Utils.NET.Database.Queries.Results
{
    public class ModelResult<T> : IQueryResult<T, List<T>> where T : DbModel, new()
    {
        private QueryBuilder<T> queryBuilder;

        public void SetQuery(QueryBuilder<T> queryBuilder)
        {
            this.queryBuilder = queryBuilder;
        }

        public async Task<List<T>> GetResult()
        {
            T model = null;
            var results = new List<T>();
            var connection = await Query.GetConnection();

            try
            {
                var command = queryBuilder.Build(connection, ref model);
                using (var r = await command.ExecuteReaderAsync())
                {
                    if (!r.HasRows)
                    {
                        return results;
                    }

                    while (await r.ReadAsync())
                    {
                        var returnedModel = new T();
                        model.CloneFieldsInto(returnedModel);
                        returnedModel.ReadFromDb(r);

                        results.Add(returnedModel);
                    }
                }
                return results;
            }
            finally
            {
                Query.ReturnConnection(connection);
            }
        }
    }
}
