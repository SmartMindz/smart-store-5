using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Extensions;
using Microsoft.EntityFrameworkCore;
using Smartstore.Core.Data;

namespace BizsolTech.Chatbot.Services
{
    public class BusinessChatService : IBusinessChatService
    {
        private readonly SmartDbContext _db;

        public BusinessChatService(SmartDbContext db)
        {
            this._db = db;
        }
        public async Task<IList<BusinessChatEntity>> GetBusinessChatAll()
        {
            var list = new List<BusinessChatEntity>();
            try
            {
                list = await _db.BusinessChats().ToListAsync();
                return list;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IList<BusinessChatEntity>> GetBusinessChatByBusinessId(int businessId)
        {
            var list = new List<BusinessChatEntity>();
            try
            {
                var id = businessId.ToString();
                list = await _db.BusinessChats().Where(c => c.BusinessId == long.Parse(id)).ToListAsync();
                return list;
            }
            catch
            {
                throw;
            }
        }
    }
}
