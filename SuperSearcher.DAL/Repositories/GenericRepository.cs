﻿using Microsoft.EntityFrameworkCore;

using SuperSearcher.DAL.Contexts;
using SuperSearcher.DAL.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SuperSearcher.DAL.Repositories
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
	{
		private readonly ApplicationContext context;
		private readonly DbSet<TEntity> dbSet;

		public GenericRepository(ApplicationContext context)
		{
			this.context = context;
			dbSet = context.Set<TEntity>();
		}

		public IEnumerable<TEntity> Get()
		{
			return dbSet.AsNoTracking().ToList();
		}

		public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
		{
			return dbSet.AsNoTracking().Where(predicate).ToList();
		}
		public TEntity FindById(int id)
		{
			return dbSet.Find(id);
		}
		public TEntity FindById(long id)
		{
			return dbSet.Find(id);
		}

		public TEntity Create(TEntity item)
		{
			dbSet.Add(item);
			context.SaveChanges();

			return item;
		}
		public void CreateRange(IEnumerable<TEntity> items)
		{
			dbSet.AddRange(items);
			context.SaveChanges();
		}

		public void Update(TEntity item)
		{
			context.Entry(item).State = EntityState.Modified;
			context.SaveChanges();
		}

		public void Remove(TEntity item)
		{
			dbSet.Remove(item);
			context.SaveChanges();
		}
		public void RemoveRange(IEnumerable<TEntity> items)
		{
			dbSet.RemoveRange(items);
			context.SaveChanges();
		}

		public void Remove(int id)
		{
			TEntity entity = FindById(id);
			dbSet.Remove(entity);
			context.SaveChanges();
		}
		public void Remove(long id)
		{
			TEntity entity = FindById(id);
			dbSet.Remove(entity);
			context.SaveChanges();
		}

		public IEnumerable<TEntity> GetWithInclude(params Expression<Func<TEntity, object>>[] includeProperties)
		{
			return Include(includeProperties).ToList();
		}

		public IEnumerable<TEntity> GetWithInclude(Func<TEntity, bool> predicate,
			params Expression<Func<TEntity, object>>[] includeProperties)
		{
			IQueryable<TEntity> query = Include(includeProperties);
			return query.Where(predicate).ToList();
		}

		private IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
		{
			IQueryable<TEntity> query = dbSet.AsNoTracking();
			return includeProperties
				.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
		}
	}
}
