using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Common.Extensions
{
    public static class PagingExtensions
    {
        public static Pager<T> ToPagination<T>(this IList<T> query, int currentPage, int pageSize)
        {
            if (currentPage == 0) currentPage = PageSetting.Page;
            if (pageSize == 0) pageSize = PageSetting.PageSize;

            int skip = (currentPage - 1) * pageSize;
            var items = query.Skip(skip).Take(pageSize).ToList();
            var totalRecord = query.Count;

            return new Pager<T>(items, totalRecord);
        }

        public static async Task<Pager<T>> ToPaginationAsync<T>(this IQueryable<T> query, int currentPage, int pageSize)
        {
            if (currentPage == 0) currentPage = PageSetting.Page;
            if (pageSize == 0) pageSize = PageSetting.PageSize;

            int skip = (currentPage - 1) * pageSize;
            var items = await query.Skip(skip).Take(pageSize).ToListAsync();
            var totalRecord = await query.CountAsync();

            return new Pager<T>(items, totalRecord);
        }

        public class Pager<T>
        {
            public Pager(IEnumerable<T> result, int total)
            {
                Result = result;
                Total = total;
            }

            public IEnumerable<T> Result { get; private set; }

            public int Total { get; private set; }
        }

        public static class PageSetting
        {
            public static int Page { get; set; } = 1;

            public static int PageSize { get; set; } = 20;
        }
    }
}
