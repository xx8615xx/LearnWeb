using Learn.DataAccess.Data;
using Learn.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Learn.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        private DbSet<T> _dbSet;
        public Repository(AppDbContext db)
        {
            _db = db;
            this._dbSet = _db.Set<T>();
            // _db.Categories == _db.Set<Categories>()
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            //Category? categoryFromDB3 = _db.Categories.Where(u=>u.ID==id).FirstOrDefault();
            IQueryable<T> querry = _dbSet.Where(filter);
            return querry.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> querry = _dbSet;
            return querry.ToList();
            //return _dbSet.ToList();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }
    }
}
