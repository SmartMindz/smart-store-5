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
    public interface IBusinessDocumentService
    {
        Task<BusinessDocumentEntity> Get(int id);
        Task<List<BusinessDocumentEntity>> GetAllAsync();
        Task<BusinessDocumentEntity> Insert(BusinessDocumentEntity entity);
        Task<bool> Update(BusinessDocumentEntity entity);
    }

    public class BusinessDocumentService : IBusinessDocumentService
    {
        private readonly SmartDbContext _db;

        public BusinessDocumentService(SmartDbContext db)
        {
            this._db = db;
        }
        public async Task<BusinessDocumentEntity> Insert(BusinessDocumentEntity entity)
        {
            await _db.BusinessDocuments().AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Update(BusinessDocumentEntity entity)
        {
            try
            {
                _db.BusinessDocuments().Update(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<BusinessDocumentEntity> Get(int id)
        {
            return await _db.BusinessDocuments().FindByIdAsync(id);
        }

        public async Task<List<BusinessDocumentEntity>> GetAllAsync()
        {
            return await _db.BusinessDocuments().ToListAsync();
        }
    }
}
