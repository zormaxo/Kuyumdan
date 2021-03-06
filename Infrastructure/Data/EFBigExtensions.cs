using Core.Interfaces;
using System.Linq.Expressions;

namespace Infrastructure.Data
{
    public static class EFBigExtensions
    {
        public static IQueryable<TSource> WhereIf<TSource>(
            this IQueryable<TSource> source,
            bool condition,
            Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IQueryable<TSource> PageBy<TSource>(
            this IQueryable<TSource> source, IPagedResultRequest request)
        {
            return source.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
        }
    }
}