using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace DotNetFive.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile(IConfiguration configuration)
        {
            //Default keys
            var UploadFolderURL = configuration["Utility:APIBaseURL"] + "/" + configuration["UploadFolders:UploadFolder"] + "/";
            var DefaultPictureURL = UploadFolderURL + configuration["UploadFolders:DefaultProfilePicture"];

            // Generic type mapping(for paging)
            CreateMap(typeof(Core.Pagination.DTO.PagedResult<>), typeof(DTO.Response.PaginatedResponse<>)).ReverseMap();

            //User Details
            //CreateMap<UserDetailsDTO, UserDetailsResponseDTO>()
            //    .ForMember(dest => dest.ProfilePictureURL, source => source.MapFrom(src => src.ProfilePictureName.Trim() == "" ? DefaultPictureURL : UploadFolderURL + src.ProfilePictureName));

            CreateMap<Core.DataModel.Administrator, DTO.Response.Administrator>()
                .ForMember(dest => dest.Id, source => source.MapFrom(src => src.AdminId))
                .ReverseMap();

            CreateMap<Core.DataModel.Administrator, DTO.Request.Administrator>()
               .ForMember(dest => dest.Id, source => source.MapFrom(src => src.AdminId))
               .ReverseMap();

            //CreateMap<FileUpload, FileUploadRequestDTO>()
            //   .ForMember(dest => dest.Id, source => source.MapFrom(src => src.FileId))
            //   .ReverseMap();
        }
    }
}
