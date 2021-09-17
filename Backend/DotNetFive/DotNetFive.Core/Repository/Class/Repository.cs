using DotNetFive.Core.Caching;
using DotNetFive.Core.DataModel;
using DotNetFive.Core.Pagination;
using DotNetFive.Core.Pagination.DTO;
using DotNetFive.Core.Repository.Interface;
using DotNetFive.Core.Repository.Transaction;
using DotNetFive.Core.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetFive.Core.Repository.Class
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICachingService cachingService;

        public Repository(IUnitOfWork unitOfWork, ICachingService cachingService)
        {
            this.unitOfWork = unitOfWork;
            this.cachingService = cachingService;
        }

        public async virtual Task<PagedResult<T>> Filter(string searchText, int pageNo, int pageSize)
        {
            var allRecords = await GetCachedList();

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
        public async virtual Task<IEnumerable<T>> All()
        {
            return await GetCachedList();
        }
        public async virtual Task<IEnumerable<T>> Get(object id)
        {
            if (id != null)
                return await GetByProperty("Id", id.ToString());

            return await GetCachedList();
        }

        public virtual string Add(T entity, IDatabaseTransaction dbTransaction)
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

                            RemoveCache();
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

        public virtual bool Add(IEnumerable<T> entities, IDatabaseTransaction dbTransaction)
        {
            using (dbTransaction)
            {
                try
                {
                    if (entities.Any())
                    {
                        unitOfWork.Context.Set<T>().AddRange(entities);

                        unitOfWork.Context.SaveChanges();

                        RemoveCache();

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

        public virtual string Update(T entity, IDatabaseTransaction dbTransaction)
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

                            RemoveCache();
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

        public virtual bool Delete(T entity, IDatabaseTransaction dbTransaction)
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

                        RemoveCache();

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

        public virtual bool Delete(IEnumerable<T> entities, IDatabaseTransaction dbTransaction)
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

                        RemoveCache();
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

        private async Task<IEnumerable<T>> GetCachedList()
        {
            Type entityType = this.GetType();

            string entityName = entityType.Name;

            var enumerable = (IQueryable<T>)(entityType.GetProperty("Name").GetValue(unitOfWork.Context, null));

            return await cachingService.GetOrCreateAsync(
                    entityName,
                    () => enumerable.ToListAsync(),
                    TimeSpan.FromMinutes(10),
                    TimeSpan.FromMinutes(60)
                );
        }

        private bool RemoveCache()
        {
            Type entityType = this.GetType();

            string entityName = entityType.Name;

            return cachingService.Remove(entityName);
        }

        private async Task<IEnumerable<T>> GetByProperty(string propertyName, string propertyValue)
        {
            var result = await GetCachedList();

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
