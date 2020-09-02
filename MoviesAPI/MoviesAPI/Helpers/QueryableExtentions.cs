using System.Linq;
using MoviesAPI.DTOs;

namespace MoviesAPI.Helpers
{
    public static class QueryableExtentions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int recordsPerPage, int page)
        {
            return queryable
                .Skip((page - 1) * recordsPerPage)
                .Take(recordsPerPage);
        }
    }
}