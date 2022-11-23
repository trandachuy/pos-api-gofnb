using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace GoFoodBeverage.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllReadOnlyAsync();

        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize);

        /// <summary>
        /// This method are AddAsync and SaveChangeAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// This method are AddRangeAsync and SaveChangeAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// This method are set EntityState.Modified and SaveChangeAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// This method are set UpdateRange and SaveChangeAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// This method are set Remove and SaveChangeAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task RemoveAsync(T entity);

        /// <summary>
        /// This method are set RemoveRange and SaveChangeAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task RemoveRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Query all records
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Using to build a query string
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        IQueryable<T> FromSqlRaw(string sql, params object[] parameters);

        /// <summary>
        /// Using Add from EntityFrameworkCore
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity);

        /// <summary>
        /// Using AddRange from EntityFrameworkCore
        /// </summary>
        /// <param name="entities"></param>
        public void AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Using Update from EntityFrameworkCore
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity);

        /// <summary>
        /// Using UpdateRange from EntityFrameworkCore
        /// </summary>
        /// <param name="entities"></param>
        public void UpdateRange(IEnumerable<T> entities);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}
