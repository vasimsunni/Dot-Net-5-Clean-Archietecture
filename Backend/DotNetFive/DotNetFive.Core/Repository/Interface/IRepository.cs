using DotNetFive.Core.Pagination.DTO;
using DotNetFive.Core.Repository.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetFive.Core.Repository.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<PagedResult<T>> Filter(string searchText, int pageNo, int pageSize);
        Task<IEnumerable<T>> All();
        Task<IEnumerable<T>> Get(object id);
        string Add(T entity, IDatabaseTransaction dbTransaction);
        bool Add(IEnumerable<T> entities, IDatabaseTransaction dbTransaction);
        string Update(T entity, IDatabaseTransaction dbTransaction);
        bool Delete(T entity, IDatabaseTransaction dbTransaction);
        bool Delete(IEnumerable<T> entities, IDatabaseTransaction dbTransaction);
    }
}
