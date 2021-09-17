using DotNetFive.Core.Caching;
using DotNetFive.Core.DataModel;
using DotNetFive.Core.Repository.Interface;
using DotNetFive.Core.Repository.Transaction;
using DotNetFive.Core.Repository.UnitOfWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetFive.Core.Repository.Class
{
    public class FileUploadRepository : Repository<FileUpload>, IFileUploadRepository
    {

        public FileUploadRepository(IUnitOfWork unitOfWork, ICachingService cachingService) : base(unitOfWork, cachingService)
        {
           
        }

        public async Task<IEnumerable<FileUpload>> GetByModule(string masterIdf, string module)
        {
            var all = await base.All();

            var result = all.Where(x => x.MasterIdf == masterIdf && x.Module == module);

            return result;
        }

       
    }
}
