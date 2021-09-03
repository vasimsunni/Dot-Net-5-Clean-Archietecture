using DotNetFive.Core.Pagination.DTO;
using DotNetFive.Core.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetFive.Core.Repository.Class
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public Task<PagedResult<T>> Filter(object Id){

        }
        public Task<IEnumerable<T>> Get(object Id)
        {

        }
        public int Add(T entity){

        }
        public bool Add(IEnumerable<T> entities){

        }
        public int Update(T entity){

        }
        public bool Delete(T entity){

        }
        public bool Delete(IEnumerable<T> entities){

        }
        public IQueryable<T> Table { get; }
        public IQueryable<T> TableNoTracking { get; }
    }
}
