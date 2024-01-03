using System.Collections.Generic;
using System.Threading.Tasks;
using BizsolTech.Chatbot.Domain;

namespace BizsolTech.Chatbot.Services
{
    public interface IBusinessService
    {
        Task<BusinessPageEntity> Get(int id);
        Task<List<BusinessPageEntity>> GetAllAsync();
        Task<BusinessPageEntity> Insert(BusinessPageEntity entity);
        Task<bool> Update(BusinessPageEntity entity);
    }
}