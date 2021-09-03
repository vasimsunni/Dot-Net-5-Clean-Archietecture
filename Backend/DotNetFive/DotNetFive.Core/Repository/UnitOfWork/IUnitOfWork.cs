using DotNetFive.Core.Database;
using DotNetFive.Core.Repository.Transaction;
using System;

namespace DotNetFive.Core.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IDatabaseTransaction BeginTransaction();
        ApplicationDbContext Context { get; }
        void Commit();
    }
}
