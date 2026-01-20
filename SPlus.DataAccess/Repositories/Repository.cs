using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Entity.Migrations;

namespace SPlus.DataAccess
{
    public class Repository<TEntity>  : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }
        public IEnumerable<TEntity> GetAll()
        {                   
            return Context.Set<TEntity>().ToList();
        }

        public TEntity Get(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }
        public void Save(TEntity entity, bool isModified = false)
        {
            if (isModified) 
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
            else
                Context.Set<TEntity>().AddOrUpdate(entity);
        }
        public void SaveRange(List<TEntity> entity, bool isModified = false)
        {
            if (isModified)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
            else
                Context.Set<TEntity>().AddRange(entity);
        }
        public void Delete(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return (IEnumerable<TEntity>)Context.Set<TEntity>().Find(predicate);
        }
        public TEntity SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }
        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression)
        {
            return Context.Set<TEntity>().Where(expression);
        }
        public IQueryable<TEntity> Query()
        {
            return Context.Set<TEntity>();
        }
        public DbContext GetContext()
        {
            return Context;
        }
     }
}
