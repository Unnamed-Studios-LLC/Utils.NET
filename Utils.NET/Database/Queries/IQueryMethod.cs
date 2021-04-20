using System;
using System.Text;

namespace Utils.NET.Database.Queries
{
    internal interface IQueryMethod<T> : IQueryPart<T> where T : DbModel
    {

    }
}
