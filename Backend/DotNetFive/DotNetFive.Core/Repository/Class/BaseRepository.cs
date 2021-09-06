using DotNetFive.Core.Caching;
using DotNetFive.Core.Pagination;
using DotNetFive.Core.Pagination.DTO;
using DotNetFive.Core.Repository.Interface;
using DotNetFive.Core.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetFive.Core.Repository.Class
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICachingService cachingService;

        public BaseRepository(IUnitOfWork unitOfWork, ICachingService cachingService)
        {
            this.unitOfWork = unitOfWork;
            this.cachingService = cachingService;
        }

        public async Task<PagedResult<T>> Filter(T entity, string searchText, int pageNo, int pageSize)
        {
            var allRecords = await GetCachedList(entity);

            if (allRecords != null && allRecords.Any() && !string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();

                allRecords = allRecords.ToList();

                allRecords = allRecords.Where(x =>
                x.GetType().GetProperties().Where(y => y.PropertyType == typeof(string) && ((string)y.GetValue(y)).ToLower().Contains(searchText)).Count() > 0
                ||
                ((x.GetType().GetProperties().Where(y => y.PropertyType == typeof(string) && y.Name.ToLower() == "firstname").Count() > 0
                &&
                x.GetType().GetProperties().Where(y => y.PropertyType == typeof(string) && y.Name.ToLower() == "lastname").Count() > 0)
                &&
                (x.GetType().GetProperties().Where(y => y.PropertyType == typeof(string) && y.Name.ToLower() == "firstname").FirstOrDefault().GetValue(this).ToString()
                + " "
                + x.GetType().GetProperties().Where(y => y.PropertyType == typeof(string) && y.Name.ToLower() == "lastname").FirstOrDefault().GetValue(this).ToString()
                ).ToLower().Contains(searchText)
                ));
            }

            return allRecords.ToList().GetPaged(pageNo, pageSize);
        }

        public async Task<IEnumerable<T>> Get(object id, T entity)
        {
            if (id != null)
                return await GetByProperty(entity, "Id", id.ToString());

            return await GetCachedList(entity);
        }

        public string Add(T entity, IDbTransaction dbTransaction)
        {
            using (dbTransaction)
            {
                string savedId = "";

                try
                {
                    if (entity != null)
                    {
                        var savedEntity = unitOfWork.Context.Set<T>().Add(entity);

                        unitOfWork.Context.SaveChanges();

                        if (savedEntity != null)
                        {
                            savedId = GetProperty(entity, "Id").ToString();

                            RemoveCache(entity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                }

                return savedId;
            }
        }

        public bool Add(IEnumerable<T> entities, IDbTransaction dbTransaction)
        {
            using (dbTransaction)
            {
                try
                {
                    if (entities.Any())
                    {
                        unitOfWork.Context.Set<T>().AddRange(entities);

                        unitOfWork.Context.SaveChanges();

                        RemoveCache(entities.FirstOrDefault());

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                }

                return false;
            }
        }

        public string Update(T entity, IDbTransaction dbTransaction)
        {
            using (dbTransaction)
            {
                string savedId = "";

                try
                {
                    if (entity != null)
                    {
                        var savedEntity = unitOfWork.Context.Set<T>().Update(entity);

                        unitOfWork.Context.SaveChanges();

                        if (savedEntity != null)
                        {
                            savedId = GetProperty(entity, "Id").ToString();

                            RemoveCache(entity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                }

                return savedId;
            }
        }

        public bool Delete(T entity, IDbTransaction dbTransaction)
        {
            using (dbTransaction)
            {
                try
                {
                    var updatedEntity = SetProperty(entity, "IsDeleted", true);

                    if (!EqualityComparer<T>.Default.Equals(updatedEntity, null))
                    {
                        var deletedEntity = unitOfWork.Context.Set<T>().Update(updatedEntity);

                        unitOfWork.Context.SaveChanges();

                        RemoveCache(updatedEntity);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                }
                return false;
            }
        }

        public bool Delete(IEnumerable<T> entities, IDbTransaction dbTransaction)
        {
            using (dbTransaction)
            {
                bool hasDeleted = false;
                try
                {
                    foreach (var entity in entities)
                    {
                        var updatedEntity = SetProperty(entity, "IsDeleted", true);

                        if (!EqualityComparer<T>.Default.Equals(updatedEntity, null))
                        {
                            var deletedEntity = unitOfWork.Context.Set<T>().Update(updatedEntity);

                            hasDeleted = true;
                        }
                    }

                    if (hasDeleted)
                    {
                        unitOfWork.Context.SaveChanges();

                        RemoveCache(entities.FirstOrDefault());
                    }

                    return hasDeleted;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                }
            }
            return false;
        }

        private async Task<IEnumerable<T>> GetCachedList(T entity)
        {
            Type entityType = entity.GetType();

            string entityName = entityType.Name;

            var enumerable = (IQueryable<T>)(entityType.GetProperty("Name").GetValue(unitOfWork.Context, null));

            return await cachingService.GetOrCreateAsync(
                    entityName,
                    () => enumerable.ToListAsync(),
                    TimeSpan.FromMinutes(10),
                    TimeSpan.FromMinutes(60)
                );
        }

        private bool RemoveCache(T entity)
        {
            Type entityType = entity.GetType();

            string entityName = entityType.Name;

            return cachingService.Remove(entityName);
        }

        private async Task<IEnumerable<T>> GetByProperty(T entity, string propertyName, string propertyValue)
        {
            var result = await GetCachedList(entity);

            Type type = typeof(T);
            var prop = type.GetProperty(propertyName);

            return result.Where(x => (prop.GetValue(x, null)).ToString() == propertyValue);
        }

        private object GetProperty(T entity, string propertyName)
        {
            Type type = typeof(T);
            var prop = type.GetProperty(propertyName);

            return (prop.GetValue(entity, null));
        }

        private T SetProperty(T entity, string propertyName, bool propertyValue)
        {
            Type type = typeof(T);
            var prop = type.GetProperty(propertyName);

            if (prop != null)
            {
                prop.SetValue(entity, propertyValue);

                return entity;
            }

            return null;
        }

        private T SetProperty(T entity, string propertyName, string propertyValue)
        {
            Type type = typeof(T);
            var prop = type.GetProperty(propertyName);

            if (prop != null)
            {
                prop.SetValue(entity, propertyValue);

                return entity;
            }

            return null;
        }
    }
}
