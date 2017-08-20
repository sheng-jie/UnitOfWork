using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace UnitOfWork
{
    public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public UnitOfWork(TDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }

}
