using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TeamSkunk.Data;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace TeamSkunk.Services
{
    public class Repository<T> : IGenericRepository<T> where T : class
    {
        private ApplicationDbContext context;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
        }

        private bool shared = true;
        private bool disposed = false;

        protected DbSet<T> DbSet
        {
            get { return context.Set<T>(); }
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        public virtual bool Any()
        {
            return DbSet.Any();
        }

        public virtual IQueryable<T> All(params string[] include)
        {
            var result = DbSet.AsQueryable();
            foreach (string item in include)
            {
                result = result.Include(item);
            }
            return result;
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).AsQueryable();
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbSet;

            foreach (var prop in includeProperties)
            {
                query = query.Include(prop);
            }

            return query.Where(predicate);
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate, out int total, int index = 0, int size = 50)
        {
            var result = predicate != null ? DbSet.Where(predicate).AsQueryable() :
                    DbSet.AsQueryable();
            total = result.Count();
            return result.Skip(index * size).Take(size);

        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual T Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbSet;

            foreach (var prop in includeProperties)
            {
                query = query.Include(prop);
            }

            return query.FirstOrDefault(predicate);
        }

        public virtual bool Contains(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public virtual T Add(T entity)
        {
            DbSet.Add(entity);
            return entity;
        }

        public virtual List<T> Add(List<T> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Add(entity);
            }
            return entities;
        }

        public virtual T Update(T entity)
        {
            DbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual List<T> Update(List<T> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
            }
            return entities;
        }

        public virtual void Delete(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
        }

        public virtual void Delete(List<T> entities)
        {
            int counter = 0;
            int startCount = entities.Count;

            while (counter < entities.Count)
            {
                if (context.Entry(entities[counter]).State == EntityState.Detached)
                {
                    DbSet.Attach(entities[counter]);
                }

                DbSet.Remove(entities[counter]);
                if (startCount == entities.Count)
                {
                    counter++;
                }
                else startCount = entities.Count;
            }
        }

        public virtual void Delete(Expression<Func<T, bool>> condition)
        {
            List<T> toDelete = DbSet.Where(condition).ToList();
            this.Delete(toDelete);
        }

        public void Dispose()
        {
            if (!shared) Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Check to see if Dispose has already been called
            if (!this.disposed)
            {
                if (disposing)
                {
                    //If disposing equals true, dispose Context.
                    context.Dispose();
                }
                disposed = true;
            }
        }
    }
}
