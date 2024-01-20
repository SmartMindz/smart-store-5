using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Domain.Subscription;
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
        public static DbSet<BusinessPageMappingEntity> BusinessMappings(this SmartDbContext db)
            => db.Set<BusinessPageMappingEntity>();
        public static DbSet<BSTPackage> BSTPackages(this SmartDbContext db)
            => db.Set<BSTPackage>();
        public static DbSet<BSTSubscription> BSTSubscriptions(this SmartDbContext db)
            => db.Set<BSTSubscription>();
    }
}
