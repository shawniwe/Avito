using Avito2.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace Avito2.Data.Repositories
{
    public class SqlRepository<T> : IRepository<T> where T: class, IEntity
    {
        protected readonly ApplicationDbContext _context;

        public SqlRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Create(T model)
        {
            var property = _context.GetType().GetProperties().Where(x => x.PropertyType == typeof(DbSet<T>)).First();
            var list = (DbSet<T>)property.GetValue(_context, null);
            list.Add(model);
            _context.SaveChanges();
        }

        public void Delete(T model)
        {
            var property = _context.GetType().GetProperties().Where(x => x.PropertyType == typeof(DbSet<T>)).First();
            var list = (DbSet<T>)property.GetValue(_context, null);
            list.Remove(model);
            _context.SaveChanges();
        }

        public virtual T Read(long? Id)
        {
            var property = _context.GetType().GetProperties().Where(x => x.PropertyType == typeof(DbSet<T>)).First();
            var list = (DbSet<T>)property.GetValue(_context, null);
            return list.Where(x => x.Id == Id).FirstOrDefault();
        }

        public virtual System.Collections.Generic.IEnumerable<T> ReadList()
        {
            var property = _context.GetType().GetProperties().Where(x => x.PropertyType == typeof(DbSet<T>)).First();
            var list = (DbSet<T>)property.GetValue(_context, null);
            return list;
        }

        public void Update(T model)
        {
            var property = _context.GetType().GetProperties().Where(x => x.PropertyType == typeof(DbSet<T>)).First();
            var list = (DbSet<T>)property.GetValue(_context, null);

            var entity = list.Where(x => x.Id == model.Id).FirstOrDefault();
            if(entity != null)
            {
                foreach (var entityProperty in entity.GetType().GetProperties())
                {
                    var modelProperty = model.GetType().GetProperties().Where(x => x.PropertyType == entityProperty.PropertyType).FirstOrDefault();
                    if (modelProperty != null)
                    {
                        var currentValue = entityProperty.GetValue(entity, null);
                        var modelValue = modelProperty.GetValue(model, null);
                        currentValue = modelValue;
                    }
                }

                _context.SaveChanges();
            }
        }
    }
}
