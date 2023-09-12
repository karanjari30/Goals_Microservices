using BusinessAccessLayer.Model;

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
