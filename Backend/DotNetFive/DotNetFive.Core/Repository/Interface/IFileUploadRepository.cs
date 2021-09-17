using DotNetFive.Core.DataModel;
using DotNetFive.Core.Repository.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetFive.Core.Repository.Interface
{
    public interface IFileUploadRepository : IRepository<FileUpload>
    {
        Task<IEnumerable<FileUpload>> GetByModule(string masterIdf,string module);
    }
}
