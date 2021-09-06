using System;

namespace DotNetFive.Core.Repository.Transaction
{
    public interface IDatabaseTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
