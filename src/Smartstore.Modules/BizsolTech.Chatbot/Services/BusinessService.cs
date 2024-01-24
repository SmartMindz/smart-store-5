using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Extensions;
using BizsolTech.Models.Business;
using Microsoft.EntityFrameworkCore;
using Smartstore;
using Smartstore.Core.Data;
using Smartstore.Core.Identity;

namespace BizsolTech.Chatbot.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly SmartDbContext _db;
        private readonly IBusinessAPIService _apiService;

        public BusinessService(SmartDbContext db, IBusinessAPIService apiService)
        {
            this._db = db;
            _apiService = apiService;
        }
        public async Task<List<BusinessPageEntity>> GetAllAsync()
        {
            return await _db.BusinessPages().ToListAsync();
        }
        public async Task<BusinessPageEntity> Get(int id)
        {
            return await _db.BusinessPages().FindByIdAsync(id);
        }
        public async Task<BusinessPageEntity> Insert(BusinessPageEntity entity)
        {
            await _db.BusinessPages().AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Update(BusinessPageEntity entity)
        {
            try
            {
                _db.BusinessPages().Update(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<BusinessPageMappingEntity>> GetBusinessMappings()
        {
            var list = new List<BusinessPageMappingEntity>();
            try
            {
                list = await _db.BusinessMappings().ToListAsync();
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        public async Task<List<Business>> GetCustomerBusinessAll(Customer customer)
        {
            Guard.NotNull(customer, nameof(Customer));
            var list = new List<Business>();
            try
            {
                var businessAll = await _apiService.GetBusinessAll();
                var businessMapping = await GetBusinessMappings();

                var customerMappings = businessMapping.Where(m => m.EntityId.Equals(customer.Id) && m.EntityName.Equals(nameof(Customer)) && businessAll.Exists(b => b.Id.Equals(m.BusinessId)));
                foreach (var map in customerMappings)
                {
                    var result = businessAll.FirstOrDefault(b => b.Id == map.BusinessId);
                    list.Add(result);
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        public async Task<bool> InsertBusinessMapping(BusinessPageMappingEntity entity)
        {
            try
            {
                Guard.NotNull(entity, nameof(entity));
                await _db.BusinessMappings().AddAsync(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<BusinessPageMappingEntity> GetBusinessMappingById(int id)
        {
            try
            {
                return await _db.BusinessMappings().FindByIdAsync(id);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
