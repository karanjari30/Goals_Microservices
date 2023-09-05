using BusinessAccessLayer.Common;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOCore
{
    public class QBOAuthClient
    {
        public static OAuth2Client auth2Client = new OAuth2Client(AppConfiguration.QBOClientID, AppConfiguration.QBOClientSecret, AppConfiguration.QBORedirectUrl, AppConfiguration.QBOEnvironment); // environment is “sandbox” or “production”

        public static string GetAuthorizationURL(Guid companyId)
        {
            string scopes = "com.intuit.quickbooks.accounting profile openid phone email";
            string authorizationEndpoint = "https://appcenter.intuit.com/connect/oauth2";

            var authorizationUrl = new StringBuilder(authorizationEndpoint);
            authorizationUrl.AppendFormat("?client_id={0}", Uri.EscapeDataString(AppConfiguration.QBOClientID));
            authorizationUrl.Append("&response_type=code");
            authorizationUrl.AppendFormat("&scope={0}", Uri.EscapeDataString(scopes));
            authorizationUrl.AppendFormat("&redirect_uri={0}", Uri.EscapeDataString(AppConfiguration.QBORedirectUrl));
            authorizationUrl.AppendFormat("&state={0}", companyId);

            return authorizationUrl.ToString();
        }

        public static TokenResponse GetBearerTokenAsync(string code)
        {
            try
            {
                var tokenResp = auth2Client.GetBearerTokenAsync(code).Result;
                return tokenResp;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static ServiceContext Service(string realmId, string accessToken)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var oauthValidator = new Intuit.Ipp.Security.OAuth2RequestValidator(accessToken);
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.BaseUrl.Qbo = AppConfiguration.QBOBaseUrl;
            return serviceContext;
        }

        public static CompanyInfo QuickBookCompanyInfo(string realmId, string accessToken)
        {
            var serviceContext = Service(realmId, accessToken);
            var quickBooksCompanyInfo = new QueryService<CompanyInfo>(serviceContext).ExecuteIdsQuery("SELECT * FROM CompanyInfo", QueryOperationType.query).FirstOrDefault();
            return quickBooksCompanyInfo;
        }

        public static void RevokeToken(string refreshToken)
        {
            try
            {
                var objRefreshToken = auth2Client.RevokeTokenAsync(refreshToken).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
