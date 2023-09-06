using BusinessAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IConnectionService
    {
        Task<QBOAuth> GetQBOAuthConfigureByCompanyId(string accountingCompanyId);
        Task<QBOAuth> GetQuickBooksConnectionByRealmId(string realmId);
        void InsertOrUpdateAuthDetails(QBOAuth qBOAuth);
        void DisconnectQuickBookCompany(QBOAuth qBOAuth);
    }
}
