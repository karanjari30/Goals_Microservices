using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessAccessLayer.Model
{
    public class QBOAuth
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string AccountingCompanyId { get; set; }
        public string ERPCompanyId { get; set; }
        public string ERPCompanyName { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? AccessTokenExpiryDate { get; set; }
        public DateTime? RefreshTokenExpiryDate { get; set; }
        public bool IsCompanyConnected { get; set; }
    }
}
