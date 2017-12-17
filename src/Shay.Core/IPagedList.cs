using System.Collections.Generic;

namespace Shay.Core
{
    public interface IPagedList<TEntity> : IList<TEntity>
    {
        int PageIndex { get; }
        int PageCount { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        PagedList<T> ConvertData<T>(IEnumerable<T> enumerable);
    }
}
