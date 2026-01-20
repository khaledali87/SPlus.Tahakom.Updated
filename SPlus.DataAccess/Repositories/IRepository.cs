using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DataAccess
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        TEntity Get(int id);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        void Save(TEntity entity, bool isModified = false);
        void SaveRange(List<TEntity> entity, bool isModified = false);
        
        void Delete(TEntity entity);
        IQueryable<TEntity> Query();
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression);
        System.Data.Entity.DbContext GetContext();

    }
}
