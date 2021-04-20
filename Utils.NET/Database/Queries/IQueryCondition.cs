using System;
namespace Utils.NET.Database.Queries
{
    internal interface IQueryCondition<T> : IQueryPart<T> where T : DbModel
    {
    }
}
