using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BusinessAccessLayer.Model
{
    public class XeroAuth
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string IdToken { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? AccessTokenExpiryDate { get; set; }
        public bool IsCompanyConnected { get; set; }
    }

    public class XeroAccessToken
    {
        public string id_token { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
    }
}
