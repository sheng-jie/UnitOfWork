using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace UnitOfWork
{
    public interface IUnitOfWork
    {
        int SaveChanges();
    }

    public interface IUnitOfWork<out TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        TDbContext DbContext { get; }
    }

    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        public UnitOfWork(TDbContext context)
        {
            DbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public TDbContext DbContext { get; }
    }

}
