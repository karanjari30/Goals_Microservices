using BusinessAccessLayer.Common;
using BusinessAccessLayer.Model;
using DataAccessLayer.Interfaces;
using MongoDB.Driver;

namespace DataAccessLayer.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IMongoCollection<QBOAuth> _qBOAuth;

        public ConnectionService()
        {
            var cscd = AppConfiguration.ConnectionString;

            var client = new MongoClient(AppConfiguration.ConnectionString);
            var database = client.GetDatabase(AppConfiguration.DatabaseName);
            _qBOAuth = database.GetCollection<QBOAuth>("QBOAuth");
        }

        public async Task<QBOAuth> GetQBOAuthConfigureByCompanyId(string accountingCompanyId)
        {
            var qboAuthConfigure = await _qBOAuth.FindAsync(x => x.AccountingCompanyId == accountingCompanyId);
            return qboAuthConfigure.FirstOrDefault();
        }

        public async Task<QBOAuth> GetQuickBooksConnectionByRealmId(string realmId)
        {
            var qboAuthConfigure = await _qBOAuth.FindAsync(x => x.ERPCompanyId == realmId);
            return qboAuthConfigure.FirstOrDefault();
        }

        public void InsertOrUpdateAuthDetails(QBOAuth qBOAuth)
        {
            var filter = Builders<QBOAuth>.Filter.Eq(x => x.ERPCompanyId, qBOAuth.ERPCompanyId) &
                         Builders<QBOAuth>.Filter.Eq(x => x.AccountingCompanyId, qBOAuth.AccountingCompanyId);

            // Check if a document with the ERPCompanyId and AccountingCompanyId exists
            var existingDocument = _qBOAuth.Find(filter).FirstOrDefault();

            if (existingDocument == null)
            {
                // If no existing document is found, insert the new document
                _qBOAuth.InsertOne(qBOAuth);
            }
            else
            {
                // If an existing document is found, update it
                qBOAuth.Id = existingDocument.Id;
                _qBOAuth.ReplaceOne(filter, qBOAuth);
            }
        }

        public void DisconnectQuickBookCompany(QBOAuth qBOAuth)
        {
            var filter = Builders<QBOAuth>.Filter.Eq(x => x.ERPCompanyId, qBOAuth.ERPCompanyId) &
                         Builders<QBOAuth>.Filter.Eq(x => x.AccountingCompanyId, qBOAuth.AccountingCompanyId);

            _qBOAuth.DeleteOne(filter);
        }
    }
}
