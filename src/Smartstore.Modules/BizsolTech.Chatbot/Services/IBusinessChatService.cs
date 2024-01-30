using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizsolTech.Chatbot.Domain;

namespace BizsolTech.Chatbot.Services
{
    public interface IBusinessChatService
    {
        Task<IList<BusinessChatEntity>> GetBusinessChatAll();
        Task<IList<BusinessChatEntity>> GetBusinessChatByBusinessId(int businessId);
    }
}
