using Microsoft.EntityFrameworkCore;

namespace UnitOfWork.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
    }
}