using System.Data.Common;
using System.Threading.Tasks;

namespace Utils.NET.Database.Queries
{
    public interface IDbConnectionFactory
    {
        Task<DbConnection> CreateDbConnection();
    }
}
