using AutoMapper;
using DotNetFive.Core.DataModel;
using DotNetFive.Core.Repository.Interface;
using DotNetFive.Core.Repository.Transaction;
using DotNetFive.Infrastructure.Configuration.Application;
using DotNetFive.Infrastructure.DTO.Response;
using DotNetFive.Infrastructure.Enum;
using DotNetFive.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetFive.Service.Class
{
    public class AdministratorService : IAdministratorService
    {

        private readonly IRepository<Core.DataModel.Administrator> administratorRepository;
        private readonly IUserRepository userRepository;
        private readonly IFileUploadRepository fileUploadRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContext;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDatabaseTransaction databaseTransaction;

        public AdministratorService(IRepository<Core.DataModel.Administrator> administratorRepository,
                                    IUserRepository userRepository, IFileUploadRepository fileUploadRepository,
                                    UserManager<ApplicationUser> userManager, IMapper mapper,
                                    IHttpContextAccessor httpContext, IConfiguration configuration,
                                    IWebHostEnvironment hostingEnvironment, IDatabaseTransaction databaseTransaction)
        {
            this.administratorRepository = administratorRepository;
            this.userRepository = userRepository;
            this.fileUploadRepository = fileUploadRepository;
            this.userManager = userManager;
            this.mapper = mapper;
            this.httpContext = httpContext;
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.databaseTransaction = databaseTransaction;
        }

        public async Task<ResponseDTO<PaginatedResponse<IEnumerable<Infrastructure.DTO.Response.Administrator>>>> Filter(string searchText, int pageNo, int pageSize)
        {
            ResponseDTO<PaginatedResponse<IEnumerable<Infrastructure.DTO.Response.Administrator>>> response = new ResponseDTO<Infrastructure.DTO.Response.PaginatedResponse<IEnumerable<Infrastructure.DTO.Response.Administrator>>>();

            int StatusCode = 0;
            bool isSuccess = false;
            PaginatedResponse<IEnumerable<Infrastructure.DTO.Response.Administrator>> Data = null;
            string Message = "";
            string ExceptionMessage = "";
            try
            {
                var filteredResult = await administratorRepository.Filter(searchText, pageNo, pageSize);

                var filteredResponse = mapper.Map<PaginatedResponse<IEnumerable<Infrastructure.DTO.Response.Administrator>>>(filteredResult);

                if (filteredResponse.Records.Any())
                {
                    string PictureURL = string.Empty;
                    string BaseURL = configuration.AppSettings().APIBaseURL;
                    string uploadFolderName = configuration["UploadFolders:UploadFolder"];

                    foreach (var admin in filteredResponse.Records)
                    {
                        PictureURL = string.Empty;

                        var applicationUser = await userRepository.GetByEmail(admin.Email);

                        if (applicationUser != null)
                        {
                            var fileUploads = await fileUploadRepository.GetByModule(admin.Id, FileUploadEnum.AdminProfilePicture.ToString());

                            if (fileUploads.Any()) PictureURL = uploadFolderName + "/" + fileUploads.LastOrDefault().Name;

                            if (string.IsNullOrEmpty(PictureURL))
                            {
                                string defaultProfilePicture = configuration["UploadFolders:DefaultProfilePicture"];
                                PictureURL = BaseURL + uploadFolderName + "/" + defaultProfilePicture;
                            }
                            else PictureURL = BaseURL + PictureURL;

                            admin.Username = applicationUser.UserName;
                            admin.Roles = (await userRepository.GetRole(applicationUser)).ToArray();
                            admin.ProfilePictureURL = PictureURL;

                            if (!string.IsNullOrEmpty(admin.CreatedBy)) admin.CreatedBy = await userRepository.GetFullName(admin.CreatedBy);
                            if (!string.IsNullOrEmpty(admin.UpdatedBy)) admin.UpdatedBy = await userRepository.GetFullName(admin.UpdatedBy);
                        }
                    }

                    isSuccess = true;
                    StatusCode = 200;
                    Data = filteredResponse;
                    Message = "Success";
                }
                else
                {
                    StatusCode = 404;
                    Data = filteredResponse;
                    Message = "No Data found.";
                }
            }
            catch (Exception ex)
            {
                StatusCode = 500;
                Message = "Failed";
                ExceptionMessage = ex.ToString();
            }

            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Data = Data;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;

            return response;
        }

        public async Task<ResponseDTO<IEnumerable<Infrastructure.DTO.Response.Administrator>>> Get(long id)
        {
            ResponseDTO<IEnumerable<Infrastructure.DTO.Response.Administrator>> response = new ResponseDTO<IEnumerable<Infrastructure.DTO.Response.Administrator>>();

            int StatusCode = 0;
            bool isSuccess = false;
            IEnumerable<Infrastructure.DTO.Response.Administrator> Data = null;
            string Message = "";
            string ExceptionMessage = "";
            try
            {
                var result = await administratorRepository.Get(id);

                var filteredResponse = mapper.Map<IEnumerable<Infrastructure.DTO.Response.Administrator>>(result);

                if (filteredResponse.Any())
                {
                    string PictureURL = string.Empty;
                    string BaseURL = configuration["Utility:APIBaseURL"].ToString();
                    string uploadFolderName = configuration["UploadFolders:UploadFolder"];

                    foreach (var admin in filteredResponse)
                    {
                        PictureURL = string.Empty;

                        var applicationUser = await userRepository.GetByEmail(admin.Email);

                        if (applicationUser != null)
                        {
                            var fileUploads = await fileUploadRepository.GetByModule(admin.Id, FileUploadEnum.AdminProfilePicture.ToString());

                            if (fileUploads.Any()) PictureURL = uploadFolderName + "/" + fileUploads.LastOrDefault().Name;

                            if (string.IsNullOrEmpty(PictureURL))
                            {
                                string defaultProfilePicture = configuration["UploadFolders:DefaultProfilePicture"];
                                PictureURL = BaseURL + uploadFolderName + "/" + defaultProfilePicture;
                            }
                            else PictureURL = BaseURL + PictureURL;

                            admin.Username = applicationUser.UserName;
                            admin.Roles = (await userRepository.GetRole(applicationUser)).ToArray();
                            admin.ProfilePictureURL = PictureURL;

                            if (!string.IsNullOrEmpty(admin.CreatedBy)) admin.CreatedBy = await userRepository.GetFullName(admin.CreatedBy);
                            if (!string.IsNullOrEmpty(admin.UpdatedBy)) admin.UpdatedBy = await userRepository.GetFullName(admin.UpdatedBy);
                        }
                    }

                    isSuccess = true;
                    StatusCode = 200;
                    Data = filteredResponse;
                    Message = "Success";
                }
                else
                {
                    StatusCode = 404;
                    Data = filteredResponse;
                    Message = "No Data found.";
                }
            }
            catch (Exception ex)
            {
                StatusCode = 500;
                Message = "Failed";
                ExceptionMessage = ex.ToString();
            }

            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Data = Data;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;

            return response;
        }

        public async Task<ResponseDTO<Infrastructure.DTO.Response.Administrator>> Create(Infrastructure.DTO.Request.Administrator model)
        {
            ResponseDTO<Infrastructure.DTO.Response.Administrator> response = new ResponseDTO<Infrastructure.DTO.Response.Administrator>();

            int StatusCode = 0;
            bool isSuccess = false;
            Infrastructure.DTO.Response.Administrator Data = null;
            string Message = "";
            string ExceptionMessage = "";
            try
            {
                ApplicationUser currentUser = await userManager.GetUserAsync(httpContext.HttpContext.User);

                ApplicationUser applicationUser = new ApplicationUser
                {
                    UserName = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    NormalizedUserName = model.Username,
                    NormalizedEmail = model.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = model.Phone,
                    TwoFactorEnabled = true,
                    LockoutEnd = new DateTime(2099, 12, 31),
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = currentUser.Id
                };

                var result = await userRepository.Add(applicationUser, model.Password, model.Roles);

                if (result.Succeeded)
                {
                    var administrator = new Core.DataModel.Administrator()
                    {
                        IdentityUserIdf = applicationUser.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Mobile = model.Mobile,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = currentUser.Id
                    };

                    model.Id = administratorRepository.Add(administrator, databaseTransaction);

                    if (!string.IsNullOrWhiteSpace(model.Id))
                    {
                        if (!string.IsNullOrWhiteSpace(model.UploadedFileId))
                        {
                            var uploadedFile = (await fileUploadRepository.Get(model.UploadedFileId)).LastOrDefault();

                            if (uploadedFile != null)
                            {
                                var uploadedFileRequestDTO = new FileUpload
                                {
                                    Module = FileUploadEnum.AdminProfilePicture.ToString(),
                                    MasterIdf = model.Id,
                                    Name = uploadedFile.Name,
                                    OriginalName = uploadedFile.OriginalName,
                                    Size = uploadedFile.Size,
                                    Type = uploadedFile.Type,
                                    OtherDetails = uploadedFile.OtherDetails
                                };

                                fileUploadRepository.Add(uploadedFile, databaseTransaction);

                                databaseTransaction.Commit();
                            }
                        }

                        Data = mapper.Map<Infrastructure.DTO.Response.Administrator>(administrator);
                        StatusCode = 200;
                        isSuccess = true;
                        Message = "Admin added successfully.";
                    }
                    else
                    {
                        StatusCode = 500;
                        Message = "Failed while saving.";
                    }
                }
                else
                {
                    StatusCode = 400;
                    Message = string.Join(" ,", result.Errors.Select(x => x.Description).ToList());
                }
            }
            catch (Exception ex)
            {
                StatusCode = 500;
                Message = "Failed";
                ExceptionMessage = ex.ToString();
            }

            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Data = Data;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;

            return response;
        }

        public async Task<ResponseDTO<Infrastructure.DTO.Response.Administrator>> Update(Infrastructure.DTO.Request.Administrator model)
        {
            ResponseDTO<Infrastructure.DTO.Response.Administrator> response = new ResponseDTO<Infrastructure.DTO.Response.Administrator>();

            int StatusCode = 0;
            bool isSuccess = false;
            Infrastructure.DTO.Response.Administrator Data = null;
            string Message = "";
            string ExceptionMessage = "";
            try
            {
                ApplicationUser currentUser = await userManager.GetUserAsync(httpContext.HttpContext.User);

                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    var admins = await administratorRepository.Get(model.Id);

                    if (admins.Any())
                    {
                        var administrator = admins.LastOrDefault();
                        var applicationUser = await userRepository.GetByEmail(administrator.Email);

                        if (applicationUser != null)
                        {
                            applicationUser.Email = model.Email;
                            applicationUser.FirstName = model.FirstName;
                            applicationUser.LastName = model.LastName;
                            applicationUser.UserName = model.Username;
                            applicationUser.IsActive = true;
                            applicationUser.IsDeleted = false;
                            applicationUser.LockoutEnd = new DateTime(2099, 12, 31);
                            applicationUser.UpdatedBy = currentUser.Id;
                            if (!string.IsNullOrEmpty(model.Password))
                            {
                                applicationUser.PasswordHash = userManager.PasswordHasher.HashPassword(applicationUser, model.Password);
                            }

                            var result = await userRepository.Update(applicationUser, model.Roles);

                            if (result.Succeeded)
                            {
                                administrator.FirstName = model.FirstName;
                                administrator.LastName = model.LastName;
                                administrator.Email = model.Email;
                                administrator.Phone = model.Phone;
                                administrator.Mobile = model.Mobile;
                                administrator.UpdatedBy = currentUser.Id;

                                model.Id = administratorRepository.Update(administrator, databaseTransaction);

                                if (string.IsNullOrWhiteSpace(model.UploadedFileId))
                                {
                                    var uploadedFile = (await fileUploadRepository.Get(model.UploadedFileId)).LastOrDefault();

                                    if (uploadedFile != null)
                                    {
                                        var uploadedFileRequestDTO = new FileUpload
                                        {
                                            Module = FileUploadEnum.AdminProfilePicture.ToString(),
                                            MasterIdf = model.Id,
                                            Name = uploadedFile.Name,
                                            OriginalName = uploadedFile.OriginalName,
                                            Size = uploadedFile.Size,
                                            Type = uploadedFile.Type,
                                            OtherDetails = uploadedFile.OtherDetails
                                        };

                                        string savedFileId = fileUploadRepository.Update(uploadedFile, databaseTransaction);
                                        if (string.IsNullOrWhiteSpace(savedFileId))
                                        {
                                            string uploadFolderName = configuration["UploadFolders:UploadFolder"];
                                            if (string.IsNullOrWhiteSpace(hostingEnvironment.WebRootPath))
                                            {
                                                hostingEnvironment.WebRootPath = Path.Combine(string.Empty, "wwwroot");
                                            }

                                            //find the previous uploaded profile picture
                                            var recentUploads = await fileUploadRepository.GetByModule(model.Id, FileUploadEnum.AdminProfilePicture.ToString());

                                            if (recentUploads.Any())
                                            {
                                                //delete previously uploaded profile picture - should not be deleted in the case of soft delete
                                                //foreach (var recentUpload in recentUploads)
                                                //{
                                                //    //string physicalStoredFile = Path.Combine(hostingEnvironment.WebRootPath + "\\" + uploadFolderName + "\\", recentUpload.Name);
                                                //    //if (System.IO.File.Exists(physicalStoredFile))
                                                //    //{
                                                //    //    System.IO.File.Delete(physicalStoredFile);
                                                //    //}
                                                //}

                                                fileUploadRepository.Delete(recentUploads, databaseTransaction);
                                            }
                                        }
                                    }

                                    databaseTransaction.Commit();

                                }

                                Data = mapper.Map<Infrastructure.DTO.Response.Administrator>(administrator);
                                StatusCode = 200;
                                isSuccess = true;
                                Message = "Admin updated successfully.";

                            }
                            else
                            {
                                StatusCode = 400;
                                Message = string.Join(" ,", result.Errors.Select(x => x.Description).ToList());
                            }
                        }
                        else
                        {
                            StatusCode = 404;
                            Message = "Admin not found";
                        }
                    }
                    else
                    {
                        StatusCode = 404;
                        Message = "Admin not found";
                    }
                }
                else
                {
                    StatusCode = 406;
                    Message = "Not Acceptable.";
                }
            }
            catch (Exception ex)
            {
                StatusCode = 500;
                Message = "Failed";
                ExceptionMessage = ex.ToString();
            }

            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Data = Data;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;

            return response;
        }

        public async Task<ResponseDTO<Infrastructure.DTO.Response.Administrator>> Activate(long id, bool activate)
        {
            ResponseDTO<Infrastructure.DTO.Response.Administrator> response = new ResponseDTO<Infrastructure.DTO.Response.Administrator>();

            int StatusCode = 0;
            bool isSuccess = false;
            Infrastructure.DTO.Response.Administrator Data = null;
            string Message = "";
            string ExceptionMessage = "";
            try
            {
                ApplicationUser currentUser = await userManager.GetUserAsync(httpContext.HttpContext.User);

                if (id > 0)
                {
                    var admins = await administratorRepository.Get(id);

                    if (admins.Any())
                    {
                        var administrator = admins.LastOrDefault();
                        var applicationUser = await userRepository.GetByEmail(administrator.Email);

                        if (applicationUser != null)
                        {
                            applicationUser.IsActive = activate;
                            applicationUser.UpdatedBy = currentUser.Id;

                            var oldRoles = await userManager.GetRolesAsync(applicationUser);

                            var result = await userRepository.Update(applicationUser, oldRoles.ToArray());

                            if (result.Succeeded)
                            {
                                administrator.IsActive = true;

                                string updatedAdminId = administratorRepository.Update(administrator, databaseTransaction);

                                if (!string.IsNullOrWhiteSpace(updatedAdminId))
                                {
                                    databaseTransaction.Commit();

                                    Data = mapper.Map<Infrastructure.DTO.Response.Administrator>(administrator);

                                    StatusCode = 200;
                                    isSuccess = true;
                                    Message = "Admin updated successfully.";
                                }
                                else
                                {
                                    StatusCode = 500;
                                    Message = "Failed";
                                }
                            }
                            else
                            {
                                StatusCode = 400;
                                Message = string.Join(" ,", result.Errors.Select(x => x.Description).ToList());
                            }
                        }
                        else
                        {
                            StatusCode = 404;
                            Message = "Admin not found";
                        }
                    }
                    else
                    {
                        StatusCode = 404;
                        Message = "Admin not found";
                    }
                }
                else
                {
                    StatusCode = 406;
                    Message = "Not Acceptable.";
                }
            }
            catch (Exception ex)
            {
                StatusCode = 500;
                Message = "Failed";
                ExceptionMessage = ex.ToString();
            }

            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Data = Data;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;

            return response;
        }

        public async Task<ResponseDTO<bool>> Delete(long id)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();

            int StatusCode = 0;
            bool isSuccess = false;
            bool Data = false;
            string Message = "";
            string ExceptionMessage = "";
            try
            {
                ApplicationUser currentUser = await userManager.GetUserAsync(httpContext.HttpContext.User);

                if (id > 0)
                {
                    var admins = await administratorRepository.Get(id);

                    if (admins.Any())
                    {
                        var administrator = admins.LastOrDefault();
                        var applicationUser = await userRepository.GetByEmail(administrator.Email);

                        if (applicationUser != null)
                        {
                            var oldRoles = await userManager.GetRolesAsync(applicationUser);

                            applicationUser.IsDeleted = true;
                            applicationUser.UpdatedBy = currentUser.Id;


                            var result = await userRepository.Update(applicationUser, oldRoles.ToArray());

                            if (result.Succeeded)
                            {
                                bool deleted = administratorRepository.Delete(administrator, databaseTransaction);

                                if (deleted)
                                {
                                    databaseTransaction.Commit();
                                    Data = true;
                                    StatusCode = 200;
                                    isSuccess = true;
                                    Message = "Admin delete successfully.";
                                }
                                else
                                {
                                    StatusCode = 500;
                                    Message = "Failed";
                                }
                            }
                            else
                            {
                                StatusCode = 400;
                                Message = string.Join(" ,", result.Errors.Select(x => x.Description).ToList());
                            }
                        }
                        else
                        {
                            StatusCode = 404;
                            Message = "Admin not found";
                        }
                    }
                    else
                    {
                        StatusCode = 404;
                        Message = "Admin not found";
                    }
                }
                else
                {
                    StatusCode = 406;
                    Message = "Not Acceptable.";
                }
            }
            catch (Exception ex)
            {
                StatusCode = 500;
                Message = "Failed";
                ExceptionMessage = ex.ToString();
            }

            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Data = Data;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;

            return response;
        }

    }

}
