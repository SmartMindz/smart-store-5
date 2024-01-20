using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BizSol.Chatbot.Services;
using BizsolTech.Chatbot.Domain.Subscription;
using BizsolTech.Chatbot.Extensions;
using Smartstore;
using Smartstore.Core;
using Smartstore.Core.Common.Services;
using Smartstore.Core.Data;
using Smartstore.Core.Stores;

namespace BizsolTech.Chatbot.Services
{
    public class BSTPackageService : IBSTPackageService
    {
        private readonly List<BSTPackage> _packages = new List<BSTPackage>();

        private readonly SmartDbContext _db;
        private readonly ICommonServices _service;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        public BSTPackageService(SmartDbContext db, ICommonServices service, IGenericAttributeService genericAttributeService, IWorkContext workContext, IStoreContext storeContext)
        {
            _db = db;
            _service = service;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _storeContext = storeContext;
        }

        public BSTPackage InsertPackage(BSTPackage package)
        {
            package.CreatedOnUtc = DateTime.UtcNow;
            _db.BSTPackages().Add(package);
            return package;
        }

        public BSTPackage UpdatePackage(BSTPackage package)
        {
            package.UpdatedOnUtc = DateTime.UtcNow;
            _db.BSTPackages().Update(package);
            return package;
        }

        public void DeletePackage(int packageId)
        {
            var package = _db.BSTPackages().FindById(packageId);
            _db.BSTPackages().Remove(package);
        }

        public BSTPackage GetPackageByStripeProductID(string stripeProductId)
        {
            var package = _db.BSTPackages().FirstOrDefault(p => p.StripeProductID == stripeProductId);
            
            if (package != null)
            {
                return package;
            }

            package = new BSTPackage();
            var service = new Stripe.ProductService();
            Stripe.Product product = service.Get(stripeProductId);
            
            package.Name = product.Name;
            package.StripeProductID = product.Id;

            if (product.Name.ToLower() == "professional")
            {
                package.BusinessLimit = 5;
                package.StorageLimitMB = 2048;
            }
            else if (product.Name.ToLower() == "team")
            {
                package.BusinessLimit = 10;
                package.StorageLimitMB = 10240;
            }

            var priceService = new PriceService();
            var price = priceService.Get(product.DefaultPriceId);

            
            package.StripePriceID = price.Id;
            package.Currency = price.Currency;
            package.Amount = price.UnitAmountDecimal;

            InsertPackage(package);

            return package;
        }
        
        public BSTPackage GetPackageByName(string packageName)
        {
            return _db.BSTPackages().FirstOrDefault(p => p.Name.ToLower() == packageName.ToLower());
        }

        public BSTPackage GetPackageById(int id)
        {
            return _db.BSTPackages().FirstOrDefault(p => p.Id == id);
        }

        public IList<BSTPackage> GetAllPackages()
        {
            return _db.BSTPackages().ToList();
        }
    }
}