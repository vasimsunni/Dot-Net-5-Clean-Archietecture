using DotNetFive.Core.Database;
using DotNetFive.Core.Repository.Transaction;

namespace DotNetFive.Core.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public ApplicationDbContext Context { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            Context = context;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public IDatabaseTransaction BeginTransaction()
        {
            return new EntityDatabaseTransaction(Context);
        }
    }
}
