using System.Threading.Tasks;

namespace Utils.NET.Database.Queries
{
    public interface IQueryResult<T, TResult> where T : DbModel, new()
    {
        void SetQuery(QueryBuilder<T> queryBuilder);

        Task<TResult> GetResult();
    }
}
