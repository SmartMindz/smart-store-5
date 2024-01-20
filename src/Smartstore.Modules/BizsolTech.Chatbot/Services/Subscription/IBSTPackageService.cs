using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BizsolTech.Chatbot.Domain.Subscription;

namespace BizSol.Chatbot.Services
{
    public interface IBSTPackageService
    {
        BSTPackage InsertPackage(BSTPackage package);
        BSTPackage UpdatePackage(BSTPackage package);
        void DeletePackage(int packageId);
        BSTPackage GetPackageByStripeProductID(string stripeProductId);
        IList<BSTPackage> GetAllPackages();
        BSTPackage GetPackageByName(string packageName);
        BSTPackage GetPackageById(int id);
    }
}