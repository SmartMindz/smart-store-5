using BizsolTech.Chatbot.Domain;
using Microsoft.EntityFrameworkCore;
using Smartstore.Core.Data;

namespace BizsolTech.Chatbot.Extensions
{
    public static class SmartDbContextExtensions
    {
        public static DbSet<BusinessPageEntity> BusinessPages(this SmartDbContext db)
            => db.Set<BusinessPageEntity>();
        public static DbSet<BusinessChatEntity> BusinessChats(this SmartDbContext db)
            => db.Set<BusinessChatEntity>();
        public static DbSet<BusinessFactEntity> BusinessFacts(this SmartDbContext db)
            => db.Set<BusinessFactEntity>();
        public static DbSet<BusinessDocumentEntity> BusinessDocuments(this SmartDbContext db)
            => db.Set<BusinessDocumentEntity>();
    }
}
