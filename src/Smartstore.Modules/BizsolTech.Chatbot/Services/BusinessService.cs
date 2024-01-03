using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Extensions;
using Microsoft.EntityFrameworkCore;
using Smartstore;
using Smartstore.Core.Data;

namespace BizsolTech.Chatbot.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly SmartDbContext _db;

        public BusinessService(SmartDbContext db)
        {
            this._db = db;
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
    }
}
