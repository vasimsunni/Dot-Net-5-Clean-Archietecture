using DotNetFive.Core.Pagination.DTO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetFive.Core.Repository.Interface
{
    public partial interface IBaseRepository<T> where T : class
    {
        Task<PagedResult<T>> Filter(T entity, string searchText, int pageNo, int pageSize);
        Task<IEnumerable<T>> Get(object id, T entity);
        string Add(T entity, IDbTransaction dbTransaction);
        bool Add(IEnumerable<T> entities, IDbTransaction dbTransaction);
        string Update(T entity, IDbTransaction dbTransaction);
        bool Delete(T entity, IDbTransaction dbTransaction);
        bool Delete(IEnumerable<T> entities, IDbTransaction dbTransaction);
    }
}
