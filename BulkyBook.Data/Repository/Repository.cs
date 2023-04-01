using BulkyBook.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> _dbset;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            //_db.Products.Include(u=>u.Category).Include(u=>u.CoverType);
            _dbset = _db.Set<T>();
        }

        public void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public IEnumerable<T> GetAll(string? includeproperties = null)
        {
            IQueryable<T> query = _dbset;
            if (includeproperties != null)
            {
                foreach(var property in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeproperties = null)
        {
            IQueryable<T> query = _dbset;

            query = query.Where(filter);
			if (includeproperties != null)
			{
				foreach (var property in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}
			return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            _dbset.RemoveRange(entity);
        }
    }
}
