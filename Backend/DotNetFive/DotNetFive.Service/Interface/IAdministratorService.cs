using DotNetFive.Infrastructure.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetFive.Service.Interface
{
    public interface IAdministratorService
    {
        Task<ResponseDTO<PaginatedResponse<IEnumerable<Infrastructure.DTO.Response.Administrator>>>> Filter(string searchText, int pageNo, int pageSize);
        Task<ResponseDTO<IEnumerable<Infrastructure.DTO.Response.Administrator>>> Get(long id);
        Task<ResponseDTO<Infrastructure.DTO.Response.Administrator>> Create(Infrastructure.DTO.Request.Administrator model);
        Task<ResponseDTO<Infrastructure.DTO.Response.Administrator>> Update(Infrastructure.DTO.Request.Administrator model);
        Task<ResponseDTO<Infrastructure.DTO.Response.Administrator>> Activate(long id, bool activate);
        Task<ResponseDTO<bool>> Delete(long id);
    }
}
