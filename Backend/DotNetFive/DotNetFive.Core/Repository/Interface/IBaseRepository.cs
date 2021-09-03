using DotNetFive.Core.Pagination.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetFive.Core.Repository.Interface
{
    public partial interface IBaseRepository<T> where T : class
    {
        Task<PagedResult<T>> Filter(object Id);
        Task<IEnumerable<T>> Get(object Id);
        int Add(T entity);
        bool Add(IEnumerable<T> entities);
        int Update(T entity);
        bool Delete(T entity);
        bool Delete(IEnumerable<T> entities);
        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }
    }
}
