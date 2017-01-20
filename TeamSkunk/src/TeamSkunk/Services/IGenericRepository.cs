using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TeamSkunk.Services
{
    public interface IGenericRepository<T> : IDisposable
            where T : class
    {
        bool Any(Expression<Func<T, bool>> predicate);

        bool Any();

        /// <summary>Get all, with optional includes</summary>
        IQueryable<T> All(params string[] include);

        /// <summary>Get items where predicate is matched</summary>
        IQueryable<T> Filter(Expression<Func<T, bool>> predicate);

        IQueryable<T> Filter(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>Get items where predicate is matched using pages</summary>
        IQueryable<T> Filter(Expression<Func<T, bool>> predicate, out int total, int index = 0, int size = 50);

        /// <summary>Find by expression. </summary>
        T Find(Expression<Func<T, bool>> predicate);

        /// <summary>Find by expression with Eager Loading includes.</summary>
        /// <param name="predicate">Expression applied to the Find query.</param>
        /// <param name="includeProperties">Properties (related classes) to be included in this query.</param>
        /// <returns>An object matching the expression.</returns>
        T Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>Returns true if the set contains the object. </summary>
        bool Contains(Expression<Func<T, bool>> predicate);

        T Add(T entity);

        List<T> Add(List<T> entities);

        T Update(T entity);

        List<T> Update(List<T> entities);

        /// <summary>Delete a single item. </summary>
        void Delete(T entity);

        /// <summary>Delete a list of entities.</summary>
        void Delete(List<T> entities);

        /// <summary>Mass deletion based on condition</summary>
        void Delete(Expression<Func<T, bool>> condition);
    }
}
